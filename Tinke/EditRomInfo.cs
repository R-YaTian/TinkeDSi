// ----------------------------------------------------------------------
// <copyright file="EditRomInfo.cs" company="none">

// Copyright (C) 2012
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

// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>28/04/2012 14:25:12</date>
// -----------------------------------------------------------------------
using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
using System.Drawing;
using System.Linq;
//using System.Text;
using System.Windows.Forms;
using System.IO;
using Ekona.Helper;

namespace Tinke
{
    public partial class EditRomInfo : Form
    {
        Nitro.Estructuras.ROMHeader header;
        Nitro.Estructuras.Banner banner;

        public EditRomInfo()
        {
            InitializeComponent();

            this.Font = new Font(
                this.Font.FontFamily,
                9F,
                this.Font.Style,
                GraphicsUnit.Point
            );
        }

        public EditRomInfo(Nitro.Estructuras.ROMHeader header, Nitro.Estructuras.Banner banner)
        {
            InitializeComponent();

            this.Font = new Font(
                this.Font.FontFamily,
                9F,
                this.Font.Style,
                GraphicsUnit.Point
            );

            ReadLanguage();

            this.header = header;
            this.banner = banner;
            LoadValues();
            if (banner.version >= 2)
                this.comboBanTitles.Items.Add(Tools.Helper.GetTranslation("EditRomInfo", "S2B"));
            if (banner.version >= 3)
                this.comboBanTitles.Items.Add(Tools.Helper.GetTranslation("EditRomInfo", "S2C"));
            comboBanTitles.SelectedIndex = 0;
            if (banner.version > 3)
                this.btnImportAdata.Enabled = true;
            if ((header.unitCode & 2) > 0 && (header.twlInternalFlags & 1) > 0 && Sistema.twl_flag != true)
                this.btnImportiheader.Enabled = true;
        }
        private void ReadLanguage()
        {
            System.Xml.Linq.XElement xml = Tools.Helper.GetTranslation("EditRomInfo");

            this.Text = xml.Element("S00").Value;
            btnSave.Text = xml.Element("S01").Value;
            btnCancel.Text = xml.Element("S02").Value;
            groupHeader.Text = xml.Element("S03").Value;
            groupBanner.Text = xml.Element("S04").Value;
            btnImage.Text = xml.Element("S05").Value;
            btnImportAdata.Text = xml.Element("S2D").Value;
            btnImportiheader.Text = xml.Element("S2E").Value;
            lblBanVer.Text = xml.Element("S06").Value;
            lblBanTitles.Text = xml.Element("S07").Value;
            comboBanTitles.Items[0] = xml.Element("S08").Value;
            comboBanTitles.Items[1] = xml.Element("S09").Value;
            comboBanTitles.Items[2] = xml.Element("S0A").Value;
            comboBanTitles.Items[3] = xml.Element("S0B").Value;
            comboBanTitles.Items[4] = xml.Element("S0C").Value;
            comboBanTitles.Items[5] = xml.Element("S0D").Value;
            lblBanReserved.Text = xml.Element("S0E").Value;
            lblGameTitle.Text = xml.Element("S0F").Value;
            lblGameCode.Text = xml.Element("S10").Value;
            lblMakerCode.Text = xml.Element("S11").Value;
            lblUnitCode.Text = xml.Element("S12").Value;
            lblReserved.Text = xml.Element("S13").Value;
            lblROMVer.Text = xml.Element("S14").Value;
            lblInternalFlag.Text = xml.Element("S15").Value;
            lblArm9Entry.Text = xml.Element("S16").Value;
            lblArm9Ram.Text = xml.Element("S17").Value;
            lblArm7Entry.Text = xml.Element("S18").Value;
            lblArm7Ram.Text = xml.Element("S19").Value;
            lblFlagsRead.Text = xml.Element("S1A").Value;
            lblFlagsInit.Text = xml.Element("S1B").Value;
            btnShowMakerCode.Text = xml.Element("S1C").Value;
            lblRomTimeout.Text = xml.Element("S1D").Value;
            lblSecureDisable.Text = xml.Element("S1E").Value;
            lblReserved3.Text = xml.Element("S1F").Value;
            lblReserved2.Text = xml.Element("S20").Value;
            lblArm9Autoload.Text = xml.Element("S21").Value;
            lblArm7Autoload.Text = xml.Element("S22").Value;
            lblDebugRomOffset.Text = xml.Element("S23").Value;
            lblDebugSize.Text = xml.Element("S24").Value;
            lblDebugRamAddress.Text = xml.Element("S25").Value;
            lblEncryptionSeed.Text = xml.Element("S27").Value;
        }

