using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 恒温测试机.Utils;

namespace 恒温测试机.UI
{
    public partial class FormDOControl : Form
    {
        public FormDOControl(FormMain formMain)
        {
            InitializeComponent();
            InitData();

            this.formMain = formMain;
            ////将当前打开的开关全部关闭
            //formMain.doData[1] = 0;
            //formMain.doData[2] = 0;
            //formMain.doData[3] = 0;
            //formMain.control.InstantDo_Write(formMain.doData);
            InitEvent();
            InitTimer();
        }
        private FormMain formMain;

        private Dictionary<string, int> DOIndexDict = new Dictionary<string, int>();
        private Dictionary<int, bool> DOOpenDict = new Dictionary<int, bool>();
        public void InitEvent()     //改成定时器轮询
        {
            foreach (Control c in this.Controls)
            {
                Button btn = c as Button;
                if (btn != null)
                {
                    //btn.MouseDown+= new System.Windows.Forms.MouseEventHandler(this.DOBtn_MouseDown);
                    //btn.MouseUp+= new System.Windows.Forms.MouseEventHandler(this.DOBtn_MouseUp);
                    btn.MouseClick += new System.Windows.Forms.MouseEventHandler(this.DOBtn_Click);
                }
            }
        }
        System.Timers.Timer monitorTimer;            //监控主界面 doData
        public void InitTimer()
        {
            monitorTimer = new System.Timers.Timer(1000);
            monitorTimer.Elapsed += (o, a) =>
            {
                MonitorActive();
            };//到达时间的时候执行事件； 
            monitorTimer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；
            monitorTimer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
        }
        private delegate void MonitorActiveDelegate();//doData的状态监控
        private void MonitorActive()
        {
            if (this.InvokeRequired)
            {
                MonitorActiveDelegate monitorActiveDelegate = MonitorActive;
                this.Invoke(monitorActiveDelegate);
            }
            else
            {
                foreach (Control c in this.Controls)
                {
                    Button btn = c as Button;
                    if (btn != null)
                    {
                        var val = formMain.doData[GetFirstIndex(btn.Text)].get_bit(GetSecondIndex(btn.Text));
                        if (val == 1)
                        {
                            btn.BackColor = Color.Green;
                        }
                    }
                }
            }
        }

        public void InitData()
        {
            DOIndexDict.Add("冷水制冷", 0);
            DOIndexDict.Add("热水加热", 1);
            DOIndexDict.Add("高温加热", 2);
            DOIndexDict.Add("中温加热", 3);
            DOIndexDict.Add("常温制冷", 4);
            DOIndexDict.Add("进冷水阀", 5);
            DOIndexDict.Add("进热水阀", 6);
            DOIndexDict.Add("进高温阀", 7);


            DOIndexDict.Add("进中温阀", 8);
            DOIndexDict.Add("进常温阀", 9);
            DOIndexDict.Add("冷循环泵", 10);
            DOIndexDict.Add("热循环泵", 11);
            DOIndexDict.Add("高循环泵", 12);
            DOIndexDict.Add("中循环泵", 13);
            DOIndexDict.Add("常循环泵", 14);
            DOIndexDict.Add("热水阀", 15);

            DOIndexDict.Add("变压热水阀", 16);
            DOIndexDict.Add("冷水阀", 17);
            DOIndexDict.Add("变压冷水阀", 18);
            DOIndexDict.Add("冷水进水阀", 19);
            DOIndexDict.Add("热水进水阀", 20);
            DOIndexDict.Add("出水阀", 21);
            DOIndexDict.Add("5S出水阀", 22);
            DOIndexDict.Add("冷水泵", 23);

            DOIndexDict.Add("冷水变压泵", 24);
            DOIndexDict.Add("热水泵", 25);
            DOIndexDict.Add("热水变压泵", 26);
            //DOIndexDict.Add("液面采集切换开关", 27);
            DOIndexDict.Add("冷水箱进水阀", 28);
            DOIndexDict.Add("热水箱进水阀", 29);
        }


        private int GetFirstIndex(string name)
        {
            return DOIndexDict[name] / 8;
        }

        private int GetSecondIndex(string name)
        {
            return DOIndexDict[name] % 8;
        }


