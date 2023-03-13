using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using Ekona;
using Ekona.Images;

namespace Images
{
    public class NCLR : PaletteBase
    {
        sNCLR nclr;

        public NCLR(string file, int id, string fileName = "") : base(file, id, fileName) { }

        public override void Read(string fileIn)
        {
            nclr = new sNCLR();

            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));

            // Generic header
            nclr.header.id = br.ReadChars(4);
            nclr.header.endianess = br.ReadUInt16();
            if (nclr.header.endianess == 0xFFFE)
                nclr.header.id.Reverse<char>();
            nclr.header.constant = br.ReadUInt16();
            nclr.header.file_size = br.ReadUInt32();
            nclr.header.header_size = br.ReadUInt16();
            nclr.header.nSection = br.ReadUInt16();

            // PLTT section
            TTLP pltt = new TTLP();

            pltt.ID = br.ReadChars(4);
            pltt.length = br.ReadUInt32();
            pltt.depth = (ColorFormat)br.ReadUInt16();
            pltt.unknown1 = br.ReadUInt16();
            pltt.unknown2 = br.ReadUInt32();

            pltt.pal_offset = br.ReadUInt32(); // Number of bytes for character data = pmcp.first_palette_num * pltt.num_colors * 2
            uint colors_startOffset = br.ReadUInt32();

            uint pal_length = pltt.length - 0x18;
            pltt.num_colors = (uint)((pltt.depth == ColorFormat.colors16) ? 0x10 : 0x100);
            pltt.palettes = new Color[pal_length / (pltt.num_colors * 2)][];

            br.BaseStream.Position = 0x18 + colors_startOffset;
            for (uint i = 0; i < pltt.palettes.Length; i++)
                pltt.palettes[i] = Actions.BGR555ToColor(br.ReadBytes((int)pltt.num_colors * 2));

            nclr.pltt = pltt;

            // PMCP section (Palette Compression Information Block)
            if (nclr.header.nSection == 1 || br.BaseStream.Position >= br.BaseStream.Length)
                goto End;

            PMCP pmcp = new PMCP();
            pmcp.ID = br.ReadChars(4);
            pmcp.blockSize = br.ReadUInt32();
            pmcp.pals_count = br.ReadUInt16();
            pmcp.padding = br.ReadUInt16();
            pmcp.data_offset = br.ReadUInt32();
            pmcp.palettes_nums = new ushort[pmcp.pals_count]; 
            for (int i = 0; i < pmcp.pals_count; i++) pmcp.palettes_nums[i] = br.ReadUInt16();
            nclr.pmcp = pmcp;

            // Decompress PMCP
            int last_num = pmcp.palettes_nums[pmcp.pals_count - 1];
            Color[][] fullPalette = new Color[last_num + 1][];
            for (int i = 0; i <= last_num; i++) fullPalette[i] = new Color[pltt.num_colors];
            for (int i = 0; i < pmcp.pals_count; i++) fullPalette[pmcp.palettes_nums[i]] = pltt.palettes[i];
            pltt.palettes = fullPalette;

        End:
            br.Close();
            Set_Palette(pltt.palettes, pltt.depth, true);
        }

        public override void Write(string fileOut)
        {
            Update_Struct();
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            bw.Write(nclr.header.id);
            bw.Write(nclr.header.endianess);
            bw.Write(nclr.header.constant);
            bw.Write(nclr.header.file_size);
            bw.Write(nclr.header.header_size);
            bw.Write(nclr.header.nSection);

            bw.Write(nclr.pltt.ID);
            bw.Write(nclr.pltt.length);
            bw.Write((ushort)(nclr.pltt.depth));
            bw.Write(nclr.pltt.unknown1);
            bw.Write(nclr.pltt.unknown2);
            bw.Write(nclr.pltt.pal_offset);
            bw.Write(0x10);                     // Colors start offset from 0x14

            if (nclr.pmcp.ID == null)
            {
                for (int i = 0; i < nclr.pltt.palettes.Length; i++)
                    bw.Write(Actions.ColorToBGR555(nclr.pltt.palettes[i]));
            }
            else
            {
                for (int i = 0; i < nclr.pmcp.palettes_nums.Length; i++)
                    bw.Write(Actions.ColorToBGR555(nclr.pltt.palettes[nclr.pmcp.palettes_nums[i]]));

                // Write PMCP
                bw.Write(nclr.pmcp.ID);
                bw.Write(nclr.pmcp.blockSize);
                bw.Write(nclr.pmcp.pals_count);
                bw.Write(nclr.pmcp.padding);
                bw.Write(nclr.pmcp.data_offset);
                for (int i = 0; i < nclr.pmcp.palettes_nums.Length; i++) bw.Write(nclr.pmcp.palettes_nums[i]);
                if (bw.BaseStream.Position % 4 != 0) bw.Write((ushort)0);
            }

            bw.Flush();
            bw.Close();
        }

        private void Update_Struct()
        {
            nclr.pltt.palettes = Palette;
            nclr.pltt.depth = Depth;
            uint pal_length = 0;
            if (nclr.pmcp.ID != null && nclr.pmcp.pals_count > 0)
            {
                for (int i = 0; i < nclr.pmcp.palettes_nums.Length; i++)
                    pal_length += (uint)(nclr.pltt.palettes[nclr.pmcp.palettes_nums[i]].Length * 2);
                nclr.pltt.length = pal_length + 0x18;
                nclr.header.file_size = nclr.pltt.length + 0x10;
                nclr.header.file_size += (uint)(2 * nclr.pmcp.pals_count + 0x10);
            }
            else
            {
                for (int i = 0; i < nclr.pltt.palettes.Length; i++)
                    pal_length += (uint)(nclr.pltt.palettes[i].Length * 2);
                nclr.pltt.length = pal_length + 0x18;
                nclr.header.file_size = nclr.pltt.length + 0x10;
            }
        }

        public struct sNCLR      // Nintendo CoLor Resource
        {
            public NitroHeader header;
            public TTLP pltt;
            public PMCP pmcp;
        }
        public struct TTLP  // PaLeTTe
        {
            public char[] ID;
            public UInt32 length;
            public ColorFormat depth;
            public UInt16 unknown1;
            public UInt32 unknown2;    // padding?
            public UInt32 pal_offset;
            public UInt32 num_colors;    // Number of colors
            public Color[][] palettes;
        }
        public struct PMCP
        {
            public char[] ID;
            public uint blockSize;
            public ushort pals_count;
            public ushort padding;     // always BEEF?
            public uint data_offset;
            public ushort[] palettes_nums;
        }
    }
}