        private void LoadValues()
        {
            #region Set banner values
            numericBanVer.Value = banner.version;
            txtBanReserved.Text = BitConverter.ToString(banner.reserved);
            txtTitles.Text = banner.japaneseTitle;
            #endregion
            #region Set header values
            txtGameTitle.Text = new String(header.gameTitle);
            txtGameCode.Text = new String(header.gameCode);
            txtMakerCode.Text = new String(header.makerCode);
            numericUnitCode.Value = header.unitCode;
            txtReserved.Text = BitConverter.ToString(header.reserved);
            numericROMVer.Value = header.ROMversion;
            numericInternalFlag.Value = header.internalFlags;
            numericArm9Entry.Value = header.ARM9entryAddress;
            numericArm9Ram.Value = header.ARM9ramAddress;
            numericArm7Entry.Value = header.ARM7entryAddress;
            numericArm7Ram.Value = header.ARM7ramAddress;
            numericFlagsRead.Value = header.flagsRead;
            numericFlagsInit.Value = header.flagsInit;
            numericRomTimeout.Value = header.ROMtimeout;
            numericSecureDisable.Value = header.secureDisable;
            numericArm9Autoload.Value = header.ARM9autoload;
            numericArm7Autoload.Value = header.ARM7autoload;
            txtReserved2.Text = BitConverter.ToString(header.reserved2);
            numericReserved3.Value = header.reserved3;
            numericDebugRamAddress.Value = header.debug_ramAddress;
            numericDebugRomOffset.Value = header.debug_romOffset;
            numericDebugSize.Value = header.debug_size;
            #endregion
        }