        /// <summary>
        /// 设置某一位的值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index">要设置的位， 值从低到高为 0-7</param>
        /// <param name="flag">要设置的值 true / false</param>
        /// 
        /// <returns></returns>
        void set_bit(ref byte data, int index, bool flag)
        {
            index++;
            if (index > 8 || index < 1)
                throw new ArgumentOutOfRangeException();
            int v = index < 2 ? index : (2 << (index - 2));
            data = flag ? (byte)(data | v) : (byte)(data & ~v);

        }
        /// <summary>
        /// 获取数据中某一位的值
        /// </summary>
        /// <param name="input">传入的数据类型,可换成其它数据类型,比如Int</param>
        /// <param name="index">要获取的第几位的序号,从0开始 0-7</param>
        /// <returns>返回值为-1表示获取值失败</returns>
        int get_bit(byte input, int index)
        {
            //if (index > sizeof(byte))
            //{
            //    return -1;
            //}

            return ((input & (1 << index)) > 0) ? 1 : 0;
        }

        private bool CheckClick(string name)
        {
            #region 液面过高时，无法开启进水阀
            if (((name == "冷水箱进水阀")
                    && (formMain.WhCool > (double)Properties.Settings.Default.WhMax))||(
                    (name == "热水箱进水阀")
                    && (formMain.WhHeat > (double)Properties.Settings.Default.WhMax)
                    ))
            {
                MessageBox.Show("液面过高，无法开启");
                return true;
            }
            #endregion

            #region 冷水箱液面过低时，无法打开
            if(((name=="冷水泵")|| (name == "冷循环泵")|| (name == "冷水制冷")||(name=="冷水变压泵"))&&
                (formMain.WhCool < (double)Properties.Settings.Default.WhMin))
            {
                MessageBox.Show("冷水箱液面过低，无法开启");
                return true;
            }

            #endregion

            #region 热水箱液面过低时，无法打开
            if (((name == "热水泵") || (name == "热循环泵") || (name == "热水加热")||(name=="热水变压泵")) &&
                (formMain.WhCool < (double)Properties.Settings.Default.WhMin))
            {
                MessageBox.Show("热水箱液面过低，无法开启");
                return true;
            }

            #endregion

            #region 数字量输入报警，冷水泵、冷水变压泵、热水泵、热水变压泵 无法开启
            if ((name == "冷水泵") && !formMain.isAlarm011)
            {
                MessageBox.Show("冷水泵报警，无法开启");
                return true;
            }
            if ((name == "冷水变压泵") && formMain.isAlarm012)
            {
                MessageBox.Show("冷水变压泵报警，无法开启");
                return true;
            }
            if ((name == "热水泵") && formMain.isAlarm021)
            {
                MessageBox.Show("热水泵报警，无法开启");
                return true;
            }
            if ((name == "热水变压泵") && formMain.isAlarm022)
            {
                MessageBox.Show("热水变压泵报警，无法开启");
                return true;
            }

            #endregion

            #region 热水加热
            if ((name == "热水加热"))
            {
                if (formMain.Temp2 >= (double)(Properties.Settings.Default.Temp2Set))
                {
                    MessageBox.Show("热水箱温度已符合设定温度，无法继续加热");
                    return true;
                }
                if (formMain.WhHeat < (double)Properties.Settings.Default.WhMin)
                {
                    MessageBox.Show("热水箱液面过低，无法开启");
                    return true;
                }
                if (formMain.doData[1].get_bit(3) == 0)
                {
                    MessageBox.Show("请先开启热循环泵");
                    return true;
                }
            }
            if((name=="热循环泵")&& (formMain.WhHeat < (double)Properties.Settings.Default.WhMin))
            {
                MessageBox.Show("热水箱液面过低，无法开启");
                return true;
            }
            #endregion

            #region 高温加热
            if ((name == "高温加热"))
            {
                if (formMain.Temp3 >= (double)(Properties.Settings.Default.Temp3Set))
                {
                    MessageBox.Show("高温水箱温度已符合设定温度，无法继续加热");
                    return true;
                }
                if (formMain.doData[1].get_bit(4) == 0)
                {
                    MessageBox.Show("请先开启高循环泵");
                    return true;
                }
            }
            #endregion

            #region 中温加热
            if ((name == "中温加热"))
            {
                if (formMain.Temp4 >= (double)(Properties.Settings.Default.Temp4Set))
                {
                    MessageBox.Show("中温水箱温度已符合设定温度，无法继续加热");
                    return true;
                }
                if (formMain.doData[1].get_bit(5) == 0)
                {
                    MessageBox.Show("请先开启中循环泵");
                    return true;
                }
            }
            #endregion

            #region 冷水制冷
            if ((name == "冷水制冷"))
            {
                if (formMain.Temp1 <= (double)(Properties.Settings.Default.Temp1Set))
                {
                    MessageBox.Show("冷水箱温度已符合设定温度，无法继续制冷");
                    return true;
                }
                if (formMain.WhCool < (double)Properties.Settings.Default.WhMin)
                {
                    MessageBox.Show("冷水箱液面过低，无法开启");
                    return true;
                }
                if (formMain.doData[1].get_bit(2) == 0)
                {
                    MessageBox.Show("请先开启冷循环泵");
                    return true;
                }
            }
            if ((name == "冷循环泵") && (formMain.WhCool < (double)Properties.Settings.Default.WhMin))
            {
                MessageBox.Show("冷水箱液面过低，无法开启");
                return true;
            }
            #endregion

            #region 常温制冷
            if ((name == "常温制冷"))
            {
                if (formMain.Temp5 <= (double)(Properties.Settings.Default.Temp5Set))
                {
                    MessageBox.Show("常温制冷温度已符合设定温度，无法继续制冷");
                    return true;
                }
                if (formMain.doData[1].get_bit(6) == 0)
                {
                    MessageBox.Show("请先开启常循环泵");
                    return true;
                }
            }
            #endregion

            return false;
        }

