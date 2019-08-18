using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using 恒温测试机.Utils;
using 恒温测试机.App;

namespace 恒温测试机
{
    public partial class FormPressureCurve : Form
    {
        public FormPressureCurve()
        {
            InitializeComponent();
        }

        public FormPressureCurve(DataTable dt)
        {
            InitializeComponent();
            this.graphDt = dt;
        }

        private DataTable graphDt;

        private void FormPressureCurve_Load(object sender, EventArgs e)
        {
            load_Data();
        }
        private void load_Data()
        {

            new Thread(new ThreadStart(ThreadReadExample1)) { IsBackground = true }.Start();

        }

        private void ThreadReadExample1()
        {
            //float[] Qc = new float[graphDt.Rows.Count];
            //float[] Qh = new float[graphDt.Rows.Count];
            float[] Qm = new float[graphDt.Rows.Count];
            //float[] Tc = new float[graphDt.Rows.Count];
            //float[] Th = new float[graphDt.Rows.Count];
            float[] Tm = new float[graphDt.Rows.Count];
            //float[] Tm2 = new float[graphDt.Rows.Count];
            //float[] Pc = new float[graphDt.Rows.Count];
            //float[] Ph = new float[graphDt.Rows.Count];
            //float[] Pm = new float[graphDt.Rows.Count];
            //float[] Qm5 = new float[graphDt.Rows.Count];
            //float[] Wh = new float[graphDt.Rows.Count];
            DateTime[] dateTime = new DateTime[graphDt.Rows.Count];
            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
            dtFormat.ShortDatePattern = "yyyy-MM-dd hh:mm:ss:fff";
            //加载流量数据
            for (int i = 0; i < graphDt.Rows.Count; i++)
            {
                try
                {
                    //Qc[i] = (float)Convert.ToDouble(graphDt.Rows[i][1]);
                    //Qh[i] = (float)Convert.ToDouble(graphDt.Rows[i][2]);
                    Qm[i] = (float)Convert.ToDouble(graphDt.Rows[i][3]);
                    //Tc[i] = (float)Convert.ToDouble(graphDt.Rows[i][4]);
                    //Th[i] = (float)Convert.ToDouble(graphDt.Rows[i][5]);
                    Tm[i] = (float)Convert.ToDouble(graphDt.Rows[i][6]);
                    //Tm2[i] = (float)Convert.ToDouble(graphDt.Rows[i][7]);
                    //Pc[i] = (float)Convert.ToDouble(graphDt.Rows[i][7]);
                    //Ph[i] = (float)Convert.ToDouble(graphDt.Rows[i][8]);
                    //Pm[i] = (float)Convert.ToDouble(graphDt.Rows[i][9]);
                    //Qm5[i] = (float)Convert.ToDouble(graphDt.Rows[i][10]);
                    //Wh[i] = (float)Convert.ToDouble(graphDt.Rows[i][11]);
                    // dateTime[i] = Convert.ToDateTime(dt.Rows[i][0],dtFormat);

                    dateTime[i] = DateTime.ParseExact((string)graphDt.Rows[i][0], "yyyy-MM-dd hh:mm:ss:fff", dtFormat);
                }
                catch(Exception ex){
                    Log.Error(ex.ToString());
                    return;
                }
            }
            //快速傅里叶变化滤波
            Console.WriteLine("1:" + Qm.Length);
            Qm = TWFFT.FFT_filter(Qm);
            Tm = TWFFT.FFT_filter(Tm);
            Console.WriteLine("2:" + Qm.Length);
            for (int i = 0; i < graphDt.Rows.Count; i++)
            {

            }



                DateTime start = dateTime[0];
            DateTime PointA = dateTime[0].AddSeconds(2);
            TimeSpan ts = new TimeSpan(1);

            // 显示出数据信息来
            Invoke(new Action(() =>
            {
                //总流量达到6L加载数据
                if (false)
                {
                    hslCurveHistory1.Text = "累计流量未达到6L...";
                    hslCurveHistory1.RemoveAllCurve();
                }
                else
                {
                   
                    hslCurveHistory1.Text = "正在加载数据...";
                    hslCurveHistory1.RemoveAllCurve();
                    hslCurveHistory1.SetLeftCurve("出水流量Qm", Qm, Color.DodgerBlue, true, "{0:F2} L/min");//布尔变量：是否开启曲线平滑
                    //hslCurveHistory1.SetLeftCurve("冷水流量Qc", Qc, Color.Tomato, true, "{0:F2} L/min");
                    //hslCurveHistory1.SetLeftCurve("热水流量Qh", Qh, Color.GreenYellow, true, "{0:F2} L/min");
                    //hslCurveHistory1.SetLeftCurve("Qm5", Qm5, Color.Purple, true, "{0:F2} L/min");
                    //hslCurveHistory1.SetLeftCurve("出水压力Pm", Pm, Color.Red, true, "{0:F2} Bar");
                    //hslCurveHistory1.SetLeftCurve("热水压力Ph", Ph, Color.Orange, true, "{0:F2} Bar");
                    //hslCurveHistory1.SetLeftCurve("冷水压力Pc", Pc, Color.Yellow, true, "{0:F2} Bar");
                    hslCurveHistory1.SetLeftCurve("出水温度Tm", Tm, Color.Red, true, "{0:F2} ℃");
                    //hslCurveHistory1.SetLeftCurve("出水温度Tm2", Tm2, Color.Yellow, true, "{0:F2} ℃");
                    //hslCurveHistory1.SetRightCurve("热水温度Th", Th, Color.Honeydew, true, "{0:F2} ℃");
                    //hslCurveHistory1.SetRightCurve("冷水温度Tc", Tc, Color.Pink, true, "{0:F2} ℃");
                    //hslCurveHistory1.SetRightCurve("液面高度Wh", Tc, Color.Blue, true, "{0:F2} mm");

                    hslCurveHistory1.SetDateTimes(dateTime);

                    // 增加一个三角形的线段标记示例 Points的每个点的X是数据索引，Y是数据值（需要选对参考坐标轴，默认为左坐标轴）                             



                    // 添加一个活动的标记
                    HslControls.HslMarkForeSection active = new HslControls.HslMarkForeSection()
                    {
                        StartIndex = 1000,
                        EndIndex = 1500,
                        Height = 0.9f,
                    };
                    //active.CursorTexts.Add("条码", "A123123124ashdiahsd是的iahsidasd");
                    //active.CursorTexts.Add("工号", "asd2sd123dasf");
                    //hslCurveHistory1.AddMarkActiveSection(active);

                    //hslCurveHistory1.SetCurveVisible("步序", false);   // 步序不是曲线信息，不用显示出来
                    //hslCurveHistory1.ValueMaxLeft = 10;
                    //hslCurveHistory1.ValueMinLeft = 0;

                    SetTempLight(28);
                    SetTempLight(29);
                    SetTempLight(30);
                    SetRemark(29f,28f,Tm);
                    hslCurveHistory1.SetScaleByXAxis((float)0.1);
                    hslCurveHistory1.RenderCurveUI();

                }



            }
            ));
        }