        public Nitro.Estructuras.ROMHeader Header
        {
            get { return header; }
        }
        public Nitro.Estructuras.Banner Banner
        {
            get { return banner; }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Calculate the new CRCs
            // Banner CRC
            string tempBanner = Path.GetTempFileName();
            Nitro.NDS.EscribirBanner(tempBanner, banner);
            BinaryReader br = new BinaryReader(File.OpenRead(tempBanner));
            br.BaseStream.Position = 0x20;
            banner.CRC16 = (ushort)CRC16.Calculate(br.ReadBytes(0x820));
            banner.CRC162 = 0;
            banner.CRC163 = 0;
            banner.CRC16i = 0;
            banner.checkCRC = true;
            if (this.banner.version == 2)
            {
                br.BaseStream.Position = 0x20;
                banner.CRC162 = (ushort)CRC16.Calculate(br.ReadBytes(0x920));
            }
            if (this.banner.version == 3)
            {
                br.BaseStream.Position = 0x20;
                banner.CRC162 = (ushort)CRC16.Calculate(br.ReadBytes(0x920));
                br.BaseStream.Position = 0x20;
                banner.CRC163 = (ushort)CRC16.Calculate(br.ReadBytes(0xA20));
            }
            if ((this.banner.version >> 8) >= 1)
            {
                br.BaseStream.Position = 0x20;
                banner.CRC162 = (ushort)CRC16.Calculate(br.ReadBytes(0x920));
                br.BaseStream.Position = 0x20;
                banner.CRC163 = (ushort)CRC16.Calculate(br.ReadBytes(0xA20));
                br.BaseStream.Position = 0x1240;
                banner.CRC16i = (ushort)CRC16.Calculate(br.ReadBytes(0x1180));
            }
            
            br.Close();
            File.Delete(tempBanner);

            // Header CRC
            string tempHeader = Path.GetTempFileName();
            Nitro.NDS.EscribirCabecera(tempHeader, header, nintendoLogo);
            br = new BinaryReader(File.OpenRead(tempHeader));
            header.headerCRC16 = (ushort)CRC16.Calculate(br.ReadBytes(0x15E));
            header.headerCRC = true;
            br.Close();
            File.Delete(tempHeader);

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        // Control events
        private void txtBanReserved_Leave(object sender, EventArgs e)
        {
            banner.reserved = BitsConverter.StringToBytes(txtBanReserved.Text, 22);
            txtBanReserved.Text = BitConverter.ToString(banner.reserved);
        }
        private void txtReserved_Leave(object sender, EventArgs e)
        {
            header.reserved = BitsConverter.StringToBytes(txtReserved.Text, 7);
            txtReserved.Text = BitConverter.ToString(header.reserved);
        }
        private void txtReserved2_Leave(object sender, EventArgs e)
        {
            header.reserved2 = BitsConverter.StringToBytes(txtReserved2.Text, 56);
            txtReserved2.Text = BitConverter.ToString(header.reserved2);
        }

        private void txtTitles_TextChanged(object sender, EventArgs e)
        {
            switch (comboBanTitles.SelectedIndex)
            {
                case 0:
                    banner.japaneseTitle = txtTitles.Text;
                    break;
                case 1:
                    banner.englishTitle = txtTitles.Text;
                    break;
                case 2:
                    banner.frenchTitle = txtTitles.Text;
                    break;
                case 3:
                    banner.germanTitle = txtTitles.Text;
                    break;
                case 4:
                    banner.italianTitle = txtTitles.Text;
                    break;
                case 5:
                    banner.spanishTitle = txtTitles.Text;
                    break;
                case 6:
                    banner.chineseTitle = txtTitles.Text;
                    break;
                case 7:
                    banner.koreanTitle = txtTitles.Text;
                    break;
            }
        }
        private void comboBanTitles_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBanTitles.SelectedIndex)
            {
                case 0:
                    txtTitles.Text = banner.japaneseTitle;
                    break;
                case 1:
                    txtTitles.Text = banner.englishTitle;
                    break;
                case 2:
                    txtTitles.Text = banner.frenchTitle;
                    break;
                case 3:
                    txtTitles.Text = banner.germanTitle;
                    break;
                case 4:
                    txtTitles.Text = banner.italianTitle;
                    break;
                case 5:
                    txtTitles.Text = banner.spanishTitle;
                    break;
                case 6:
                    txtTitles.Text = banner.chineseTitle;
                    break;
                case 7:
                    txtTitles.Text = banner.koreanTitle;
                    break;
            }
        }

