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
 * 
 */
using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
using System.Drawing;
using System.IO;
//using System.Linq;
//using System.Text;
using System.Windows.Forms;
using Ekona.Helper;

namespace Tinke
{
    public partial class RomInfo : Form
    {
        Nitro.Estructuras.ROMHeader cabecera;
        Nitro.Estructuras.Banner banner;
        Bitmap picBanner;
        string[] titulos;

        public RomInfo()
        {
            InitializeComponent();
        }
        public RomInfo(string archivo)
        {
            InitializeComponent();
            this.btnBannerGuardar.Image = Properties.Resources.picture_save;

            try
            {
                Nitro.Estructuras.ROMHeader cabecera = Nitro.NDS.LeerCabecera(archivo);
                Nitro.Estructuras.Banner banner = Nitro.NDS.LeerBanner(
                    archivo,
                    cabecera.bannerOffset,
                    cabecera.banner_size);
                Mostrar_Informacion(cabecera, banner);
                this.checkTrans.Checked = true;
            }
            catch
            {
                MessageBox.Show(Tools.Helper.GetTranslation("Messages", "S02"),
                    Tools.Helper.GetTranslation("Messages", "S01"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(Tools.Helper.GetTranslation("Messages", "S02") + ": " + archivo);
                return;
            }

            LeerIdioma();
        }
        public void LeerIdioma()
        {
            System.Xml.Linq.XElement xml = Tools.Helper.GetTranslation("RomInfo");

            this.Text = xml.Element("S01").Value;
            groupBanner.Text = xml.Element("S02").Value;
            btnBannerGuardar.Text = xml.Element("S03").Value;
            label2.Text = xml.Element("S04").Value;
            label4.Text = xml.Element("S05").Value;
            lblGameTitle.Text = xml.Element("S41").Value;
            //comboBannerLang.Text = xml.Element("S06").Value;
            comboBannerLang.Items[0] = xml.Element("S06").Value;
            comboBannerLang.Items[1] = xml.Element("S07").Value;
            comboBannerLang.Items[2] = xml.Element("S08").Value;
            comboBannerLang.Items[3] = xml.Element("S09").Value;
            comboBannerLang.Items[4] = xml.Element("S0A").Value;
            comboBannerLang.Items[5] = xml.Element("S0B").Value;
            columnPosicion.Text = xml.Element("S0C").Value;
            columnCampo.Text = xml.Element("S0D").Value;
            columnValor.Text = xml.Element("S0E").Value;
            listInfo.Items[0].SubItems[1].Text = xml.Element("S0F").Value;
            listInfo.Items[1].SubItems[1].Text = xml.Element("S10").Value;
            listInfo.Items[2].SubItems[1].Text = xml.Element("S11").Value;
            listInfo.Items[3].SubItems[1].Text = xml.Element("S12").Value;
            listInfo.Items[4].SubItems[1].Text = xml.Element("S13").Value;
            listInfo.Items[5].SubItems[1].Text = xml.Element("S14").Value;
            listInfo.Items[6].SubItems[1].Text = xml.Element("S15").Value;
            listInfo.Items[7].SubItems[1].Text = xml.Element("S16").Value;
            listInfo.Items[8].SubItems[1].Text = xml.Element("S17").Value;
            listInfo.Items[9].SubItems[1].Text = xml.Element("S18").Value;
            listInfo.Items[10].SubItems[1].Text = xml.Element("S19").Value;
            listInfo.Items[11].SubItems[1].Text = xml.Element("S1A").Value;
            listInfo.Items[12].SubItems[1].Text = xml.Element("S1B").Value;
            listInfo.Items[13].SubItems[1].Text = xml.Element("S1C").Value;
            listInfo.Items[14].SubItems[1].Text = xml.Element("S1D").Value;
            listInfo.Items[15].SubItems[1].Text = xml.Element("S1E").Value;
            listInfo.Items[16].SubItems[1].Text = xml.Element("S1F").Value;
            listInfo.Items[17].SubItems[1].Text = xml.Element("S20").Value;
            listInfo.Items[18].SubItems[1].Text = xml.Element("S21").Value;
            listInfo.Items[19].SubItems[1].Text = xml.Element("S22").Value;
            listInfo.Items[20].SubItems[1].Text = xml.Element("S23").Value;
            listInfo.Items[21].SubItems[1].Text = xml.Element("S24").Value;
            listInfo.Items[22].SubItems[1].Text = xml.Element("S25").Value;
            listInfo.Items[23].SubItems[1].Text = xml.Element("S26").Value;
            listInfo.Items[24].SubItems[1].Text = xml.Element("S27").Value;
            listInfo.Items[25].SubItems[1].Text = xml.Element("S28").Value;
            listInfo.Items[26].SubItems[1].Text = xml.Element("S29").Value;
            listInfo.Items[27].SubItems[1].Text = xml.Element("S2A").Value;
            listInfo.Items[28].SubItems[1].Text = xml.Element("S2B").Value;
            listInfo.Items[29].SubItems[1].Text = xml.Element("S2C").Value;
            listInfo.Items[30].SubItems[1].Text = xml.Element("S2D").Value;
            listInfo.Items[31].SubItems[1].Text = xml.Element("S2E").Value;
            listInfo.Items[32].SubItems[1].Text = xml.Element("S2F").Value;
            listInfo.Items[33].SubItems[1].Text = xml.Element("S30").Value;
            listInfo.Items[34].SubItems[1].Text = xml.Element("S31").Value;
            listInfo.Items[35].SubItems[1].Text = xml.Element("S32").Value;
            listInfo.Items[36].SubItems[1].Text = xml.Element("S33").Value;
            listInfo.Items[37].SubItems[1].Text = xml.Element("S34").Value;
            listInfo.Items[38].SubItems[1].Text = xml.Element("S35").Value;
            listInfo.Items[39].SubItems[1].Text = xml.Element("S36").Value;
            listInfo.Items[40].SubItems[1].Text = xml.Element("S37").Value;
            listInfo.Items[41].SubItems[1].Text = xml.Element("S38").Value;
            checkTrans.Text = xml.Element("S3A").Value;
            btnEdit.Text = xml.Element("S3B").Value;
            btnDumpicondata.Text = xml.Element("S3E").Value;
            btnDumpAdata.Text = xml.Element("S3F").Value;
            btnDumpiheader.Text = xml.Element("S40").Value;
            //Place these code here to fix compilation error...
            if (banner.version >= 2)
                this.comboBannerLang.Items.Add(Tools.Helper.GetTranslation("RomInfo", "S3C"));
            if (banner.version >= 3)
                this.comboBannerLang.Items.Add(Tools.Helper.GetTranslation("RomInfo", "S3D"));
            if (banner.version > 3 && (banner.GetDefSize(Cabecera.banner_size, true) != 0x840))
                this.btnDumpAdata.Enabled = true;
        }
        public void Refresh_flag()
        {
            if ((Cabecera.unitCode & 2) > 0 && Sistema.twl_flag == true)
                this.btnDumpiheader.Enabled = true;
        }
        private void Mostrar_Informacion(Nitro.Estructuras.ROMHeader cabecera, Nitro.Estructuras.Banner banner)
        {
            this.cabecera = cabecera;
            this.banner = banner;

            // Remove older values
            if (listInfo.Items[0].SubItems.Count == 3)
                for (int i = 0; i < listInfo.Items.Count; i++)
                    listInfo.Items[i].SubItems.RemoveAt(2);

            #region Muestra la información de la cabecera
            listInfo.Items[0].SubItems.Add(new String(cabecera.gameTitle));
            listInfo.Items[1].SubItems.Add(new string(cabecera.gameCode));
            try
            {

                listInfo.Items[2].SubItems.Add(new String(cabecera.makerCode) + " (" +
                    Nitro.Estructuras.makerCode[new String(cabecera.makerCode)] + ')');
            }
            catch
            {
                listInfo.Items[2].SubItems.Add(new String(cabecera.makerCode) + " (Desconocido)");
            }
            try
            {
                listInfo.Items[3].SubItems.Add("0x" + String.Format("{0:X}", cabecera.unitCode) +
                    " (" + Nitro.Estructuras.unitCode[cabecera.unitCode] + ')');
            }
            catch
            {
                listInfo.Items[3].SubItems.Add("0x" + String.Format("{0:X}", cabecera.unitCode) + " (Desconocido)");
            }
            listInfo.Items[4].SubItems.Add(Convert.ToString(cabecera.encryptionSeed));
            listInfo.Items[5].SubItems.Add((cabecera.tamaño / Math.Pow(2, 20)).ToString() + " MB");
            listInfo.Items[6].SubItems.Add(BitsConverter.BytesToHexString(cabecera.reserved));
            listInfo.Items[7].SubItems.Add(Convert.ToString(cabecera.ROMversion));
            listInfo.Items[8].SubItems.Add(Convert.ToString(cabecera.internalFlags));
            listInfo.Items[9].SubItems.Add("0x" + String.Format("{0:X}", cabecera.ARM9romOffset));
            listInfo.Items[10].SubItems.Add("0x" + String.Format("{0:X}", cabecera.ARM9entryAddress));
            listInfo.Items[11].SubItems.Add("0x" + String.Format("{0:X}", cabecera.ARM9ramAddress));
            listInfo.Items[12].SubItems.Add("0x" + String.Format("{0:X}", cabecera.ARM9size) + " bytes");
            listInfo.Items[13].SubItems.Add("0x" + String.Format("{0:X}", cabecera.ARM7romOffset));
            listInfo.Items[14].SubItems.Add("0x" + String.Format("{0:X}", cabecera.ARM7entryAddress));
            listInfo.Items[15].SubItems.Add("0x" + String.Format("{0:X}", cabecera.ARM7ramAddress));
            listInfo.Items[16].SubItems.Add("0x" + String.Format("{0:X}", cabecera.ARM7size) + " bytes");
            listInfo.Items[17].SubItems.Add("0x" + String.Format("{0:X}", cabecera.fileNameTableOffset));
            listInfo.Items[18].SubItems.Add("0x" + String.Format("{0:X}", cabecera.fileNameTableSize) + " bytes");
            listInfo.Items[19].SubItems.Add("0x" + String.Format("{0:X}", cabecera.FAToffset));
            listInfo.Items[20].SubItems.Add("0x" + String.Format("{0:X}", cabecera.FATsize) + " bytes");
            listInfo.Items[21].SubItems.Add("0x" + String.Format("{0:X}", cabecera.ARM9overlayOffset));
            listInfo.Items[22].SubItems.Add("0x" + String.Format("{0:X}", cabecera.ARM9overlaySize) + " bytes");
            listInfo.Items[23].SubItems.Add("0x" + String.Format("{0:X}", cabecera.ARM7overlayOffset));
            listInfo.Items[24].SubItems.Add("0x" + String.Format("{0:X}", cabecera.ARM7overlaySize) + " bytes");
            listInfo.Items[25].SubItems.Add(Convert.ToString(cabecera.flagsRead, 2));
            listInfo.Items[26].SubItems.Add(Convert.ToString(cabecera.flagsInit, 2));
            listInfo.Items[27].SubItems.Add("0x" + String.Format("{0:X}", cabecera.bannerOffset));
            listInfo.Items[28].SubItems.Add(cabecera.secureCRC16.ToString() + " (" + Convert.ToString(cabecera.secureCRC) + ")");
            listInfo.Items[29].SubItems.Add(cabecera.ROMtimeout.ToString());
            listInfo.Items[30].SubItems.Add("0x" + String.Format("{0:X}", cabecera.ARM9autoload));
            listInfo.Items[31].SubItems.Add("0x" + String.Format("{0:X}", cabecera.ARM7autoload));
            listInfo.Items[32].SubItems.Add(cabecera.secureDisable.ToString());
            listInfo.Items[33].SubItems.Add("0x" + String.Format("{0:X}", cabecera.ROMsize) + " bytes");
            listInfo.Items[34].SubItems.Add("0x" + String.Format("{0:X}", cabecera.headerSize) + " bytes");
            listInfo.Items[35].SubItems.Add(BitsConverter.BytesToHexString(cabecera.reserved2));
            listInfo.Items[36].SubItems.Add(cabecera.logoCRC16.ToString() + " (" + Convert.ToString(cabecera.logoCRC) + ")");
            listInfo.Items[37].SubItems.Add(cabecera.headerCRC16.ToString() + " (" + Convert.ToString(cabecera.headerCRC) + ")");
            listInfo.Items[38].SubItems.Add("0x" + String.Format("{0:X}", cabecera.debug_romOffset));
            listInfo.Items[39].SubItems.Add("0x" + String.Format("{0:X}", cabecera.debug_size) + " bytes");
            listInfo.Items[40].SubItems.Add("0x" + String.Format("{0:X}", cabecera.debug_ramAddress));
            listInfo.Items[41].SubItems.Add("0x" + String.Format("{0:X}", cabecera.reserved3));
            #endregion
            #region Muestra la información del banner
            picIcon.BorderStyle = BorderStyle.None;
            picBanner = Nitro.NDS.IconoToBitmap(banner.tileData, banner.palette);
            picIcon.Image = picBanner;

            txtBannerVer.Text = banner.version.ToString("X");
            txtBannerCRC.Text = String.Format("{0:X}", banner.CRC16) + " (" +
                (banner.checkCRC ? "OK)" : Tools.Helper.GetTranslation("RomInfo", "S39") + ')');
            txtBannerReserved.Text = BitsConverter.BytesToHexString(banner.reserved);

            if (banner.version >= 3)
                titulos = new string[] { banner.japaneseTitle, banner.englishTitle, banner.frenchTitle, banner.germanTitle,
                    banner.italianTitle, banner.spanishTitle, banner.chineseTitle, banner.koreanTitle };
            else if (banner.version == 2)
                titulos = new string[] { banner.japaneseTitle, banner.englishTitle, banner.frenchTitle, banner.germanTitle,
                    banner.italianTitle, banner.spanishTitle, banner.chineseTitle };
            else
                titulos = new string[] { banner.japaneseTitle, banner.englishTitle, banner.frenchTitle, banner.germanTitle, banner.italianTitle, banner.spanishTitle};
            txtBannerTitle.Text = titulos[0];
            comboBannerLang.SelectedIndex = 0;
            #endregion
        }

        public Nitro.Estructuras.ROMHeader Cabecera
        {
            get { return cabecera; }
            set { cabecera = value; }
        }
        public Nitro.Estructuras.Banner Banner
        {
            get { return banner; }
            set { banner = value; }
        }
        private void btnBannerGuardar_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.CheckPathExists = true;
            o.DefaultExt = ".bmp";
            o.OverwritePrompt = true;
            o.Filter = "Imagen Bitmap (*.bmp)|*.bmp|PNG image|*.png|GIF image|*.gif|JPEG image|*.jpg;*.jpeg;*.jpe;*.jif;*.jfif;*.jfi|TIFF image|*.tif;*.tiff";
            if (o.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                picIcon.Image.Save(o.FileName);
        }
        private void comboBannerLang_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBannerLang.SelectedIndex)
            {
                case 0:
                    txtBannerTitle.Text = titulos[0];
                    break;
                case 1:
                    txtBannerTitle.Text = titulos[1];
                    break;
                case 2:
                    txtBannerTitle.Text = titulos[2];
                    break;
                case 3:
                    txtBannerTitle.Text = titulos[3];
                    break;
                case 4:
                    txtBannerTitle.Text = titulos[4];
                    break;
                case 5:
                    txtBannerTitle.Text = titulos[5];
                    break;
                case 6:
                    txtBannerTitle.Text = titulos[6];
                    break;
                case 7:
                    txtBannerTitle.Text = titulos[7];
                    break;
            }
        }

