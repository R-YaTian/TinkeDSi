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
 * By: pleoNeX
 * Update: mn1712trungson
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Ekona;

namespace SF_FEATHER
{
    public static class PAC
    {

        public static sFolder Unpack(string file, string name)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            ushort fileNumber = br.ReadUInt16();
            ushort uValue1 = br.ReadUInt16();
            ushort uValue2 = br.ReadUInt16();
            ushort padding = br.ReadUInt16();

            for (int fCount = 0; fCount < fileNumber; fCount++)
            {
                sFile newFile = new sFile();
                newFile.name = name + '_' + fCount.ToString();
                newFile.offset = br.ReadUInt32() * 0x10;
                newFile.size = br.ReadUInt32() * 0x10;
                newFile.path = file;

                // Extension check
                if (newFile.size != 0x00)
                {
                    bool compressed = false;

                    // Check if this file is pac, it searches the extension
                    long currentPos = br.BaseStream.Position;
                    br.BaseStream.Position = newFile.offset;
                    byte typeLZ = br.ReadByte();
                    uint compressedSize = br.ReadUInt32();
                    if ((typeLZ == 0x11 || typeLZ == 0x10) && compressedSize < 0x2000000)
                        compressed = true;

                    // Search the indicator of the pac file
                    if (compressed)
                        br.BaseStream.Position = newFile.offset + 9;
                    else
                        br.BaseStream.Position = newFile.offset + 4;
                    uint fileType = br.ReadUInt32();

                    if (fileType == 0x04)
                        newFile.name += ".pac";
                    else
                    {
                        // Search for the header extension
                        if (compressed)
                            br.BaseStream.Position = newFile.offset + 5;
                        else
                            br.BaseStream.Position = newFile.offset;
                           
                        fileType = br.ReadUInt32();
                        char[] ext = Encoding.ASCII.GetChars(BitConverter.GetBytes(fileType));
                        String extS = ".";
                        for (int sCount = 0; sCount < 4; sCount++)
                            if (Char.IsLetterOrDigit(ext[sCount]) || ext[sCount] == 0x20)
                                extS += ext[sCount];

                        if (extS != "." && extS.Length == 5)
                            newFile.name += extS;
                        else
                            newFile.name += ".bin";
                    }

                    br.BaseStream.Position = currentPos;
                }
                else
                    continue;

                unpacked.files.Add(newFile);
            }            

            br.Close();
            return unpacked;
        }

        public static void Pack(string fileOG, string fileOut, ref sFolder unpacked)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileOG));
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));
            List<byte> buffer = new List<byte>();


            ushort fileNumber = br.ReadUInt16();
            bw.Write(fileNumber);
            //uValue1, uValue2, padding
            bw.Write(br.ReadUInt16());
            bw.Write(br.ReadUInt16());
            bw.Write(br.ReadUInt16());

            uint fOffset = (uint)fileNumber * 8 + 8;
            uint fSize;
            int unpackedPointer = 0;  // Pointer to the unpacked.files array

            // Write the final padding of the dict section
            if (fOffset % 0x10 != 0)
            {
                for (int dictPadding = 0; dictPadding < 0x10 - (fOffset % 0x10); dictPadding++)
                    buffer.Add(0x00);

                fOffset += 0x10 - (fOffset % 0x10);
            }


            for (int dictCount = 0; dictCount < fileNumber; dictCount++)
            {
                uint older_offset = br.ReadUInt32();
                fSize = br.ReadUInt32();

                // If it's a null file
                if (fSize == 0)
                {
                    bw.Write(older_offset);
                    bw.Write(fSize);
                    continue;
                }

                // Get a normalized size
                fSize = unpacked.files[unpackedPointer].size;
                if (fSize % 0x10 != 0)
                    fSize += 0x10 - (fSize % 0x10);

                // Write the FAT section
                bw.Write((uint)(fOffset / 0x10));
                bw.Write((uint)(fSize / 0x10));

                // Write file
                BinaryReader fileRead = new BinaryReader(File.OpenRead(unpacked.files[unpackedPointer].path));
                fileRead.BaseStream.Position = unpacked.files[unpackedPointer].offset;
                buffer.AddRange(fileRead.ReadBytes((int)unpacked.files[unpackedPointer].size));                
                fileRead.Close();

                // Write the padding
                for (int fPadding = 0; fPadding < (fSize - unpacked.files[unpackedPointer].size); fPadding++)
                    buffer.Add(0x00);

                // Set the new offset
                sFile newFile = unpacked.files[unpackedPointer];
                newFile.offset = fOffset;
                newFile.path = fileOut;
                unpacked.files[unpackedPointer] = newFile;

                // Set new offset
                fOffset += fSize;
                unpackedPointer++;
            }
            bw.Flush();

            bw.Write(buffer.ToArray());     

            br.Close();
            bw.Flush();
            bw.Close();
        }
    }
}