        private void btnImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog
            {
                CheckFileExists = true,
                DefaultExt = "idat",
                Filter = "Tinke icon data (*.idat)|*.idat"
            };
            if (o.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    BinaryReader br = new BinaryReader(File.OpenRead(o.FileName));
                    banner.tileData = br.ReadBytes(0x200);
                    banner.palette = br.ReadBytes(0x20);
                    br.Close();
                    //PluginInterface.NCGR tile = Imagen_NCGR.BitmapToTile(o.FileName, PluginInterface.TileOrder.Horizontal);
                    //if (tile.rahc.depth == ColorDepth.Depth8Bit)
                    //    throw new NotSupportedException(Tools.Helper.GetTranslation("EditRomInfo", "S26"));
                    //banner.tileData = Convertir.TilesToBytes(tile.rahc.tileData.tiles);
                    //banner.tileData = Tools.Helper.Bits4ToBits8(banner.tileData);
                    //// TODO: banner.palette = Convertir.ColorToBGR555(Imagen_NCLR.BitmapToPalette(o.FileName).pltt.palettes[0].colors);
                    //txtImage.BackColor = Color.LightGreen;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    txtImage.BackColor = Color.Magenta;
                }
                finally { txtImage.Text = System.IO.Path.GetFileName(o.FileName); }
            }
        }

        private void txtGameTitle_TextChanged(object sender, EventArgs e)
        {
            header.gameTitle = txtGameTitle.Text.ToCharArray();
        }
        private void txtGameCode_TextChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < txtGameCode.Text.Length; i++)
                if (!Char.IsLetterOrDigit(txtGameCode.Text, i))
                    return;

            header.gameCode = txtGameCode.Text.ToCharArray();
        }
        private void txtMakerCode_TextChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < txtMakerCode.Text.Length; i++)
                if (!Char.IsLetterOrDigit(txtMakerCode.Text, i))
                    return;

            header.makerCode = txtMakerCode.Text.ToCharArray();
        }
        private void numericUnitCode_ValueChanged(object sender, EventArgs e)
        {
            header.unitCode = (byte)numericUnitCode.Value;
        }
        private void numericROMVer_ValueChanged(object sender, EventArgs e)
        {
            header.ROMversion = (byte)numericROMVer.Value;
        }
        private void numericInternalFlag_ValueChanged(object sender, EventArgs e)
        {
            header.internalFlags = (byte)numericInternalFlag.Value;
        }
        private void numericArm9Entry_ValueChanged(object sender, EventArgs e)
        {
            header.ARM9entryAddress = (uint)numericArm9Entry.Value;
        }
        private void numericArm9Ram_ValueChanged(object sender, EventArgs e)
        {
            header.ARM9ramAddress = (uint)numericArm9Ram.Value;
        }
        private void numericArm7Entry_ValueChanged(object sender, EventArgs e)
        {
            header.ARM7entryAddress = (uint)numericArm7Entry.Value;
        }
        private void numericArm7Ram_ValueChanged(object sender, EventArgs e)
        {
            header.ARM7ramAddress = (uint)numericArm7Ram.Value;
        }
        private void numericFlagsRead_ValueChanged(object sender, EventArgs e)
        {
            header.flagsRead = (uint)numericFlagsRead.Value;
        }
        private void numericFlagsInit_ValueChanged(object sender, EventArgs e)
        {
            header.flagsInit = (uint)numericFlagsInit.Value;
        }
        private void numericRomTimeout_ValueChanged(object sender, EventArgs e)
        {
            header.ROMtimeout = (ushort)numericRomTimeout.Value;
        }
        private void numericSecureDisable_ValueChanged(object sender, EventArgs e)
        {
            header.secureDisable = (ulong)numericSecureDisable.Value;
        }
        private void numericArm9Autoload_ValueChanged(object sender, EventArgs e)
        {
            header.ARM9autoload = (uint)numericArm9Autoload.Value;
        }
        private void numericArm7Autoload_ValueChanged(object sender, EventArgs e)
        {
            header.ARM7autoload = (uint)numericArm7Autoload.Value;
        }
        private void numericReserved3_ValueChanged(object sender, EventArgs e)
        {
            header.reserved3 = (uint)numericReserved3.Value;
        }
        private void numericDebugRomOffset_ValueChanged(object sender, EventArgs e)
        {
            header.debug_romOffset = (uint)numericDebugRomOffset.Value;
        }
        private void numericDebugSize_ValueChanged(object sender, EventArgs e)
        {
            header.debug_size = (uint)numericDebugSize.Value;
        }
        private void numericDebugRamAddress_ValueChanged(object sender, EventArgs e)
        {
            header.debug_ramAddress = (uint)numericDebugRamAddress.Value;
        }
        private void numericEncryptionSeed_ValueChanged(object sender, EventArgs e)
        {
            header.encryptionSeed = (byte)numericEncryptionSeed.Value;
        }


        byte[] nintendoLogo = {
	        0x24, 0xFF, 0xAE, 0x51, 0x69, 0x9A, 0xA2, 0x21, 0x3D, 0x84, 0x82, 0x0A,
	        0x84, 0xE4, 0x09, 0xAD, 0x11, 0x24, 0x8B, 0x98, 0xC0, 0x81, 0x7F, 0x21,
	        0xA3, 0x52, 0xBE, 0x19, 0x93, 0x09, 0xCE, 0x20, 0x10, 0x46, 0x4A, 0x4A,
	        0xF8, 0x27, 0x31, 0xEC, 0x58, 0xC7, 0xE8, 0x33, 0x82, 0xE3, 0xCE, 0xBF,
	        0x85, 0xF4, 0xDF, 0x94, 0xCE, 0x4B, 0x09, 0xC1, 0x94, 0x56, 0x8A, 0xC0,
	        0x13, 0x72, 0xA7, 0xFC, 0x9F, 0x84, 0x4D, 0x73, 0xA3, 0xCA, 0x9A, 0x61,
	        0x58, 0x97, 0xA3, 0x27, 0xFC, 0x03, 0x98, 0x76, 0x23, 0x1D, 0xC7, 0x61,
	        0x03, 0x04, 0xAE, 0x56, 0xBF, 0x38, 0x84, 0x00, 0x40, 0xA7, 0x0E, 0xFD,
	        0xFF, 0x52, 0xFE, 0x03, 0x6F, 0x95, 0x30, 0xF1, 0x97, 0xFB, 0xC0, 0x85,
	        0x60, 0xD6, 0x80, 0x25, 0xA9, 0x63, 0xBE, 0x03, 0x01, 0x4E, 0x38, 0xE2,
	        0xF9, 0xA2, 0x34, 0xFF, 0xBB, 0x3E, 0x03, 0x44, 0x78, 0x00, 0x90, 0xCB,
	        0x88, 0x11, 0x3A, 0x94, 0x65, 0xC0, 0x7C, 0x63, 0x87, 0xF0, 0x3C, 0xAF,
	        0xD6, 0x25, 0xE4, 0x8B, 0x38, 0x0A, 0xAC, 0x72, 0x21, 0xD4, 0xF8, 0x07
            };

        private void btnShowMakerCode_Click(object sender, EventArgs e)
        {
            System.Xml.Linq.XElement xml = Tools.Helper.GetTranslation("EditRomInfo");

            Form ven = new Form();
            ven.AutoScroll = true;
            ven.BackColor = SystemColors.GradientInactiveCaption;
            ven.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            ven.ShowIcon = false;
            ven.ShowInTaskbar = false;
            ven.MaximizeBox = false;
            ven.MinimizeBox = false;
            ven.Size = new System.Drawing.Size(300, 700);

            ListView list = new ListView();
            list.Dock = DockStyle.Fill;
            list.View = View.Details;
            ColumnHeader columnCode = new ColumnHeader();
            columnCode.Text = xml.Element("S28").Value;
            ColumnHeader columnCompany = new ColumnHeader();
            columnCompany.Text = xml.Element("S29").Value;
            list.Columns.Add(columnCode);
            list.Columns.Add(columnCompany);
            foreach (String code in Nitro.Estructuras.makerCode.Keys)
            {
                list.Items.Add(code);
                list.Items[list.Items.Count - 1].SubItems.Add(Nitro.Estructuras.makerCode[code]);
            }
            list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            list.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.HeaderSize);
            ven.Controls.Add(list);

            ven.Text = xml.Element("S2A").Value;
            ven.Show();
        }

        private void btnImportAdata_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog
            {
                CheckFileExists = true,
                DefaultExt = "adat",
                Filter = "Tinke animation data(*.adat)|*.adat"
            };
            if (o.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                BinaryReader br = new BinaryReader(File.OpenRead(o.FileName));
                banner.aniIconData = br.ReadBytes(0x1180);
                br.Close();
                byte[] zbyte = new byte[0x800];
                banner.reservedDsi = zbyte;
            }
        }

        private void btnImportiheader_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog
            {
                CheckFileExists = true,
                DefaultExt = "ihdr",
                Filter = "Tinke donor iheader (*.ihdr)|*.ihdr"
            };
            if (o.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                BinaryReader br = new BinaryReader(File.OpenRead(o.FileName));
                char[] temp_gameCode = br.ReadChars(4);
                if (Enumerable.SequenceEqual(temp_gameCode, header.gameCode) != true)
                {
                    MessageBox.Show(Tools.Helper.GetTranslation("Messages", "S2D"));
                    br.Close();
                    return;
                }
                header.global_mbk_setting = new byte[5][];
                for (int i = 0; i < 5; i++) header.global_mbk_setting[i] = br.ReadBytes(4);
                header.arm9_mbk_setting = new uint[3];
                for (int i = 0; i < 3; i++) header.arm9_mbk_setting[i] = br.ReadUInt32();
                header.arm7_mbk_setting = new uint[3];
                for (int i = 0; i < 3; i++) header.arm7_mbk_setting[i] = br.ReadUInt32();
                header.mbk9_wramcnt_setting = br.ReadUInt32();

                header.region_flags = br.ReadUInt32();
                header.access_control = br.ReadUInt32();
                header.scfg_ext_mask = br.ReadUInt32();
                header.appflags = br.ReadBytes(4);

                header.dsi9_rom_offset = br.ReadUInt32();
                header.offset_0x1C4 = br.ReadUInt32();
                header.dsi9_ram_address = br.ReadUInt32();
                header.dsi9_size = br.ReadUInt32();
                header.dsi7_rom_offset = br.ReadUInt32();
                header.offset_0x1D4 = br.ReadUInt32();
                header.dsi7_ram_address = br.ReadUInt32();
                header.dsi7_size = br.ReadUInt32();

                header.digest_ntr_start = br.ReadUInt32();
                header.digest_ntr_size = br.ReadUInt32();
                header.digest_twl_start = br.ReadUInt32();
                header.digest_twl_size = br.ReadUInt32();

                header.sector_hashtable_start = br.ReadUInt32();
                header.sector_hashtable_size = br.ReadUInt32();
                header.block_hashtable_start = br.ReadUInt32();
                header.block_hashtable_size = br.ReadUInt32();

                header.digest_sector_size = br.ReadUInt32();
                header.digest_block_sectorcount = br.ReadUInt32();
                header.banner_size = br.ReadUInt32();
                header.offset_0x20C = br.ReadUInt32();

                header.total_rom_size = br.ReadUInt32();
                header.offset_0x214 = br.ReadUInt32();
                header.offset_0x218 = br.ReadUInt32();
                header.offset_0x21C = br.ReadUInt32();

                header.modcrypt1_start = br.ReadUInt32();
                header.modcrypt1_size = br.ReadUInt32();
                header.modcrypt2_start = br.ReadUInt32();
                header.modcrypt2_size = br.ReadUInt32();

                header.tid_low = br.ReadUInt32();
                header.tid_high = br.ReadUInt32();
                header.public_sav_size = br.ReadUInt32();
                header.private_sav_size = br.ReadUInt32();

                header.reserved5 = br.ReadBytes(0xB0);
                header.age_ratings = br.ReadBytes(0x10);
                header.hmac_arm9 = br.ReadBytes(20);
                header.hmac_arm7 = br.ReadBytes(20);
                header.hmac_digest_master = br.ReadBytes(20);
                header.hmac_icon_title = br.ReadBytes(20);
                header.hmac_arm9i = br.ReadBytes(20);
                header.hmac_arm7i = br.ReadBytes(20);
                header.reserved6 = br.ReadBytes(40);
                header.hmac_arm9_no_secure = br.ReadBytes(20);
                header.reserved7 = br.ReadBytes(0xA4C);
                header.debug_args = br.ReadBytes(0x180);
                header.rsa_signature = br.ReadBytes(0x80);
                br.Close();
                header.trimmedRom = false;
            }
        }
    }
}
