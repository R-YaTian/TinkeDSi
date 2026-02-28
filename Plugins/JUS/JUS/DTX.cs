// ----------------------------------------------------------------------
// <copyright file="DTX.cs" company="none">

// Copyright (C) 2019
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// </copyright>

// <author>Priverop</author>
// <contact>https://github.com/priverop/</contact>
// <date>03/10/2025</date>
// -----------------------------------------------------------------------
using System;
using System.Drawing;
using System.IO;
using Ekona;
using Ekona.Images;

namespace JUS
{
    class DTX : ImageBase
    {
        IPluginHost pluginHost;

        public DTX(IPluginHost pluginHost, string file, int id, string fileName = "")
        {
            this.pluginHost = pluginHost;
            this.id = id;
            this.fileName = fileName;

            Read(file);
        }

        public override void Read(string fileIn)
        {
            byte[] data = File.ReadAllBytes(fileIn);

            // The DTX file has a DSIG inside, so we get its offset first
            int dsigStart = (int)BitConverter.ToInt16(data, 8);
            Console.WriteLine("dsigStart: " + dsigStart);

            // Then we will add the position of the DSIG to every position
            byte formatType = data[5 + dsigStart];
            byte numPaletteLines = data[6 + dsigStart];

            short width = BitConverter.ToInt16(data, 8 + dsigStart);
            short height = BitConverter.ToInt16(data, 10 + dsigStart);

            int paletteEnd = numPaletteLines * 32 + 12;

            int startPalette = 12 + dsigStart;
            Console.WriteLine("startPalette: " + startPalette);
            int position = startPalette;

            int paletteSize = paletteEnd - startPalette;

            ColorFormat format;
            RawPalette palette;
            Color[][] palettes;

            if ((formatType & 0x0F) == 0)
            {
                format = ColorFormat.colors16;
                palettes = new Color[numPaletteLines][];

                for (int i = 0; i < numPaletteLines; i++)
                {
                    byte[] aux = new byte[0x20];
                    Array.Copy(data, startPalette + (i * 0x20), aux, 0, 0x20);

                    palettes[i] = Actions.BGR555ToColor(aux);
                }

                palette = new RawPalette(palettes, false, format);
            }
            else
            {
                format = ColorFormat.colors256;
                int numPalettes = ((numPaletteLines - 1) / 16) + 1;

                palettes = new Color[numPalettes][];

                for (int i = 0; i < numPalettes; i++)
                {
                    byte[] aux = new byte[0x200];
                    Array.Copy(data, startPalette + (i * 0x200), aux, 0, 0x200);
                    palettes[i] = Actions.BGR555ToColor(aux);
                }

                palette = new RawPalette(palettes, false, format);
            }

            pluginHost.Set_Palette(palette);

            // Get image
            byte[] tiles = new byte[data.Length - paletteEnd];
            Array.Copy(data, paletteEnd, tiles, 0, data.Length - paletteEnd);
            Set_Tiles(
                tiles,
                width,
                height,
                format,
                formatType >> 4 == 1 ? TileForm.Horizontal : TileForm.Lineal,
                false
            );

            pluginHost.Set_Image(this);
        }

        public override void Write(string fileOut, PaletteBase palette) { }
    }
}
