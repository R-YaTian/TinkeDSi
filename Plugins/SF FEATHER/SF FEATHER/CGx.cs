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
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using Ekona;
using Ekona.Images;
using static System.Net.Mime.MediaTypeNames;

namespace SF_FEATHER
{
    public static class CGx
    {
        public static sFolder Unpack(sFile file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file.path));

            char[] id = br.ReadChars(4);
            uint u_Value = br.ReadUInt32();
            uint mappingType = br.ReadUInt32();
            uint NULL = br.ReadUInt32();

            uint transparency = br.ReadUInt32();
            uint bitmapSize = br.ReadUInt32();
            uint paletteSize = br.ReadUInt32();
            uint tileNumber = br.ReadUInt32();

            uint objectType = br.ReadUInt32();
            uint bitmapPointer = br.ReadUInt32();
            uint palettePointer = br.ReadUInt32();
            uint positionPointer = br.ReadUInt32();

            sFolder unpack = new sFolder();
            unpack.files = new List<sFile>();

            if (positionPointer != 0)
            {
                uint fileNumber = 3;
                for (int count = 0; count < fileNumber; count++)
                {
                    sFile newFile = new sFile();
                    newFile.name = file.name + '_' + count.ToString();
                    string ext = new string(id);

                    if (count == 0)
                    {
                        if (ext == "CG4 ")
                            newFile.name += ".TIL4";
                        else if (ext == "CG8 ")
                            newFile.name += ".TIL8";
                        newFile.offset = bitmapPointer;
                        newFile.size = bitmapSize;
                    }
                    else if (count == 1)
                    {
                        if (ext == "CG4 ")
                            newFile.name += ".P16";
                        else if (ext == "CG8 ")
                            newFile.name += ".P256";
                        newFile.offset = palettePointer;
                        newFile.size = paletteSize;
                    }
                    else if (count == 2)
                    {
                        newFile.name += ".CPOS";
                        newFile.offset = positionPointer;
                        newFile.size = 0x8;
                    }

                    newFile.path = file.path;
                    unpack.files.Add(newFile);
                }
            }

            else if (positionPointer == 0)
            {
                uint fileNumber = 2;
                for (int count = 0; count < fileNumber; count++)
                {
                    sFile newFile = new sFile();
                    newFile.name = file.name + '_' + count.ToString();
                    string ext = new string(id);

                    if (count == 0)
                    {
                        if (ext == "CG4 ")
                            newFile.name += ".TIL4";
                        else if (ext == "CG8 ")
                            newFile.name += ".TIL8";
                        newFile.offset = bitmapPointer;
                        newFile.size = bitmapSize;
                    }
                    else if (count == 1)
                    {
                        if (ext == "CG4 ")
                            newFile.name += ".P16";
                        else if(ext == "CG8 ")
                            newFile.name += ".P256";
                        newFile.offset = palettePointer;
                        newFile.size = paletteSize;
                    }

                    newFile.path = file.path;
                    unpack.files.Add(newFile);
                }
            }

            br.Close();
            return unpack;
        }

        public static void Pack(string fileOG, string fileOut, ref sFolder unpacked)
        {
            throw new NotImplementedException();
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

            public byte[] bitmapArray;
        }
    }

    public static class CGT
    {
        public static sFolder Unpack(sFile file)
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
            sFolder unpack = new sFolder();
            unpack.files = new List<sFile>();

            if (positionPointer != 0)
            {
                uint fileNumber = 3;
                for (int count = 0; count < fileNumber; count++)
                {
                    sFile newFile = new sFile();
                    newFile.name = file.name + '_' + count.ToString();

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

                    unpack.files.Add(newFile);
                }
            }

            else if (positionPointer == 0)
            {
                uint fileNumber = 2;
                for (int count = 0; count < fileNumber; count++)
                {
                    sFile newFile = new sFile();
                    newFile.name = file.name + '_' + count.ToString();

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

                    unpack.files.Add(newFile);
                }
            }

            br.Close();
            return unpack;
        }
        public static void Pack(string fileOG, string fileOut, ref sFolder unpacked)
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