        private void DOBtn_Click(object sender, MouseEventArgs e)
        {
            
            Button btn = sender as Button;
            var val = formMain.doData[GetFirstIndex(btn.Text)].get_bit(GetSecondIndex(btn.Text));
            Console.WriteLine("Click Before:" + val);
            if (btn.BackColor == Color.LightGray)     // 关->开
            {
                if (CheckClick(btn.Text))
                    return;

                btn.BackColor = Color.Green;
                set_bit(ref formMain.doData[GetFirstIndex(btn.Text)], GetSecondIndex(btn.Text), true);
                formMain.control.InstantDo_Write(formMain.doData);
                if (DOOpenDict.ContainsKey(DOIndexDict[btn.Text]) == false)
                {
                    DOOpenDict.Add(DOIndexDict[btn.Text], true);
                }
            }
            else
            {
                btn.BackColor = Color.LightGray;
                set_bit(ref formMain.doData[GetFirstIndex(btn.Text)], GetSecondIndex(btn.Text), false);
                formMain.control.InstantDo_Write(formMain.doData);
                if (DOOpenDict.ContainsKey(DOIndexDict[btn.Text]))
                {
                    DOOpenDict.Remove(DOIndexDict[btn.Text]);
                }
            }
            val = formMain.doData[GetFirstIndex(btn.Text)].get_bit(GetSecondIndex(btn.Text));
            Console.WriteLine("Click Afters:" + val);
        }

        private void TrackBar1_ValueChanged(object sender, EventArgs e)
        {
            label4.Text = trackBar1.Value.ToString() + "%";
            var value = Convert.ToDouble(trackBar1.Value.ToString()) * 0.1;
            formMain.AO_Func(0, value);            //输出模拟量
        }

        private void TrackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            var value = Convert.ToDouble(trackBar1.Value.ToString()) * 0.1;
            formMain.AO_Func(0,value);            //输出模拟量
            //MessageBox.Show("对应0-10V 电压值："+value);
        }

        private void FormDOControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                //将当前打开的开关全部关闭
                foreach (var item in DOOpenDict.Keys)
                {
                    set_bit(ref formMain.doData[item / 8], item % 8, false);
                    Console.WriteLine("doData[" + item / 8 + "]" + (item % 8));
                }
                formMain.control.InstantDo_Write(formMain.doData);
                formMain.IsOpenDC = false;
                monitorTimer.Enabled = false;
                monitorTimer.Dispose();
            }
            catch(Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }
}
