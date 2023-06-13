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
 */
using System.IO;
using Ekona;
using Ekona.Images;

namespace SF_FEATHER
{
    public class SCx : MapBase
    {
        sSCx typeSC;

        public SCx(string file, int id, string fileName = "") : base(file, id, fileName) { }

        public override void Read(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            typeSC = new sSCx();

            typeSC.mSeed = br.ReadChars(4);
            typeSC.u_Value = br.ReadUInt32();
            typeSC.mappingType = br.ReadUInt32();
            typeSC.NULL = br.ReadUInt32();
            typeSC.u_Value2 = br.ReadUInt32();

            ushort width = br.ReadUInt16();
            ushort height = br.ReadUInt16();
            typeSC.mapSize1 = br.ReadUInt32();
            typeSC.mapSize2 = br.ReadUInt32();

            NTFS[] map = new NTFS[typeSC.mapSize2 / 2];
            for (int i = 0; i < map.Length; i++)
                map[i] = Actions.MapInfo(br.ReadUInt16());

            br.Close();
            Set_Map(map, false, width, height);
        }
        public override void Write(string fileOut, ImageBase image, PaletteBase palette)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            bw.Write(typeSC.mSeed);
            bw.Write(typeSC.u_Value);
            bw.Write(typeSC.mappingType);
            bw.Write(typeSC.NULL);
            bw.Write(typeSC.u_Value2);

            bw.Write((ushort)Width);
            bw.Write((ushort)Height);
            bw.Write(Map.Length * 2);
            bw.Write(Map.Length * 2);

            for (int i = 0; i < Map.Length; i++)
                bw.Write(Actions.MapInfo(Map[i]));

            bw.Flush();
            bw.Close();
        }

        public struct sSCx
        {
            public char[] mSeed;
            public uint u_Value;
            public uint mappingType;
            public uint NULL;
            public uint u_Value2;

            public uint mapSize1;
            public uint mapSize2;
        }
    }
}
