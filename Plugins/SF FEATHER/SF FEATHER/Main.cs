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
using System.IO;
using System.Text;
using Ekona;
using Ekona.Images;
using static SF_FEATHER.CGT;

namespace SF_FEATHER
{
    public class Main : IGamePlugin
    {

        IPluginHost pluginHost;
        string gameCode;

        public void Initialize(IPluginHost pluginHost, string gameCode)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
        }
        public bool IsCompatible()
        {
            if (gameCode == "CS4J")
            {
                return true;
            }
            return false;
        }

        public Format Get_Format(sFile file, byte[] mSeed)
        {
            string ext = new string(Encoding.ASCII.GetChars(mSeed));

            if (file.name.EndsWith(".pac") && (mSeed[2] == 0x01 && mSeed[3] == 0x00))
                return Format.Pack;
            if (ext == "CG4 " || ext == "CG8 ")
                return Format.Pack;
            else if (ext == "SC4 " || ext == "SC8 ")
                return Format.Map;
            else if (ext == "CGT ")
                return Format.Pack;
            else if (file.name.EndsWith(".TIL4"))
                return Format.Tile;
            else if (file.name.EndsWith(".TIL8"))
                return Format.Tile;
            else if (file.name.EndsWith(".TILT"))
                return Format.Tile;
            else if (file.name.EndsWith(".T3I5"))
                return Format.Tile;
            else if (file.name.EndsWith(".T5I3"))
                return Format.Tile;
            else if (file.name.EndsWith(".P16"))
                return Format.Palette;
            else if (file.name.EndsWith(".P256"))
                return Format.Palette;
            else if (file.name.EndsWith(".P3I5"))
                return Format.Palette;
            else if (file.name.EndsWith(".P5I3"))
                return Format.Palette;
            else if (file.name.EndsWith(".CPOS"))
                return Format.Cell;
            else if (ext == "PSI3")
                return Format.Script;
            else if (file.name.EndsWith(".BIT"))
                return Format.Font;
            else if (ext == "ANT ")
                return Format.Texture;
            else if (ext == "ANP ")
                return Format.Animation;
            else if (ext == "MBG " || ext == "ABG " || ext == "HBG ")
                return Format.Model3D;

            return Format.Unknown;
        }

        public string Pack(ref sFolder unpacked, sFile file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file.path));
            string ext = new string(br.ReadChars(4));
            br.Close();

            if (file.name.EndsWith(".pac"))
            {
                return new PAC(pluginHost).Pack(file, ref unpacked);
            }

            else if (ext == "CG4 " || ext == "CG8 ")
            {
                return new CGx(pluginHost).Pack(file, ref unpacked);
            }

            else if (ext == "CGT ")
            {
                return new CGT(pluginHost).Pack(file, ref unpacked);
            }
            return null;
        }

        public sFolder Unpack(sFile file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file.path));
            string ext = new string(br.ReadChars(4));
            br.Close();
            if (file.name.EndsWith(".pac"))
            {
                return new PAC(pluginHost).Unpack(file);
            }
            else if (ext == "CG4 " || ext == "CG8 ")
            {
                return new CGx(pluginHost).Unpack(file);
            }
            else if (ext == "CGT ")
            {
                return new CGT(pluginHost).Unpack(file);
            }
            return new sFolder();
        }

        public void Read(sFile file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file.path));
            string ext = new string(Encoding.ASCII.GetChars(br.ReadBytes(4)));
            br.Close();

            if (ext == "SC4 " || ext == "SC8 ")
            {
                SCx scx = new SCx(file.path, file.id, file.name);
                pluginHost.Set_Map(scx);
            }
            else
            {
                Helper.ReadRAW(file, pluginHost);
            }
        }
        public System.Windows.Forms.Control Show_Info(sFile file)
        {
            Read(file);

            BinaryReader br = new BinaryReader(File.OpenRead(file.path));
            string ext = new string(Encoding.ASCII.GetChars(br.ReadBytes(4)));
            br.Close();

            if (ext == "SC4 " || ext == "SC8 ")
                return new ImageControl(pluginHost, true);

            Format format = Helper.ReadRAW(file, pluginHost);

            if (format == Format.Palette)
                return new PaletteControl(pluginHost, pluginHost.Get_Palette());

            if (format == Format.Tile && pluginHost.Get_Palette().Loaded)
                return new ImageControl(pluginHost, pluginHost.Get_Image(), pluginHost.Get_Palette());

            return new System.Windows.Forms.Control();
        }
    }
}
