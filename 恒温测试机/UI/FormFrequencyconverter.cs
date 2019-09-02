using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 恒温测试机.UI
{
    public partial class FormFrequencyconverter : Form
    {

        private FormMain formMain;
        private byte station = 1;
        public FormFrequencyconverter(FormMain formMain)
        {
            InitializeComponent();
            this.formMain = formMain;
        }

        #region 变频器读写相关


        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReadBtn_Click(object sender, EventArgs e)
        {
            switch (comboBox1.Text)
            {
                case "1":
                    station = 1;
                    break;
                case "2":
                    station = 2;
                    break;
                case "3":
                    station = 3;
                    break;
                case "4":
                    station = 4;
                    break;
            }
            //Read
            var val = formMain.Read(textBox2.Text, station);
            //SystemInfoPrint("读取：【" + textBox2.Text + "】【" + val + "】\n");
        }


        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WriteBtn_Click(object sender, EventArgs e)
        {
            switch (comboBox1.Text)
            {
                case "1":
                    station = 1;
                    break;
                case "2":
                    station = 2;
                    break;
                case "3":
                    station = 3;
                    break;
                case "4":
                    station = 4;
                    break;
            }
            int val2 = Convert.ToInt32(textBox3.Text) * 500;

            if (val2 < 0 || val2 > 5000)
            {
                MessageBox.Show("请输入0-10 范围内的值");
                textBox3.Text = "";
            }
            else
            {
                short val3 = Convert.ToInt16("" + val2);
                formMain.Write_short(textBox2.Text, val3, station);
                //SystemInfoPrint("写入：【" + textBox2.Text + "】【" + val3 + "】\n");
            }
        }

        #endregion
    }
}
