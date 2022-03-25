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
using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Be.Windows.Forms;
using Ekona;

namespace Tinke
{
    public partial class VisorHex : Form
    {
        int id;
        string hexFile;
        bool fileEdited;
        IByteCharConverter bcc;
        bool allowEdit;

        public VisorHex(string hexFile, int id, bool edit)
        {
            InitializeComponent();
            ReadLanguage();
            this.id = id;
            this.hexFile = hexFile;

            saveToolStripMenuItem.Enabled = edit;
            allowEdit = edit;

            hexBox1.ByteProvider = new DynamicFileByteProvider(hexFile, !edit); 
            encodingCombo.SelectedIndex = 0;
            toolStripComboBox1.SelectedIndex = 0;
        }
        public VisorHex(sFile file)
        {
            InitializeComponent();
            ReadLanguage();

            hexFile = Path.GetTempFileName();
            BinaryReader br = new BinaryReader(File.OpenRead(file.path));
            br.BaseStream.Position = file.offset;
            File.WriteAllBytes(hexFile, br.ReadBytes((int)file.size));
            br.Close();

            saveToolStripMenuItem.Enabled = false;
            allowEdit = false;

            hexBox1.ByteProvider = new DynamicFileByteProvider(hexFile, true);
            encodingCombo.SelectedIndex = 0;
            toolStripComboBox1.SelectedIndex = 0;
        }
        private void VisorHex_FormClosed(object sender, FormClosedEventArgs e)
        {
            hexBox1.Dispose();
            ((DynamicFileByteProvider)hexBox1.ByteProvider).Dispose();
            if (!allowEdit)
                File.Delete(hexFile);
        }

        private void ReadLanguage()
        {
            try
            {
                System.Xml.Linq.XElement xml = Tools.Helper.GetTranslation("VisorHex");

                this.Text = Tools.Helper.GetTranslation("Sistema", "S41");
                fileToolStripMenuItem.Text = xml.Element("S00").Value;
                saveToolStripMenuItem.Text = xml.Element("S01").Value;
                toolsToolStripMenuItem.Text = xml.Element("S02").Value;
                gotoToolStripMenuItem.Text = xml.Element("S03").Value;
                goToolStripMenuItem.Text = xml.Element("S05").Value;
                goToolStripMenuItem1.Text = xml.Element("S05").Value;
                selectRangeToolStripMenuItem.Text = xml.Element("S04").Value;
                startOffsetToolStripMenuItem.Text = xml.Element("S06").Value;
                endOffsetToolStripMenuItem.Text = xml.Element("S07").Value;
                relativeToolStripMenuItem.Text = xml.Element("S08").Value;
                searchToolStripMenuItem.Text = xml.Element("S09").Value;
                optionsToolStripMenuItem.Text = xml.Element("S0A").Value;
                encodingToolStripMenuItem.Text = xml.Element("S0B").Value;
                encodingCombo.Items[0] = xml.Element("S0D").Value;
                toolStripComboBox1.Items[0] = xml.Element("S0E").Value;
                toolStripComboBox1.Items[1] = xml.Element("S0F").Value;
                toolStripComboBox1.Items[4] = xml.Element("S10").Value;
                openTabletblToolStripMenuItem.Text = xml.Element("S12").Value;
                createTableToolStripMenuItem.Text = xml.Element("S13").Value;
                showToolStripMenuItem.Text = xml.Element("S14").Value;
                hideToolStripMenuItem.Text = xml.Element("S15").Value;
                saveToolStripMenuItem1.Text = xml.Element("S01").Value;
                openOneToolStripMenuItem.Text = xml.Element("S16").Value;
                createBasicToolStripMenuItem.Text = xml.Element("S17").Value;
                codeColumn.HeaderText = xml.Element("S18").Value;
                charColumn.HeaderText = xml.Element("S19").Value;
                FindPrvToolStripMenuItem.Text = xml.Element("S1A").Value;
                FindNextToolStripMenuItem.Text = xml.Element("S1B").Value;
            }
            catch { throw new NotSupportedException("There was an error reading the language file"); }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!allowEdit)
                return;

            fileEdited = true;
            ((DynamicFileByteProvider)hexBox1.ByteProvider).ApplyChanges();
        }

        public Boolean Edited
        {
            get { return fileEdited; }
        }
        public string NewFile
        {
            get { return hexFile; }
        }
        public int FileID
        {
            get { return id; }
        }

