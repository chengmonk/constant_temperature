using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 恒温测试机.Model.Enum;

namespace 恒温测试机.UI
{
    public partial class FormStandardChoose : Form
    {
        private FormMain formMain;
        private TestStandardEnum testStandardTemp = TestStandardEnum.default1711;

        public FormStandardChoose(FormMain formMain)
        {
            InitializeComponent();
            this.formMain = formMain;
            InitControl();
        }

        private void InitControl()
        {
            switch (formMain.testStandard)
            {
                case Model.Enum.TestStandardEnum.default1711:
                    radioButton1.Checked = true;
                    break;
                case Model.Enum.TestStandardEnum.default2806:
                    radioButton2.Checked = true;
                    break;
                case Model.Enum.TestStandardEnum.default1016:
                    radioButton4.Checked = true;
                    break;
                case Model.Enum.TestStandardEnum.blank:
                    radioButton3.Checked = true;
                    break;
            }
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!((RadioButton)sender).Checked)
            {
                return;
            }
            switch (((RadioButton)sender).Text.ToString())
            {
                case "EN1111-2017":
                    testStandardTemp = TestStandardEnum.default1711;
                    break;
                case "QB/T 2806-2017标准":
                    testStandardTemp = TestStandardEnum.default2806;
                    break;
                case "ASEE1016-2017":
                    testStandardTemp = TestStandardEnum.default1016;
                    break;
                case "自定义":
                    testStandardTemp = TestStandardEnum.blank;
                    break;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            formMain.testStandard = testStandardTemp;
            this.DialogResult = DialogResult.OK;
        }
    }
}
