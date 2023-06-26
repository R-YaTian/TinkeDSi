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

using Ekona;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF_FEATHER
{
    public class CGT
    {
        IPluginHost pluginHost;
        CGTs sCGT;
        public CGT(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
        }

        public sFolder Unpack(sFile file)
        {
            CGTs cgt = new CGTs();
            BinaryReader br = new BinaryReader(File.OpenRead(file.path));

            cgt.id = br.ReadChars(4);
            cgt.NULL1 = br.ReadUInt32();
            cgt.mappingType = br.ReadUInt32();
            cgt.NULL2 = br.ReadUInt32();

            cgt.pxFormat = br.ReadUInt32();
            cgt.bitmapSize = br.ReadUInt32();
            cgt.bitmapPointer = br.ReadUInt32();
            cgt.paletteSize = br.ReadUInt32();

            cgt.palettePointer = br.ReadUInt32();
            cgt.positionSize = br.ReadUInt32();
            cgt.positionPointer = br.ReadUInt32();
            cgt.NULL3 = br.ReadUInt32();

            br.BaseStream.Position = 0;
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            if (cgt.positionPointer != 0)
            {
                uint fileNumber = 3;
                for (int count = 0; count < fileNumber; count++)
                {
                    sFile newFile = new sFile();
                    newFile.name = "0" + count.ToString();

                    if (count == 0)
                    {
                        if (cgt.pxFormat == 0x01)
                            newFile.name += ".T3I5";
                        else if (cgt.pxFormat == 0x03)
                            newFile.name += ".TILT";
                        else if (cgt.pxFormat == 0x04)
                            newFile.name += ".TILT";
                        else if (cgt.pxFormat == 0x06)
                            newFile.name += ".T5I3";
                        else if (cgt.pxFormat == 0x07)
                            newFile.name += ".TILT";
                        newFile.offset = cgt.bitmapPointer;
                        newFile.size = cgt.bitmapSize + 0xC;
                    }
                    else if (count == 1)
                    {
                        if (cgt.pxFormat == 0x01)
                            newFile.name += ".P3I5";
                        else if (cgt.pxFormat == 0x03)
                            newFile.name += ".P16";
                        else if (cgt.pxFormat == 0x04)
                            newFile.name += ".P256";
                        else if (cgt.pxFormat == 0x06)
                            newFile.name += ".P5I3";
                        else if (cgt.pxFormat == 0x07 && cgt.palettePointer == cgt.positionPointer)
                            newFile.name = ".dummy";
                        newFile.offset = cgt.palettePointer;
                        newFile.size = cgt.paletteSize;
                    }
                    else if (count == 2)
                    {
                        newFile.name += ".CPOS";
                        newFile.offset = cgt.positionPointer;
                        newFile.size = file.size - cgt.positionPointer;
                    }

                    newFile.path = file.path;
                    unpacked.files.Add(newFile);
                }
            }

            else if (cgt.positionPointer == 0)
            {
                uint fileNumber = 2;
                for (int count = 0; count < fileNumber; count++)
                {
                    sFile newFile = new sFile();
                    newFile.name = "0" + count.ToString();

                    if (count == 0)
                    {
                        newFile.name += ".TILT";
                        newFile.offset = cgt.bitmapPointer;
                        newFile.size = cgt.bitmapSize + 0xC;
                    }
                    else if (count == 1)
                    {
                        if (cgt.pxFormat == 0x01)
                            newFile.name += ".P3I5";
                        else if (cgt.pxFormat == 0x03)
                            newFile.name += ".P16";
                        else if (cgt.pxFormat == 0x04)
                            newFile.name += ".P256";
                        else if (cgt.pxFormat == 0x06)
                            newFile.name += ".P5I3";
                        else if (cgt.pxFormat == 0x07 && cgt.palettePointer == cgt.positionPointer)
                            newFile.name = ".dummy";
                        newFile.offset = cgt.palettePointer;
                        newFile.size = cgt.paletteSize;
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
            sCGT = cgt;
            return unpacked;
        }

        public string Pack(sFile file, ref sFolder unpacked)
        {
            Unpack(file);
            string fileout = pluginHost.Get_TempFile();

            SaveCGT(fileout, ref unpacked);
            return fileout;
        }

        private void SaveCGT(string fileOut, ref sFolder decompressed)
        {
            string byteArrayTMP = Path.GetTempFileName();
            Write_byteArray(byteArrayTMP, decompressed);

            uint bitmapSize = sCGT.bitmapSize - sCGT.paletteSize - sCGT.positionSize*8 - 0xC;

            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            bw.Write(sCGT.id);
            bw.Write(sCGT.NULL1);
            bw.Write(sCGT.mappingType);
            bw.Write(sCGT.NULL2);
            bw.Write(sCGT.pxFormat);

            bw.Write(bitmapSize);
            bw.Write(sCGT.bitmapPointer);
            bw.Write(sCGT.paletteSize);
            uint palettePointer = bitmapSize + sCGT.bitmapPointer + 0xC;
            bw.Write(palettePointer);
            bw.Write(sCGT.positionSize);
            uint positionPointer = 0;
            if (sCGT.pxFormat == 0x07)
                positionPointer += palettePointer;
            else
                positionPointer += palettePointer + sCGT.paletteSize;
            bw.Write(positionPointer);
            
            bw.Write(sCGT.NULL3);

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

            sCGT.bitmapSize = (uint)new FileInfo(fileOut).Length;
            bw.Close();
        }

        private sFile Search_File(int id, sFolder unpacked)
        {
            if (unpacked.files is List<sFile>)
                foreach (sFile archive in unpacked.files)
                    if (archive.id == id)
                        return archive;

            return new sFile();
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