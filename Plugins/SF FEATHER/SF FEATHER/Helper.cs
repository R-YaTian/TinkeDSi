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

using Ekona.Images;
using Ekona;
using System;
using System.Drawing;
using System.IO;

namespace SF_FEATHER
{
    public class Helper
    {
        public static Format ReadRAW(sFile file, IPluginHost pluginHost)
        {
            //Palette
            if (file.name.EndsWith(".P3I5"))
            {
                RawPalette palette = new RawPalette(file.path, file.id, true, ColorFormat.A3I5, 0, -1, file.name);
                pluginHost.Set_Palette(palette);
                return Format.Palette;
            }
            else if (file.name.EndsWith(".P5I3"))
            {
                RawPalette palette = new RawPalette(file.path, file.id, true, ColorFormat.A5I3, 0, -1, file.name);
                pluginHost.Set_Palette(palette);
                return Format.Palette;
            }
            else if (file.name.EndsWith(".P16"))
            {
                RawPalette palette = new RawPalette(file.path, file.id, true, ColorFormat.colors16, 0, -1, file.name);
                pluginHost.Set_Palette(palette);
                return Format.Palette;
            }
            else if (file.name.EndsWith(".P256"))
            {
                BinaryReader br = new BinaryReader(File.OpenRead(file.path));
                int palette_length = 0x200;
                Color[][] palette = new Color[1][];
                for (int i = 0; i < palette.Length; i++)
                    palette[i] = Actions.BGR555ToColor(br.ReadBytes(palette_length));

                br.Close();

                RawPalette pb = new RawPalette(palette, true, ColorFormat.colors256);
                pluginHost.Set_Palette(pb);
                return Format.Palette;
            }

            //Tile
            ColorFormat depth;
            depth = ColorFormat.direct;

            if (pluginHost.Get_Palette().Loaded)
                depth = pluginHost.Get_Palette().Depth;

            if (file.name.EndsWith(".TIL8"))
            {
                BinaryReader br = new BinaryReader(File.OpenRead(file.path));
                byte[] data = br.ReadBytes((int)file.size);

                br.Close();

                RawImage image = new RawImage(data, TileForm.Horizontal, ColorFormat.colors256, 0x8, data.Length / 8, true, file.name);
                pluginHost.Set_Image(image);
                return Format.Tile;
            }

            else if (file.name.EndsWith(".TIL4"))
            {
                BinaryReader br = new BinaryReader(File.OpenRead(file.path));
                byte[] data = br.ReadBytes((int)file.size);

                br.Close();

                RawImage image = new RawImage(data, TileForm.Horizontal, ColorFormat.colors16, 0x8, data.Length / 4, true, file.name);
                pluginHost.Set_Image(image);
                return Format.Tile;
            }

            else if (file.name.EndsWith(".TILT"))
            {
                BinaryReader br = new BinaryReader(File.OpenRead(file.path));
                uint transparency = br.ReadUInt32();

                int width = (int)Math.Pow(2, br.ReadUInt16() + 3);
                int height = (int)Math.Pow(2, br.ReadUInt16() + 3);
                uint bitmapArrayPointer = br.ReadUInt32();

                br.Close();

                RawImage image = new RawImage(file.path, file.id, TileForm.Lineal, depth, width, height, true, (int)bitmapArrayPointer, (int)(file.size - bitmapArrayPointer), file.name);
                pluginHost.Set_Image(image);
                return Format.Tile;
            }

            else if (file.name.EndsWith(".T3I5"))
            {
                BinaryReader br = new BinaryReader(File.OpenRead(file.path));
                uint transparency = br.ReadUInt32();

                int width = (int)Math.Pow(2, br.ReadUInt16() + 3);
                int height = (int)Math.Pow(2, br.ReadUInt16() + 3);
                uint bitmapArrayPointer = br.ReadUInt32();

                RawImage image = new RawImage(file.path, file.id, TileForm.Lineal, ColorFormat.A3I5, width, height, true, (int)bitmapArrayPointer, (int)(file.size - bitmapArrayPointer), file.name);
                pluginHost.Set_Image(image);
                return Format.Tile;
            }
            else if (file.name.EndsWith(".T5I3"))
            {
                BinaryReader br = new BinaryReader(File.OpenRead(file.path));
                uint transparency = br.ReadUInt32();

                int width = (int)Math.Pow(2, br.ReadUInt16() + 3);
                int height = (int)Math.Pow(2, br.ReadUInt16() + 3);
                uint bitmapArrayPointer = br.ReadUInt32();

                br.Close();

                RawImage image = new RawImage(file.path, file.id, TileForm.Lineal, ColorFormat.A5I3, width, height, true, (int)bitmapArrayPointer, (int)(file.size - bitmapArrayPointer), file.name);
                pluginHost.Set_Image(image);
                return Format.Tile;
            }

            return Format.Unknown;
        }
    }
}