        private void checkTrans_CheckedChanged(object sender, EventArgs e)
        {
            if (checkTrans.Checked)
            {
                Bitmap imagen = (Bitmap)picBanner.Clone();
                imagen.MakeTransparent(Ekona.Images.Actions.BGR555ToColor(banner.palette)[0]);
                picIcon.Image = imagen;
            }
            else
                picIcon.Image = picBanner;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            EditRomInfo editor = new EditRomInfo(cabecera, banner);
            editor.FormClosed += new FormClosedEventHandler(editor_FormClosed);
            editor.Show();
        }
        void editor_FormClosed(object sender, FormClosedEventArgs e)
        {
            EditRomInfo editor = (EditRomInfo)sender;
            if (editor.DialogResult != System.Windows.Forms.DialogResult.OK)
                return;

            cabecera = editor.Header;
            banner = editor.Banner;

            Mostrar_Informacion(cabecera, banner);
            if (checkTrans.Checked)
            {
                Bitmap imagen = (Bitmap)picBanner.Clone();
                imagen.MakeTransparent(Ekona.Images.Actions.BGR555ToColor(banner.palette)[0]);
                picIcon.Image = imagen;
            }
            else
                picIcon.Image = picBanner;
        }

        //private void RomInfo_Resize(object sender, EventArgs e)
        //{
            //btnEdit.Location = new Point(groupBanner.Location.X + 5, 313);
        //}

