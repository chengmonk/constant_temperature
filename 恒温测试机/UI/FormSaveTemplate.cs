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
using 恒温测试机.Utils;

namespace 恒温测试机.UI
{
    public partial class FormSaveTemplate : Form
    {
        public FormSaveTemplate()
        {
            InitializeComponent();
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
                ////声明书签数组
                //object[] oBookMark = new object[3];
                ////赋值书签名
                //oBookMark[0] = "Name";
                //oBookMark[1] = "Born";
                //oBookMark[2] = "Like";
                ////赋值任意数据到书签的位置
                //oDoc.Bookmarks.get_Item(ref oBookMark[0]).Range.Text = "使用模板实现Word生成阿三多方位哦加哦绯闻啊我额技法及违法教务科啊王姐佛埃及文哦啊我了就哦啊我姐夫安慰剂阿娇位骄傲文件佛垃圾未就爱为房价未接咯啊就为奥";
                //oDoc.Bookmarks.get_Item(ref oBookMark[1]).Range.Text = "李四";
                //oDoc.Bookmarks.get_Item(ref oBookMark[2]).Range.Text = "女";
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
