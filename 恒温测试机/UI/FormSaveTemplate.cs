using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Word;
using 恒温测试机.Model;
using 恒温测试机.Utils;

namespace 恒温测试机.UI
{
    public partial class FormSaveTemplate : Form
    {
        public FormSaveTemplate()
        {
            InitializeComponent();
        }

        private Model_2806 model_2806;

        public FormSaveTemplate(Model_2806 model)
        {
            InitializeComponent();
            this.model_2806 = model;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                object oMissing = System.Reflection.Missing.Value;
                //创建一个word实例
                Word._Application oWord = new Word.Application();
                //设置为不可见
                oWord.Visible = false;
                //模板文件地址，这里假设在程序根目录
                object oTemplate = System.AppDomain.CurrentDomain.BaseDirectory + "//"+comboBox1.Text+".dot";
                //以模板为基础生成文档
                Word._Document oDoc = oWord.Documents.Add(ref oTemplate, ref oMissing, ref oMissing, ref oMissing);
                //声明书签数组
                object[] oBookMark = new object[41];
                //赋值书签名
                oBookMark[0] = "Pc";
                oBookMark[1] = "Tc";
                oBookMark[2] = "Ph";
                oBookMark[3] = "Th";
                oBookMark[4] = "Qm";
                oBookMark[5] = "Tm";
                oBookMark[6] = "A_1_Qc";
                oBookMark[7] = "A_1_Tc";
                oBookMark[8] = "A_1_Tcdiff";
                oBookMark[9] = "A_2_Qc";
                oBookMark[10] = "A_2_Tc";
                oBookMark[11] = "A_2_Tcdiff";
                oBookMark[12] = "B_1_Qh";
                oBookMark[13] = "B_1_Th";
                oBookMark[14] = "B_2_Qh";
                oBookMark[15] = "B_2_Th";
                oBookMark[16] = "B_2_Thdiff";

                oBookMark[17] = "C_1_Tm";
                oBookMark[18] = "C_1_3";
                oBookMark[19] = "C_1_5";
                oBookMark[20] = "C_1_Tmdiff";
                oBookMark[21] = "C_2_Tm";
                oBookMark[22] = "C_2_Tmdiff";
                oBookMark[23] = "C_3_Tm";
                oBookMark[24] = "C_3_3";
                oBookMark[25] = "C_3_5";
                oBookMark[26] = "C_3_Tmdiff";
                oBookMark[27] = "C_4_Tm";
                oBookMark[28] = "C_4_Tmdiff";

                oBookMark[29] = "H_1_Tm";
                oBookMark[30] = "H_1_3";
                oBookMark[31] = "H_1_5";
                oBookMark[32] = "H_1_Tmdiff";
                oBookMark[33] = "H_2_Tm";
                oBookMark[34] = "H_2_Tmdiff";
                oBookMark[35] = "H_3_Tm";
                oBookMark[36] = "H_3_3";
                oBookMark[37] = "H_3_5";
                oBookMark[38] = "H_3_Tmdiff";
                oBookMark[39] = "H_4_Tm";
                oBookMark[40] = "H_4_Tmdiff";
                //赋值任意数据到书签的位置
                oDoc.Bookmarks.get_Item(ref oBookMark[0]).Range.Text = model_2806.Pc+"";
                oDoc.Bookmarks.get_Item(ref oBookMark[1]).Range.Text = model_2806.Tc+"";
                oDoc.Bookmarks.get_Item(ref oBookMark[2]).Range.Text = model_2806.Ph+"";
                oDoc.Bookmarks.get_Item(ref oBookMark[3]).Range.Text = model_2806.Th+"";
                oDoc.Bookmarks.get_Item(ref oBookMark[4]).Range.Text = model_2806.Qm+"";
                oDoc.Bookmarks.get_Item(ref oBookMark[5]).Range.Text = model_2806.Tm+"";

                oDoc.Bookmarks.get_Item(ref oBookMark[6]).Range.Text = model_2806.A_1_Qc + "";
                oDoc.Bookmarks.get_Item(ref oBookMark[7]).Range.Text = model_2806.A_1_Tc + "";
                oDoc.Bookmarks.get_Item(ref oBookMark[8]).Range.Text = model_2806.A_1_Tcdiff + "";
                oDoc.Bookmarks.get_Item(ref oBookMark[9]).Range.Text = model_2806.A_2_Qc + "";
                oDoc.Bookmarks.get_Item(ref oBookMark[10]).Range.Text = model_2806.A_2_Tc + "";
                oDoc.Bookmarks.get_Item(ref oBookMark[11]).Range.Text = model_2806.A_2_Tcdiff + "";

                oDoc.Bookmarks.get_Item(ref oBookMark[12]).Range.Text = model_2806.B_1_Qh + "";
                oDoc.Bookmarks.get_Item(ref oBookMark[13]).Range.Text = model_2806.B_1_Th + "";
                oDoc.Bookmarks.get_Item(ref oBookMark[14]).Range.Text = model_2806.B_2_Qh + "";
                oDoc.Bookmarks.get_Item(ref oBookMark[15]).Range.Text = model_2806.B_2_Th + "";
                oDoc.Bookmarks.get_Item(ref oBookMark[16]).Range.Text = model_2806.B_2_Thdiff + "";

                oDoc.Bookmarks.get_Item(ref oBookMark[17]).Range.Text = model_2806.C_1_Tm + "";
                oDoc.Bookmarks.get_Item(ref oBookMark[18]).Range.Text = model_2806.C_1_3 + "";
                oDoc.Bookmarks.get_Item(ref oBookMark[19]).Range.Text = model_2806.C_1_5 + "";
                oDoc.Bookmarks.get_Item(ref oBookMark[20]).Range.Text = model_2806.C_1_Tmdiff + "";
                oDoc.Bookmarks.get_Item(ref oBookMark[21]).Range.Text = model_2806.C_2_Tm + "";
                oDoc.Bookmarks.get_Item(ref oBookMark[22]).Range.Text = model_2806.C_2_Tmdiff + "";
                oDoc.Bookmarks.get_Item(ref oBookMark[23]).Range.Text = model_2806.C_3_Tm + "";
                oDoc.Bookmarks.get_Item(ref oBookMark[24]).Range.Text = model_2806.C_3_3 + "";
                oDoc.Bookmarks.get_Item(ref oBookMark[25]).Range.Text = model_2806.C_3_5 + "";
                oDoc.Bookmarks.get_Item(ref oBookMark[26]).Range.Text = model_2806.C_3_Tmdiff + "";
                oDoc.Bookmarks.get_Item(ref oBookMark[27]).Range.Text = model_2806.C_4_Tm + "";
                oDoc.Bookmarks.get_Item(ref oBookMark[28]).Range.Text = model_2806.C_4_Tmdiff + "";

                oDoc.Bookmarks.get_Item(ref oBookMark[29]).Range.Text = model_2806.H_1_Tm + "";
                oDoc.Bookmarks.get_Item(ref oBookMark[30]).Range.Text = model_2806.H_1_3 + "";
                oDoc.Bookmarks.get_Item(ref oBookMark[31]).Range.Text = model_2806.H_1_5 + "";
                oDoc.Bookmarks.get_Item(ref oBookMark[32]).Range.Text = model_2806.H_1_Tmdiff + "";
                oDoc.Bookmarks.get_Item(ref oBookMark[33]).Range.Text = model_2806.H_2_Tm + "";
                oDoc.Bookmarks.get_Item(ref oBookMark[34]).Range.Text = model_2806.H_2_Tmdiff + "";
                oDoc.Bookmarks.get_Item(ref oBookMark[35]).Range.Text = model_2806.H_3_Tm + "";
                oDoc.Bookmarks.get_Item(ref oBookMark[36]).Range.Text = model_2806.H_3_3 + "";
                oDoc.Bookmarks.get_Item(ref oBookMark[37]).Range.Text = model_2806.H_3_5 + "";
                oDoc.Bookmarks.get_Item(ref oBookMark[38]).Range.Text = model_2806.H_3_Tmdiff + "";
                oDoc.Bookmarks.get_Item(ref oBookMark[39]).Range.Text = model_2806.H_4_Tm + "";
                oDoc.Bookmarks.get_Item(ref oBookMark[40]).Range.Text = model_2806.H_4_Tmdiff + "";
                //弹出保存文件对话框，保存生成的Word
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Word Document(*.doc)|*.doc";
                sfd.DefaultExt = "Word Document(*.doc)|*.doc";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    object filename = sfd.FileName;

                    oDoc.SaveAs(ref filename, ref oMissing, ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing);
                    oDoc.Close(ref oMissing, ref oMissing, ref oMissing);
                    //关闭word
                    oWord.Quit(ref oMissing, ref oMissing, ref oMissing);
                    MessageBox.Show("保存成功!");
                    this.Close();
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return;
            }
            
        }
    }
}
