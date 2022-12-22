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

        private void btn_OK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