        /// <summary>
        /// 区间标注（超过或低于设定的温度）
        /// </summary>
        /// <param name="tempMax">设定的温度Max</param>
        /// <param name="tempMin">设定的温度Min</param>
        /// <param name="Tm">出水温度数组</param>
        private void SetRemark(float tempMax,float tempMin,float[] Tm)
        {
            bool isHigh = false;
            int highStartIndex = 0;
            int highEndIndex = 0;

            bool islow = false;
            int belowStartIndex = 0;
            int belowEndIndex = 0;

            for(int i = 0; i < Tm.Length; i++)
            {
                if ((isHigh == false) && (Tm[i] > tempMax))
                {
                    isHigh = true;
                    highStartIndex = i;
                }
                if(isHigh&&(Tm[i] <= tempMax))
                {
                    isHigh = false;
                    highEndIndex = i;
                    int totalTimes = (highEndIndex - highStartIndex) * 10;
                    HslControls.HslMarkForeSection foreSection = new HslControls.HslMarkForeSection()       //区间表示操作
                    {
                        StartIndex = highStartIndex,
                        EndIndex = highEndIndex,
                        StartHeight = 0.3f,         // 如果值是(0-1)的话，表示的是位置百分比，0.9就是曲线高度为90%，从上往下看的视角，如果填了600，那就是绝对坐标
                        Height = 0.4f,              // 和上面同理
                        LinePen = Pens.Chocolate,   // 指定颜色
                        IsRenderTimeText = false,   // 是否显示额外的起始时间和结束时间，此处就不要了
                        MarkText = "温度超标了:"+ totalTimes+"ms",
                    };
                    hslCurveHistory1.AddMarkForeSection(foreSection);
                }

                if ((islow == false) && (Tm[i] < tempMin))
                {
                    islow = true;
                    belowStartIndex = i;
                }
                if (islow && (Tm[i] >= tempMin))
                {
                    islow = false;
                    belowEndIndex = i;
                    int totalTimes = (belowEndIndex - belowStartIndex) * 10;
                    HslControls.HslMarkForeSection foreSection = new HslControls.HslMarkForeSection()       //区间表示操作
                    {
                        StartIndex = belowStartIndex,
                        EndIndex = belowEndIndex,
                        StartHeight = 0.3f,         // 如果值是(0-1)的话，表示的是位置百分比，0.9就是曲线高度为90%，从上往下看的视角，如果填了600，那就是绝对坐标
                        Height = 0.9f,              // 和上面同理
                        LinePen = Pens.Chocolate,   // 指定颜色
                        IsRenderTimeText = false,   // 是否显示额外的起始时间和结束时间，此处就不要了
                        MarkText = "温度过低了:" + totalTimes + "ms",
                    };
                    hslCurveHistory1.AddMarkForeSection(foreSection);
                }
            }
            
        }

        /// <summary>
        /// 画虚线
        /// </summary>
        private void SetTempLight(float temp)
        {
            hslCurveHistory1.AddLeftAuxiliary(temp);
        }

        private void HslButton1_Click(object sender, EventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "图片|*.png";
            fileDialog.InitialDirectory = Application.StartupPath;
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                hslCurveHistory1.SaveToBitmap().Save(fileDialog.FileName);
                MessageBox.Show("保存成功!");
            }
            fileDialog.Dispose();
        }
        float xAxis = 1;
        private void HslButton4_Click(object sender, EventArgs e)
        {

            hslCurveHistory1.SetScaleByXAxis((float)(xAxis /2));
            hslCurveHistory1.RenderCurveUI();
        }

        private void HslButton3_Click(object sender, EventArgs e)
        {
            //hslCurveHistory1.SetScaleByXAxis(++xAxis > 0 ? xAxis : (xAxis = 1));
            hslCurveHistory1.SetScaleByXAxis((float)(xAxis*2));
            hslCurveHistory1.RenderCurveUI();
        }
    }
}
