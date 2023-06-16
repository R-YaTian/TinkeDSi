/*
 * Copyright (C) 2011  pleoNeX
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
 *
 * By: mn1712trungson
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Ekona;

namespace SF_FEATHER
{
    public class CGx
    {
        IPluginHost pluginHost;
        CGxS sCGx;

        public CGx(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
        }

        public sFolder Unpack(sFile file)
        {
            CGxS cgx = new CGxS();
            BinaryReader br = new BinaryReader(File.OpenRead(file.path));

            cgx.id = br.ReadChars(4);
            cgx.u_Value = br.ReadUInt32();
            cgx.mappingType = br.ReadUInt32();
            cgx.NULL = br.ReadUInt32();
            cgx.transparency = br.ReadUInt32();
            bool transparency = cgx.transparency == 0x00 ? false : true;

            cgx.bitmapSize = br.ReadUInt32();
            cgx.paletteSize = br.ReadUInt32();
            cgx.tileNumber = br.ReadUInt32();

            cgx.objectType = br.ReadUInt32();
            cgx.bitmapPointer = br.ReadUInt32();
            cgx.palettePointer = br.ReadUInt32();
            cgx.positionPointer = br.ReadUInt32();

            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            if (cgx.positionPointer != 0)
            {
                uint fileNumber = 3;
                for (int count = 0; count < fileNumber; count++)
                {
                    sFile newFile = new sFile();
                    newFile.name = "0" + count.ToString();
                    string ext = new string(cgx.id);

                    if (count == 0)
                    {
                        if (ext == "CG4 ")
                            newFile.name += ".TIL4";
                        else if (ext == "CG8 ")
                            newFile.name += ".TIL8";
                        newFile.offset = cgx.bitmapPointer;
                        newFile.size = cgx.bitmapSize;
                    }
                    else if (count == 1)
                    {
                        if (ext == "CG4 ")
                            newFile.name += ".P16";
                        else if (ext == "CG8 ")
                            newFile.name += ".P256";
                        newFile.offset = cgx.palettePointer;
                        newFile.size = cgx.paletteSize;
                    }
                    else if (count == 2)
                    {
                        newFile.name += ".CPOS";
                        newFile.offset = cgx.positionPointer;
                        newFile.size = file.size - cgx.positionPointer;
                    }

                    newFile.path = file.path;
                    unpacked.files.Add(newFile);
                }
            }

            else if (cgx.positionPointer == 0)
            {
                uint fileNumber = 2;
                for (int count = 0; count < fileNumber; count++)
                {
                    sFile newFile = new sFile();
                    newFile.name = "0" + count.ToString();
                    string ext = new string(cgx.id);

                    if (count == 0)
                    {
                        if (ext == "CG4 ")
                            newFile.name += ".TIL4";
                        else if (ext == "CG8 ")
                            newFile.name += ".TIL8";
                        newFile.offset = cgx.bitmapPointer;
                        newFile.size = cgx.bitmapSize;
                    }
                    else if (count == 1)
                    {
                        if (ext == "CG4 ")
                            newFile.name += ".P16";
                        else if(ext == "CG8 ")
                            newFile.name += ".P256";
                        newFile.offset = cgx.palettePointer;
                        newFile.size = cgx.paletteSize;
                    }

                    newFile.path = file.path;
                    unpacked.files.Add(newFile);
                }
            }

            if (transparency == true)
                Console.WriteLine("Transparency = True");
            else
                Console.WriteLine("Transparency = False");

            br.Close();
            sCGx = cgx;
            return unpacked;
        }

        public string Pack(sFile file, ref sFolder unpacked)
        {
            Unpack(file);
            string fileout = pluginHost.Get_TempFile();

            SaveCGx(file.path, fileout, ref unpacked);
            return fileout;
        }

        private void SaveCGx(string fileOG, string fileOut, ref sFolder decompressed)
        {        
            string byteArrayTMP = Path.GetTempFileName();
            Write_byteArray(byteArrayTMP, decompressed);

            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            bw.Write(sCGx.id);
            bw.Write(sCGx.u_Value);
            bw.Write(sCGx.mappingType);
            bw.Write(sCGx.NULL);
            bw.Write(sCGx.transparency);
            bw.Write(sCGx.bitmapSize);
            bw.Write(sCGx.paletteSize);

            uint tileNumber = sCGx.bitmapSize / 0x20;
            bw.Write(tileNumber);

            bw.Write(sCGx.objectType);
            bw.Write(sCGx.bitmapPointer);
            uint palettePointer = sCGx.bitmapPointer + sCGx.bitmapSize;
            bw.Write(palettePointer);
            if (decompressed.files.Count == 2)
            {
                uint positionPointer = 0x00000000;
                bw.Write(positionPointer);
            }
            else
            {
                uint positionPointer = sCGx.palettePointer + sCGx.paletteSize;
                bw.Write(positionPointer);
            }
            bw.Write(File.ReadAllBytes(byteArrayTMP));

            bw.Flush();
            bw.Close();

            File.Delete(byteArrayTMP);
        }

        private void Write_byteArray(string fileOut, sFolder decompressed)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));
            
            for (int fCount = 0; fCount < decompressed.files.Count; fCount++)
            {
                sFile newFile = Search_File(fCount + decompressed.id, decompressed);
                BinaryReader br = new BinaryReader(File.OpenRead(newFile.path));
                br.BaseStream.Position = newFile.offset;

                bw.Write(br.ReadBytes((int)newFile.size));

                br.Close();
                bw.Flush();
            }

            bw.Close();
            sCGx.bitmapSize = (uint)new FileInfo(fileOut).Length - 0x40;
            Console.WriteLine(fileOut.Length);
        }

        private sFile Search_File(int id, sFolder unpacked)
        {
            if (unpacked.files is List<sFile>)
                foreach (sFile archive in unpacked.files)
                    if (archive.id == id)
                        return archive;
            
            return new sFile();
        }
        
        public struct CGxS
        {
            public char[] id;
            public uint u_Value;
            public uint mappingType;
            public uint NULL;
            public uint transparency;

            public uint bitmapSize;
            public uint paletteSize;
            public uint tileNumber;

            public uint objectType;
            public uint bitmapPointer;
            public uint palettePointer;
            public uint positionPointer;
        }
    }

    public class CGT
    {
        IPluginHost pluginHost;

        public CGT(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
        }

        public sFolder Unpack(sFile file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file.path));

            char[] id = br.ReadChars(4);
            uint NULL1 = br.ReadUInt32();
            uint mappingType = br.ReadUInt32();
            uint NULL2 = br.ReadUInt32();

            uint pxFormat = br.ReadUInt32();
            uint bitmapSize = br.ReadUInt32();
            uint bitmapPointer = br.ReadUInt32();
            uint paletteSize = br.ReadUInt32();

            uint palettePointer = br.ReadUInt32();
            uint positionSize = br.ReadUInt32();
            uint positionPointer = br.ReadUInt32();
            uint NULL3 = br.ReadUInt32();

            br.BaseStream.Position = 0;
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            if (positionPointer != 0)
            {
                uint fileNumber = 3;
                for (int count = 0; count < fileNumber; count++)
                {
                    sFile newFile = new sFile();
                    newFile.name = "0" + count.ToString();

                    if (count == 0)
                    {
                        if (pxFormat == 0x01)
                            newFile.name += ".T3I5";
                        else if (pxFormat == 0x03)
                            newFile.name += ".TILT";
                        else if (pxFormat == 0x04)
                            newFile.name += ".TILT";
                        else if (pxFormat == 0x06)
                            newFile.name += ".T5I3";
                        newFile.offset = bitmapPointer;
                        newFile.size = bitmapSize;
                    }
                    else if (count == 1 && palettePointer != positionPointer)
                    {
                        if (pxFormat == 0x01)
                            newFile.name += ".P3I5";
                        else if (pxFormat == 0x03)
                            newFile.name += ".P16";
                        else if (pxFormat == 0x04)
                            newFile.name += ".P256";
                        else if (pxFormat == 0x06)
                            newFile.name += ".P5I3";
                        newFile.offset = palettePointer;
                        newFile.size = paletteSize;
                    }
                    else if (count == 1 && palettePointer == positionPointer)
                    {
                        newFile.name = ".dummy";
                        newFile.offset = palettePointer;
                        newFile.size = paletteSize;
                    }
                    else if (count == 2)
                    {
                        newFile.name += ".CPOS";
                        newFile.offset = positionPointer;
                        newFile.size = positionSize * 8;
                    }
                    newFile.path = file.path;

                    unpacked.files.Add(newFile);
                }
            }

            else if (positionPointer == 0)
            {
                uint fileNumber = 2;
                for (int count = 0; count < fileNumber; count++)
                {
                    sFile newFile = new sFile();
                    newFile.name = "0" + count.ToString();

                    if (count == 0)
                    {
                        newFile.name += ".TILT";
                        newFile.offset = bitmapPointer;
                        newFile.size = bitmapSize;
                    }
                    else if (count == 1)
                    {
                        if (pxFormat == 0x01)
                            newFile.name += ".P3I5";
                        else if (pxFormat == 0x03)
                            newFile.name += ".P16";
                        else if (pxFormat == 0x04)
                            newFile.name += ".P256";
                        else if (pxFormat == 0x06)
                            newFile.name += ".P5I3";
                        newFile.offset = palettePointer;
                        newFile.size = paletteSize;
                    }

                    newFile.path = file.path;

                    unpacked.files.Add(newFile);
                }
            }

            bool transparency = br.ReadUInt32() == 0x00 ? false : true;
            if (transparency == true)
                Console.WriteLine("Transparency = True");
            else
                Console.WriteLine("Transparency = False");

            br.Close();
            return unpacked;
        }
        public string Pack(sFile file, ref sFolder unpacked)
        {
            Unpack(file);
            string fileout = pluginHost.Get_TempFile();

            SaveCGT(file.path, fileout, ref unpacked);
            return fileout;
        }

        private void SaveCGT(string fileOG, string fileOut, ref sFolder unpacked)
        {
            throw new NotImplementedException();
        }

        public struct CGTs
        {
            public char[] id;
            public uint NULL1;
            public uint mappingType;
            public uint NULL2;

            public uint pxFormat;
            public uint bitmapSize;
            public uint bitmapPointer;
            public uint paletteSize;

            public uint palettePointer;
            public uint positionSize;
            public uint positionPointer;
            public uint NULL3;
        }
    }
}