        private void VisorHex_Resize(object sender, EventArgs e)
        {
            hexBox1.Height = this.Height - 83;

            tableGrid.Width = this.Width - 652;

            if (tableGrid.Visible) hexBox1.Width = this.Width - (tableGrid.Width + 15);
            else hexBox1.Width = this.Width - 16;
            
        }

        private void comboBoxEncoding_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (encodingCombo.SelectedIndex == 0)
                bcc = new DefaultByteCharConverter();
            else
                bcc = new ByteCharConveter(encodingCombo.Text);

            hexBox1.ByteCharConverter = bcc;
        }

        private void numericOffset_ValueChanged(object sender, EventArgs e)
        {
            hexBox1.Select(Convert.ToInt64(toolStripTextBox1.Text, 16), 1);
        }
        private void hexBox1_SelectionLengthChanged(object sender, EventArgs e)
        {
            toolStripSelect.Text = String.Format(Tools.Helper.GetTranslation("VisorHex", "S0C"),
                hexBox1.SelectionStart.ToString("x"), hexBox1.SelectionLength.ToString("x"));
        }
        private void goToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            long size = 0;
            long start = Convert.ToInt64(startOffsetSelect.Text, 16);
            if (relativeToolStripMenuItem.Checked)
                size = Convert.ToInt64(endOffsetSelect.Text, 16);
            else
                size = Convert.ToInt64(endOffsetSelect.Text, 16) - start;

            hexBox1.Select(start, size);
        }

        private void RawBytes_Find(bool prv_flag)
        {
            string text = toolStripSearchBox.Text;
            text = text.Replace(" ", "");

            if (text != "" && text.Length%2 == 0)
            {
                this.Cursor = Cursors.WaitCursor;

                List<byte> search = new List<byte>();
                for (int i = 0; i < text.Length; i += 2)
                    search.Add(Convert.ToByte(text.Substring(i, 2), 16));
                if (!prv_flag)
                    hexBox1.Find(search.ToArray(), hexBox1.SelectionStart + hexBox1.SelectionLength);
                else
                    hexBox1.Find_Prv(search.ToArray(), hexBox1.SelectionStart - hexBox1.SelectionLength);

                this.Cursor = Cursors.Default;
            }
        }
        private void ShiftjisText_Find(bool prv_flag)
        {
            string text = toolStripSearchBox.Text;
            if (text != "")
            {
                this.Cursor = Cursors.WaitCursor;

                byte[] search = Encoding.GetEncoding("shift_jis").GetBytes(text.ToCharArray());
                if (!prv_flag)
                    hexBox1.Find(search, hexBox1.SelectionStart + hexBox1.SelectionLength);
                else
                    hexBox1.Find_Prv(search, hexBox1.SelectionStart - hexBox1.SelectionLength);

                this.Cursor = Cursors.Default;
            }
        }
        private void DefaultChars_Find(bool prv_flag)
        {
            string text = toolStripSearchBox.Text;
            if (text != "")
            {
                this.Cursor = Cursors.WaitCursor;

                byte[] search = Encoding.Default.GetBytes(text.ToCharArray());
                if (!prv_flag)
                    hexBox1.Find(search, hexBox1.SelectionStart + hexBox1.SelectionLength);
                else
                    hexBox1.Find_Prv(search, hexBox1.SelectionStart - hexBox1.SelectionLength);

                this.Cursor = Cursors.Default;
            }
        }
        private void UnicodeText_Find(bool prv_flag)
        {
            string text = toolStripSearchBox.Text;
            if (text != "")
            {
                this.Cursor = Cursors.WaitCursor;

                byte[] search = Encoding.Unicode.GetBytes(text.ToCharArray());
                if (!prv_flag)
                    hexBox1.Find(search, hexBox1.SelectionStart + hexBox1.SelectionLength);
                else
                    hexBox1.Find_Prv(search, hexBox1.SelectionStart - hexBox1.SelectionLength);

                this.Cursor = Cursors.Default;
            }
        }
        private void UnicodeBigEndianText_Find(bool prv_flag)
        {
            string text = toolStripSearchBox.Text;
            if (text != "")
            {
                this.Cursor = Cursors.WaitCursor;

                byte[] search = Encoding.BigEndianUnicode.GetBytes(text.ToCharArray());
                if (!prv_flag)
                    hexBox1.Find(search, hexBox1.SelectionStart + hexBox1.SelectionLength);
                else
                    hexBox1.Find_Prv(search, hexBox1.SelectionStart - hexBox1.SelectionLength);

                this.Cursor = Cursors.Default;
            }
        }

        private void openTabletblToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.CheckFileExists = true;
            o.DefaultExt = ".tbl";
            o.Filter = "TaBLe (*.tbl)|*.tbl|" +
                       String.Format(Tools.Helper.GetTranslation("VisorHex", "S11"));
            o.Multiselect = false;
            if (o.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                hexBox1.ByteCharConverter = new ByteCharTable(o.FileName);
            o.Dispose();
        }

        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tableGrid.Hide();
            hexBox1.Width = this.Width - 16;
        }
        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hexBox1.Width = this.Width - (tableGrid.Width + 15);
            tableGrid.Show();
        }

        private void tableGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            List<ulong> codes = new List<ulong>();
            List<char> charas = new List<char>();

            for (int i = 0; i < tableGrid.RowCount; i++)
            {
                if (!(tableGrid.Rows[i].Cells[0].Value is object) ||
                    !(tableGrid.Rows[i].Cells[1].Value is object))
                    continue;

                codes.Add(Convert.ToUInt64((string)tableGrid.Rows[i].Cells[0].Value, 16));
                charas.Add(Convert.ToChar(tableGrid.Rows[i].Cells[1].Value));
            }
            hexBox1.ByteCharConverter = new ByteCharTable(codes.ToArray(), charas.ToArray());
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog s = new SaveFileDialog();
            s.DefaultExt = ".tbl";
            s.Filter = "TaBLe (*.tbl)|*.tbl|" +
                       String.Format(Tools.Helper.GetTranslation("VisorHex", "S11"));
            s.OverwritePrompt = true;
            if (s.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            BinaryWriter bw = new BinaryWriter(File.OpenWrite(s.FileName));

            for (int i = 0; i < tableGrid.RowCount; i++)
            {
                if (!(tableGrid.Rows[i].Cells[0].Value is object) ||
                    !(tableGrid.Rows[i].Cells[1].Value is object))
                    continue;

                bw.Write(((string)tableGrid.Rows[i].Cells[0].Value).ToCharArray());
                bw.Write('=');
                bw.Write(Convert.ToChar(tableGrid.Rows[i].Cells[1].Value));
                bw.Write('\n');
            }

            bw.Flush();
            bw.Close();
        }

        private void openOneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.CheckFileExists = true;
            o.DefaultExt = ".tbl";
            o.Filter = "TaBLe (*.tbl)|*.tbl|" +
                       String.Format(Tools.Helper.GetTranslation("VisorHex", "S11"));
            o.Multiselect = false;
            if (o.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            tableGrid.Rows.Clear();
            String[] lines = File.ReadAllLines(o.FileName);
            for (int i = 0; i < lines.Length; i++)
            {
                int sign_pos = lines[i].IndexOf('=');
                ushort code = Convert.ToUInt16(lines[i].Substring(0, sign_pos), 16);
                char chara = lines[i].Substring(sign_pos + 1)[0];

                if (code <= 0x7E)
                    tableGrid.Rows.Add(code.ToString("x").ToUpper(), chara);
                else
                    tableGrid.Rows.Add(code.ToString("x").ToUpper().PadLeft(4, '0'), chara);
            }

            tableGrid_CellEndEdit(null, null);
            showToolStripMenuItem_Click(null, null);
            o.Dispose();
        }
        private void createBasicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tableGrid.Rows.Clear();
            for (int i = 0x20; i < 0x7F; i++)
            {
                tableGrid.Rows.Add(i.ToString("x"), (char)i);
            }

            tableGrid_CellEndEdit(null, null);
            showToolStripMenuItem_Click(null, null);
        }

        private void encodingCombo_DropDownClosed(object sender, EventArgs e)
        {
            encodingToolStripMenuItem.HideDropDown();
            optionsToolStripMenuItem.HideDropDown();
            optionsToolStripMenuItem.ShowDropDown();
            encodingToolStripMenuItem.ShowDropDown();
            encodingCombo.Select();
        }

        private void goToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            if (toolStripTextBox1.Focused) goToolStripMenuItem.Select();
        }

        private void relativeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolsToolStripMenuItem.ShowDropDown();
            selectRangeToolStripMenuItem.ShowDropDown();
            relativeToolStripMenuItem.Select();
        }

        private void toolStripComboBox1_DropDownClosed(object sender, EventArgs e)
        {
            searchToolStripMenuItem.HideDropDown();
            toolsToolStripMenuItem.HideDropDown();
            toolsToolStripMenuItem.ShowDropDown();
            searchToolStripMenuItem.ShowDropDown();
            toolStripComboBox1.Select();
        }

        private void FindPrvToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (toolStripComboBox1.SelectedIndex)
            {
                case 0:
                    RawBytes_Find(true);
                    break;
                case 1:
                    DefaultChars_Find(true);
                    break;
                case 2:
                    ShiftjisText_Find(true);
                    break;
                case 3:
                    UnicodeText_Find(true);
                    break;
                case 4:
                    UnicodeBigEndianText_Find(true);
                    break;
            }
        }

        private void FindPrvToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            if (toolStripSearchBox.Focused) FindPrvToolStripMenuItem.Select();
        }

        private void FindNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (toolStripComboBox1.SelectedIndex)
            {
                case 0:
                    RawBytes_Find(false);
                    break;
                case 1:
                    DefaultChars_Find(false);
                    break;
                case 2:
                    ShiftjisText_Find(false);
                    break;
                case 3:
                    UnicodeText_Find(false);
                    break;
                case 4:
                    UnicodeBigEndianText_Find(false);
                    break;
            }
        }

        private void FindNextToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            if (toolStripSearchBox.Focused) FindNextToolStripMenuItem.Select();
        }
    }

    public class ByteCharTable : IByteCharConverter
    {
        Dictionary<ulong, char> tableChar;
        Dictionary<char, ulong> tableByte;

        public ByteCharTable(string tablePath)
        {
            tableChar = new Dictionary<ulong, char>();
            tableByte = new Dictionary<char, ulong>();

            String[] lines = File.ReadAllLines(tablePath);
            for (int i = 0; i < lines.Length; i++)
            {
                int sign_pos = lines[i].IndexOf('=');
                ulong code = Convert.ToUInt64(lines[i].Substring(0, sign_pos), 16);
                char chara = lines[i].Substring(sign_pos + 1)[0];

                tableChar.Add(code, chara);
                tableByte.Add(chara, code);
            }
        }
        public ByteCharTable(ulong[] codes, char[] charas)
        {
            tableByte = new Dictionary<char, ulong>();
            tableChar = new Dictionary<ulong, char>();

            for (int i = 0; i < codes.Length; i++)
            {
                if (tableByte.ContainsKey(charas[i]) || tableChar.ContainsKey(codes[i]))
                    continue;
                tableByte.Add(charas[i], codes[i]);
                tableChar.Add(codes[i], charas[i]);
            }
        }

        public char ToChar(byte b)
        {
            if (tableChar.ContainsKey(b))
                return tableChar[b];
            else
                return '.';
        }
        public byte ToByte(char c)
        {
            if (tableByte.ContainsKey(c))
                return (byte)tableByte[c];
            else
                return 0;
        }
    }
    public class ByteCharConveter : IByteCharConverter
    {
        Encoding encoding;
        List<byte> requeridedChar;
        List<char> requeridedByte;

        public ByteCharConveter(string encoding)
        {
            this.encoding = Encoding.GetEncoding(encoding);
            requeridedChar = new List<byte>();
            requeridedByte = new List<char>();
        }

        public byte ToByte(char c)
        {
            if (encoding.WebName == "shift_jis")
                return ToByteShiftJis(c);

            return (byte)c;
        }
        public char ToChar(byte b)
        {
            if (encoding.WebName == "shift_jis")
                return ToCharShiftJis(b);

            return encoding.GetChars(new byte[] { b })[0];
        }

        public byte ToByteShiftJis(char c)
        {
            return (byte)c;
        }
        public char ToCharShiftJis(byte b)
        {
            if (requeridedChar.Count == 0 && b > 0x7F)
            {
                requeridedChar.Add(b);
                return '\x20';
            }

            requeridedChar.Add(b);
            string c = new String(encoding.GetChars(requeridedChar.ToArray()));
            requeridedChar.Clear();
            return (c[0] > '\x1F' ? c[0] : '.');
        }
    }
}