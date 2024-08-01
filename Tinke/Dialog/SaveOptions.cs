using System;
using System.Xml.Linq;
using System.Windows.Forms;

namespace Tinke.Dialog
{
    public partial class SaveOptions : Form
    {
        public SaveOptions()
        {
            InitializeComponent();
            ReadLanguage();
        }
        private void ReadLanguage()
        {
            try
            {
                XElement xml = Tools.Helper.GetTranslation("Dialog");

                this.Text = xml.Element("S07").Value;
                btn_OK.Text = xml.Element("S01").Value;
                btn_Cancel.Text = xml.Element("S1C").Value;
                checkBox1.Text = xml.Element("S1E").Value;
                checkBox2.Text = xml.Element("S1F").Value;
                checkBox3.Text = xml.Element("S20").Value;
                checkBox4.Text = xml.Element("S21").Value;
                checkBox5.Text = xml.Element("S22").Value;
            }
            catch { throw new NotImplementedException("There was an error reading the language file"); }
        }

        public bool IsKeepSignature
        {
            get { return checkBox1.Checked; }
        }

        public bool IsSafeTrim
        { 
            get { return checkBox2.Checked; }
        }

        public bool IsReCompress
        {
            get { return checkBox3.Checked; }
        }

        public bool IsBetterCompress
        {
            get { return checkBox4.Checked; }
        }

        public bool IsFlashCartFirmware
        {
            get { return checkBox5.Checked; }
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                checkBox4.Enabled = true;
            } else
            {
                checkBox4.Checked = false;
                checkBox4.Enabled= false;
            }
        }
    }
}