        private void RomInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void btnDumpicondata_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog
            {
                AddExtension = true,
                CheckPathExists = true,
                DefaultExt = ".idat",
                OverwritePrompt = true,
                Filter = "Tinke icon data (*.idat)|*.idat"
            };
            if (o.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                BinaryWriter bw;
                bw = new BinaryWriter(new FileStream(o.FileName, FileMode.Create));
                bw.Write(banner.tileData);
                bw.Write(banner.palette);
                bw.Flush();
                bw.Close();
            }
        }

        private void btnDumpAdata_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog
            {
                AddExtension = true,
                CheckPathExists = true,
                DefaultExt = ".adat",
                OverwritePrompt = true,
                Filter = "Tinke animation data(*.adat)|*.adat"
            };
            if (o.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                BinaryWriter bw;
                bw = new BinaryWriter(new FileStream(o.FileName, FileMode.Create));
                bw.Write(banner.aniIconData);
                bw.Flush();
                bw.Close();
            }
        }

        private void btnDumpiheader_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog
            {
                AddExtension = true,
                CheckPathExists = true,
                DefaultExt = ".ihdr",
                OverwritePrompt = true,
                Filter = "Tinke donor iheader (*.ihdr)|*.ihdr"
            };
            if (o.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                BinaryWriter bw;
                bw = new BinaryWriter(new FileStream(o.FileName, FileMode.Create));
                bw.Write(cabecera.gameCode);
                for (int i = 0; i < 5; i++) bw.Write(cabecera.global_mbk_setting[i]);
                for (int i = 0; i < 3; i++) bw.Write(cabecera.arm9_mbk_setting[i]);
                for (int i = 0; i < 3; i++) bw.Write(cabecera.arm7_mbk_setting[i]);
                bw.Write(cabecera.mbk9_wramcnt_setting);
                bw.Write(cabecera.region_flags);
                bw.Write(cabecera.access_control);
                bw.Write(cabecera.scfg_ext_mask);
                bw.Write(cabecera.appflags);
                bw.Write(cabecera.dsi9_rom_offset);
                bw.Write(cabecera.offset_0x1C4);
                bw.Write(cabecera.dsi9_ram_address);
                bw.Write(cabecera.dsi9_size);
                bw.Write(cabecera.dsi7_rom_offset);
                bw.Write(cabecera.offset_0x1D4);
                bw.Write(cabecera.dsi7_ram_address);
                bw.Write(cabecera.dsi7_size);
                bw.Write(cabecera.digest_ntr_start);
                bw.Write(cabecera.digest_ntr_size);
                bw.Write(cabecera.digest_twl_start);
                bw.Write(cabecera.digest_twl_size);
                bw.Write(cabecera.sector_hashtable_start);
                bw.Write(cabecera.sector_hashtable_size);
                bw.Write(cabecera.block_hashtable_start);
                bw.Write(cabecera.block_hashtable_size);
                bw.Write(cabecera.digest_sector_size);
                bw.Write(cabecera.digest_block_sectorcount);
                bw.Write(cabecera.banner_size);
                bw.Write(cabecera.offset_0x20C);
                bw.Write(cabecera.total_rom_size);
                bw.Write(cabecera.offset_0x214);
                bw.Write(cabecera.offset_0x218);
                bw.Write(cabecera.offset_0x21C);
                bw.Write(cabecera.modcrypt1_start);
                bw.Write(cabecera.modcrypt1_size);
                bw.Write(cabecera.modcrypt2_start);
                bw.Write(cabecera.modcrypt2_size);
                bw.Write(cabecera.tid_low);
                bw.Write(cabecera.tid_high);
                bw.Write(cabecera.public_sav_size);
                bw.Write(cabecera.private_sav_size);
                bw.Write(cabecera.reserved5);
                bw.Write(cabecera.age_ratings);
                bw.Write(cabecera.hmac_arm9);
                bw.Write(cabecera.hmac_arm7);
                bw.Write(cabecera.hmac_digest_master);
                bw.Write(cabecera.hmac_icon_title);
                bw.Write(cabecera.hmac_arm9i);
                bw.Write(cabecera.hmac_arm7i);
                bw.Write(cabecera.reserved6);
                bw.Write(cabecera.hmac_arm9_no_secure);
                bw.Write(cabecera.reserved7);
                bw.Write(cabecera.debug_args);
                bw.Write(cabecera.rsa_signature);
                bw.Flush();
                bw.Close();
            }
        }
    }
}
