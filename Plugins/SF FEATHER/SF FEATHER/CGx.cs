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
using static SF_FEATHER.CGx;
using static SF_FEATHER.SCx;

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
            cgx.u_Value2 = br.ReadUInt32();

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

            br.Close();
            sCGx = cgx;
            return unpacked;
        }

        public string Pack(sFile file, ref sFolder unpacked)
        {
            Unpack(file);
            string fileout = pluginHost.Get_TempFile();

            SaveCGx(fileout, ref unpacked);
            return fileout;
        }

        private void SaveCGx(string fileOut, ref sFolder decompressed)
        {        
            string byteArrayTMP = Path.GetTempFileName();
            Write_byteArray(byteArrayTMP, decompressed);

            uint palettePointer = sCGx.bitmapPointer + sCGx.bitmapSize;
            uint positionSize = sCGx.bitmapSize - (palettePointer + sCGx.paletteSize);
            uint bitmapSize = sCGx.bitmapSize - (positionSize - sCGx.paletteSize - sCGx.bitmapPointer);

            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            bw.Write(sCGx.id);
            bw.Write(sCGx.u_Value);
            bw.Write(sCGx.mappingType);
            bw.Write(sCGx.NULL);
            bw.Write(sCGx.u_Value2);

            bw.Write(bitmapSize);
            bw.Write(sCGx.paletteSize);

            uint tileNumber = sCGx.bitmapSize / 0x20;
            bw.Write(tileNumber);

            bw.Write(sCGx.objectType);
            bw.Write(sCGx.bitmapPointer);

            bw.Write(palettePointer);
            if (decompressed.files.Count == 2)
            {
                uint positionPointer = 0x00000000;
                bw.Write(positionPointer);
            }
            else
            {
                uint positionPointer = palettePointer + sCGx.paletteSize;
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
            sCGx.bitmapSize = (uint)new FileInfo(fileOut).Length;
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
            public uint u_Value2;

            public uint bitmapSize;
            public uint paletteSize;
            public uint tileNumber;

            public uint objectType;
            public uint bitmapPointer;
            public uint palettePointer;
            public uint positionPointer;
        }
    }
}