using Automation.BDaq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnscentedKalmanFilter;
using 恒温测试机.App;
using 恒温测试机.Model;
using 恒温测试机.Model.Enum;
using 恒温测试机.Utils;

namespace 恒温测试机.UI
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            InitControl();
            InitData();
            InitTimer();
            //Load_settingForm();
        }

        private void Load_settingForm()
        {

            new Thread(new ThreadStart(LoadSettingForm)) { IsBackground = true }.Start();

        }
        private delegate void LoadSettingFormDelegate();//开启通讯控制
        private void LoadSettingForm()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    LoadSettingFormDelegate loadSettingFormDelegate = LoadSettingForm;
                    this.Invoke(loadSettingFormDelegate);
                }
                else
                {
                    settingForm = new FormConnectValueSetting(this);
                    settingForm.Show();
                }
            }
            catch (Exception ex)
            {
                Log.Error("开启通讯控制异常：" + ex.ToString());
                return;
            }
        }

        #region 变量
        LogicTypeEnum logicType = LogicTypeEnum.safeTest;
        public TestStandardEnum testStandard = TestStandardEnum.default2806;
            
        System.Timers.Timer safetyTimer;             //安全性测试 定时器               1111     2806    1016
        System.Timers.Timer pressureTimer;           //压力变化测试 定时器             1111     2806     1016
        System.Timers.Timer coolTimer;               //降温测试 定时器                 1111
        System.Timers.Timer steadyTimer;             //温度稳定性测试 定时器           1111
        System.Timers.Timer flowTimer;               //流量减少测试 定时器             1111

        System.Timers.Timer heatTimer;                  //升温测试 定时器              2806     1016
        System.Timers.Timer maxHeatTimer;               //最高限温测试 定时器           2806     1016

        System.Timers.Timer changeTmTimer;               //温度变化测试                 1016

        System.Timers.Timer monitorWhTimer;          //监控液面高度定时器
        System.Timers.Timer monitorDiTimer;          //监控数字量定时器
        System.Timers.Timer monitorTimer;            //监控管道泵 阀  状态
        COMconfig bpq_conf;
        public M_485Rtu bpq;
        public DAQ_profile collectData;
        public DAQ_profile control;
        public byte[] doData = new byte[4];    //数字量输出数据
        public byte[] diData = new byte[4];    //数字量输入数据
        double[] aoData = new double[2];           //模拟量输出数据

        private config collectConfig;
        private config controlConfig;
        public const int CHANNEL_COUNT_MAX = 16;
        private double[] m_dataScaled = new double[CHANNEL_COUNT_MAX];

        bool collectDataFlag = false;           //是否采集数据
        bool runFlag = false;                   //是否执行流程

        bool graphFlag = false;          //
        bool whFlag = true;// ture表示 当前为热水箱液面 flase表示 当前为冷水箱液面 
        bool autoRunFlag = false;               //是否自动运行
        bool stopFlag = false;                  //是否手动停止

        public DataTable dt;
        public DataTable GraphDt;
        public DataTable ElectDt;
        public double Temp1;
        public double Temp2;
        public double Temp3;
        public double Temp4;
        public double Temp5;
        double Pm;
        double Pc;
        double Ph;
        double Tm;
        double Tc;
        double Th;
        double Qm;
        double Qc;
        double Qh;
        double Qm5;
        public double Wh;
        public double WhHeat;
        public double WhCool;
        public double SteadyTm; //稳定后的出水温度
        int index;
        int startIndex;
        int endIndex;

        public Dictionary<string, DataTable> analyseDataDic;
        public Dictionary<LogicTypeEnum, string> analyseReportDic = new Dictionary<LogicTypeEnum, string>();
        public DataReportAnalyseApp dataReportAnalyseApp;

        public static Model_2806 model_2806 = new Model_2806();         //2806导出报告模板

        #endregion

        #region 委托
        private delegate void MonitorActiveDelegate();//doData的状态监控
        private void MonitorActive()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    MonitorActiveDelegate monitorActiveDelegate = MonitorActive;
                    this.Invoke(monitorActiveDelegate);
                }
                else
                {
                    #region 管道状态
                    //进冷水阀
                    var val = doData[0].get_bit(5);
                    if (val == 1)
                        hslValves8.EdgeColor = Color.DodgerBlue;
                    else
                        hslValves8.EdgeColor = Color.Gray;
                    //进热水阀
                    val = doData[0].get_bit(6);
                    if (val == 1)
                        hslValves1.EdgeColor = Color.Red;
                    else
                        hslValves1.EdgeColor = Color.Gray;
                    //进高温阀
                    val = doData[0].get_bit(7);
                    if (val == 1)
                        hslValves2.EdgeColor = Color.Red;
                    else
                        hslValves2.EdgeColor = Color.Gray;
                    //进中温阀
                    val = doData[1].get_bit(0);
                    if (val == 1)
                        hslValves3.EdgeColor = Color.Red;
                    else
                        hslValves3.EdgeColor = Color.Gray;
                    //进常温阀
                    val = doData[1].get_bit(1);
                    if (val == 1)
                        hslValves7.EdgeColor = Color.DodgerBlue;
                    else
                        hslValves7.EdgeColor = Color.Gray;
                    //冷循环泵
                    val = doData[1].get_bit(2);
                    if (val == 1)
                        hslPumpOne3.MoveSpeed = 1;
                    else
                        hslPumpOne3.MoveSpeed = 0;
                    //热循环泵
                    val = doData[1].get_bit(3);
                    if (val == 1)
                        hslPumpOne2.MoveSpeed = 1;
                    else
                        hslPumpOne2.MoveSpeed = 0;
                    //高循环泵
                    val = doData[1].get_bit(4);
                    if (val == 1)
                        hslPumpOne4.MoveSpeed = 1;
                    else
                        hslPumpOne4.MoveSpeed = 0;
                    //中循环泵
                    val = doData[1].get_bit(5);
                    if (val == 1)
                        hslPumpOne1.MoveSpeed = 1;
                    else
                        hslPumpOne1.MoveSpeed = 0;
                    //常循环泵
                    val = doData[1].get_bit(6);
                    if (val == 1)
                        hslPumpOne5.MoveSpeed = 1;
                    else
                        hslPumpOne5.MoveSpeed = 0;
                    //热水阀
                    val = doData[1].get_bit(7);
                    if (val == 1)
                        hslValves4.EdgeColor = Color.Red;
                    else
                        hslValves4.EdgeColor = Color.Gray;
                    //变压热水阀
                    val = doData[2].get_bit(0);
                    if (val == 1)
                        hslValves5.EdgeColor = Color.Red;
                    else
                        hslValves5.EdgeColor = Color.Gray;
                    //冷水阀
                    val = doData[2].get_bit(1);
                    if (val == 1)
                        hslValves10.EdgeColor = Color.DodgerBlue;
                    else
                        hslValves10.EdgeColor = Color.Gray;
                    //变压冷水阀
                    val = doData[2].get_bit(2);
                    if (val == 1)
                        hslValves9.EdgeColor = Color.DodgerBlue;
                    else
                        hslValves9.EdgeColor = Color.Gray;
                    //冷水进水阀
                    val = doData[2].get_bit(3);
                    if (val == 1)
                        hslValves13.EdgeColor = Color.DodgerBlue;
                    else
                        hslValves13.EdgeColor = Color.Gray;
                    //热水进水阀
                    val = doData[2].get_bit(4);
                    if (val == 1)
                        hslValves6.EdgeColor = Color.Red;
                    else
                        hslValves6.EdgeColor = Color.Gray;
                    //出水阀
                    val = doData[2].get_bit(5);
                    if (val == 1)
                        hslValves11.EdgeColor = Color.Goldenrod;
                    else
                        hslValves11.EdgeColor = Color.Gray;
                    //5s出水阀
                    val = doData[2].get_bit(5);//通道改变，详情见readme
                    if (val != 1)
                        hslValves12.EdgeColor = Color.Goldenrod;
                    else
                        hslValves12.EdgeColor = Color.Gray;
                    //冷水泵
                    val = doData[2].get_bit(7);
                    if (val == 1)
                        hslPumpOne8.MoveSpeed = 1;
                    else
                        hslPumpOne8.MoveSpeed = 0;
                    //冷水变压泵
                    val = doData[3].get_bit(0);
                    if (val == 1)
                        hslPumpOne6.MoveSpeed = 1;
                    else
                        hslPumpOne6.MoveSpeed = 0;
                    //热水泵
                    val = doData[3].get_bit(1);
                    if (val == 1)
                        reshui.MoveSpeed = 1;
                    else
                        reshui.MoveSpeed = 0;
                    //热水变压泵
                    val = doData[3].get_bit(2);
                    if (val == 1)
                        hslPumpOne7.MoveSpeed = 1;
                    else
                        hslPumpOne7.MoveSpeed = 0;
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Log.Error("管道监控异常：" + ex.ToString());
                return;
            }
        }

        private delegate void MonitorWhActiveDelegate();//液面高度状态的监控，进行对应行为逻辑
        private void MonitorWhActive()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    MonitorWhActiveDelegate monitorWhActiveDelegate = MonitorWhActive;
                    this.Invoke(monitorWhActiveDelegate);
                }
                else
                {
                    //冷水箱进水阀 28 - 4
                    //热水箱进水阀 29 - 5
                    if (Wh < (double)Properties.Settings.Default.WhMin)  //液面高度小于下限时，关闭加热、制冷功能，关闭对应泵
                    {
                        if (whFlag)
                        {
                            set_bit(ref doData[0], 1, false);       //热水加热
                            set_bit(ref doData[3], 1, false);       //热水泵
                            set_bit(ref doData[1], 3, false);       //热循环泵
                        }
                        else
                        {
                            set_bit(ref doData[0], 0, false);       //冷水制冷
                            set_bit(ref doData[2], 7, false);       //冷水泵
                            set_bit(ref doData[1], 2, false);       //冷循环泵
                        }
                    }

                    if (Wh > (double)Properties.Settings.Default.WhMax) //液面高度大于上限时，关闭加水
                    {
                        if (whFlag)
                            set_bit(ref doData[3], 5, false);
                        else
                            set_bit(ref doData[3], 4, false);
                    }
                    if (whFlag) //热水箱->冷水箱
                    {
                        WhHeat = Wh;
                        set_bit(ref doData[3], 3, false);//wh
                    }
                    else
                    {
                        WhCool = Wh;
                        set_bit(ref doData[3], 3, true);//wh
                    }
                    whFlag = !whFlag;
                    control.InstantDo_Write(doData);
                }
            }
            catch (Exception ex)
            {
                Log.Error("液面监控异常：" + ex.ToString());
                return;
            }
        }

        public bool isAlarm011 = false;//冷水泵
        public bool isAlarm012 = false;//冷水变压泵
        public bool isAlarm021 = false;//热水泵
        public bool isAlarm022 = false;//热水变压泵
        public bool isAlarmA = false;//伺服电机A
        //public bool isAlarmL = false;//伺服电机L
        private delegate void MonitorDiActiveDelegate();//数字量输入，进行对应行为逻辑
        private void MonitorDiActive()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    MonitorDiActiveDelegate monitorDiActiveDelegate = MonitorDiActive;
                    this.Invoke(monitorDiActiveDelegate);
                }
                else
                {
                    diData[0] = control.InstantDi_Read();//读取数字量函数
                    //监控数字量
                    if (diData[0].get_bit(7) == 0)
                    {
                        set_bit(ref doData[2], 7, false);
                        control.InstantDo_Write(doData);
                        isAlarm011 = true;
                    }
                    else
                    {
                        isAlarm011 = false;
                    }

                    if ((diData[0].get_bit(4) == 0))
                    {
                        set_bit(ref doData[3], 0, false);
                        control.InstantDo_Write(doData);
                        isAlarm012 = true;
                    }
                    else
                    {
                        isAlarm012 = false;
                    }

                    if ((diData[0].get_bit(5) == 0))
                    {
                        set_bit(ref doData[3], 1, false);
                        control.InstantDo_Write(doData);
                        isAlarm021 = true;
                    }
                    else
                    {
                        isAlarm021 = false;
                    }

                    if ((diData[0].get_bit(6) == 0))
                    {
                        set_bit(ref doData[3], 2, false);
                        control.InstantDo_Write(doData);
                        isAlarm022 = true;
                    }
                    else
                    {
                        isAlarm022 = false;
                    }
                    if ((diData[0].get_bit(0) == 0))
                    {
                        //bpq.write_coil("2048", false, 5);
                        //bpq.write_coil("2049", false, 5);
                        //bpq.write_coil("2050", false, 5);
                        //bpq.write_coil("2051", false, 5);
                        //bpq.write_coil("2052", false, 5);
                        isAlarmA = true;
                    }
                    else
                    {
                        isAlarmA = false;
                    }
                    //if ((diData[0].get_bit(1) == 0))
                    //{
                    //    bpq.write_coil("2058", false, 5);
                    //    bpq.write_coil("2059", false, 5);
                    //    bpq.write_coil("2060", false, 5);
                    //    bpq.write_coil("2061", false, 5);
                    //    bpq.write_coil("2062", false, 5);
                    //    isAlarmM = true;
                    //}
                    //else
                    //{
                    //    isAlarmM = false;
                    //}
                    //if ((diData[0].get_bit(2) == 0))
                    //{
                    //    Console.WriteLine("伺服电机M报警");

                    //}
                    if (isAlarm011)
                        Console.WriteLine("冷水泵报警");
                    if (isAlarm012)
                        Console.WriteLine("冷水变压泵报警");
                    if (isAlarm021)
                        Console.WriteLine("热水泵报警");
                    if (isAlarm022)
                        Console.WriteLine("热水变压泵报警");
                    if (isAlarmA)
                        Console.WriteLine("伺服电机A报警");
                    //if (isAlarmM)
                    //    Console.WriteLine("伺服电机L报警");
                }
            }
            catch (Exception ex)
            {
                Log.Error("数字量输入监控异常:" + ex.ToString());
                return;
            }
        }

        private delegate void SystemInfoPrintDelegate(string s);//记录数据  
        private void SystemInfoPrint(string msg)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    SystemInfoPrintDelegate systemInfoDelegate = SystemInfoPrint;
                    this.Invoke(systemInfoDelegate, new object[] { msg });
                }
                else
                {
                    systemInfoTb.AppendText("[时间:" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "] " + msg);
                    systemInfoTb.AppendText("\r\n");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return;
            }

        }

        private delegate void ChangeRadioButtonDelegate();          //自动切换按钮
        private void ChangeRadioButton()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    ChangeRadioButtonDelegate changeRadioButtonDelegate = ChangeRadioButton;
                    this.Invoke(changeRadioButtonDelegate);
                }
                else
                {
                    if (safeTestRbt.Checked)
                    {
                        safeTestRbt.Checked = false;
                        pressureTestRbt.Checked = true;
                    }
                    if (pressureTestRbt.Checked)
                    {
                        pressureTestRbt.Checked = false;
                        coolTestRbt.Checked = true;
                    }
                    if (coolTestRbt.Checked)
                    {
                        coolTestRbt.Checked = false;
                        tmpTestRbt.Checked = true;
                    }
                    if (tmpTestRbt.Checked)
                    {
                        tmpTestRbt.Checked = false;
                        FlowTestRbt.Checked = true;
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        private delegate void DataReadyDelegate();//数据采集委托  
        private void DataReadyToUpdateStatus()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    DataReadyDelegate dataReadyDelegate = DataReadyToUpdateStatus;
                    this.Invoke(dataReadyDelegate);
                }
                else
                {
                    QmShow.Text = Qm.ToString();
                    QcShow.Text = Qc.ToString();
                    QhShow.Text = Qh.ToString();
                    TmShow.Text = Tm.ToString();
                    TcShow.Text = Tc.ToString();
                    ThShow.Text = Th.ToString();
                    PmShow.Text = Pm.ToString();
                    PcShow.Text = Pc.ToString();
                    PhShow.Text = Ph.ToString();


                    #region LED显示
                    hslLedQm.DisplayText = Qm.ToString();
                    hslLedQc.DisplayText = Qc.ToString();
                    hslLedQh.DisplayText = Qh.ToString();
                    hslLedTm.DisplayText = Tm.ToString();
                    hslLedTc.DisplayText = Tc.ToString();
                    hslLedTh.DisplayText = Th.ToString();
                    hslLedPm.DisplayText = Pm.ToString();
                    hslLedPc.DisplayText = Pc.ToString();
                    hslLedPh.DisplayText = Ph.ToString();
                    hslLedQm5.DisplayText = Qm5.ToString();
                    #endregion

                    //if (graphFlag)
                    //{
                    for (int i = 3; i < 103; i++)
                    {
                        var qcTemp = CoolFlow[i - 3] + (double)Properties.Settings.Default.QcAdjust;
                        var qhTemp = HotFlow[i - 3] + (double)Properties.Settings.Default.QhAdjust;
                        var qmTemp = (sourceDataQm[i] - 1) < 0 ? 0 : (sourceDataQm[i] - 1) * 5;
                        hslCurve1.AddCurveData(
                            new string[] {
                                    //"冷水箱温度","热水箱温度","高温水箱温度","中温水箱温度","常温水箱温度"
                                    "冷水流量Qc", "热水流量Qh", "出水流量Qm",
                                    "冷水温度Tc", "热水温度Th", "出水温度Tm",
                                    "冷水压力Pc", "热水压力Ph", "出水压力Pm",
                                    //"出水重量Qm5"
                            },
                            new float[]
                            {
                                //(float)sourceDataTemp1[i],(float)sourceDataTemp2[i],(float)sourceDataTemp3[i]
                                //,(float)sourceDataTemp4[i],(float)sourceDataTemp5[i]
                                    //(float)Qc,(float)Qh,(float)Qm,
                                    //(float)Tc,(float)Th,(float)Tm,
                                    //(float)Pc,(float)Ph,(float)Pm,
                                    //(float)Qm5
                                    //(float)Qm5
                                    (float)qcTemp,
                                (float)qhTemp,
                                (float)qmTemp,
                                    (float)sourceDataTc[i]*10,(float)sourceDataTh[i]*10,(float)sourceDataTm[i]*10,
                                    //(float)sourceDataPc[i],(float)sourceDataPh[i],
                                    (float)Pc,(float)Ph,
                                (float)sourceDataPm[i],
                                    //(float)sourceDataQm5[i]
                            }
                        );
                    }
                    DataReadyToControlTemp();
                }
            }
            catch (Exception ex)
            {
                Log.Error("数据采集异常:" + ex.ToString());
                return;
            }

        }
        public bool IsOpenDC = false;
        private void DataReadyToControlTemp()   //程序根据按钮颜色决定是否自动控制温度
        {
            if (Temp1 <= (double)(Properties.Settings.Default.Temp1Set))
            {
                set_bit(ref doData[0], 0, false);
                //set_bit(ref doData[1], 2, false);    //冷循环泵
            }
            else
            {
                //Temp1高于预设温度时，根据是否打开冷水制冷按钮决定是否控制温度
                //冷水液面要高于下限，方可制冷
                if ((WhCool >= (double)Properties.Settings.Default.WhMin)
                    && doData00Flag)
                {
                    set_bit(ref doData[0], 0, true);
                    set_bit(ref doData[1], 2, true);    //冷循环泵
                }
            }
            if (Temp2 >= (double)(Properties.Settings.Default.Temp2Set))
            {
                set_bit(ref doData[0], 1, false);
                //set_bit(ref doData[1], 3, false);    //热循环泵
            }
            else
            {
                //Temp2低于预设温度时，根据是否打开热水加热按钮决定是否控制温度
                //热水液面要高于下限，方可加热
                if((WhHeat>=(double)Properties.Settings.Default.WhMin)
                    && doData01Flag)
                {
                    set_bit(ref doData[0], 1, true);
                    set_bit(ref doData[1], 3, true);    //热循环泵
                }
            }

            if (Temp3 >= (double)(Properties.Settings.Default.Temp3Set))
            {
                set_bit(ref doData[0], 2, false);
                //set_bit(ref doData[1], 4, false);    //高循环泵
            }
            else
            {
                if (doData02Flag)
                {
                    set_bit(ref doData[0], 2, true);
                    set_bit(ref doData[1], 4, true);    //高循环泵
                }
            }

            if (Temp4 >= (double)(Properties.Settings.Default.Temp4Set))
            {
                set_bit(ref doData[0], 3, false);
                //set_bit(ref doData[1], 5, false);    //中循环泵
            }
            else
            {
                if (doData03Flag)
                {
                    set_bit(ref doData[0], 3, true);
                    set_bit(ref doData[1], 5, true);    //中循环泵
                }
            }
            if (Temp5 <= (double)(Properties.Settings.Default.Temp5Set))
            {
                set_bit(ref doData[0], 4, false);
                //set_bit(ref doData[1], 6, false);    //常循环泵
            }
            else
            {
                if (doData04Flag)
                {
                    set_bit(ref doData[0], 4, false);
                    set_bit(ref doData[1], 6, false);    //常循环泵
                }
            }
            control.InstantDo_Write(doData);
            Temp1Status.Text = Temp1 + "℃\n";
            label1.Text = WhCool + "mm";
            Temp2Status.Text = Temp2 + "℃\n";
            label5.Text = WhHeat + "mm";
            Temp3Status.Text = Temp3 + "℃\n";
            Temp4Status.Text = Temp4 + "℃\n";
            Temp5Status.Text = Temp5 + "℃\n";
        }


        private delegate void StopDelegate();   //手动停止委托
        private void StopPro()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    StopDelegate stopDelegate = StopPro;
                    this.Invoke(stopDelegate);
                }
                else
                {
                    //将数字量输出置为0
                    doData[0] = 0;
                    doData[1] = 0;
                    doData[2] = 0;
                    doData[3] = 0;
                    control.InstantDo_Write(doData);
                    //当前采集的数据清空
                    analyseReportDic.Remove(logicType);
                    dt.Clear();
                    index = 0;
                    //输出面板清空
                    systemInfoTb.Text = "";

                    //弹窗提示
                    MessageBox.Show("当前测试流程已停止");
                    stopFlag = false;
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.Error("手动停止异常:" + ex.ToString());
                return;
            }

        }

        #endregion

        #region UI相关
        AutoSizeFormClass asc = new AutoSizeFormClass();

        /// <summary>
        /// 初始化控件
        /// </summary>
        private void InitControl()
        {
            //safeTestRbt.Checked = true;
            //hslCurve1.SetLeftCurve("冷水箱温度", null, Color.OrangeRed);
            //hslCurve1.SetLeftCurve("热水箱温度", null, Color.Orchid);
            //hslCurve1.SetLeftCurve("高温水箱温度", null, Color.White);
            //hslCurve1.SetLeftCurve("中温水箱温度", null, Color.GreenYellow);
            //hslCurve1.SetLeftCurve("常温水箱温度", null, Color.BlueViolet);
            hslCurve1.SetLeftCurve("冷水流量Qc", null, Color.Red);
            hslCurve1.SetLeftCurve("热水流量Qh", null, Color.Orange);
            hslCurve1.SetLeftCurve("出水流量Qm", null, Color.Yellow);
            hslCurve1.SetLeftCurve("冷水温度Tc", null, Color.Green);
            hslCurve1.SetLeftCurve("热水温度Th", null, Color.Blue);
            hslCurve1.SetLeftCurve("出水温度Tm", null, Color.Purple);
            hslCurve1.SetLeftCurve("冷水压力Pc", null, Color.White);
            hslCurve1.SetLeftCurve("热水压力Ph", null, Color.DarkOrange);
            hslCurve1.SetLeftCurve("出水压力Pm", null, Color.DodgerBlue);
            //hslCurve1.SetLeftCurve("出水重量Qm5", null, Color.YellowGreen);
            hslCurve1.TextAddFormat = "hh:mm:ss:fff";
           
        }

        /// <summary>
        /// 初始化数据采集
        /// </summary>
        private void InitData()
        {
            systemInfoTb.Text = "";
            dt = new DataTable();
            dt.Columns.Add("时间", typeof(string));
            dt.Columns.Add("分析时间", typeof(DateTime));
            dt.Columns.Add("冷水流量Qc", typeof(double));   //新建第一列 通道0
            dt.Columns.Add("热水流量Qh", typeof(double));   //1
            dt.Columns.Add("出水流量Qm", typeof(double));   //2
            dt.Columns.Add("冷水温度Tc", typeof(double));   //3
            dt.Columns.Add("热水温度Th", typeof(double));   //4
            dt.Columns.Add("出水温度Tm", typeof(double));   //5
            dt.Columns.Add("冷水压力Pc", typeof(double));   //6
            dt.Columns.Add("热水压力Ph", typeof(double));   //7
            dt.Columns.Add("出水压力Pm", typeof(double));   //8
            dt.Columns.Add("出水重量Qm5", typeof(double));   //9
            dt.Columns.Add("液面高度Wh", typeof(double));   //10
            dt.Columns.Add("索引", typeof(int));

            GraphDt = new DataTable();
            GraphDt.Columns.Add("时间", typeof(string));
            GraphDt.Columns.Add("冷水流量Qc", typeof(double));   //新建第一列 通道0
            GraphDt.Columns.Add("热水流量Qh", typeof(double));   //1
            GraphDt.Columns.Add("出水流量Qm", typeof(double));   //2
            GraphDt.Columns.Add("冷水温度Tc", typeof(double));   //3
            GraphDt.Columns.Add("热水温度Th", typeof(double));   //4
            GraphDt.Columns.Add("出水温度Tm", typeof(double));   //5
            //GraphDt.Columns.Add("出水温度Tm2", typeof(double));   //5
            GraphDt.Columns.Add("冷水压力Pc", typeof(double));   //6
            GraphDt.Columns.Add("热水压力Ph", typeof(double));   //7
            GraphDt.Columns.Add("出水压力Pm", typeof(double));   //8
            GraphDt.Columns.Add("出水重量Qm5", typeof(double));   //9
            GraphDt.Columns.Add("液面高度Wh", typeof(double));   //10

            ElectDt = new DataTable();
            ElectDt.Columns.Add("时间", typeof(string));
            ElectDt.Columns.Add("冷水流量Qc", typeof(double));   //新建第一列 通道0
            ElectDt.Columns.Add("热水流量Qh", typeof(double));   //1
            ElectDt.Columns.Add("出水流量Qm", typeof(double));   //2
            ElectDt.Columns.Add("冷水温度Tc", typeof(double));   //3
            ElectDt.Columns.Add("热水温度Th", typeof(double));   //4
            ElectDt.Columns.Add("出水温度Tm", typeof(double));   //5
            ElectDt.Columns.Add("冷水压力Pc", typeof(double));   //6
            ElectDt.Columns.Add("热水压力Ph", typeof(double));   //7
            ElectDt.Columns.Add("出水压力Pm", typeof(double));   //8
            ElectDt.Columns.Add("出水重量Qm5", typeof(double));   //9
            ElectDt.Columns.Add("液面高度Wh", typeof(double));   //10

            timeAngleDt = new DataTable();
            timeAngleDt.Columns.Add("时间", typeof(string));
            timeAngleDt.Columns.Add("角度", typeof(double));

            bpq_conf.botelv = "19200";
            bpq_conf.zhanhao = "1";//站号
            bpq_conf.shujuwei = "8";
            bpq_conf.tingzhiwei = "1";
            bpq_conf.dataFromZero = true;
            bpq_conf.stringReverse = false;
            bpq_conf.COM_Name = "COM11";
            bpq_conf.checkInfo = 2;
            bpq_conf.dataFrame = 2;

            bpq = new M_485Rtu(bpq_conf);
            bpq.connect();


            collectConfig = new config();
            collectConfig.channelCount = 16;
            collectConfig.convertClkRate = 100;
            collectConfig.deviceDescription = "PCI-1710HG,BID#0";//"PCI-1710HG,BID#0";//选择设备以这个为准，不用考虑设备序号            
            collectConfig.sectionCount = 0;//The 0 means setting 'streaming' mode.
            collectConfig.sectionLength = 100;
            collectConfig.startChannel = 0;

            controlConfig = new config();
            controlConfig.deviceDescription = "PCI-1756,BID#0";
            controlConfig.sectionCount = 0;//The 0 means setting 'streaming' mode.

            collectData = new DAQ_profile(0, collectConfig);
            collectData.InstantAo();
            collectData.InstantAo_Write(aoData);//输出模拟量函数

            control = new DAQ_profile(0, controlConfig);
            control.InstantDo();
            control.InstantDi();

            for (int i = 0; i < 4; i++)     //初始化数字量输出
            {
                doData[i] = 0x00;
            }
            control.InstantDo_Write(doData);//输出数字量函数
            control.InstantDi();
            diData[0] = control.InstantDi_Read();//读取数字量函数
            Console.WriteLine("D0" + diData[0].get_bit(0));
            Console.WriteLine("D7" + diData[0].get_bit(7));
            Console.WriteLine("D4" + diData[0].get_bit(4));
            Console.WriteLine("D5" + diData[0].get_bit(5));
            Console.WriteLine("D6" + diData[0].get_bit(6));
            set_bit(ref doData[2], 5, true);//打开出水阀
            control.InstantDo_Write(doData);
            WaveformAi();//
            WaveformAiCtrl1_Start();//开始高速读取模拟量数据

        }

        /// <summary>
        /// 初始化计时器
        /// </summary>
        private void InitTimer()
        {
            monitorTimer = new System.Timers.Timer(1000);
            monitorTimer.Elapsed += (o, a) =>
            {
                MonitorActive();
            };//到达时间的时候执行事件； 
            monitorTimer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；
            monitorTimer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；

            monitorDiTimer = new System.Timers.Timer(5000);
            monitorDiTimer.Elapsed += (o, a) =>
            {
                MonitorDiActive();
            };//到达时间的时候执行事件； 
            monitorDiTimer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；
            monitorDiTimer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；

            monitorWhTimer = new System.Timers.Timer(5000);
            monitorWhTimer.Elapsed += (o, a) =>
            {
                MonitorWhActive();
            };//到达时间的时候执行事件； 
            monitorWhTimer.AutoReset = true;
            monitorWhTimer.Enabled = true;


            safetyTimer = new System.Timers.Timer(2);
            safetyTimer.Elapsed += new System.Timers.ElapsedEventHandler(SafetyTimer_Action);//到达时间的时候执行事件； 
            safetyTimer.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；
            safetyTimer.Enabled = false;//是否执行System.Timers.Timer.Elapsed事件；

            pressureTimer = new System.Timers.Timer(2);//87/4
            pressureTimer.Elapsed += new System.Timers.ElapsedEventHandler(PressureTimer_Action);//到达时间的时候执行事件； 
            pressureTimer.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；
            pressureTimer.Enabled = false;//是否执行System.Timers.Timer.Elapsed事件； 

            coolTimer = new System.Timers.Timer(2);
            coolTimer.Elapsed += new System.Timers.ElapsedEventHandler(CoolTimer_Action);//到达时间的时候执行事件； 
            coolTimer.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；
            coolTimer.Enabled = false;//是否执行System.Timers.Timer.Elapsed事件；

            steadyTimer = new System.Timers.Timer(2);
            steadyTimer.Elapsed += new System.Timers.ElapsedEventHandler(SteadyTimer_Action);//到达时间的时候执行事件； 
            steadyTimer.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；
            steadyTimer.Enabled = false;//是否执行System.Timers.Timer.Elapsed事件；

            flowTimer = new System.Timers.Timer(2);
            flowTimer.Elapsed += new System.Timers.ElapsedEventHandler(FlowTimer_Action);//到达时间的时候执行事件； 
            flowTimer.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；
            flowTimer.Enabled = false;//是否执行System.Timers.Timer.Elapsed事件；

            heatTimer = new System.Timers.Timer(2);
            heatTimer.Elapsed += new System.Timers.ElapsedEventHandler(HeatTimer_Action);//到达时间的时候执行事件； 
            heatTimer.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；
            heatTimer.Enabled = false;//是否执行System.Timers.Timer.Elapsed事件；

            maxHeatTimer = new System.Timers.Timer(2);
            maxHeatTimer.Elapsed += new System.Timers.ElapsedEventHandler(MaxHeatTimer_Action);//到达时间的时候执行事件； 
            maxHeatTimer.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；
            maxHeatTimer.Enabled = false;//是否执行System.Timers.Timer.Elapsed事件；

            changeTmTimer = new System.Timers.Timer(2);
            changeTmTimer.Elapsed += new System.Timers.ElapsedEventHandler(ChangeTmTimer_Action);//到达时间的时候执行事件； 
            changeTmTimer.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；
            changeTmTimer.Enabled = false;//是否执行System.Timers.Timer.Elapsed事件；

            //monitorMTimer = new System.Timers.Timer(1000);
            //monitorMTimer.Elapsed += (o, a) =>
            //{
            //    MonitorMActive();
            //};//到达时间的时候执行事件；
            //monitorMTimer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；
            //monitorMTimer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；

            //monitorDTimer = new System.Timers.Timer(1000);
            //monitorDTimer.Elapsed += (o, a) =>
            //{
            //    MonitorDActive();
            //};//到达时间的时候执行事件；
            //monitorDTimer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；
            //monitorDTimer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；

        }

        /// <summary>
        /// 切换定时器
        /// </summary>
        private void ChangeTimer()
        {
            if (logicType == LogicTypeEnum.safeTest)
                safetyTimer.Enabled = true;

            if (logicType == LogicTypeEnum.PressureTest)
                pressureTimer.Enabled = true;

            if (logicType == LogicTypeEnum.CoolTest)
                coolTimer.Enabled = true;

            if (logicType == LogicTypeEnum.TemTest)
                steadyTimer.Enabled = true;

            if (logicType == LogicTypeEnum.FlowTest)
                flowTimer.Enabled = true;
            graphFlag = true;
            //HideOrShowCurve();
        }

        #endregion

        #region 系统控制区按钮

        private void StartBtn_Click(object sender, EventArgs e)
        {
            //collectDataFlag = true;
            if (runFlag)    //避免重复开启
            {
                return;
            }
            switch (logicType)
            {
                case LogicTypeEnum.safeTest:
                    safetyTimer.Enabled = true;
                    break;
                case LogicTypeEnum.PressureTest:
                    pressureTimer.Enabled = true;
                    break;
                case LogicTypeEnum.CoolTest:
                    coolTimer.Enabled = true;
                    break;
                case LogicTypeEnum.TemTest:
                    steadyTimer.Enabled = true;
                    break;
                case LogicTypeEnum.FlowTest:
                    flowTimer.Enabled = true;
                    break;
                case LogicTypeEnum.HeatTest:
                    heatTimer.Enabled = true;
                    break;
                case LogicTypeEnum.MaxHeatTest:
                    maxHeatTimer.Enabled = true;
                    break;
                case LogicTypeEnum.ChangeTmTest:
                    changeTmTimer.Enabled = true;
                    break;
            }
        }

        private void StopBtn_Click(object sender, EventArgs e)
        {
            collectDataFlag = false;
            stopFlag = true;
            runFlag = false;
            autoRunFlag = false;

            doData[0] = 0;
            doData[1] = 0;
            doData[2] = 0;
            doData[3] = 0;
            control.InstantDo_Write(doData);
        }


        private void SetParamBtn_Click(object sender, EventArgs e)
        {
            Hide();
            System.Threading.Thread.Sleep(10);
            using (FormValueRangeSet form = new FormValueRangeSet(this))
            {
                form.ShowDialog();
            }
            System.Threading.Thread.Sleep(10);
            Show();
        }

        private void ExportReportBtn_Click(object sender, EventArgs e)
        {
            if (testStandard == TestStandardEnum.default2806)
            {
                FormSaveTemplate formSaveTemplate = new FormSaveTemplate(model_2806);
                formSaveTemplate.Show();
            }
            

            //TODO：导出测试报告
            //DataReportExportApp exportApp = new DataReportExportApp(analyseReportDic);
            //SaveFileDialog fileDialog = new SaveFileDialog();
            //fileDialog.Filter = "文档|*.txt";
            //fileDialog.InitialDirectory = Application.StartupPath;
            //if (fileDialog.ShowDialog() == DialogResult.OK)
            //{
            //    if (exportApp.Export(logicType))
            //    {
            //        MessageBox.Show("导出成功");
            //    }
            //    else
            //    {
            //        MessageBox.Show("导出失败，当前测试流程未有相应数据");
            //    }
            //}
            //fileDialog.Dispose();
        }



        private void SaveDataBtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "文档|*.csv";
            fileDialog.InitialDirectory = Application.StartupPath;
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                DataTableUtils.DataTableToCsvT(GraphDt, fileDialog.FileName);
                MessageBox.Show("保存成功!");
                index = 0;
            }
            fileDialog.Dispose();
        }

        private void ClearDataBtn_Click(object sender, EventArgs e)
        {
            systemInfoTb.Text = "";
            dt.Clear();
            index = 0;
        }

        /// <summary>
        /// 标准选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StandardChose_Click(object sender, EventArgs e)
        {
            if (runFlag)
            {
                MessageBox.Show("当前已有测试流程正在执行，无法切换标准！");
                return;
            }
            FormStandardChoose formStandardChoose = new FormStandardChoose(this);
            if (formStandardChoose.ShowDialog() == DialogResult.OK)
            {
                switch (testStandard)
                {
                    case TestStandardEnum.default1711:
                        safeTestRbt.Visible = true;
                        pressureTestRbt.Visible = true;
                        coolTestRbt.Visible = true;
                        tmpTestRbt.Visible = true;
                        FlowTestRbt.Visible = true;

                        heatRbt.Visible = false;
                        maxHeatRbt.Visible = false;
                        changeTmRbt.Visible = false;

                        break;
                    case TestStandardEnum.default2806:
                        coolTestRbt.Visible = false;
                        tmpTestRbt.Visible = false;
                        FlowTestRbt.Visible = false;
                        changeTmRbt.Visible = false;

                        safeTestRbt.Visible = true;
                        pressureTestRbt.Visible = true;
                        heatRbt.Visible = true;
                        maxHeatRbt.Visible = true;
                        break;
                    case TestStandardEnum.default1016:
                        coolTestRbt.Visible = false;
                        tmpTestRbt.Visible = false;
                        FlowTestRbt.Visible = false;

                        changeTmRbt.Visible = true;
                        safeTestRbt.Visible = true;
                        pressureTestRbt.Visible = true;
                        heatRbt.Visible = true;
                        maxHeatRbt.Visible = true;
                        break;
                    
                }
                MessageBox.Show("当前标准已切换为：" + testStandard.ToDescription());
            }
        }

        public FormConnectValueSetting settingForm;
        private void ElectControlBtn_Click(object sender, EventArgs e)
        {
            if (settingForm != null)
            {
                return;
            }
            settingForm = new FormConnectValueSetting(this);
            settingForm.Show();
        }

        private void GraphDataBtn_Click(object sender, EventArgs e)
        {
            if (GraphDt.Rows != null && GraphDt.Rows.Count > 1)
            {
                Log.Info("启动图像界面");
                FormPressureCurve form = new FormPressureCurve(GraphDt);
                form.Show();
            }
            else
            {
                MessageBox.Show("未采集到数据");
            }
        }

        private void AutoRunBtn_Click(object sender, EventArgs e)
        {
            if (autoRunFlag)    //停止自动运行
            {
                autoRunBtn.OriginalColor = Color.Lavender;
                autoRunFlag = false;
            }
            else        //启动自动运行
            {
                autoRunFlag = true;
                if (runFlag)
                {
                    MessageBox.Show("当前已有测试流程正在执行，请等待！");
                    return;
                }
                else
                {
                    autoRunBtn.OriginalColor = Color.Green;
                    //switch (testStandard)
                    //{
                    //    case TestStandardEnum.default1711:
                    //        safetyTimer.Enabled = true;
                    //        break;
                    //}
                }
            }

        }

        public bool electDataFlag = false;
        private void ShutDownBtn_Click(object sender, EventArgs e)
        {
            electDataFlag = false;
            powerState_A = false;
            bpq.write_coil(shutdownAddress_A, true, 5);
            System.Threading.Thread.Sleep((int)(200));
            bpq.write_coil(shutdownAddress_A, false, 5);

            powerState_L = false;
            bpq.write_coil(shutdownAddress_L, true, 5);
            System.Threading.Thread.Sleep((int)(200));
            bpq.write_coil(shutdownAddress_L, false, 5);
        }

        private void DOControlBtn_Click(object sender, EventArgs e)
        {
            IsOpenDC = true;
            FormDOControl doControlForm = new FormDOControl(this);
            doControlForm.Show();
        }
        #endregion

        #region 流程委托

        private void SafetyTimer_Action(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                runFlag = true;
                graphFlag = true;

                analyseDataDic = new Dictionary<string, DataTable>();
                #region 启动a、c、11、011、12、021、vc、vh、vm 保持t1时间 然后关闭vc vm 打开v5
                set_bit(ref doData[1], 7, true);//a
                set_bit(ref doData[2], 1, true);//c
                set_bit(ref doData[0], 5, true);//11
                set_bit(ref doData[2], 7, true);//011
                set_bit(ref doData[0], 6, true);//12
                set_bit(ref doData[3], 1, true);//021
                set_bit(ref doData[2], 3, true);//vc
                set_bit(ref doData[2], 4, true);//vh
                set_bit(ref doData[2], 5, true);//vm
                control.InstantDo_Write(doData);
                SystemInfoPrint("[初始化系统]\n");

                System.Threading.Thread.Sleep((int)(1000 * Properties.Settings.Default.t1));
                //记录2806模板相关数据
                model_2806.Pc = Pc;
                model_2806.Tc = Tc;
                model_2806.Ph = Ph;
                model_2806.Th = Th;
                model_2806.Qm = Qm;
                model_2806.Tm = Tm;

                var orgPc = Pc;//记录初始压力，压力恢复阶段作为判断条件
                var orgPh = Ph;
                var orgPm = Pm;

                set_bit(ref doData[2], 3, false);//vc
                set_bit(ref doData[2], 5, false);//vm
                set_bit(ref doData[2], 6, false);//v5
                control.InstantDo_Write(doData);
                SystemInfoPrint("[t1 = " + Properties.Settings.Default.t1.ToString() + " s 计时结束，关闭Vc、Vm打开V5，开始冷水失效测试]\n");

                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                #endregion

                #region 冷水失效  持续t3后，打开VC Vm，同时关闭V5
                //测试标准：T5s内出水流量降至 1.9L/min 以下记录 Qm5
                //测试标准：T5s内出水温度应 ≤ 49℃

                System.Threading.Thread.Sleep((int)(1000 * Properties.Settings.Default.t2));//可调节时间 t2
                SystemInfoPrint("[t2 = " + Properties.Settings.Default.t2.ToString() + "s 延时结束，开始记录冷水失效数据]\n");

                dt.Rows.Add("开始采集冷水失效数据",
                                DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                startIndex = index;
                index++;
                collectDataFlag = true;   //开始收集数据
                System.Threading.Thread.Sleep(1000 * 5);
                //记录2806模板相关数据
                model_2806.A_1_Qc = Qm5;
                model_2806.A_1_Tc = Tm;
                model_2806.A_1_Tcdiff = Math.Round(Tm - model_2806.Tm, 2, MidpointRounding.AwayFromZero);
                System.Threading.Thread.Sleep((int)(1000 * (Properties.Settings.Default.t3 - 5)));
                collectDataFlag = false;  //停止收集数据
                dt.Rows.Add("冷水失效数据采集完毕",
                                DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                index++;
                endIndex = index;
                analyseDataDic.Add("冷水失效数据", DataTableUtils.SubDataTable(dt, startIndex, endIndex));
                SystemInfoPrint("[t3 = " + Properties.Settings.Default.t3.ToString() + " s 冷水测试阶段结束，停止记录数据。关闭V5，打开Vc、Vm，压力开始恢复]\n");

                set_bit(ref doData[2], 3, true);//vc
                set_bit(ref doData[2], 5, true);//vm
                set_bit(ref doData[2], 6, true);//出水重量传感器开始排水
                control.InstantDo_Write(doData);

                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                #endregion

                #region 压力回复初始压力后，开始收集数据 T5  
                //测试标准：混合水出水温度与所设定的温度偏差应 ≤ ±2 ℃  
                for (; true;)
                {//压力恢复到初始压力
                    //if (Math.Abs(Pc - (double)Properties.Settings.Default.CoolPump011) <= (double)Properties.Settings.Default.pressureThreshold)
                    //{
                    //    break;
                    //}
                    if ((Math.Abs(Pc - orgPc) <= (double)Properties.Settings.Default.pressureThreshold) && (Math.Abs(Qm5) <= 0.1))//出水重量传感器里面的水排干净，压力恢复到之前的压力
                    {
                        break;
                    }

                }
                SystemInfoPrint("[压力恢复到初始压力，开始记录 5s 的数据]\n");
                dt.Rows.Add("开始采集冷水恢复数据",
                                DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                startIndex = index;
                index++;
                collectDataFlag = true;
                System.Threading.Thread.Sleep((int)(1000 * 5));//延时5s
                //记录2806模板相关数据
                model_2806.A_2_Qc = Qm5;
                model_2806.A_2_Tc = Tm;
                model_2806.A_2_Tcdiff = Math.Round(Tm - model_2806.Tm, 2, MidpointRounding.AwayFromZero);
                collectDataFlag = false;
                dt.Rows.Add("冷水恢复数据采集完毕",
                    DateTime.Now,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    index);
                index++;
                endIndex = index;
                analyseDataDic.Add("冷水恢复数据", DataTableUtils.SubDataTable(dt, startIndex, endIndex));
                SystemInfoPrint("[ 5s 的数据记录完毕]\n");

                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                #endregion

                #region t1后，关闭vh vm，同时打开v5
                System.Threading.Thread.Sleep((int)(1000 * Properties.Settings.Default.t1));
                doData[2].set_bit(4, false);//vh
                doData[2].set_bit(5, false);//vm
                //doData[2].set_bit(6, true);//v5
                doData[2].set_bit(6, false);//v5
                control.InstantDo_Write(doData);
                SystemInfoPrint("[t1 = " + Properties.Settings.Default.t1.ToString() + " s 计时结束，关闭Vh、Vm打开V5，开始热水失效测试]\n");
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                #endregion

                #region 热水失效  持续t3后，打开vh vm，同时关闭v5
                //测试标准：T5s内出水流量降至 1.9L/min 以下记录 Qm5
                //测试标准：T5s内出水温度应 ≤ 49℃

                System.Threading.Thread.Sleep((int)(1000 * Properties.Settings.Default.t2));//可调节时间 t2
                SystemInfoPrint("[t2 = " + Properties.Settings.Default.t2.ToString() + "s 延时结束，开始记录热水失效数据]\n");

                dt.Rows.Add("开始采集热水失效数据",
                                DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                startIndex = index;
                index++;
                collectDataFlag = true;   //开始收集数据
                System.Threading.Thread.Sleep(1000 * 5);
                //记录2806模板相关数据
                model_2806.B_1_Qh = Qm5;
                model_2806.B_1_Th = Tm;
                System.Threading.Thread.Sleep((int)(1000 * (Properties.Settings.Default.t3 - 5)));
                collectDataFlag = false;  //停止收集数据
                dt.Rows.Add("热水失效数据采集完毕",
                                DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                index++;
                endIndex = index;
                analyseDataDic.Add("热水失效数据", DataTableUtils.SubDataTable(dt, startIndex, endIndex));
                SystemInfoPrint("[t3 = " + Properties.Settings.Default.t3.ToString() + " s 热水测试阶段结束，停止记录数据。关闭V5，打开Vh、Vm，压力开始恢复]\n");
                doData[2].set_bit(4, true);//vh
                doData[2].set_bit(5, true);//vm
                doData[2].set_bit(6, true);//v5
                control.InstantDo_Write(doData);
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                #endregion

                #region 压力回复初始压力后，开始收集数据 T5
                //测试标准：混合水出水温度与所设定的温度偏差应 ≤ ±2 ℃  
                for (; true;)
                {
                    //if (Math.Abs(Ph - (double)Properties.Settings.Default.HotPump021) <= (double)Properties.Settings.Default.pressureThreshold)
                    //{
                    //    break;
                    //}
                    if ((Math.Abs(Ph - orgPh) <= (double)Properties.Settings.Default.pressureThreshold) && (Math.Abs(Qm5) <= 0.1))//出水重量传感器里面的水排干净，压力恢复到之前的压力
                    {
                        break;
                    }

                }
                SystemInfoPrint("[压力恢复到初始压力，开始记录 5s 的数据]\n");
                dt.Rows.Add("开始采集热水恢复数据",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                startIndex = index;
                index++;
                collectDataFlag = true;
                System.Threading.Thread.Sleep((int)(1000 * 5));//延时5s
                //记录2806模板相关数据
                model_2806.B_2_Qh = Qm5;
                model_2806.B_2_Th = Tm;
                model_2806.B_2_Thdiff = Math.Round(Tm - model_2806.Tm, 2, MidpointRounding.AwayFromZero);
                collectDataFlag = false;
                dt.Rows.Add("热水恢复数据采集完毕",
                    DateTime.Now,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    index);
                index++;
                endIndex = index;
                analyseDataDic.Add("热水恢复数据", DataTableUtils.SubDataTable(dt, startIndex, endIndex));
                SystemInfoPrint("[ 5s 的数据记录完毕]\n");
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                #endregion

                MessageBox.Show("安全性测试结束，请注意保存数据！");
                set_bit(ref doData[1], 7, false);//a
                set_bit(ref doData[2], 1, false);//c
                set_bit(ref doData[0], 5, false);//11
                set_bit(ref doData[2], 7, false);//011
                set_bit(ref doData[0], 6, false);//12
                set_bit(ref doData[3], 1, false);//021
                set_bit(ref doData[2], 3, false);//vc
                set_bit(ref doData[2], 4, false);//vh
                set_bit(ref doData[2], 5, false);//vm
                control.InstantDo_Write(doData);
                dataReportAnalyseApp = new DataReportAnalyseApp(logicType, analyseDataDic, model_2806);
                if (analyseReportDic.ContainsKey(logicType))
                {
                    analyseReportDic[logicType] = dataReportAnalyseApp.AnalyseResult();
                }
                else
                {
                    analyseReportDic.Add(logicType, dataReportAnalyseApp.AnalyseResult());
                }
                SystemInfoPrint(analyseReportDic[logicType] + "\n");

                runFlag = false;
                graphFlag = false;
                if (autoRunFlag)
                {
                    if (stopFlag)   //手动停止
                    {
                        StopPro();
                        return;
                    }
                    safeTestRbt.Checked = false;
                    pressureTestRbt.Checked = true;
                }
                //HideOrShowCurve();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            finally
            {
                runFlag = false;
                graphFlag = false;
                if (autoRunFlag)
                {
                    if (stopFlag)   //手动停止
                    {
                        StopPro();
                    }
                    safeTestRbt.Checked = false;
                    pressureTestRbt.Checked = true;
                }
            }
        }

        private void PressureTimer_Action(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                runFlag = true;
                graphFlag = true;
                analyseDataDic = new Dictionary<string, DataTable>();

                #region 启动a、c、11、011、12、012、022、021、vc、vh、vm 保持t1时间 然后关闭a 打开b
                set_bit(ref doData[1], 7, true);//a
                set_bit(ref doData[2], 0, false);//b  
                set_bit(ref doData[2], 1, true);//c
                set_bit(ref doData[2], 2, false);//d  
                set_bit(ref doData[0], 5, true);//11
                set_bit(ref doData[2], 7, true);//011
                set_bit(ref doData[0], 6, true);//12
                set_bit(ref doData[3], 0, true);//012
                set_bit(ref doData[3], 2, true);//022
                set_bit(ref doData[3], 1, true);//021
                set_bit(ref doData[2], 3, true);//vc
                set_bit(ref doData[2], 4, true);//vh
                set_bit(ref doData[2], 5, true);//vm
                control.InstantDo_Write(doData);
                SystemInfoPrint("[初始化系统...]\n");
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }

                System.Threading.Thread.Sleep((int)(1000 * Properties.Settings.Default.t1));

                set_bit(ref doData[1], 7, false);//a
                set_bit(ref doData[2], 0, true);//b            
                control.InstantDo_Write(doData);
                //022压力切换为低压 用485切换    
                Write_short("125", Convert.ToInt16(Properties.Settings.Default.PumpHotLow022 * 500), 3);
                SystemInfoPrint("[t1 = " + Properties.Settings.Default.t1.ToString() + " s 计时结束，关闭a打开b，开始压力变化测试-热水降压测试]\n");
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                #endregion

                #region 热水降压 持续t3后打开a同时关闭b  022压力切换为高压 用485切换
                //测试标准：【TODO】
                //1、T5 秒内超过 3℃的时间不大于 T1.5 秒
                //2、T5 秒内低于 5℃的时间不大于 T1 秒
                //3、T5 秒后出水温度偏差 ≤ 2℃
                SystemInfoPrint("[等待压力到达设定值...]\n");
                for (; true;)
                {
                    if (Math.Abs(Ph - (double)Properties.Settings.Default.PumpHotLow022) <= (double)Properties.Settings.Default.pressureThreshold)
                        break;
                    System.Threading.Thread.Sleep((int)(100));
                }
                SystemInfoPrint("[压力达到设定值，开始记录热水降压数据]\n");
                dt.Rows.Add("开始采集热水降压测试数据",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                startIndex = index;
                index++;
                collectDataFlag = true;
                System.Threading.Thread.Sleep(1000 * 5);
                //记录2806模板相关数据
                model_2806.H_1_Tm = Tm;
                model_2806.H_1_Tmdiff = Math.Round(Tm - model_2806.Tm, 2, MidpointRounding.AwayFromZero);
                System.Threading.Thread.Sleep((int)(1000 * (Properties.Settings.Default.t3 - 5)));
                SystemInfoPrint("[t3 = " + Properties.Settings.Default.t3.ToString() + " s 热水降压测试阶段结束，停止记录数据。关闭b，打开a，压力开始恢复，022压力由低压切换为高压]\n");
                collectDataFlag = false;
                dt.Rows.Add("热水降压测试数据采集完毕",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                index++;
                endIndex = index;
                analyseDataDic.Add("热水降压测试数据", DataTableUtils.SubDataTable(dt, startIndex, endIndex));
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                set_bit(ref doData[1], 7, true);//a
                set_bit(ref doData[2], 0, false);//b 
                control.InstantDo_Write(doData);
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                //022压力由低压切换为高压   
                #endregion

                #region 热水压力恢复到初始压力，开始记录 5s 的数据
                //测试标准：
                //1、T5 秒内混合水出水温度与所设定的温度偏差应 ≤ ±2 ℃
                for (; true;)
                {
                    if (Math.Abs(Ph - (double)Properties.Settings.Default.HotPump021) <=
                       (double)Properties.Settings.Default.pressureThreshold) break;
                    System.Threading.Thread.Sleep((int)(100));
                }
                SystemInfoPrint("[热水压力恢复到初始压力，开始记录 5s 的数据]\n");
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                dt.Rows.Add("开始采集热水降压测试压力恢复数据",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                startIndex = index;
                index++;
                collectDataFlag = true;

                System.Threading.Thread.Sleep((int)(1000 * 5));//延时5s
                //记录2806模板相关数据
                model_2806.H_2_Tm = Tm;
                model_2806.H_2_Tmdiff = Math.Round(Tm - model_2806.Tm, 2, MidpointRounding.AwayFromZero);

                collectDataFlag = false;
                dt.Rows.Add("热水降压测试压力恢复数据采集完毕",
                    DateTime.Now,
                               0,
                               0,
                               0,
                               0,
                               0,
                               0,
                               0,
                               0,
                               0,
                               0,
                               0,
                               index);
                index++;
                endIndex = index;
                analyseDataDic.Add("热水降压恢复数据", DataTableUtils.SubDataTable(dt, startIndex, endIndex));

                SystemInfoPrint("[ 5s 的数据记录完毕]\n");
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                #endregion

                #region t1 同时 关闭a 打开b
                System.Threading.Thread.Sleep((int)(1000 * Properties.Settings.Default.t1));

                //022压力切换为高压 用485切换
                set_bit(ref doData[1], 7, false);//a
                set_bit(ref doData[2], 0, true);//b  
                control.InstantDo_Write(doData);
                //022高压切换          
                Write_short("125", Convert.ToInt16(Properties.Settings.Default.PumpHotHigh022 * 500), 3);
                SystemInfoPrint("[t1 = " + Properties.Settings.Default.t1.ToString() + " s 计时结束，关闭a打开b，开始压力变化测试-热水升压测试]\n");
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                #endregion

                #region 热水升压 持续t3后打开a同时关闭b  022压力切换为高压 用485切换
                //测试标准：【TODO】
                //1、T5 秒内超过 3℃的时间不大于 T1.5 秒
                //2、T5 秒内低于 5℃的时间不大于 T1 秒
                //3、T5 秒后出水温度偏差 ≤ 2℃
                SystemInfoPrint("[等待压力到达设定值...]\n");
                for (; true;)
                {
                    if (Math.Abs(Ph - (double)Properties.Settings.Default.PumpHotHigh022) <= (double)Properties.Settings.Default.pressureThreshold)
                        break;
                    System.Threading.Thread.Sleep((int)(100));
                }
                SystemInfoPrint("[压力达到设定值，开始记录热水升压数据]\n");
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                //开始收集数据
                dt.Rows.Add("开始采集热水升压测试数据",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                startIndex = index;
                index++;
                collectDataFlag = true;
                System.Threading.Thread.Sleep(1000 * 5);
                //记录2806模板相关数据
                model_2806.H_3_Tm = Tm;
                model_2806.H_3_Tmdiff = Math.Round(Tm - model_2806.Tm, 2, MidpointRounding.AwayFromZero);
                System.Threading.Thread.Sleep((int)(1000 * (Properties.Settings.Default.t3 - 5)));
                SystemInfoPrint("[t3 = " + Properties.Settings.Default.t3.ToString() + " s 热水升压测试阶段结束，停止记录数据。关闭b，打开a，压力开始恢复，022压力由低压切换为高压]\n");
                collectDataFlag = false;
                dt.Rows.Add("热水升压测试数据采集完毕",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                index++;
                endIndex = index;
                analyseDataDic.Add("热水升压测试数据", DataTableUtils.SubDataTable(dt, startIndex, endIndex));
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                set_bit(ref doData[1], 7, true);//a
                set_bit(ref doData[2], 0, false);//b 
                control.InstantDo_Write(doData);
                #endregion

                #region 热水压力恢复到初始压力，开始记录 5s 的数据
                //测试标准：
                //1、T5 秒内混合水出水温度与所设定的温度偏差应 ≤ ±2 ℃
                for (; true;)
                {
                    if (Math.Abs(Ph - (double)Properties.Settings.Default.HotPump021) <=
                       (double)Properties.Settings.Default.pressureThreshold) break;
                    System.Threading.Thread.Sleep((int)(100));
                }
                SystemInfoPrint("[热水压力恢复到初始压力，开始记录 5s 的数据]\n");
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                dt.Rows.Add("开始采集热水升压测试压力恢复数据",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                startIndex = index;
                index++;
                collectDataFlag = true;

                System.Threading.Thread.Sleep((int)(1000 * 5));//延时5s
                //记录2806模板相关数据
                model_2806.H_4_Tm = Tm;
                model_2806.H_4_Tmdiff = Math.Round(Tm - model_2806.Tm, 2, MidpointRounding.AwayFromZero);
                collectDataFlag = false;
                dt.Rows.Add("热水升压测试压力恢复数据采集完毕",
                    DateTime.Now,
                               0,
                               0,
                               0,
                               0,
                               0,
                               0,
                               0,
                               0,
                               0,
                               0,
                               0,
                               index);
                index++;
                endIndex = index;
                analyseDataDic.Add("热水升压恢复数据", DataTableUtils.SubDataTable(dt, startIndex, endIndex));

                SystemInfoPrint("[ 5s 的数据记录完毕]\n");
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                #endregion

                #region t1 同时 关闭 c 打开d

                set_bit(ref doData[2], 1, true);//c
                set_bit(ref doData[2], 2, false);//d 
                control.InstantDo_Write(doData);
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                System.Threading.Thread.Sleep((int)(1000 * Properties.Settings.Default.t1));
                //012压力切换为低压 用485切换
                set_bit(ref doData[2], 1, false);//c
                set_bit(ref doData[2], 2, true);//d 
                control.InstantDo_Write(doData);
                //012输出低压 485输出 
                Write_short("125", Convert.ToInt16(Properties.Settings.Default.PumpCoolLow012 * 500), 1);
                SystemInfoPrint("[t1 = " + Properties.Settings.Default.t1.ToString() + " s 计时结束，关闭c打开d，开始压力变化测试-冷水降压测试]\n");
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                #endregion

                #region 冷水降压 持续t3后打开c同时关闭d  012压力切换为高压 用485切换
                //测试标准：【TODO】
                //1、T5 秒内超过 3℃的时间不大于 T1.5 秒
                //2、T5 秒内低于 5℃的时间不大于 T1 秒
                //3、T5 秒后出水温度偏差 ≤ 2℃
                SystemInfoPrint("[等待压力到达设定值...]\n");

                for (; true;)
                {
                    if (Math.Abs(Pc - (double)Properties.Settings.Default.PumpCoolLow012) <=
                        (double)Properties.Settings.Default.pressureThreshold)
                        break;
                    System.Threading.Thread.Sleep((int)(100));
                }
                SystemInfoPrint("[压力达到设定值，开始记录冷水降压数据]\n");
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                dt.Rows.Add("开始采集冷水降压测试数据",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                startIndex = index;
                index++;
                collectDataFlag = true;
                System.Threading.Thread.Sleep(1000 * 5);
                //记录2806模板相关数据
                model_2806.C_1_Tm = Tm;
                model_2806.C_1_Tmdiff = Math.Round(Tm - model_2806.Tm, 2, MidpointRounding.AwayFromZero);
                System.Threading.Thread.Sleep((int)(1000 * (Properties.Settings.Default.t3 - 5)));
                SystemInfoPrint("[t3 = " + Properties.Settings.Default.t3.ToString() + " s 冷水降压测试阶段结束，停止记录数据。关闭b，打开a，压力开始恢复，022压力由低压切换为高压]\n");
                collectDataFlag = false;
                dt.Rows.Add("冷水降压测试数据采集完毕",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                index++;
                endIndex = index;
                analyseDataDic.Add("冷水降压测试数据", DataTableUtils.SubDataTable(dt, startIndex, endIndex));
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                set_bit(ref doData[2], 1, true);//c
                set_bit(ref doData[2], 2, false);//d 
                control.InstantDo_Write(doData);
                #endregion

                #region 冷水压力恢复到初始压力，开始记录 5s 的数据
                //测试标准：
                //1、T5 秒内混合水出水温度与所设定的温度偏差应 ≤ ±2 ℃
                for (; true;)
                {
                    if (Math.Abs(Pc - (double)Properties.Settings.Default.CoolPump011) <=
                       (double)Properties.Settings.Default.pressureThreshold) break;
                    System.Threading.Thread.Sleep((int)(100));
                }
                SystemInfoPrint("[冷水压力恢复到初始压力，开始记录 5s 的数据]\n");
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                dt.Rows.Add("开始采集冷水降压测试压力恢复数据",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                startIndex = index;
                index++;
                collectDataFlag = true;
                System.Threading.Thread.Sleep((int)(1000 * 5));//延时5s
                //记录2806模板相关数据
                model_2806.C_2_Tm = Tm;
                model_2806.C_2_Tmdiff = Math.Round(Tm - model_2806.Tm, 2, MidpointRounding.AwayFromZero);
                collectDataFlag = false;
                dt.Rows.Add("冷水降压测试压力恢复数据采集完毕",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                index++;
                endIndex = index;

                SystemInfoPrint("[ 5s 的数据记录完毕]\n");
                analyseDataDic.Add("冷水降压恢复数据", DataTableUtils.SubDataTable(dt, startIndex, endIndex));
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                #endregion

                #region t1 同时 关闭c 打开d
                //冷水升压测试
                set_bit(ref doData[2], 1, true);//c
                set_bit(ref doData[2], 2, false);//d 
                                                 //012输出高压 485输出

                control.InstantDo_Write(doData);
                System.Threading.Thread.Sleep((int)(1000 * Properties.Settings.Default.t1));

                //012压力切换为高压 用485切换
                set_bit(ref doData[2], 1, false);//c
                set_bit(ref doData[2], 2, true);//d 
                control.InstantDo_Write(doData);
                Write_short("125", Convert.ToInt16(Properties.Settings.Default.PumpCoolHigh012 * 500), 1);
                SystemInfoPrint("[t1 = " + Properties.Settings.Default.t1.ToString() + " s 计时结束，关闭c打开d，开始压力变化测试-冷水升压测试]\n");
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                #endregion

                #region 冷水升压 持续t3后打开a同时关闭b  022压力切换为高压 用485切换
                //测试标准：【TODO】
                //1、T5 秒内超过 3℃的时间不大于 T1.5 秒
                //2、T5 秒内低于 5℃的时间不大于 T1 秒
                //3、T5 秒后出水温度偏差 ≤ 2℃
                SystemInfoPrint("[等待压力到达设定值...]\n");
                for (; true;)
                {
                    if (Math.Abs(Pc - (double)Properties.Settings.Default.PumpCoolHigh012) <=
                        (double)Properties.Settings.Default.pressureThreshold)
                        break;
                    System.Threading.Thread.Sleep((int)(100));
                }
                SystemInfoPrint("[压力达到设定值，开始记录冷水升压数据]\n");
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                //开始收集数据
                dt.Rows.Add("开始采集冷水升压测试数据",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                startIndex = index;
                index++;
                collectDataFlag = true;
                System.Threading.Thread.Sleep(1000 * 5);
                //记录2806模板相关数据
                model_2806.C_3_Tm = Tm;
                model_2806.C_3_Tmdiff = Math.Round(Tm - model_2806.Tm, 2, MidpointRounding.AwayFromZero);
                System.Threading.Thread.Sleep((int)(1000 * (Properties.Settings.Default.t3 - 5)));
                SystemInfoPrint("[t3 = " + Properties.Settings.Default.t3.ToString() + " s 冷水升压测试阶段结束，停止记录数据。关闭d，打开c，压力开始恢复，022压力由低压切换为高压]\n");
                //停止收集数据,持续t3后
                collectDataFlag = false;
                dt.Rows.Add("冷水升压测试数据采集完毕",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                index++;
                endIndex = index;
                analyseDataDic.Add("冷水升压测试数据", DataTableUtils.SubDataTable(dt, startIndex, endIndex));
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                set_bit(ref doData[2], 1, true);//c
                set_bit(ref doData[2], 2, false);//d            
                control.InstantDo_Write(doData);
                #endregion

                #region 冷水压力恢复到初始压力，开始记录 5s 的数据
                //测试标准：
                //1、T5 秒内混合水出水温度与所设定的温度偏差应 ≤ ±2 ℃
                for (; true;)
                {
                    if (Math.Abs(Pc - (double)Properties.Settings.Default.CoolPump011) <=
                       (double)Properties.Settings.Default.pressureThreshold) break;
                    System.Threading.Thread.Sleep((int)(100));
                }
                SystemInfoPrint("[冷水压力恢复到初始压力，开始记录 5s 的数据]\n");
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                dt.Rows.Add("开始采集冷水升压测试压力恢复数据",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                startIndex = index;
                index++;
                collectDataFlag = true;
                System.Threading.Thread.Sleep((int)(1000 * 5));//延时5s
                //记录2806模板相关数据
                model_2806.C_4_Tm = Tm;
                model_2806.C_4_Tmdiff = Math.Round(Tm - model_2806.Tm, 2, MidpointRounding.AwayFromZero);
                collectDataFlag = false;
                dt.Rows.Add("冷水降压测试压力恢复数据采集完毕",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                index++;
                endIndex = index;
                analyseDataDic.Add("冷水升压恢复数据", DataTableUtils.SubDataTable(dt, startIndex, endIndex));

                SystemInfoPrint("[ 5s 的数据记录完毕]\n");
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                #endregion

                MessageBox.Show("压力变化测试结束，请注意保存数据！");
                set_bit(ref doData[1], 7, false);//a
                set_bit(ref doData[2], 0, false);//b  
                set_bit(ref doData[2], 1, false);//c
                set_bit(ref doData[2], 2, false);//d  
                set_bit(ref doData[0], 5, false);//11
                set_bit(ref doData[2], 7, false);//011
                set_bit(ref doData[0], 6, false);//12
                set_bit(ref doData[3], 0, false);//012
                set_bit(ref doData[3], 2, false);//022
                set_bit(ref doData[3], 1, false);//021
                set_bit(ref doData[2], 3, false);//vc
                set_bit(ref doData[2], 4, false);//vh
                set_bit(ref doData[2], 5, false);//vm
                control.InstantDo_Write(doData);
                dataReportAnalyseApp = new DataReportAnalyseApp(logicType, analyseDataDic, model_2806);
                if (analyseReportDic.ContainsKey(logicType))
                {
                    analyseReportDic[logicType] = dataReportAnalyseApp.AnalyseResult();
                }
                else
                {
                    analyseReportDic.Add(logicType, dataReportAnalyseApp.AnalyseResult());
                }
                SystemInfoPrint(analyseReportDic[logicType] + "\n");
                runFlag = false;
                graphFlag = false;
                if (autoRunFlag)
                {
                    if (stopFlag)   //手动停止
                    {
                        StopPro();
                        return;
                    }
                    pressureTestRbt.Checked = false;
                    coolTestRbt.Checked = true;
                }
                //HideOrShowCurve();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            finally
            {
                runFlag = false;
                graphFlag = false;
                if (autoRunFlag)
                {
                    if (stopFlag)   //手动停止
                    {
                        StopPro();
                        //return;
                    }
                    pressureTestRbt.Checked = false;
                    coolTestRbt.Checked = true;
                }
            }
        }

        private void CoolTimer_Action(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                runFlag = true;
                graphFlag = true;

                analyseDataDic = new Dictionary<string, DataTable>();


                #region 启动a、c、11、011、12、021、vc、vh、vm 保持t1时间 然后关闭12 打开14
                set_bit(ref doData[1], 7, true);//a
                set_bit(ref doData[2], 1, true);//c
                set_bit(ref doData[0], 5, true);//11
                set_bit(ref doData[2], 7, true);//011
                set_bit(ref doData[0], 6, true);//12
                set_bit(ref doData[3], 1, true);//021
                set_bit(ref doData[2], 3, true);//vc
                set_bit(ref doData[2], 4, true);//vh
                set_bit(ref doData[2], 5, true);//vm
                control.InstantDo_Write(doData);
                SystemInfoPrint("[初始化系统...]\n");

                System.Threading.Thread.Sleep((int)(1000 * Properties.Settings.Default.t1));
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                set_bit(ref doData[0], 6, false);//12
                set_bit(ref doData[1], 6, true);//14            
                control.InstantDo_Write(doData);
                SystemInfoPrint("[t1 = " + Properties.Settings.Default.t1.ToString() + " s 计时结束，关闭12 打开14，开始降温测试]\n");
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                #endregion

                #region 温度达到设定稳定温度后  持续t3后 关闭14 打开12
                //测试标准：【TODO】
                //1、T5 秒内超过 3℃的时间不大于 T1 秒
                //2、T5 秒出水温度波动 ≤ 1℃
                //3、T5 秒后出水温度偏差 ≤ 2℃
                SystemInfoPrint("[等待温度到达设定值...]\n");
                for (; true;)
                {
                    if (stopFlag)   //手动停止
                    {
                        StopPro();
                        return;
                    }
                    if (true)       //???
                        break;
                    System.Threading.Thread.Sleep((int)(100));
                }

                SystemInfoPrint("[温度达到设定值，开始记录降温测试数据]\n");
                dt.Rows.Add("开始采集降温测试数据",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                startIndex = index;
                index++;
                collectDataFlag = true;
                System.Threading.Thread.Sleep((int)(1000 * Properties.Settings.Default.t3));
                SystemInfoPrint("[t3 = " + Properties.Settings.Default.t3.ToString() + " s 降温测试阶段结束，停止记录数据。关闭14，打开12]\n");
                collectDataFlag = false;
                dt.Rows.Add("降温测试数据采集完毕",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                index++;
                endIndex = index;
                analyseDataDic.Add("降温测试数据", DataTableUtils.SubDataTable(dt, startIndex, endIndex));
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                set_bit(ref doData[0], 6, true);//12
                set_bit(ref doData[1], 6, false);//14  
                control.InstantDo_Write(doData);
                #endregion

                #region 温度达到设定稳定温度后,开始记录40s 的数据
                //测试标准：
                //1、T40 秒后混合水出水温度与所设定的温度偏差应 ≤ ±2 ℃
                for (; true;)
                {
                    if (stopFlag)   //手动停止
                    {
                        StopPro();
                        return;
                    }
                    if (true) break;                //???
                    System.Threading.Thread.Sleep((int)(100));
                }
                SystemInfoPrint("[温度达到设定值，开始记录 45s 的数据]\n");
                dt.Rows.Add("开始采集降温测试恢复数据",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                index++;
                collectDataFlag = true;

                System.Threading.Thread.Sleep((int)(1000 * 45));//延时40s
                collectDataFlag = false;
                dt.Rows.Add("降温测试恢复数据采集完毕",
                    DateTime.Now,
                               0,
                               0,
                               0,
                               0,
                               0,
                               0,
                               0,
                               0,
                               0,
                               0,
                               0,
                               index);
                index++;
                analyseDataDic.Add("降温测试恢复数据", DataTableUtils.SubDataTable(dt, startIndex, endIndex));
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                SystemInfoPrint("[ 45s 的数据记录完毕]\n");
                #endregion

                dataReportAnalyseApp = new DataReportAnalyseApp(logicType, analyseDataDic, model_2806);
                if (analyseReportDic.ContainsKey(logicType))
                {
                    analyseReportDic[logicType] = dataReportAnalyseApp.AnalyseResult();
                }
                else
                {
                    analyseReportDic.Add(logicType, dataReportAnalyseApp.AnalyseResult());
                }
                SystemInfoPrint(analyseReportDic[logicType] + "\n");
                set_bit(ref doData[0], 5, false);//11、进冷水阀5-5    
                set_bit(ref doData[0], 6, false);//12、进热水阀6 - 6
                set_bit(ref doData[0], 7, false);//13、进高温阀7 - 7
                doData[1] = 0;
                doData[2] = 0;
                set_bit(ref doData[3], 0, false);//012、冷水变压泵24-0    
                set_bit(ref doData[3], 1, false);// 021、热水泵25 - 1
                set_bit(ref doData[3], 2, false);//022、热水变压泵26 - 2
                control.InstantDo_Write(doData);
                runFlag = false;
                graphFlag = false;
                if (autoRunFlag)
                {
                    if (stopFlag)   //手动停止
                    {
                        StopPro();
                        return;
                    }
                    coolTestRbt.Checked = false;
                    tmpTestRbt.Checked = true;
                }
                //HideOrShowCurve();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            finally
            {
                runFlag = false;
                graphFlag = false;
                if (autoRunFlag)
                {
                    if (stopFlag)   //手动停止
                    {
                        StopPro();
                        //return;
                    }
                    coolTestRbt.Checked = false;
                    tmpTestRbt.Checked = true;
                }
            }
        }

        private void SteadyTimer_Action(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                runFlag = true;
                graphFlag = true;

                analyseDataDic = new Dictionary<string, DataTable>();

                MessageBox.Show("请确认电机已设置好相关参数！");

                #region 启动 a、c、11、12、vc、vh
                set_bit(ref doData[1], 7, true);//a
                set_bit(ref doData[2], 1, true);//c
                set_bit(ref doData[0], 5, true);//11
                set_bit(ref doData[0], 6, true);//12
                set_bit(ref doData[2], 3, true);//vc
                set_bit(ref doData[2], 4, true);//vh
                control.InstantDo_Write(doData);
                SystemInfoPrint("[初始化系统...]\n");
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                #endregion

                #region 开启电机，原点，旋转记录 Tm36 38 40 位置
                try
                {
                    timeAngleDt.Clear();
                    ElectDt.Clear();
                    SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "开始找点");
                    isAutoFindAngle = true;
                    electDataFlag = true;
                    System.Threading.Thread.Sleep(100);
                    //正传
                    bpq.write_coil(forwardWriteAddress_A, true, 5);
                    SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "开始正传...");
                    while (isAutoFindAngle && (angleValue_A <= autoFindAngle_A))
                    {
                        //等待角度达到 预设角度
                    }
                    bpq.write_coil(forwardWriteAddress_A, false, 5);
                    SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "结束正传...");
                    if (isAutoFindAngle == false)
                    {
                        SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "当前已手动停止找点...");
                        timeAngleDt.Clear();
                        ElectDt.Clear();
                        return;
                    }
                    if (stopFlag)   //手动停止
                    {
                        StopPro();
                        return;
                    }
                    //反传
                    bpq.write_coil(noForwardWriteAddress_A, true, 5);
                    SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "开始反传...");
                    while (isAutoFindAngle && (angleValue_A >= 0))
                    {
                        //等待角度达到 原点
                    }
                    bpq.write_coil(noForwardWriteAddress_A, false, 5);
                    SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "结束反传...");
                    if (isAutoFindAngle == false)
                    {
                        SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "当前已手动停止找点...");
                        timeAngleDt.Clear();
                        ElectDt.Clear();
                        return;
                    }
                    if (stopFlag)   //手动停止
                    {
                        StopPro();
                        return;
                    }
                    SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "数据采集完毕，开始分析...");
                    AnalyseElect();
                    if (tempAngleDict.Count > 0)
                    {
                        SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "采集的点位如下...");
                        foreach (var dic in tempAngleDict)
                        {
                            SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "温度：" + dic.Key + "-----> 角度：" + dic.Value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                    return;
                }
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                #endregion

                #region 从原点转到 38℃位置
                System.Threading.Thread.Sleep(100);
                //正传
                bpq.write_coil(forwardWriteAddress_A, true, 5);
                SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "开始正传...");
                while (isAutoFindAngle && (angleValue_A <= tempAngleDict[38]))
                {
                    //等待角度达到 预设角度
                }
                bpq.write_coil(forwardWriteAddress_A, false, 5);
                SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "结束正传...");
                System.Threading.Thread.Sleep((int)(1000 * Properties.Settings.Default.t1));
                #endregion

                #region 38℃到36℃位置
                //反传
                bpq.write_coil(noForwardWriteAddress_A, true, 5);
                SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "开始反传...");
                while (isAutoFindAngle && (angleValue_A >= 0)&& (angleValue_A >= tempAngleDict[36]))
                {
                    //等待角度达到 原点
                }
                bpq.write_coil(noForwardWriteAddress_A, false, 5);
                SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "结束反传...");
                System.Threading.Thread.Sleep((int)(1000 * Properties.Settings.Default.t2));

                #endregion

                #region t2秒后开始记录数据
                SystemInfoPrint("[温度达到设定值，开始记录出水温度数据]\n");
                dt.Rows.Add("开始采集36℃出水温度数据",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                startIndex = index;
                index++;
                collectDataFlag = true;
                System.Threading.Thread.Sleep((int)(1000 * Properties.Settings.Default.t3));
                SystemInfoPrint("[t3 = " + Properties.Settings.Default.t3.ToString() + " s ，停止记录数据。]\n");
                collectDataFlag = false;
                dt.Rows.Add("36℃出水温度数据采集完毕",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                index++;
                endIndex = index;
                analyseDataDic.Add("36℃出水温度数据", DataTableUtils.SubDataTable(dt, startIndex, endIndex));
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                #endregion

                #region 36℃到 38℃位置
                //正传
                bpq.write_coil(forwardWriteAddress_A, true, 5);
                SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "开始正传...");
                while (isAutoFindAngle && (angleValue_A <= tempAngleDict[38]))
                {
                    //等待角度达到 预设角度
                }
                bpq.write_coil(forwardWriteAddress_A, false, 5);
                SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "结束正传...");
                System.Threading.Thread.Sleep((int)(1000 * Properties.Settings.Default.t1));
                #endregion

                #region 38℃ 到 40 ℃
                //正传
                bpq.write_coil(forwardWriteAddress_A, true, 5);
                SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "开始正传...");
                while (isAutoFindAngle && (angleValue_A <= tempAngleDict[40]))
                {
                    //等待角度达到 预设角度
                }
                bpq.write_coil(forwardWriteAddress_A, false, 5);
                SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "结束正传...");
                System.Threading.Thread.Sleep((int)(1000 * Properties.Settings.Default.t2));
                #endregion

                #region t2秒后开始记录数据
                SystemInfoPrint("[温度达到设定值，开始记录出水温度数据]\n");
                dt.Rows.Add("开始采集40℃出水温度数据",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                startIndex = index;
                index++;
                collectDataFlag = true;
                System.Threading.Thread.Sleep((int)(1000 * Properties.Settings.Default.t3));
                SystemInfoPrint("[t3 = " + Properties.Settings.Default.t3.ToString() + " s ，停止记录数据。]\n");
                collectDataFlag = false;
                dt.Rows.Add("40℃出水温度数据采集完毕",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                index++;
                endIndex = index;
                analyseDataDic.Add("40℃出水温度数据", DataTableUtils.SubDataTable(dt, startIndex, endIndex));
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                #endregion

                #region 40℃到38℃位置
                //反传
                bpq.write_coil(noForwardWriteAddress_A, true, 5);
                SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "开始反传...");
                while (isAutoFindAngle && (angleValue_A >= 0) && (angleValue_A >= tempAngleDict[38]))
                {
                    //等待角度达到 原点
                }
                bpq.write_coil(noForwardWriteAddress_A, false, 5);
                SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "结束反传...");

                #endregion

                dataReportAnalyseApp = new DataReportAnalyseApp(logicType, analyseDataDic, model_2806);
                if (analyseReportDic.ContainsKey(logicType))
                {
                    analyseReportDic[logicType] = dataReportAnalyseApp.AnalyseResult();
                }
                else
                {
                    analyseReportDic.Add(logicType, dataReportAnalyseApp.AnalyseResult());
                }
                SystemInfoPrint(analyseReportDic[logicType] + "\n");
                set_bit(ref doData[0], 5, false);//11、进冷水阀5-5    
                set_bit(ref doData[0], 6, false);//12、进热水阀6 - 6
                set_bit(ref doData[0], 7, false);//13、进高温阀7 - 7
                doData[1] = 0;
                doData[2] = 0;
                set_bit(ref doData[3], 0, false);//012、冷水变压泵24-0    
                set_bit(ref doData[3], 1, false);// 021、热水泵25 - 1
                set_bit(ref doData[3], 2, false);//022、热水变压泵26 - 2
                control.InstantDo_Write(doData);
                runFlag = false;
                graphFlag = false;
                if (autoRunFlag)
                {
                    if (stopFlag)   //手动停止
                    {
                        StopPro();
                        //return;
                    }
                    tmpTestRbt.Checked = false;
                    FlowTestRbt.Checked = true;
                }
                //HideOrShowCurve();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            finally
            {
                runFlag = false;
                graphFlag = false;
                if (autoRunFlag)
                {
                    tmpTestRbt.Checked = false;
                    FlowTestRbt.Checked = true;
                }
            }

        }

        private void FlowTimer_Action(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                runFlag = true;
                graphFlag = true;

                analyseDataDic = new Dictionary<string, DataTable>();

                #region 启动 a、c、11、011、12、012、vc、vh、vm
                set_bit(ref doData[1], 7, true);//a
                set_bit(ref doData[2], 1, true);//c
                set_bit(ref doData[0], 5, true);//11
                set_bit(ref doData[2], 7, true);//011
                set_bit(ref doData[0], 6, true);//12
                set_bit(ref doData[3], 0, true);//012
                set_bit(ref doData[2], 3, true);//vc
                set_bit(ref doData[2], 4, true);//vh
                set_bit(ref doData[2], 5, true);//vm
                control.InstantDo_Write(doData);
                SystemInfoPrint("[初始化系统...]\n");
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                System.Threading.Thread.Sleep((int)(1000 * Properties.Settings.Default.t1));
                //TODO 电机控制
                SystemInfoPrint("[t1 = " + Properties.Settings.Default.t1.ToString() + " s 计时结束，电机开始转动，开始流量减少测试]\n");

                #endregion

                #region 5-6s 内降低50%流量
                SystemInfoPrint("[开始记录流量减少测试数据]\n");
                dt.Rows.Add("开始采集流量减少测试数据",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                startIndex = index;
                index++;
                collectDataFlag = true;
                System.Threading.Thread.Sleep((int)(1000 * 6));
                SystemInfoPrint("6s 流量减少测试阶段结束，停止记录数据。]\n");
                collectDataFlag = false;
                dt.Rows.Add("流量减少测试数据采集完毕",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                index++;
                endIndex = index;
                analyseDataDic.Add("流量减少测试数据", DataTableUtils.SubDataTable(dt, startIndex, endIndex));
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                //TODO 电机控制

                #endregion

                #region T30秒后
                //System.Threading.Thread.Sleep((int)(1000 * 30));
                //SystemInfoPrint("T30s 计时结束]\n");
                dt.Rows.Add("开始采集T30秒后温度稳定的数据",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                startIndex = index;
                index++;
                collectDataFlag = true;
                System.Threading.Thread.Sleep((int)(1000 * 35));
                SystemInfoPrint("温度稳定的数据采集完毕，停止记录数据。]\n");
                collectDataFlag = false;
                dt.Rows.Add("温度稳定的数据采集完毕",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                index++;
                endIndex = index;
                analyseDataDic.Add("温度稳定的测试数据", DataTableUtils.SubDataTable(dt, startIndex, endIndex));
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                #endregion

                dataReportAnalyseApp = new DataReportAnalyseApp(logicType, analyseDataDic, model_2806);
                if (analyseReportDic.ContainsKey(logicType))
                {
                    analyseReportDic[logicType] = dataReportAnalyseApp.AnalyseResult();
                }
                else
                {
                    analyseReportDic.Add(logicType, dataReportAnalyseApp.AnalyseResult());
                }
                SystemInfoPrint(analyseReportDic[logicType] + "\n");
                set_bit(ref doData[0], 5, false);//11、进冷水阀5-5    
                set_bit(ref doData[0], 6, false);//12、进热水阀6 - 6
                set_bit(ref doData[0], 7, false);//13、进高温阀7 - 7
                doData[1] = 0;
                doData[2] = 0;
                set_bit(ref doData[3], 0, false);//012、冷水变压泵24-0    
                set_bit(ref doData[3], 1, false);// 021、热水泵25 - 1
                set_bit(ref doData[3], 2, false);//022、热水变压泵26 - 2
                control.InstantDo_Write(doData);
                runFlag = false;
                graphFlag = false;
                //HideOrShowCurve();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            finally
            {
                runFlag = false;
                graphFlag = false;
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    //return;
                }
            }
        }

        private void HeatTimer_Action(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                runFlag = true;
                graphFlag = true;
                analyseDataDic = new Dictionary<string, DataTable>();

                #region 启动 a、c、11、011、12、021、vc、vh、vm
                set_bit(ref doData[1], 7, true);//a
                set_bit(ref doData[2], 1, true);//c
                set_bit(ref doData[0], 5, true);//11
                set_bit(ref doData[2], 7, true);//011
                set_bit(ref doData[0], 6, true);//12
                set_bit(ref doData[3], 1, true);//021
                set_bit(ref doData[2], 3, true);//vc
                set_bit(ref doData[2], 4, true);//vh
                set_bit(ref doData[2], 5, true);//vm
                control.InstantDo_Write(doData);
                SystemInfoPrint("[初始化系统...]\n");
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                System.Threading.Thread.Sleep((int)(1000 * Properties.Settings.Default.t1));
                SteadyTm = Tm;  //稳定的出水温度
                SystemInfoPrint("[t1 = " + Properties.Settings.Default.t1.ToString() + " s 计时结束，关闭12，打开13,开始升温测试]\n");
                set_bit(ref doData[0], 6, false);//12
                set_bit(ref doData[0], 7, true);//13
                control.InstantDo_Write(doData);
                #endregion

                #region t3s内出水温度与所设定的温度偏差 ±2℃
                SystemInfoPrint("[开始收集t3秒内出水温度数据]\n");
                dt.Rows.Add("开始收集t3秒内出水温度数据",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                startIndex = index;
                index++;
                collectDataFlag = true;
                System.Threading.Thread.Sleep((int)(1000 * Properties.Settings.Default.t3));
                //记录2806模板相关数据
                model_2806.Up_Tm = Tm;
                model_2806.Up_Tmdiff = Math.Round(Tm - model_2806.Tm, 2, MidpointRounding.AwayFromZero);

                collectDataFlag = false;
                dt.Rows.Add("t3秒内出水温度数据采集完毕",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                index++;
                endIndex = index;
                analyseDataDic.Add("t3秒内出水温度数据", DataTableUtils.SubDataTable(dt, startIndex, endIndex));
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                SystemInfoPrint("[t3 = " + Properties.Settings.Default.t3.ToString() + " s 计时结束，结束升温测试]\n");
                #endregion

                dataReportAnalyseApp = new DataReportAnalyseApp(logicType, analyseDataDic, model_2806);
                if (analyseReportDic.ContainsKey(logicType))
                {
                    analyseReportDic[logicType] = dataReportAnalyseApp.AnalyseResult();
                }
                else
                {
                    analyseReportDic.Add(logicType, dataReportAnalyseApp.AnalyseResult());
                }
                SystemInfoPrint(analyseReportDic[logicType] + "\n");

                set_bit(ref doData[0], 5, false);//11、进冷水阀5-5    
                set_bit(ref doData[0], 6, false);//12、进热水阀6 - 6
                set_bit(ref doData[0], 7, false);//13、进高温阀7 - 7
                doData[1] = 0;
                doData[2] = 0;
                set_bit(ref doData[3], 0, false);//012、冷水变压泵24-0    
                set_bit(ref doData[3], 1, false);// 021、热水泵25 - 1
                set_bit(ref doData[3], 2, false);//022、热水变压泵26 - 2
                control.InstantDo_Write(doData);
                runFlag = false;
                graphFlag = false;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            finally
            {
                runFlag = false;
                graphFlag = false;
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    //return;
                }
            }
        }

        private void MaxHeatTimer_Action(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                runFlag = true;
                graphFlag = true;
                analyseDataDic = new Dictionary<string, DataTable>();

                MessageBox.Show("请确认电机已设置好相关参数！");

                #region 启动 a、c、11、011、12、012、vc、vh、vm
                set_bit(ref doData[1], 7, true);//a
                set_bit(ref doData[2], 1, true);//c
                set_bit(ref doData[0], 5, true);//11
                set_bit(ref doData[2], 7, true);//011
                set_bit(ref doData[0], 6, true);//12
                set_bit(ref doData[3], 0, true);//012
                set_bit(ref doData[2], 3, true);//vc
                set_bit(ref doData[2], 4, true);//vh
                set_bit(ref doData[2], 5, true);//vm
                control.InstantDo_Write(doData);
                SystemInfoPrint("[初始化系统...]\n");
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                System.Threading.Thread.Sleep((int)(1000 * Properties.Settings.Default.t1));
                SystemInfoPrint("[t1 = " + Properties.Settings.Default.t1.ToString() + " s 计时结束,开始最高限温测试]\n");
                control.InstantDo_Write(doData);
                #endregion

                #region 电机从0->90度
                //正传
                bpq.write_coil(forwardWriteAddress_A, true, 5);
                while (angleValue_A <= autoFindAngle_A)
                {
                    //等待角度达到 预设角度
                }
                bpq.write_coil(forwardWriteAddress_A, false, 5);
                #endregion

                #region 旋转到位置后，t2s后开始收集数据，
                System.Threading.Thread.Sleep((int)(1000 * Properties.Settings.Default.t2));
                SystemInfoPrint("[t1 = " + Properties.Settings.Default.t2.ToString() + " s 计时结束，开始收集数据]\n");
                SystemInfoPrint("[开始收集T25秒内出水温度数据]\n");
                dt.Rows.Add("开始收集T25秒内出水温度数据",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                startIndex = index;
                index++;
                collectDataFlag = true;
                System.Threading.Thread.Sleep(1000 * 25);
                //记录2806模板相关数据
                model_2806.TmMax = Tm;

                collectDataFlag = false;
                dt.Rows.Add("T25秒内出水温度数据采集完毕",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                index++;
                endIndex = index;
                analyseDataDic.Add("t3秒内出水温度数据", DataTableUtils.SubDataTable(dt, startIndex, endIndex));
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                SystemInfoPrint("[T25s 内混合水出水温度数据收集完毕]\n");
                #endregion

                set_bit(ref doData[1], 7, false);//a
                set_bit(ref doData[2], 1, false);//c
                set_bit(ref doData[0], 5, false);//11
                set_bit(ref doData[2], 7, false);//011
                set_bit(ref doData[0], 6, false);//12
                set_bit(ref doData[3], 0, false);//012
                set_bit(ref doData[2], 3, false);//vc
                set_bit(ref doData[2], 4, false);//vh
                set_bit(ref doData[2], 5, false);//vm
                control.InstantDo_Write(doData);
                dataReportAnalyseApp = new DataReportAnalyseApp(logicType, analyseDataDic, model_2806);
                if (analyseReportDic.ContainsKey(logicType))
                {
                    analyseReportDic[logicType] = dataReportAnalyseApp.AnalyseResult();
                }
                else
                {
                    analyseReportDic.Add(logicType, dataReportAnalyseApp.AnalyseResult());
                }
                SystemInfoPrint(analyseReportDic[logicType] + "\n");
                runFlag = false;
                graphFlag = false;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            finally
            {
                runFlag = false;
                graphFlag = false;
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    //return;
                }
            }
        }

        private void ChangeTmTimer_Action(object source,System.Timers.ElapsedEventArgs e)
        {
            try
            {
                runFlag = true;
                graphFlag = true;
                analyseDataDic = new Dictionary<string, DataTable>();

                MessageBox.Show("请确认电机已设置好相关参数！");

                #region 启动 a c  11 011 14 021 vc vh vm  t1时间到
                set_bit(ref doData[1], 7, true);//a
                set_bit(ref doData[2], 1, true);//c
                set_bit(ref doData[0], 5, true);//11
                set_bit(ref doData[2], 7, true);//011
                set_bit(ref doData[1], 0, true);//14
                set_bit(ref doData[3], 1, true);//021
                set_bit(ref doData[2], 3, true);//vc
                set_bit(ref doData[2], 4, true);//vh
                set_bit(ref doData[2], 5, true);//vm
                control.InstantDo_Write(doData);
                SystemInfoPrint("[初始化系统...]\n");
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                System.Threading.Thread.Sleep((int)(1000 * Properties.Settings.Default.t1));
                SystemInfoPrint("[t1 = " + Properties.Settings.Default.t1.ToString() + " s 计时结束,开始温度变化测试]\n");
                #endregion

                #region 开启电机，原点，旋转记录 Tm36 38 40 位置
                timeAngleDt.Clear();
                ElectDt.Clear();
                SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "开始找点");
                isAutoFindAngle = true;
                electDataFlag = true;
                System.Threading.Thread.Sleep(100);
                //正传
                bpq.write_coil(forwardWriteAddress_A, true, 5);
                SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "开始正传...");
                while (isAutoFindAngle && (angleValue_A <= autoFindAngle_A))
                {
                    //等待角度达到 预设角度
                }
                bpq.write_coil(forwardWriteAddress_A, false, 5);
                SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "结束正传...");
                if (isAutoFindAngle == false)
                {
                    SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "当前已手动停止找点...");
                    timeAngleDt.Clear();
                    ElectDt.Clear();
                    return;
                }
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                //反传
                bpq.write_coil(noForwardWriteAddress_A, true, 5);
                SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "开始反传...");
                while (isAutoFindAngle && (angleValue_A >= 0))
                {
                    //等待角度达到 原点
                }
                bpq.write_coil(noForwardWriteAddress_A, false, 5);
                SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "结束反传...");
                if (isAutoFindAngle == false)
                {
                    SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "当前已手动停止找点...");
                    timeAngleDt.Clear();
                    ElectDt.Clear();
                    return;
                }
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "数据采集完毕，开始分析...");
                AnalyseElect();
                if (tempAngleDict.Count > 0)
                {
                    SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "采集的点位如下...");
                    foreach (var dic in tempAngleDict)
                    {
                        SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "温度：" + dic.Key + "-----> 角度：" + dic.Value);
                    }
                }
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                #endregion

                #region 从原点转到 38℃位置
                System.Threading.Thread.Sleep(100);
                //正传
                bpq.write_coil(forwardWriteAddress_A, true, 5);
                SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "开始正传...");
                while (isAutoFindAngle && (angleValue_A <= tempAngleDict[38]))
                {
                    //等待角度达到 预设角度
                }
                bpq.write_coil(forwardWriteAddress_A, false, 5);
                SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "结束正传...");
                System.Threading.Thread.Sleep(1000*60);
                #endregion

                #region 60秒后开始记录数据
                SystemInfoPrint("[温度达到设定值，开始记录数据]\n");
                dt.Rows.Add("开始采集38℃数据",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                startIndex = index;
                index++;
                collectDataFlag = true;
                System.Threading.Thread.Sleep((int)(1000 * Properties.Settings.Default.t3));
                SystemInfoPrint("[t3 = " + Properties.Settings.Default.t3.ToString() + " s ，停止记录数据。]\n");
                collectDataFlag = false;
                dt.Rows.Add("38℃数据采集完毕",
                    DateTime.Now,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                index);
                index++;
                endIndex = index;
                analyseDataDic.Add("38℃数据", DataTableUtils.SubDataTable(dt, startIndex, endIndex));
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    return;
                }
                #endregion


                dataReportAnalyseApp = new DataReportAnalyseApp(logicType, analyseDataDic, model_2806);
                if (analyseReportDic.ContainsKey(logicType))
                {
                    analyseReportDic[logicType] = dataReportAnalyseApp.AnalyseResult();
                }
                else
                {
                    analyseReportDic.Add(logicType, dataReportAnalyseApp.AnalyseResult());
                }
                SystemInfoPrint(analyseReportDic[logicType] + "\n");
                runFlag = false;
                graphFlag = false;
            }
            catch(Exception ex)
            {
                Log.Error(ex.ToString());
            }
            finally
            {
                runFlag = false;
                graphFlag = false;
                if (stopFlag)   //手动停止
                {
                    StopPro();
                    //return;
                }
            }
        }

        #endregion

        #region 窗体控件事件
        private void FormMain_Load(object sender, EventArgs e)
        {
            //asc.Initialize(this);
            bpq.write_coil("2078", false, 5);//松开产品
            WaterOut.Value = Properties.Settings.Default.WaterOut;
        }

        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            //asc.ReSize(this);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            doData[0] = 0;
            doData[1] = 0;
            doData[2] = 0;
            doData[3] = 0;
            control.InstantDo_Write(doData);

            if (monitorTimer != null)
            {
                monitorTimer.Enabled = false;
                monitorTimer.Dispose();
            }
            if (monitorWhTimer != null)
            {
                monitorWhTimer.Enabled = false;
                monitorWhTimer.Dispose();
            }
            if (monitorDiTimer != null)
            {
                monitorDiTimer.Enabled = false;
                monitorDiTimer.Dispose();
            }
        }

        private void RadioBtn_CheckedChange(object sender, EventArgs e)
        {
            if (!((RadioButton)sender).Checked)
            {
                return;
            }
            if (runFlag)
            {
                MessageBox.Show("当前已有测试流程正在执行，请等待！");
                return;
            }
            //TODO:
            if (autoRunFlag || (MessageBox.Show("确认切换子操作界面？注意保存数据", "", MessageBoxButtons.YesNo) == DialogResult.Yes))
            {
                //monitorTimer.Enabled = false;
                safetyTimer.Enabled = false;
                pressureTimer.Enabled = false;
                coolTimer.Enabled = false;
                steadyTimer.Enabled = false;
                flowTimer.Enabled = false;
                heatTimer.Enabled = false;
                maxHeatTimer.Enabled = false;
                changeTmTimer.Enabled = false;
                switch (((RadioButton)sender).Text.ToString())
                {
                    case "安全性测试":
                        logicType = LogicTypeEnum.safeTest;
                        break;
                    case "压力变化测试":
                        logicType = LogicTypeEnum.PressureTest;
                        break;
                    case "降温测试":
                        logicType = LogicTypeEnum.CoolTest;
                        break;
                    case "温度稳定性测试":
                        logicType = LogicTypeEnum.TemTest;
                        break;
                    case "流量减少测试":
                        logicType = LogicTypeEnum.FlowTest;
                        break;
                    case "灵敏度测试":
                        logicType = LogicTypeEnum.SensitivityTest;
                        break;
                    case "保真度测试":
                        logicType = LogicTypeEnum.FidelityTest;
                        break;
                    case "出水温度稳定性测试":
                        logicType = LogicTypeEnum.TmSteadyTest;
                        break;
                    case "升温测试":
                        logicType = LogicTypeEnum.HeatTest;
                        break;
                    case "最高限温测试":
                        logicType = LogicTypeEnum.MaxHeatTest;
                        break;
                }
                Console.WriteLine(logicType.ToDescription());
                //InitData();
                //ChangeTimer();
            }
        }

        
        

        public short Read(string address, byte station)
        {
            var val = bpq.read_short(address, station);
            return val;
        }

        public void Write_uint(string address, uint val, byte station)
        {

            bpq.write_uint(address, val, station);
        }
        public void Write_short(string address, short val, byte station)
        {
            bpq.write_short(address, val, station);
        }
        
        #endregion

        #region WaveformAiCtrl
        //private Automation.BDaq.WaveformAiCtrl waveformAiCtrl1;

        public void WaveformAi()
        {
            waveformAiCtrl1 = new Automation.BDaq.WaveformAiCtrl();
            waveformAiCtrl1.SelectedDevice = new DeviceInformation(collectConfig.deviceDescription);

            Conversion conversion = waveformAiCtrl1.Conversion;

            conversion.ChannelStart = collectConfig.startChannel;
            conversion.ChannelCount = collectConfig.channelCount;
            conversion.ClockRate = collectConfig.convertClkRate;
            Record record = waveformAiCtrl1.Record;
            record.SectionCount = collectConfig.sectionCount;//The 0 means setting 'streaming' mode.
            record.SectionLength = collectConfig.sectionLength;

            this.waveformAiCtrl1.Overrun += new System.EventHandler<Automation.BDaq.BfdAiEventArgs>(this.WaveformAiCtrl1_Overrun);
            this.waveformAiCtrl1.CacheOverflow += new System.EventHandler<Automation.BDaq.BfdAiEventArgs>(this.WaveformAiCtrl1_CacheOverflow);
            this.waveformAiCtrl1.DataReady += new System.EventHandler<Automation.BDaq.BfdAiEventArgs>(this.WaveformAiCtrl1_DataReady);

        }

        public void WaveformAiCtrl1_Start()
        {
            ErrorCode err = ErrorCode.Success;
            err = waveformAiCtrl1.Prepare();//准备缓存区
            if (err == ErrorCode.Success)
            {
                err = waveformAiCtrl1.Start();
            }

            if (err != ErrorCode.Success)
            {
                HandleError(err);
                return;
            }
            //Log.Info(m_dataScaled.Length.ToString());
        }

        public void WaveformAiCtrl1_Stop()
        {
            ErrorCode err = ErrorCode.Success;
            err = waveformAiCtrl1.Stop();
            if (err != ErrorCode.Success)
            {
                HandleError(err);
                return;
            }
            Array.Clear(m_dataScaled, 0, m_dataScaled.Length);
        }
        private double[] lastTempData = new double[90];

        private double[] sourceDataQc = new double[106];


        private double[] sourceDataQh = new double[106];

        private double[] sourceDataQm = new double[106];

        private double[] sourceDataTh = new double[106];

        private double[] sourceDataTm = new double[106];
        //private double[] sourceDataTm2 = new double[106];

        private double[] sourceDataTc = new double[106];

        private double[] sourceDataPc = new double[106];

        private double[] sourceDataPh = new double[106];

        private double[] sourceDataPm = new double[106];

        private double[] sourceDataQm5 = new double[106];

        private double[] sourceDataTemp1 = new double[106];

        private double[] sourceDataTemp2 = new double[106];

        private double[] sourceDataTemp3 = new double[106];

        private double[] sourceDataTemp4 = new double[106];

        private double[] sourceDataTemp5 = new double[106];

        private double[] sourceDataWh = new double[64];
        private float[] resultDataWh = new float[64];

        double[] tempCoolFlow = new double[11];//读取plc的原始数据
        double[] tempHotFlow = new double[11];
        double[] CoolFlow = new double[100];//扩充后的数据
        double[] HotFlow = new double[100];

        private void WaveformAiCtrl1_DataReady(object sender, BfdAiEventArgs args)
        {
            ErrorCode err = ErrorCode.Success;
            try
            {
                //The WaveformAiCtrl has been disposed.
                if (waveformAiCtrl1.State == ControlState.Idle)
                {
                    return;
                }
                if (m_dataScaled.Length < args.Count)
                {
                    m_dataScaled = new double[args.Count];
                }

                int chanCount = waveformAiCtrl1.Conversion.ChannelCount;
                int sectionLength = waveformAiCtrl1.Record.SectionLength;
                err = waveformAiCtrl1.GetData(args.Count, m_dataScaled);//读取数据     

                DateTime t = DateTime.Now;
                t = t.AddSeconds(-1.03);//采集到的是一秒钟之前的数据，因此需要对当前的时间减去1s，再减去30ms是因为该30ms的数据，并在当前缓冲区做平滑
                t.ToString("yyyy-MM-dd hh:mm:ss:fff");
                //Log.Info(t.ToString("yyyy-MM-dd hh:mm:ss:fff"));

                int dataIndex = 0;
                for (int i = 0; i < m_dataScaled.Length; i += 16)
                {
                    Qc = Math.Round(m_dataScaled[i + 0], 2, MidpointRounding.AwayFromZero);// * 5;          流量的量程 1-5V 对应  0-50L/min
                    Qh = Math.Round(m_dataScaled[i + 1], 2, MidpointRounding.AwayFromZero);// * 5;
                    Qm = Math.Round(m_dataScaled[i + 2], 2, MidpointRounding.AwayFromZero);// * 5;
                    Tc = Math.Round(m_dataScaled[i + 3], 2, MidpointRounding.AwayFromZero);// * 10;
                    Th = Math.Round(m_dataScaled[i + 4], 2, MidpointRounding.AwayFromZero);// * 10;
                    Tm = Math.Round(m_dataScaled[i + 5], 2, MidpointRounding.AwayFromZero);// * 10;
                    Pc = Math.Round(m_dataScaled[i + 6], 2, MidpointRounding.AwayFromZero);
                    Ph = Math.Round(m_dataScaled[i + 7], 2, MidpointRounding.AwayFromZero);
                    Pm = Math.Round(m_dataScaled[i + 8], 2, MidpointRounding.AwayFromZero);
                    Qm5 = Math.Round(m_dataScaled[i + 9], 2, MidpointRounding.AwayFromZero);
                    Temp1 = Math.Round(m_dataScaled[i + 10], 2, MidpointRounding.AwayFromZero);// * 10;
                    Temp2 = Math.Round(m_dataScaled[i + 11], 2, MidpointRounding.AwayFromZero);// * 10;
                    Temp3 = Math.Round(m_dataScaled[i + 12], 2, MidpointRounding.AwayFromZero);// * 10;
                    Temp4 = Math.Round(m_dataScaled[i + 13], 2, MidpointRounding.AwayFromZero);// * 10;
                    Temp5 = Math.Round(m_dataScaled[i + 14], 2, MidpointRounding.AwayFromZero);// * 10;
                    Wh = Math.Round(m_dataScaled[i + 15], 2, MidpointRounding.AwayFromZero) * 200;
                    //Console.WriteLine("Qh:" + Qh);
                    //Console.WriteLine("Qc:" + Qc);
                    //Console.WriteLine("Qm:" + Qm);
                    if (dataIndex < 6)
                    {
                        int typeIndex = 0;
                        sourceDataQc[dataIndex] = isFirstAver ? Qc : lastTempData[typeIndex * 6 + dataIndex]; typeIndex++;
                        sourceDataQh[dataIndex] = isFirstAver ? Qh : lastTempData[typeIndex * 6 + dataIndex]; typeIndex++;
                        sourceDataQm[dataIndex] = isFirstAver ? Qm : lastTempData[typeIndex * 6 + dataIndex]; typeIndex++;
                        sourceDataTc[dataIndex] = isFirstAver ? Tc : lastTempData[typeIndex * 6 + dataIndex]; typeIndex++;
                        sourceDataTh[dataIndex] = isFirstAver ? Th : lastTempData[typeIndex * 6 + dataIndex]; typeIndex++;
                        sourceDataTm[dataIndex] = isFirstAver ? Tm : lastTempData[typeIndex * 6 + dataIndex]; typeIndex++;
                        sourceDataPc[dataIndex] = isFirstAver ? Pc : lastTempData[typeIndex * 6 + dataIndex]; typeIndex++;
                        sourceDataPh[dataIndex] = isFirstAver ? Ph : lastTempData[typeIndex * 6 + dataIndex]; typeIndex++;
                        sourceDataPm[dataIndex] = isFirstAver ? Pm : lastTempData[typeIndex * 6 + dataIndex]; typeIndex++;
                        sourceDataQm5[dataIndex] = isFirstAver ? Qm5 : lastTempData[typeIndex * 6 + dataIndex]; typeIndex++;
                        sourceDataTemp1[dataIndex] = isFirstAver ? Temp1 : lastTempData[typeIndex * 6 + dataIndex]; typeIndex++;
                        sourceDataTemp2[dataIndex] = isFirstAver ? Temp2 : lastTempData[typeIndex * 6 + dataIndex]; typeIndex++;
                        sourceDataTemp3[dataIndex] = isFirstAver ? Temp3 : lastTempData[typeIndex * 6 + dataIndex]; typeIndex++;
                        sourceDataTemp4[dataIndex] = isFirstAver ? Temp4 : lastTempData[typeIndex * 6 + dataIndex]; typeIndex++;
                        sourceDataTemp5[dataIndex] = isFirstAver ? Temp5 : lastTempData[typeIndex * 6 + dataIndex]; typeIndex++;
                    }
                    if (dataIndex >= 18 && dataIndex <= 81)
                    {
                        sourceDataWh[dataIndex - 18] = Wh;
                    }
                    sourceDataQc[dataIndex + 5] = Qc;
                    sourceDataQh[dataIndex + 5] = Qh;
                    sourceDataQm[dataIndex + 5] = Qm;
                    sourceDataTc[dataIndex + 5] = Tc;
                    sourceDataTh[dataIndex + 5] = Th;
                    sourceDataTm[dataIndex + 5] = Tm;
                    //sourceDataTm2[index + 5] = Tm;

                    sourceDataPc[dataIndex + 5] = Pc;
                    sourceDataPh[dataIndex + 5] = Ph;
                    sourceDataPm[dataIndex + 5] = Pm;
                    sourceDataQm5[dataIndex + 5] = Qm5;
                    sourceDataTemp1[dataIndex + 5] = Temp1;
                    sourceDataTemp2[dataIndex + 5] = Temp2;
                    sourceDataTemp3[dataIndex + 5] = Temp3;
                    sourceDataTemp4[dataIndex + 5] = Temp4;
                    sourceDataTemp5[dataIndex + 5] = Temp5;
                    dataIndex++;
                    if (dataIndex >= 101)
                        dataIndex = 0;
                }
                //Console.WriteLine("液面高度：" + Wh);

                //用485 读取冷水、热水的流量共十个数据

                short[] temp1 = bpq.read_short_batch("4110", 10, 5);
                short[] temp2 = bpq.read_short_batch("4120", 10, 5);
                for (int i = 0; i < 10; i++)
                {//读取
                    //数值转换 short：0~32000 对应0~50  故需要除以640
                    tempCoolFlow[i] = temp1[i] / 640;
                    tempHotFlow[i] = temp2[i] / 640;
                }
                tempCoolFlow[10] = tempCoolFlow[9];
                tempHotFlow[10] = tempHotFlow[9];
                //将每秒采集到的10个流量数据扩充到100个
                //
                for (int i = 0; i < 10; i++)
                {

                    double[] CoolFill = dataFill(tempCoolFlow[i], tempCoolFlow[i + 1], 11);//扩充数据
                    double[] HotFill = dataFill(tempHotFlow[i], tempHotFlow[i + 1], 11); ;//
                    for (int j = 0; j < 10; j++)
                    {
                        CoolFlow[i * 10 + j] = CoolFill[j];
                        HotFlow[i * 10 + j] = HotFill[j];
                    }
                }

                sourceDataQc = averge(ref sourceDataQc, 0);
                //sourceDataQc = filterKalMan(sourceDataQc);
                //sourceDataQc = averge(ref sourceDataQc, 0);

                sourceDataQh = averge(ref sourceDataQh, 1);
                //sourceDataQh = filter(ref sourceDataQh, 10);

                sourceDataQm = averge(ref sourceDataQm, 2);
                //sourceDataQm = filter(ref sourceDataQm, 10);

                sourceDataTc = averge(ref sourceDataTc, 3);
                //sourceDataTc = filter(ref sourceDataTc, 10);

                sourceDataTh = averge(ref sourceDataTh, 4);
                // sourceDataTh = filter(ref sourceDataTh, 10);

                //unsen卡尔曼滤波算法，比纯卡尔曼滤波效果要好

                sourceDataTm = averge(ref sourceDataTm, 5);
                //sourceDataTm = midFilter(ref sourceDataTm, 5);
                //sourceDataTm = UFK_filter(sourceDataTm);


                sourceDataPc = averge(ref sourceDataPc, 6);
                //sourceDataPc = filter(ref sourceDataPc, 10);

                sourceDataPh = averge(ref sourceDataPh, 7);
                // sourceDataPh = filter(ref sourceDataPh, 10);

                sourceDataPm = averge(ref sourceDataPm, 8);
                // sourceDataPm = filter(ref sourceDataPm, 10);

                sourceDataQm5 = averge(ref sourceDataQm5, 9);
                //sourceDataQm5 = filter(ref sourceDataQm5, 10);

                sourceDataTemp1 = averge(ref sourceDataTemp1, 10);
                //sourceDataTemp1 = filter(ref sourceDataTemp1, 10);

                sourceDataTemp2 = averge(ref sourceDataTemp2, 11);
                //sourceDataTemp2 = filter(ref sourceDataTemp2, 10);

                sourceDataTemp3 = averge(ref sourceDataTemp3, 12);
                //sourceDataTemp3 = filter(ref sourceDataTemp3, 10);

                sourceDataTemp4 = averge(ref sourceDataTemp4, 13);
                //sourceDataTemp4 = filter(ref sourceDataTemp4, 10);

                sourceDataTemp5 = averge(ref sourceDataTemp5, 14);
                //sourceDataTemp5 = filter(ref sourceDataTemp5, 10);

                resultDataWh = TWFFT.FFT_filter(sourceDataWh);

                if (isFirstAver == false)
                {
                    for (int i = 3; i < sourceDataQc.Length - 3; i++)
                    {
                        //Log.Info(t.ToString("yyyy-MM-dd hh:mm:ss:fff"));
                        //Log.Info("index:" + i + " Value:" + (float)sourceDataTemp1[i]);
                        //Console.WriteLine("index:"+i);
                        if (collectDataFlag)
                        {
                            dt.Rows.Add(t.ToString("yyyy-MM-dd hh:mm:ss:fff"),
                                t,
                                // (sourceDataQc[i] - 1) < 0 ? 0 : (sourceDataQc[i] - 1) * 12.5 + (double)Properties.Settings.Default.QcAdjust,
                                //(sourceDataQh[i] - 1) < 0 ? 0 : (sourceDataQh[i] - 1) * 12.5 + (double)Properties.Settings.Default.QhAdjust,
                                CoolFlow[i - 3] + (double)Properties.Settings.Default.QcAdjust,
                                HotFlow[i - 3] + (double)Properties.Settings.Default.QhAdjust,
                                (sourceDataQm[i] - 1) < 0 ? 0 : (sourceDataQm[i] - 1) * 5 + (double)Properties.Settings.Default.QmAdjust,
                                sourceDataTc[i] * 10 + (double)Properties.Settings.Default.TcAdjust,
                                sourceDataTh[i] * 10 + (double)Properties.Settings.Default.ThAdjust,
                                sourceDataTm[i] * 10 + (double)Properties.Settings.Default.TmAdjust,
                                sourceDataPc[i] + (double)Properties.Settings.Default.PcAdjust,
                                sourceDataPh[i] + (double)Properties.Settings.Default.PhAdjust,
                                sourceDataPm[i] + (double)Properties.Settings.Default.PmAdjust,
                                sourceDataQm5[i],
                                Wh + (double)Properties.Settings.Default.WhAdjust,
                            index);
                            index++;
                        }
                        if (graphFlag)          //记录流程测试中的，曲线变化
                        {
                            GraphDt.Rows.Add(t.ToString("yyyy-MM-dd hh:mm:ss:fff"),
                                // (sourceDataQc[i] - 1) < 0 ? 0 : (sourceDataQc[i] - 1) * 12.5 + (double)Properties.Settings.Default.QcAdjust,
                                //(sourceDataQh[i] - 1) < 0 ? 0 : (sourceDataQh[i] - 1) * 12.5 + (double)Properties.Settings.Default.QhAdjust,
                                CoolFlow[i - 3] + (double)Properties.Settings.Default.QcAdjust,
                                HotFlow[i - 3] + (double)Properties.Settings.Default.QhAdjust,
                                (sourceDataQm[i] - 1) < 0 ? 0 : (sourceDataQm[i] - 1) * 5 + (double)Properties.Settings.Default.QmAdjust,
                                sourceDataTc[i] * 10 + (double)Properties.Settings.Default.TcAdjust,
                                sourceDataTh[i] * 10 + (double)Properties.Settings.Default.ThAdjust,
                                sourceDataTm[i] * 10 + (double)Properties.Settings.Default.TmAdjust,
                                sourceDataPc[i] + (double)Properties.Settings.Default.PcAdjust,
                                sourceDataPh[i] + (double)Properties.Settings.Default.PhAdjust,
                                sourceDataPm[i] + (double)Properties.Settings.Default.PmAdjust,
                                sourceDataQm5[i],
                                Wh + (double)Properties.Settings.Default.WhAdjust);
                        }
                        if (electDataFlag)  //电机自动找点时候采集的相关数据
                        {
                            ElectDt.Rows.Add(t.ToString("yyyy-MM-dd hh:mm:ss:fff"),
                                //(sourceDataQc[i] - 1) < 0 ? 0 : (sourceDataQc[i] - 1) * 12.5 + (double)Properties.Settings.Default.QcAdjust,
                                //(sourceDataQh[i] - 1) < 0 ? 0 : (sourceDataQh[i] - 1) * 12.5 + (double)Properties.Settings.Default.QhAdjust,
                                CoolFlow[i - 3] + (double)Properties.Settings.Default.QcAdjust,
                                HotFlow[i - 3] + (double)Properties.Settings.Default.QhAdjust,
                                (sourceDataQm[i] - 1) < 0 ? 0 : (sourceDataQm[i] - 1) * 5 + (double)Properties.Settings.Default.QmAdjust,
                                sourceDataTc[i] * 10 + (double)Properties.Settings.Default.TcAdjust,
                                sourceDataTh[i] * 10 + (double)Properties.Settings.Default.ThAdjust,
                                sourceDataTm[i] * 10 + (double)Properties.Settings.Default.TmAdjust,
                                sourceDataPc[i] + (double)Properties.Settings.Default.PcAdjust,
                                sourceDataPh[i] + (double)Properties.Settings.Default.PhAdjust,
                                sourceDataPm[i] + (double)Properties.Settings.Default.PmAdjust,
                                sourceDataQm5[i],
                                Wh + (double)Properties.Settings.Default.WhAdjust);
                        }
                        t = t.AddMilliseconds(10.0);
                    }
                    //Qc = (sourceDataQc[3] + sourceDataQc[102] - 2) < 0 ? 0 : Math.Round((sourceDataQc[3] + sourceDataQc[102] - 2) * 12.5 * 0.5, 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.QcAdjust;
                    //Qh = (sourceDataQh[3] + sourceDataQh[102] - 2) < 0 ? 0 : Math.Round((sourceDataQh[3] + sourceDataQh[102] - 2) * 12.5 * 0.5, 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.QhAdjust;
                    Qc = CoolFlow[CoolFlow.Length - 1] < 0 ? 0 : Math.Round(CoolFlow[CoolFlow.Length - 1], 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.QcAdjust;
                    Qh = HotFlow[CoolFlow.Length - 1] < 0 ? 0 : Math.Round(HotFlow[CoolFlow.Length - 1], 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.QhAdjust;
                    Qm = (sourceDataQm[3] + sourceDataQm[102] - 2) < 0 ? 0 : Math.Round((sourceDataQm[3] + sourceDataQm[102] - 2) * 5 * 0.5, 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.QmAdjust;
                    Tc = Math.Round((sourceDataTc[3] + sourceDataTc[102]) * 5, 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.TcAdjust;
                    Th = Math.Round((sourceDataTh[3] + sourceDataTh[102]) * 5, 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.ThAdjust;
                    Tm = Math.Round((sourceDataTm[3] + sourceDataTm[102]) * 5, 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.TmAdjust;
                    Pc = Math.Round((sourceDataPc[3] + sourceDataPc[102]) * 0.5, 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.PcAdjust;
                    Ph = Math.Round((sourceDataPh[3] + sourceDataPh[102]) * 0.5, 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.PhAdjust;

                    //热水压力
                    if (doData[2].get_bit(4) == 0)//热水进水阀关闭的时候，默认是0
                    {
                        Ph = 0;
                    }
                    else
                    {
                        if((doData[1].get_bit(7) == 0) && (doData[2].get_bit(0) == 0))//热水阀 热水变压阀都是关闭的时候，默认是0
                        {
                            Ph = 0;
                        }
                        if((doData[1].get_bit(7) == 1) && (doData[2].get_bit(0) == 0))//从热水泵的变频器读取
                        {
                            Ph = 0;
                        }
                        if ((doData[1].get_bit(7) == 0) && (doData[2].get_bit(0) == 1))//从热水变压泵的变频器读取
                        {
                            Ph = 0;
                        }
                    }

                    //冷水压力
                    if (doData[2].get_bit(3) == 0)//冷水进水阀关闭的时候，默认是0
                    {
                        Pc = 0;
                    }
                    else
                    {
                        if ((doData[2].get_bit(1) == 0) && (doData[2].get_bit(2) == 0))//冷水阀 冷水变压阀都是关闭的时候，默认是0
                        {
                            Pc = 0;
                        }
                        if ((doData[2].get_bit(1) == 1) && (doData[2].get_bit(2) == 0))//从冷水泵的变频器读取
                        {
                            Pc = 0;
                        }
                        if ((doData[2].get_bit(1) == 0) && (doData[2].get_bit(2) == 1))//从冷水变压泵的变频器读取
                        {
                            Pc = 0;
                        }
                    }
                    Pm = Math.Round((sourceDataPm[3] + sourceDataPm[102]) * 0.5, 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.PmAdjust;
                    Qm5 = Math.Round((sourceDataQm5[3] + sourceDataQm5[102]) * 0.5, 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.Qm5Adjust;
                    Temp1 = Math.Round((sourceDataTemp1[3] + sourceDataTemp1[102]) * 5, 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.Test1;
                    Temp2 = Math.Round((sourceDataTemp2[3] + sourceDataTemp2[102]) * 5, 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.Test2;
                    Temp3 = Math.Round((sourceDataTemp3[3] + sourceDataTemp3[102]) * 5, 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.Test3;
                    Temp4 = Math.Round((sourceDataTemp4[3] + sourceDataTemp4[102]) * 5, 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.Test4;
                    Temp5 = Math.Round((sourceDataTemp5[3] + sourceDataTemp5[102]) * 5, 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.Test5;
                    Wh = Math.Round(resultDataWh.ToList().Average(), 0, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.WhAdjust;
                    DataReadyToUpdateStatus();
                }
                isFirstAver = false;
                if (err != ErrorCode.Success && err != ErrorCode.WarningRecordEnd)
                {
                    HandleError(err);
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.Error("数据采集报错：" + ex.ToString());
                HandleError(err);
            }
        }

        private void WaveformAiCtrl1_DataReady_backup(object sender, BfdAiEventArgs args)
        {
            ErrorCode err = ErrorCode.Success;
            try
            {
                //The WaveformAiCtrl has been disposed.
                if (waveformAiCtrl1.State == ControlState.Idle)
                {
                    return;
                }
                if (m_dataScaled.Length < args.Count)
                {
                    m_dataScaled = new double[args.Count];
                }

                int chanCount = waveformAiCtrl1.Conversion.ChannelCount;
                int sectionLength = waveformAiCtrl1.Record.SectionLength;
                err = waveformAiCtrl1.GetData(args.Count, m_dataScaled);//读取数据     

                DateTime t = DateTime.Now;
                t = t.AddSeconds(-1.03);//采集到的是一秒钟之前的数据，因此需要对当前的时间减去1s，再减去30ms是因为该30ms的数据，并在当前缓冲区做平滑
                t.ToString("yyyy-MM-dd hh:mm:ss:fff");
                //Log.Info(t.ToString("yyyy-MM-dd hh:mm:ss:fff"));

                int dataIndex = 0;
                for (int i = 0; i < m_dataScaled.Length; i += 16)
                {
                    Qc = Math.Round(m_dataScaled[i + 0], 2, MidpointRounding.AwayFromZero);// * 5;          流量的量程 1-5V 对应  0-50L/min
                    Qh = Math.Round(m_dataScaled[i + 1], 2, MidpointRounding.AwayFromZero);// * 5;
                    Qm = Math.Round(m_dataScaled[i + 2], 2, MidpointRounding.AwayFromZero);// * 5;
                    Tc = Math.Round(m_dataScaled[i + 3], 2, MidpointRounding.AwayFromZero);// * 10;
                    Th = Math.Round(m_dataScaled[i + 4], 2, MidpointRounding.AwayFromZero);// * 10;
                    Tm = Math.Round(m_dataScaled[i + 5], 2, MidpointRounding.AwayFromZero);// * 10;
                    Pc = Math.Round(m_dataScaled[i + 6], 2, MidpointRounding.AwayFromZero);
                    Ph = Math.Round(m_dataScaled[i + 7], 2, MidpointRounding.AwayFromZero);
                    Pm = Math.Round(m_dataScaled[i + 8], 2, MidpointRounding.AwayFromZero);
                    Qm5 = Math.Round(m_dataScaled[i + 9], 2, MidpointRounding.AwayFromZero);
                    Temp1 = Math.Round(m_dataScaled[i + 10], 2, MidpointRounding.AwayFromZero);// * 10;
                    Temp2 = Math.Round(m_dataScaled[i + 11], 2, MidpointRounding.AwayFromZero);// * 10;
                    Temp3 = Math.Round(m_dataScaled[i + 12], 2, MidpointRounding.AwayFromZero);// * 10;
                    Temp4 = Math.Round(m_dataScaled[i + 13], 2, MidpointRounding.AwayFromZero);// * 10;
                    Temp5 = Math.Round(m_dataScaled[i + 14], 2, MidpointRounding.AwayFromZero);// * 10;
                    Wh = Math.Round(m_dataScaled[i + 15], 2, MidpointRounding.AwayFromZero) * 200;
                    //Console.WriteLine("Qh:" + Qh);
                    //Console.WriteLine("Qc:" + Qc);
                    //Console.WriteLine("Qm:" + Qm);
                    if (dataIndex < 6)
                    {
                        int typeIndex = 0;
                        sourceDataQc[dataIndex] = isFirstAver ? Qc : lastTempData[typeIndex * 6 + dataIndex]; typeIndex++;
                        sourceDataQh[dataIndex] = isFirstAver ? Qh : lastTempData[typeIndex * 6 + dataIndex]; typeIndex++;
                        sourceDataQm[dataIndex] = isFirstAver ? Qm : lastTempData[typeIndex * 6 + dataIndex]; typeIndex++;
                        sourceDataTc[dataIndex] = isFirstAver ? Tc : lastTempData[typeIndex * 6 + dataIndex]; typeIndex++;
                        sourceDataTh[dataIndex] = isFirstAver ? Th : lastTempData[typeIndex * 6 + dataIndex]; typeIndex++;
                        sourceDataTm[dataIndex] = isFirstAver ? Tm : lastTempData[typeIndex * 6 + dataIndex]; typeIndex++;
                        sourceDataPc[dataIndex] = isFirstAver ? Pc : lastTempData[typeIndex * 6 + dataIndex]; typeIndex++;
                        sourceDataPh[dataIndex] = isFirstAver ? Ph : lastTempData[typeIndex * 6 + dataIndex]; typeIndex++;
                        sourceDataPm[dataIndex] = isFirstAver ? Pm : lastTempData[typeIndex * 6 + dataIndex]; typeIndex++;
                        sourceDataQm5[dataIndex] = isFirstAver ? Qm5 : lastTempData[typeIndex * 6 + dataIndex]; typeIndex++;
                        sourceDataTemp1[dataIndex] = isFirstAver ? Temp1 : lastTempData[typeIndex * 6 + dataIndex]; typeIndex++;
                        sourceDataTemp2[dataIndex] = isFirstAver ? Temp2 : lastTempData[typeIndex * 6 + dataIndex]; typeIndex++;
                        sourceDataTemp3[dataIndex] = isFirstAver ? Temp3 : lastTempData[typeIndex * 6 + dataIndex]; typeIndex++;
                        sourceDataTemp4[dataIndex] = isFirstAver ? Temp4 : lastTempData[typeIndex * 6 + dataIndex]; typeIndex++;
                        sourceDataTemp5[dataIndex] = isFirstAver ? Temp5 : lastTempData[typeIndex * 6 + dataIndex]; typeIndex++;
                    }
                    if (dataIndex >= 18 && dataIndex <= 81)
                    {
                        sourceDataWh[dataIndex - 18] = Wh;
                    }
                    sourceDataQc[dataIndex + 5] = Qc;
                    sourceDataQh[dataIndex + 5] = Qh;
                    sourceDataQm[dataIndex + 5] = Qm;
                    sourceDataTc[dataIndex + 5] = Tc;
                    sourceDataTh[dataIndex + 5] = Th;
                    sourceDataTm[dataIndex + 5] = Tm;
                    //sourceDataTm2[index + 5] = Tm;

                    sourceDataPc[dataIndex + 5] = Pc;
                    sourceDataPh[dataIndex + 5] = Ph;
                    sourceDataPm[dataIndex + 5] = Pm;
                    sourceDataQm5[dataIndex + 5] = Qm5;
                    sourceDataTemp1[dataIndex + 5] = Temp1;
                    sourceDataTemp2[dataIndex + 5] = Temp2;
                    sourceDataTemp3[dataIndex + 5] = Temp3;
                    sourceDataTemp4[dataIndex + 5] = Temp4;
                    sourceDataTemp5[dataIndex + 5] = Temp5;
                    dataIndex++;
                    if (dataIndex >= 101)
                        dataIndex = 0;
                }
                //Console.WriteLine("液面高度：" + Wh);

                sourceDataQc = averge(ref sourceDataQc, 0);
                //sourceDataQc = filterKalMan(sourceDataQc);
                //sourceDataQc = averge(ref sourceDataQc, 0);

                sourceDataQh = averge(ref sourceDataQh, 1);
                //sourceDataQh = filter(ref sourceDataQh, 10);

                sourceDataQm = averge(ref sourceDataQm, 2);
                //sourceDataQm = filter(ref sourceDataQm, 10);

                sourceDataTc = averge(ref sourceDataTc, 3);
                //sourceDataTc = filter(ref sourceDataTc, 10);

                sourceDataTh = averge(ref sourceDataTh, 4);
                // sourceDataTh = filter(ref sourceDataTh, 10);

                //unsen卡尔曼滤波算法，比纯卡尔曼滤波效果要好

                sourceDataTm = averge(ref sourceDataTm, 5);
                //sourceDataTm = midFilter(ref sourceDataTm, 5);
                //sourceDataTm = UFK_filter(sourceDataTm);


                sourceDataPc = averge(ref sourceDataPc, 6);
                //sourceDataPc = filter(ref sourceDataPc, 10);

                sourceDataPh = averge(ref sourceDataPh, 7);
                // sourceDataPh = filter(ref sourceDataPh, 10);

                sourceDataPm = averge(ref sourceDataPm, 8);
                // sourceDataPm = filter(ref sourceDataPm, 10);

                sourceDataQm5 = averge(ref sourceDataQm5, 9);
                //sourceDataQm5 = filter(ref sourceDataQm5, 10);

                sourceDataTemp1 = averge(ref sourceDataTemp1, 10);
                //sourceDataTemp1 = filter(ref sourceDataTemp1, 10);

                sourceDataTemp2 = averge(ref sourceDataTemp2, 11);
                //sourceDataTemp2 = filter(ref sourceDataTemp2, 10);

                sourceDataTemp3 = averge(ref sourceDataTemp3, 12);
                //sourceDataTemp3 = filter(ref sourceDataTemp3, 10);

                sourceDataTemp4 = averge(ref sourceDataTemp4, 13);
                //sourceDataTemp4 = filter(ref sourceDataTemp4, 10);

                sourceDataTemp5 = averge(ref sourceDataTemp5, 14);
                //sourceDataTemp5 = filter(ref sourceDataTemp5, 10);

                resultDataWh = TWFFT.FFT_filter(sourceDataWh);

                if (isFirstAver == false)
                {
                    for (int i = 3; i < sourceDataQc.Length - 3; i++)
                    {
                        //Log.Info(t.ToString("yyyy-MM-dd hh:mm:ss:fff"));
                        //Log.Info("index:" + i + " Value:" + (float)sourceDataTemp1[i]);
                        //Console.WriteLine("index:"+i);
                        if (collectDataFlag)
                        {
                            dt.Rows.Add(t.ToString("yyyy-MM-dd hh:mm:ss:fff"),
                                t,
                                 (sourceDataQc[i] - 1) < 0 ? 0 : (sourceDataQc[i] - 1) * 12.5 + (double)Properties.Settings.Default.QcAdjust,
                                (sourceDataQh[i] - 1) < 0 ? 0 : (sourceDataQh[i] - 1) * 12.5 + (double)Properties.Settings.Default.QhAdjust,
                                (sourceDataQm[i] - 1) < 0 ? 0 : (sourceDataQm[i] - 1) * 5 + (double)Properties.Settings.Default.QmAdjust,
                                sourceDataTc[i] * 10 + (double)Properties.Settings.Default.TcAdjust,
                                sourceDataTh[i] * 10 + (double)Properties.Settings.Default.ThAdjust,
                                sourceDataTm[i] * 10 + (double)Properties.Settings.Default.TmAdjust,
                                sourceDataPc[i] + (double)Properties.Settings.Default.PcAdjust,
                                sourceDataPh[i] + (double)Properties.Settings.Default.PhAdjust,
                                sourceDataPm[i] + (double)Properties.Settings.Default.PmAdjust,
                                sourceDataQm5[i],
                                Wh + (double)Properties.Settings.Default.WhAdjust,
                            index);
                            index++;
                        }
                        if (graphFlag)          //记录流程测试中的，曲线变化
                        {
                            GraphDt.Rows.Add(t.ToString("yyyy-MM-dd hh:mm:ss:fff"),
                                 (sourceDataQc[i] - 1) < 0 ? 0 : (sourceDataQc[i] - 1) * 12.5 + (double)Properties.Settings.Default.QcAdjust,
                                (sourceDataQh[i] - 1) < 0 ? 0 : (sourceDataQh[i] - 1) * 12.5 + (double)Properties.Settings.Default.QhAdjust,
                                (sourceDataQm[i] - 1) < 0 ? 0 : (sourceDataQm[i] - 1) * 5 + (double)Properties.Settings.Default.QmAdjust,
                                sourceDataTc[i] * 10 + (double)Properties.Settings.Default.TcAdjust,
                                sourceDataTh[i] * 10 + (double)Properties.Settings.Default.ThAdjust,
                                sourceDataTm[i] * 10 + (double)Properties.Settings.Default.TmAdjust,
                                sourceDataPc[i] + (double)Properties.Settings.Default.PcAdjust,
                                sourceDataPh[i] + (double)Properties.Settings.Default.PhAdjust,
                                sourceDataPm[i] + (double)Properties.Settings.Default.PmAdjust,
                                sourceDataQm5[i],
                                Wh + (double)Properties.Settings.Default.WhAdjust);
                        }
                        if (electDataFlag)
                        {
                            ElectDt.Rows.Add(t.ToString("yyyy-MM-dd hh:mm:ss:fff"),
                                (sourceDataQc[i] - 1) < 0 ? 0 : (sourceDataQc[i] - 1) * 12.5 + (double)Properties.Settings.Default.QcAdjust,
                                (sourceDataQh[i] - 1) < 0 ? 0 : (sourceDataQh[i] - 1) * 12.5 + (double)Properties.Settings.Default.QhAdjust,
                                (sourceDataQm[i] - 1) < 0 ? 0 : (sourceDataQm[i] - 1) * 5 + (double)Properties.Settings.Default.QmAdjust,
                                sourceDataTc[i] * 10 + (double)Properties.Settings.Default.TcAdjust,
                                sourceDataTh[i] * 10 + (double)Properties.Settings.Default.ThAdjust,
                                sourceDataTm[i] * 10 + (double)Properties.Settings.Default.TmAdjust,
                                sourceDataPc[i] + (double)Properties.Settings.Default.PcAdjust,
                                sourceDataPh[i] + (double)Properties.Settings.Default.PhAdjust,
                                sourceDataPm[i] + (double)Properties.Settings.Default.PmAdjust,
                                sourceDataQm5[i],
                                Wh + (double)Properties.Settings.Default.WhAdjust);
                        }
                        t = t.AddMilliseconds(10.0);
                    }
                    Qc = (sourceDataQc[3] + sourceDataQc[102] - 2) < 0 ? 0 : Math.Round((sourceDataQc[3] + sourceDataQc[102] - 2) * 12.5 * 0.5, 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.QcAdjust;
                    Qh = (sourceDataQh[3] + sourceDataQh[102] - 2) < 0 ? 0 : Math.Round((sourceDataQh[3] + sourceDataQh[102] - 2) * 12.5 * 0.5, 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.QhAdjust;
                    Qm = (sourceDataQm[3] + sourceDataQm[102] - 2) < 0 ? 0 : Math.Round((sourceDataQm[3] + sourceDataQm[102] - 2) * 5 * 0.5, 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.QmAdjust;
                    Tc = Math.Round((sourceDataTc[3] + sourceDataTc[102]) * 5, 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.TcAdjust;
                    Th = Math.Round((sourceDataTh[3] + sourceDataTh[102]) * 5, 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.ThAdjust;
                    Tm = Math.Round((sourceDataTm[3] + sourceDataTm[102]) * 5, 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.TmAdjust;
                    Pc = Math.Round((sourceDataPc[3] + sourceDataPc[102]) * 0.5, 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.PcAdjust;
                    Ph = Math.Round((sourceDataPh[3] + sourceDataPh[102]) * 0.5, 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.PhAdjust;
                    Pm = Math.Round((sourceDataPm[3] + sourceDataPm[102]) * 0.5, 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.PmAdjust;
                    Qm5 = Math.Round((sourceDataQm5[3] + sourceDataQm5[102]) * 0.5, 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.Qm5Adjust;
                    Temp1 = Math.Round((sourceDataTemp1[3] + sourceDataTemp1[102]) * 5, 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.Test1;
                    Temp2 = Math.Round((sourceDataTemp2[3] + sourceDataTemp2[102]) * 5, 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.Test2;
                    Temp3 = Math.Round((sourceDataTemp3[3] + sourceDataTemp3[102]) * 5, 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.Test3;
                    Temp4 = Math.Round((sourceDataTemp4[3] + sourceDataTemp4[102]) * 5, 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.Test4;
                    Temp5 = Math.Round((sourceDataTemp5[3] + sourceDataTemp5[102]) * 5, 2, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.Test5;
                    Wh = Math.Round(resultDataWh.ToList().Average(), 0, MidpointRounding.AwayFromZero) + (double)Properties.Settings.Default.WhAdjust;
                    DataReadyToUpdateStatus();
                }
                isFirstAver = false;
                if (err != ErrorCode.Success && err != ErrorCode.WarningRecordEnd)
                {
                    HandleError(err);
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.Error("数据采集报错：" + ex.ToString());
                HandleError(err);
            }
        }

        //输入：任意长度的数组
        //输出：滤波之后相同长度的数组
        double Average;
        public double[] filterKalMan(double[] Observe)
        {
            double[] CanShu = { 1, 1, 1, 1, 1, 0, 0, 0 };
            //导入参数
            double KamanX = CanShu[0];
            double KamanP = CanShu[1];
            double KamanQ = CanShu[2];
            double KamanR = CanShu[3];
            double KamanY = CanShu[4];
            double KamanKg = CanShu[5];
            double KamanSum = CanShu[6];

            //加载观察值
            double[] True = new double[Observe.Length];
            for (int i = 0; i <= Observe.Length - 1; i++)
            {
                //对每个观察值迭代
                KamanY = KamanX;
                KamanP = KamanP + KamanQ;
                KamanKg = KamanP / (KamanP + KamanR);
                KamanX = (KamanY + KamanKg * (Observe[i] - KamanY));
                KamanSum += KamanX;
                True[i] = KamanX;
                KamanP = (1 - KamanKg) * KamanP;
            }
            Average = KamanSum / Observe.Length;
            return True;

        }

        private void WaveformAiCtrl1_Overrun(object sender, BfdAiEventArgs e)
        {
            return;
        }

        private void WaveformAiCtrl1_CacheOverflow(object sender, BfdAiEventArgs e)
        {
            return;
        }

        private void HandleError(ErrorCode err)
        {
            if (err != ErrorCode.Success)
            {
                MessageBox.Show("Sorry ! some errors happened, the error code is: " + err.ToString(), "AI_InstantAI");
            }
        }


        #endregion


        #region 预留模拟量输出

        public void AO_Func(int index, double value)
        {
            aoData[index] = value;
            collectData.InstantAo_Write(aoData);//输出模拟量函数
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


        //此算法用于历史曲线的处理，不适用于实时曲线的处理。一次性将所有的数据扔进来处理，如果是缓存区进来处理，会有边界效应。
        double[] UFK_filter(double[] res)
        {
            var filter = new UKF();
            double[] result = new double[res.Length];
            for (int i = 0; i < res.ToArray().Length; i++)
            {
                filter.Update(new[] { res[i] });
                result[i] = filter.getState()[0];
            }
            res = result;
            for (int i = 0; i < res.ToArray().Length; i++)
            {
                filter.Update(new[] { res[i] });
                result[i] = filter.getState()[0];
            }
            return result;
        }


        /// <summary>
        /// N为7的平均数据处理
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool isFirstMid = true;
        double[] midFilter(ref double[] data, int type)
        {
            if (isFirstMid)        //i=6開始
            {
                for (int i = 6; i < 100; i++)
                {
                    List<double> temp = new List<double>();
                    for (int j = 0; j < 7; j++)
                    {
                        temp.Add(data[i + j]);
                    }
                    temp.Sort();
                    data[i + 3] = temp[3];
                }
            }
            else
            {
                for (int i = 0; i < 100; i++)               //3——102
                {
                    List<double> temp = new List<double>();
                    for (int j = 0; j < 7; j++)
                    {
                        temp.Add(data[i + j]);
                    }
                    temp.Sort();
                    data[i + 3] = temp[3];
                }
            }
            //该六位数据，供下一组数据使用
            lastTempData[type * 6 + 0] = data[100];
            lastTempData[type * 6 + 1] = data[101];
            lastTempData[type * 6 + 2] = data[102];

            lastTempData[type * 6 + 3] = data[103];
            lastTempData[type * 6 + 4] = data[104];
            lastTempData[type * 6 + 5] = data[105];
            return data;
        }

        /// <summary>
        /// N为7的平均数据处理
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool isFirstAver = true;
        double[] averge(ref double[] data, int type)
        {
            if (isFirstAver)        //i=6開始
            {
                for (int i = 6; i < 100; i++)
                {
                    List<double> temp = new List<double>();
                    for (int j = 0; j < 7; j++)
                    {
                        temp.Add(data[i + j]);
                    }
                    double sum = temp.Sum() - temp.Max() - temp.Min();
                    data[i + 3] = Math.Round(sum / 5, 2, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                for (int i = 0; i < 100; i++)               //3——102
                {
                    List<double> temp = new List<double>();
                    for (int j = 0; j < 7; j++)
                    {
                        temp.Add(data[i + j]);
                    }
                    double sum = temp.Sum() - temp.Max() - temp.Min();
                    data[i + 3] = Math.Round(sum / 5, 2, MidpointRounding.AwayFromZero);
                }
            }
            //该六位数据，供下一组数据使用
            lastTempData[type * 6 + 0] = data[100];
            lastTempData[type * 6 + 1] = data[101];
            lastTempData[type * 6 + 2] = data[102];

            lastTempData[type * 6 + 3] = data[103];
            lastTempData[type * 6 + 4] = data[104];
            lastTempData[type * 6 + 5] = data[105];
            return data;
        }

        //
        public double[] dataFill(double begin, double end, int len)
        {
            double[] data = new double[len];
            data[0] = begin;
            data[len - 1] = end;
            double d = (end - begin) / (len - 1);
            for (int i = 1; i < len - 1; i++)
            {
                data[i] = begin + i * d;
            }
            return data;
        }

        /// <summary>
        /// 传入的数组为：前6位上次数据+后100位缓冲区数据
        /// 消除后100位滤波抖动
        /// </summary>
        /// <param name="sourceData"></param>
        /// <param name="N"></param>
        /// <returns></returns>
        double[] filter(ref double[] sourceData, int N)
        {
            double[] data = sourceData.ToList().Skip(6).Take(100).ToArray();
            double[] temp = new double[100];
            double value = -99;//前一个可信的value
            double new_value;
            int pos = 0;//记录上一个value的下标
            int count = 0;//缓存区计数器
            int addLength = 0;//每次遇到新的可用的value，要根据前一个value 复制的长度
            for (int i = 0; i < 100; i++)
            {
                count++;
                if (-99 == value)
                {
                    value = data[i];
                    //temp[i] = data[i];
                    pos = i;
                }
                else
                {
                    if (data[i] == value)
                    {
                        // addLength += count;
                        count = 0;
                    }

                    else
                    {

                        if (count >= N)
                        {
                            new_value = data[i];
                            Console.WriteLine(data[i]);
                            addLength = i - pos + 1;
                            count = 0;
                            double addvalue = (new_value - value) / addLength;
                            for (int j = 0; j < addLength; j++)
                            {
                                temp[pos + j] = value + addvalue * j;
                            }
                            value = data[i];
                            pos = i;
                            count = 0;
                            //addLength = 0;
                        }

                    }
                }
            }


            {
                new_value = data[99];
                Console.WriteLine("++:" + data[99]);
                addLength = 99 - pos + 1;
                count = 0;
                double addvalue = (new_value - value) / addLength;
                for (int j = 0; j < addLength; j++)
                {
                    temp[pos + j] = value + addvalue * j;
                }

            }
            for (int i = 6; i < sourceData.Length; i++)
            {
                sourceData[i] = temp[i - 6];
            }
            return sourceData;
        }

        #endregion

        #region 水箱按钮
        private bool doData00Flag = false;
        private void doData00_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn.BackColor == Color.Green)
            {
                //关闭 冷水制冷
                btn.BackColor = Color.LightGray;
                set_bit(ref doData[0], 0, false);
                //set_bit(ref doData[1], 2, false);
                control.InstantDo_Write(doData);
                doData00Flag = false;
            }
            else
            {
                //开启 冷水制冷
                if (Temp1 <= (double)(Properties.Settings.Default.Temp1Set))
                {
                    MessageBox.Show("冷水箱温度已符合设定温度，无法继续制冷");
                    return;
                }
                if (WhCool < (double)Properties.Settings.Default.WhMin)
                {
                    MessageBox.Show("冷水箱液面过低，无法开启");
                    return;
                }
                btn.BackColor = Color.Green;
                set_bit(ref doData[0], 0, true);
                set_bit(ref doData[1], 2, true);
                control.InstantDo_Write(doData);
                doData00Flag = true;
            }
        }

        private bool doData01Flag = false;
        private void doData01_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn.BackColor == Color.Green)
            {
                //关闭 热水加热
                doData01Flag = false;
                btn.BackColor = Color.LightGray;
                set_bit(ref doData[0], 1, false);
                //set_bit(ref doData[1], 3, false);  //循环泵不关
                control.InstantDo_Write(doData);
            }
            else
            {
                //开启 热水加热
                if (Temp2 >= (double)(Properties.Settings.Default.Temp2Set))
                {
                    MessageBox.Show("热水箱温度已符合设定温度，无法继续加热");
                    return;
                }
                if (WhHeat < (double)Properties.Settings.Default.WhMin)
                {
                    MessageBox.Show("热水箱液面过低，无法开启");
                    return;
                }
                doData01Flag = true;
                btn.BackColor = Color.Green;
                set_bit(ref doData[0], 1, true);
                set_bit(ref doData[1], 3, true);
                control.InstantDo_Write(doData);
            }
        }

        private bool doData02Flag = false;
        private void doData02_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn.BackColor == Color.Green)
            {
                //关闭 高温加热
                btn.BackColor = Color.LightGray;
                set_bit(ref doData[0], 2, false);
                //set_bit(ref doData[1], 4, false);
                control.InstantDo_Write(doData);
                doData02Flag = false;
            }
            else
            {
                //开启 高温加热
                if (Temp3 >= (double)(Properties.Settings.Default.Temp3Set))
                {
                    MessageBox.Show("高温水箱温度已符合设定温度，无法继续加热");
                    return;
                }
                btn.BackColor = Color.Green;
                set_bit(ref doData[0], 2, true);
                set_bit(ref doData[1], 4, true);
                control.InstantDo_Write(doData);
                doData02Flag = true;
            }
        }

        private bool doData03Flag = false;
        private void doData03_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn.BackColor == Color.Green)
            {
                //关闭 中温加热
                btn.BackColor = Color.LightGray;
                set_bit(ref doData[0], 3, false);
                //set_bit(ref doData[1], 5, false);
                control.InstantDo_Write(doData);
                doData03Flag = false;
            }
            else
            {
                //开启 中温加热
                if (Temp4 >= (double)(Properties.Settings.Default.Temp4Set))
                {
                    MessageBox.Show("中水箱温度已符合设定温度，无法继续加热");
                    return;
                }
                btn.BackColor = Color.Green;
                set_bit(ref doData[0], 3, true);
                set_bit(ref doData[1], 5, true);
                control.InstantDo_Write(doData);
                doData03Flag = true;
            }
        }

        private bool doData04Flag = false;
        private void doData04_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn.BackColor == Color.Green)
            {
                //关闭 常温制冷
                btn.BackColor = Color.LightGray;
                set_bit(ref doData[0], 4, false);
                //set_bit(ref doData[1], 6, false);
                control.InstantDo_Write(doData);
                doData04Flag = false;
            }
            else
            {
                //开启 常温制冷
                if (Temp5 <= (double)(Properties.Settings.Default.Temp5Set))
                {
                    MessageBox.Show("常温水箱温度已符合设定温度，无法继续制冷");
                    return;
                }
                btn.BackColor = Color.Green;
                set_bit(ref doData[0], 4, true);
                set_bit(ref doData[1], 6, true);
                control.InstantDo_Write(doData);
                doData04Flag = true;
            }
        }
        #endregion

        #region 管道图按钮


        /// <summary>
        /// 管道按钮检查约束
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool CheckClick(string name)
        {

            #region 冷水箱液面过低时，无法打开
            if (((name == "冷水泵") || (name == "冷循环泵") || (name == "冷水制冷") || (name == "冷水变压泵")) &&
                (WhCool < (double)Properties.Settings.Default.WhMin))
            {
                MessageBox.Show("冷水箱液面过低，无法开启");
                return true;
            }

            #endregion

            #region 热水箱液面过低时，无法打开
            if (((name == "热水泵") || (name == "热循环泵") || (name == "热水加热") || (name == "热水变压泵")) &&
                (WhCool < (double)Properties.Settings.Default.WhMin))
            {
                MessageBox.Show("热水箱液面过低，无法开启");
                return true;
            }

            #endregion

            #region 数字量输入报警，冷水泵、冷水变压泵、热水泵、热水变压泵 无法开启
            if ((name == "冷水泵") && isAlarm011)
            {
                MessageBox.Show("冷水泵报警，无法开启");
                return true;

            }
            if ((name == "冷水变压泵") && isAlarm012)
            {
                MessageBox.Show("冷水变压泵报警，无法开启");
                return true;
            }
            if ((name == "热水泵") && isAlarm021)
            {
                MessageBox.Show("热水泵报警，无法开启");
                return true;
            }
            if ((name == "热水变压泵") && isAlarm022)
            {
                MessageBox.Show("热水变压泵报警，无法开启");
                return true;
            }

            #endregion

            #region 热水加热

            if ((name == "热循环泵") && (WhHeat < (double)Properties.Settings.Default.WhMin))
            {
                MessageBox.Show("热水箱液面过低，无法开启");
                return true;
            }
            #endregion

            #region 冷水制冷

            if ((name == "冷循环泵") && (WhCool < (double)Properties.Settings.Default.WhMin))
            {
                MessageBox.Show("冷水箱液面过低，无法开启");
                return true;
            }
            #endregion

            #region 变压热水阀与热水阀不能同时打开
            if ((name == "热水阀") &&
                (doData[2].get_bit(0)==1))
            {
                MessageBox.Show("热水阀与变压热水阀无法同时打开");
                return true;
            }
            if ((name == "变压热水阀") &&
                (doData[1].get_bit(7) == 1))
            {
                MessageBox.Show("热水阀与变压热水阀无法同时打开");
                return true;
            }
            #endregion

            #region 变压冷水阀与冷水阀不能同时打开
            if ((name == "冷水阀") &&
                (doData[2].get_bit(2) == 1))
            {
                MessageBox.Show("冷水阀与变压冷水阀无法同时打开");
                return true;
            }
            if ((name == "变压冷水阀") &&
                (doData[2].get_bit(1) == 1))
            {
                MessageBox.Show("冷水阀与变压冷水阀无法同时打开");
                return true;
            }
            #endregion
            return false;
        }

        /// <summary>
        /// 进高温阀
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HslValves2_Click(object sender, EventArgs e)
        {
            var btn = sender as HslControls.HslValves;
            if (btn.EdgeColor == Color.Gray)     // 关->开
            {
                if (CheckClick(btn.Text))
                    return;
                btn.EdgeColor = Color.Red;
                set_bit(ref doData[0], 7, true);
                control.InstantDo_Write(doData);
            }
            else
            {
                btn.EdgeColor = Color.Gray;
                set_bit(ref doData[0], 7, false);
                control.InstantDo_Write(doData);
            }
        }

        /// <summary>
        /// 进中温阀
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HslValves3_Click(object sender, EventArgs e)
        {
            var btn = sender as HslControls.HslValves;
            if (btn.EdgeColor == Color.Gray)     // 关->开
            {
                if (CheckClick(btn.Text))
                    return;
                btn.EdgeColor = Color.Red;
                set_bit(ref doData[1], 0, true);
                control.InstantDo_Write(doData);
            }
            else
            {
                btn.EdgeColor = Color.Gray;
                set_bit(ref doData[1], 0, false);
                control.InstantDo_Write(doData);
            }
        }

        /// <summary>
        /// 进热水阀
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HslValves1_Click(object sender, EventArgs e)
        {
            var btn = sender as HslControls.HslValves;
            if (btn.EdgeColor == Color.Gray)     // 关->开
            {
                if (CheckClick(btn.Text))
                    return;
                btn.EdgeColor = Color.Red;
                set_bit(ref doData[0], 6, true);
                control.InstantDo_Write(doData);
            }
            else
            {
                btn.EdgeColor = Color.Gray;
                set_bit(ref doData[0], 6, false);
                control.InstantDo_Write(doData);
            }
        }

        /// <summary>
        /// 变压热水阀
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HslValves5_Click(object sender, EventArgs e)
        {
            var btn = sender as HslControls.HslValves;
            if (btn.EdgeColor == Color.Gray)     // 关->开
            {
                if (CheckClick(btn.Text))
                    return;
                btn.EdgeColor = Color.Red;
                set_bit(ref doData[2], 0, true);
                control.InstantDo_Write(doData);
            }
            else
            {
                btn.EdgeColor = Color.Gray;
                set_bit(ref doData[2], 0, false);
                control.InstantDo_Write(doData);
            }
        }

        /// <summary>
        /// 热水阀
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HslValves4_Click(object sender, EventArgs e)
        {
            var btn = sender as HslControls.HslValves;
            if (btn.EdgeColor == Color.Gray)     // 关->开
            {
                if (CheckClick(btn.Text))
                    return;
                btn.EdgeColor = Color.Red;
                set_bit(ref doData[1], 7, true);
                control.InstantDo_Write(doData);
            }
            else
            {
                btn.EdgeColor = Color.Gray;
                set_bit(ref doData[1], 7, false);
                control.InstantDo_Write(doData);
            }
        }

        /// <summary>
        /// 热水进水阀
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HslValves6_Click(object sender, EventArgs e)
        {
            var btn = sender as HslControls.HslValves;
            if (btn.EdgeColor == Color.Gray)     // 关->开
            {
                if (CheckClick(btn.Text))
                    return;
                btn.EdgeColor = Color.Red;
                set_bit(ref doData[2], 4, true);
                Console.WriteLine("true");
                control.InstantDo_Write(doData);
            }
            else
            {
                btn.EdgeColor = Color.Gray;
                set_bit(ref doData[2], 4, false);
                control.InstantDo_Write(doData);
            }
        }

        /// <summary>
        /// 冷水进水阀
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HslValves13_Click(object sender, EventArgs e)
        {
            var btn = sender as HslControls.HslValves;
            if (btn.EdgeColor == Color.Gray)     // 关->开
            {
                if (CheckClick(btn.Text))
                    return;
                btn.EdgeColor = Color.DodgerBlue;
                set_bit(ref doData[2], 3, true);
                control.InstantDo_Write(doData);
            }
            else
            {
                btn.EdgeColor = Color.Gray;
                set_bit(ref doData[2], 3, false);
                control.InstantDo_Write(doData);
            }
        }

        /// <summary>
        /// 冷水阀
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HslValves10_Click(object sender, EventArgs e)
        {
            var btn = sender as HslControls.HslValves;
            if (btn.EdgeColor == Color.Gray)     // 关->开
            {
                if (CheckClick(btn.Text))
                    return;
                btn.EdgeColor = Color.DodgerBlue;
                set_bit(ref doData[2], 1, true);
                control.InstantDo_Write(doData);
            }
            else
            {
                btn.EdgeColor = Color.Gray;
                set_bit(ref doData[2], 1, false);
                control.InstantDo_Write(doData);
            }
        }

        private void HslValves9_Click(object sender, EventArgs e)
        {
            var btn = sender as HslControls.HslValves;
            if (btn.EdgeColor == Color.Gray)     // 关->开
            {
                if (CheckClick(btn.Text))
                    return;
                btn.EdgeColor = Color.DodgerBlue;
                set_bit(ref doData[2], 2, true);
                control.InstantDo_Write(doData);
            }
            else
            {
                btn.EdgeColor = Color.Gray;
                set_bit(ref doData[2], 2, false);
                control.InstantDo_Write(doData);
            }
        }

        /// <summary>
        /// 进冷水阀
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HslValves8_Click(object sender, EventArgs e)
        {
            var btn = sender as HslControls.HslValves;
            if (btn.EdgeColor == Color.Gray)     // 关->开
            {
                if (CheckClick(btn.Text))
                    return;
                btn.EdgeColor = Color.DodgerBlue;
                set_bit(ref doData[0], 5, true);
                control.InstantDo_Write(doData);
            }
            else
            {
                btn.EdgeColor = Color.Gray;
                set_bit(ref doData[0], 5, false);
                control.InstantDo_Write(doData);
            }
        }

        /// <summary>
        /// 进常温阀
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HslValves7_Click(object sender, EventArgs e)
        {
            var btn = sender as HslControls.HslValves;
            if (btn.EdgeColor == Color.Gray)     // 关->开
            {
                if (CheckClick(btn.Text))
                    return;
                btn.EdgeColor = Color.DodgerBlue;
                set_bit(ref doData[1], 1, true);
                control.InstantDo_Write(doData);
            }
            else
            {
                btn.EdgeColor = Color.Gray;
                set_bit(ref doData[1], 1, false);
                control.InstantDo_Write(doData);
            }
        }

        private void HslPumpOne1_Click(object sender, EventArgs e)
        {
            var pump = sender as HslControls.HslPumpOne;
            if (pump.MoveSpeed == 0)//说明水泵当前处于关闭状态
            {
                if (CheckClick(pump.Text))
                    return;
                //执行打开水泵的代码
                pump.MoveSpeed = 1;
                set_bit(ref doData[1], 5, true);
                control.InstantDo_Write(doData);
            }
            else//说明水泵当前处于打开状态
            {
                pump.MoveSpeed = 0;
                set_bit(ref doData[1], 5, false);
                control.InstantDo_Write(doData);
            }
        }



        private void HslPumpOne3_Click(object sender, EventArgs e)
        {
            var pump = sender as HslControls.HslPumpOne;
            if (pump.MoveSpeed == 0)//说明水泵当前处于关闭状态
            {
                if (CheckClick(pump.Text))
                    return;
                //执行打开水泵的代码
                pump.MoveSpeed = 1;
                set_bit(ref doData[1], 2, true);
                control.InstantDo_Write(doData);
            }
            else//说明水泵当前处于打开状态
            {
                pump.MoveSpeed = 0;
                set_bit(ref doData[1], 2, false);
                control.InstantDo_Write(doData);
            }
        }

        private void HslPumpOne2_Click(object sender, EventArgs e)
        {
            var pump = sender as HslControls.HslPumpOne;
            if (pump.MoveSpeed == 0)//说明水泵当前处于关闭状态
            {
                if (CheckClick(pump.Text))
                    return;
                //执行打开水泵的代码
                pump.MoveSpeed = 1;
                set_bit(ref doData[1], 3, true);
                control.InstantDo_Write(doData);
            }
            else//说明水泵当前处于打开状态
            {
                pump.MoveSpeed = 0;
                set_bit(ref doData[1], 3, false);
                control.InstantDo_Write(doData);
            }
        }

        private void HslPumpOne4_Click(object sender, EventArgs e)
        {
            var pump = sender as HslControls.HslPumpOne;
            if (pump.MoveSpeed == 0)//说明水泵当前处于关闭状态
            {
                if (CheckClick(pump.Text))
                    return;
                //执行打开水泵的代码
                pump.MoveSpeed = 1;
                set_bit(ref doData[1], 4, true);
                control.InstantDo_Write(doData);
            }
            else//说明水泵当前处于打开状态
            {
                pump.MoveSpeed = 0;
                set_bit(ref doData[1], 4, false);
                control.InstantDo_Write(doData);
            }
        }

        private void HslPumpOne5_Click(object sender, EventArgs e)
        {
            var pump = sender as HslControls.HslPumpOne;
            if (pump.MoveSpeed == 0)//说明水泵当前处于关闭状态
            {
                if (CheckClick(pump.Text))
                    return;
                //执行打开水泵的代码
                pump.MoveSpeed = 1;
                set_bit(ref doData[1], 6, true);
                control.InstantDo_Write(doData);
            }
            else//说明水泵当前处于打开状态
            {
                pump.MoveSpeed = 0;
                set_bit(ref doData[1], 6, false);
                control.InstantDo_Write(doData);
            }
        }

        /// <summary>
        /// 出水阀
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HslValves11_Click(object sender, EventArgs e)
        {
            var btn = sender as HslControls.HslValves;
            if (btn.EdgeColor == Color.Gray)     // 关->开
            {
                if (CheckClick(btn.Text))
                    return;
                btn.EdgeColor = Color.Goldenrod;
                set_bit(ref doData[2], 5, true);//vm
                set_bit(ref doData[2], 6, true);//v5
                control.InstantDo_Write(doData);
            }
            else
            {
                btn.EdgeColor = Color.Gray;
                set_bit(ref doData[2], 5, false);
                set_bit(ref doData[2], 6, false);//v5
                control.InstantDo_Write(doData);
            }
        }

        /// <summary>
        /// 5s出水阀
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HslValves12_Click(object sender, EventArgs e)
        {
            var btn = sender as HslControls.HslValves;
            if (btn.EdgeColor == Color.Gray)     // 关->开
            {
                if (CheckClick(btn.Text))
                    return;
                btn.EdgeColor = Color.Goldenrod;
                set_bit(ref doData[2], 6, true);
                control.InstantDo_Write(doData);
            }
            else
            {
                btn.EdgeColor = Color.Gray;
                set_bit(ref doData[2], 6, false);
                control.InstantDo_Write(doData);
            }
        }

        /// <summary>
        /// 冷水泵
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HslPumpOne8_Click(object sender, EventArgs e)
        {
            var pump = sender as HslControls.HslPumpOne;
            if (pump.MoveSpeed == 0)//说明水泵当前处于关闭状态
            {
                if (CheckClick(pump.Text))
                    return;
                //执行打开水泵的代码
                pump.MoveSpeed = 1;
                set_bit(ref doData[2], 7, true);
                control.InstantDo_Write(doData);
            }
            else//说明水泵当前处于打开状态
            {
                pump.MoveSpeed = 0;
                set_bit(ref doData[2], 7, false);
                control.InstantDo_Write(doData);
            }
        }

        /// <summary>
        /// 冷水变压泵
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HslPumpOne6_Click(object sender, EventArgs e)
        {
            var pump = sender as HslControls.HslPumpOne;
            if (pump.MoveSpeed == 0)//说明水泵当前处于关闭状态
            {
                if (CheckClick(pump.Text))
                    return;
                //执行打开水泵的代码
                pump.MoveSpeed = 1;
                set_bit(ref doData[3], 0, true);
                control.InstantDo_Write(doData);
            }
            else//说明水泵当前处于打开状态
            {
                pump.MoveSpeed = 0;
                set_bit(ref doData[3], 0, false);
                control.InstantDo_Write(doData);
            }
        }

        /// <summary>
        /// 热水泵
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Reshui_Click(object sender, EventArgs e)
        {
            var pump = sender as HslControls.HslPumpOne;
            if (pump.MoveSpeed == 0)//说明水泵当前处于关闭状态
            {
                if (CheckClick(pump.Text))
                    return;
                //执行打开水泵的代码
                pump.MoveSpeed = 1;
                set_bit(ref doData[3], 1, true);
                control.InstantDo_Write(doData);
            }
            else//说明水泵当前处于打开状态
            {
                pump.MoveSpeed = 0;
                set_bit(ref doData[3], 1, false);
                control.InstantDo_Write(doData);
            }
        }

        /// <summary>
        /// 热水变压泵
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HslPumpOne7_Click(object sender, EventArgs e)
        {
            var pump = sender as HslControls.HslPumpOne;
            if (pump.MoveSpeed == 0)//说明水泵当前处于关闭状态
            {
                if (CheckClick(pump.Text))
                    return;
                //执行打开水泵的代码
                pump.MoveSpeed = 1;
                set_bit(ref doData[3], 2, true);
                control.InstantDo_Write(doData);
            }
            else//说明水泵当前处于打开状态
            {
                pump.MoveSpeed = 0;
                set_bit(ref doData[3], 2, false);
                control.InstantDo_Write(doData);
            }
        }

        private void TrackBar1_ValueChanged(object sender, EventArgs e)
        {     //输出模拟量
        }

        private void HslSwitch1_OnSwitchChanged(object arg1, bool arg2)
        {
            if (arg2)
            {
                bpq.write_coil("2078", true, 5);
                hslSwitch1.Text = "夹紧产品";
            }
            else
            {
                bpq.write_coil("2078", false, 5);
                hslSwitch1.Text = "松开产品";
            }
        }


        private void TrackBar1_Scroll(object sender, EventArgs e)
        {

        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            var value = Convert.ToDouble(WaterOut.Value.ToString()) * 0.1;
            AO_Func(0, value);            //输出模拟量
            Properties.Settings.Default.WaterOut = WaterOut.Value;
            Properties.Settings.Default.Save();
        }

        #endregion

        #region 电机控制


        public double autoFindAngle_A = 0;         //自动找点角度A
        private double autoFindAngle_L = 0;         //自动找点角度A    

        private bool isAutoFindAngle = false;
        public DataTable timeAngleDt;
        public Dictionary<double, double> tempAngleDict = new Dictionary<double, double>();
        public Dictionary<double, DateTime> tmTimeDict = new Dictionary<double, DateTime>();
        public Dictionary<DateTime, double> timeAngleDict = new Dictionary<DateTime, double>();

        #region 伺服电机A  地址变量

        private string powerAddress_A = "2056";   //M8-2056   M18-2066
        public bool powerState_A = false;

        public string forwardWriteAddress_A = "2048";
        private string forwardReadAddress_A = "2053";
        public bool forwardState_A = false;

        private string noForwardWriteAddress_A = "2049";
        private string noForwardReadAddress_A = "2054";
        private bool noForwadState_A = false;

        private string orignWriteAddress_A = "2050";
        private string orignReadAddress_A = "2055";
        private bool orignState_A = false;

        private string autoRunAddress_A = "2051";
        private string backOrignAddress_A = "2052";
        public string shutdownAddress_A = "2057";

        private string radioAddress_A = "6096";
        private uint radioValue_A = 0;
        private string angleAddress_A = "4100";
        public int angleValue_A = 0;

        #endregion

        #region 伺服电机L  地址变量

        private string powerAddress_L = "2066";   //M8-2056   M18-2066
        public bool powerState_L = false;

        private string forwardWriteAddress_L = "2058";
        private string forwardReadAddress_L = "2063";
        private bool forwardState_L = false;

        private string noForwardWriteAddress_L = "2059";
        private string noForwardReadAddress_L = "2064";
        private bool noForwadState_L = false;

        private string orignWriteAddress_L = "2060";
        private string orignReadAddress_L = "2065";
        private bool orignState_L = false;

        private string autoRunAddress_L = "2061";
        private string backOrignAddress_L = "2062";
        public string shutdownAddress_L = "2067";

        private string radioAddress_L = "6098";
        private uint radioValue_L = 0;
        private string angleAddress_L = "4102";
        private int angleValue_L = 0;

        #endregion

        //#region 伺服电机A事件
        ///// <summary>
        ///// 伺服开关
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void PowerSwitchBtn_A_Click(object sender, EventArgs e)
        //{
        //    if (powerState_A)
        //    {
        //        bpq.write_coil(powerAddress_A, false, 5);
        //        isAutoFindAngle = false;
        //    }
        //    else
        //        bpq.write_coil(powerAddress_A, true, 5);
        //}

        //private void ForwardBtn_A_MouseDown(object sender, MouseEventArgs e)
        //{
        //    if (!powerState_A)
        //    {
        //        MessageBox.Show("请开启伺服电机A");
        //    }
        //    else if (isAutoFindAngle)
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        forwardBtn_A.BackColor = Color.Green;
        //        bpq.write_coil(forwardWriteAddress_A, true, 5);
        //    }
        //}

        //private void ForwardBtn_A_MouseUp(object sender, MouseEventArgs e)
        //{
        //    if (!powerState_A)
        //    {
        //        MessageBox.Show("请开启伺服电机A");
        //    }
        //    else if (isAutoFindAngle)
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        forwardBtn_A.BackColor = DefaultBackColor;
        //        bpq.write_coil(forwardWriteAddress_A, false, 5);
        //    }
        //}

        //private void NoForwardBtn_A_MouseDown(object sender, MouseEventArgs e)
        //{
        //    if (!powerState_A)
        //    {
        //        MessageBox.Show("请开启伺服电机A");
        //    }
        //    else if (isAutoFindAngle)
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        noForwardBtn_A.BackColor = Color.Green;
        //        bpq.write_coil(noForwardWriteAddress_A, true, 5);
        //    }
        //}

        //private void NoForwardBtn_A_MouseUp(object sender, MouseEventArgs e)
        //{
        //    if (!powerState_A)
        //    {
        //        MessageBox.Show("请开启伺服电机A");
        //    }
        //    else if (isAutoFindAngle)
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        noForwardBtn_A.BackColor = DefaultBackColor;
        //        bpq.write_coil(noForwardWriteAddress_A, false, 5);
        //    }
        //}

        //private void ShutdownBtn_A_MouseDown(object sender, MouseEventArgs e)
        //{
        //    if (!powerState_A)
        //    {
        //        MessageBox.Show("请开启伺服电机A");
        //    }
        //    else if (isAutoFindAngle)
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        shutdownBtn_A.BackColor = Color.Green;
        //        bpq.write_coil(shutdownAddress_A, true, 5);
        //        isAutoFindAngle = false;
        //    }
        //}

        //private void ShutdownBtn_A_MouseUp(object sender, MouseEventArgs e)
        //{
        //    if (!powerState_A)
        //    {
        //        MessageBox.Show("请开启伺服电机A");
        //    }
        //    else if (isAutoFindAngle)
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        shutdownBtn_A.BackColor = DefaultBackColor;
        //        bpq.write_coil(shutdownAddress_A, false, 5);
        //        isAutoFindAngle = false;
        //    }
        //}
        //private void OrignBtn_A_MouseDown(object sender, MouseEventArgs e)
        //{
        //    if (!powerState_A)
        //    {
        //        MessageBox.Show("请开启伺服电机A");
        //    }
        //    else if (isAutoFindAngle)
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        orignBtn_A.BackColor = Color.Green;
        //        bpq.write_coil(orignWriteAddress_A, true, 5);
        //    }
        //}

        //private void OrignBtn_A_MouseUp(object sender, MouseEventArgs e)
        //{
        //    if (!powerState_A)
        //    {
        //        MessageBox.Show("请开启伺服电机A");
        //    }
        //    else if (isAutoFindAngle)
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        orignBtn_A.BackColor = DefaultBackColor;
        //        bpq.write_coil(orignWriteAddress_A, false, 5);
        //    }
        //}
        //private void BackOrignBtn_A_Click(object sender, EventArgs e)
        //{
        //    if (!powerState_A)
        //    {
        //        MessageBox.Show("请开启伺服电机A");
        //    }
        //    else if (isAutoFindAngle)
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        bpq.write_coil(backOrignAddress_A, true, 5);
        //    }
        //}
        //private void AutoRunBtn_A_Click(object sender, EventArgs e)
        //{
        //    if (!powerState_A)
        //    {
        //        MessageBox.Show("请开启伺服电机A");
        //    }
        //    else if (isAutoFindAngle)
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        bpq.write_coil(autoRunAddress_A, true, 5);
        //    }
        //}
        //private void AutoFind_A_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (!powerState_A)
        //        {
        //            MessageBox.Show("请开启伺服电机A");
        //        }
        //        if (autoFindAngle_A == 0)
        //        {
        //            MessageBox.Show("请先设置自动找点，电机预转动的角度");
        //            return;
        //        }
        //        if (MessageBox.Show("请确认是否设置好原点、转速等相关参数", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
        //        {
        //            timeAngleDt.Clear();
        //            ElectDt.Clear();
        //            SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "开始找点");
        //            isAutoFindAngle = true;
        //            electDataFlag = true;
        //            System.Threading.Thread.Sleep(100);
        //            //正传
        //            bpq.write_coil(forwardWriteAddress_A, true, 5);
        //            SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "开始正传...");
        //            while (isAutoFindAngle && (angleValue_A <= autoFindAngle_A))
        //            {
        //                //等待角度达到 预设角度
        //            }
        //            bpq.write_coil(forwardWriteAddress_A, false, 5);
        //            SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "结束正传...");
        //            if (isAutoFindAngle == false)
        //            {
        //                SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "当前已手动停止找点...");
        //                timeAngleDt.Clear();
        //                ElectDt.Clear();
        //                return;
        //            }
        //            //反传
        //            bpq.write_coil(noForwardWriteAddress_A, true, 5);
        //            SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "开始反传...");
        //            while (isAutoFindAngle && (angleValue_A >= 0))
        //            {
        //                //等待角度达到 原点
        //            }
        //            bpq.write_coil(noForwardWriteAddress_A, false, 5);
        //            SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "结束反传...");
        //            if (isAutoFindAngle == false)
        //            {
        //                SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "当前已手动停止找点...");
        //                timeAngleDt.Clear();
        //                ElectDt.Clear();
        //                return;
        //            }

        //            SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "数据采集完毕，开始分析...");
        //            AnalyseElect();
        //            if (tempAngleDict.Count > 0)
        //            {
        //                SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "采集的点位如下...");
        //                foreach (var dic in tempAngleDict)
        //                {
        //                    SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "温度：" + dic.Key + "-----> 角度：" + dic.Value);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            return;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error("自动找点异常：" + ex.ToString());
        //        return;
        //    }
        //    finally
        //    {
        //        isAutoFindAngle = false;
        //        electDataFlag = false;
        //    }
        //}
        /// <summary>
        /// 分析温度与角度对应关系
        /// </summary>
        private void AnalyseElect()
        {
            tmTimeDict = new Dictionary<double, DateTime>();
            timeAngleDict = new Dictionary<DateTime, double>();
            tempAngleDict = new Dictionary<double, double>();
            foreach (DataRow tmRow in ElectDt.Rows)
            {
                if (tmTimeDict.Keys.Count == 3)
                {
                    break;
                }
                var tm = tmRow["出水温度Tm"].AsDouble();
                var tmTime = tmRow["时间"].AsDateTime();
                if (tm == 36)
                {
                    if (tmTimeDict.ContainsKey(36))
                        continue;
                    else
                        tmTimeDict.Add(36, tmTime);
                }
                if (tm == 38)
                {
                    if (tmTimeDict.ContainsKey(38))
                        continue;
                    else
                        tmTimeDict.Add(38, tmTime);
                }
                if (tm == 40)
                {
                    if (tempAngleDict.ContainsKey(40))
                        continue;
                    else
                        tmTimeDict.Add(40, tmTime);
                }
            }
            foreach (DataRow eleRow in timeAngleDt.Rows)
            {
                if (timeAngleDict.Keys.Count == 3)
                {
                    break;
                }
                var angle = eleRow["角度"].AsDouble();
                var angleTime = eleRow["时间"].AsDateTime();

                if (timeAngleDict.ContainsKey(tmTimeDict[36]) == false && (angleTime - tmTimeDict[36]).TotalMilliseconds < 0.2)
                {
                    timeAngleDict.Add(tmTimeDict[36], angle);
                }

                if (timeAngleDict.ContainsKey(tmTimeDict[38]) == false && (angleTime - tmTimeDict[38]).TotalMilliseconds < 0.2)
                {
                    timeAngleDict.Add(tmTimeDict[38], angle);
                }

                if (timeAngleDict.ContainsKey(tmTimeDict[40]) == false && (angleTime - tmTimeDict[40]).TotalMilliseconds < 0.2)
                {
                    timeAngleDict.Add(tmTimeDict[40], angle);
                }
            }

            tempAngleDict.Add(36, timeAngleDict[tmTimeDict[36]]);
            tempAngleDict.Add(38, timeAngleDict[tmTimeDict[38]]);
            tempAngleDict.Add(40, timeAngleDict[tmTimeDict[40]]);
        }

        //private void RadioBtn_A_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        uint val2 = (uint)(double.Parse(radioTb_A.Text) * 10000);

        //        if (val2 < 0 || val2 > 200000)
        //        {
        //            MessageBox.Show("请输入0-20 范围内的值");
        //            radioTb_A.Text = "";
        //        }
        //        else if (isAutoFindAngle)
        //        {
        //            return;
        //        }
        //        else
        //        {
        //            Write_uint(radioAddress_A, val2, 5);
        //            SystemInfoPrint("写入：【" + radioAddress_A + "】【" + val2 + "】\n");
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        MessageBox.Show("请输入正确的格式");
        //        return;
        //    }

        //}
        //private void AngleBtn_A_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (isAutoFindAngle)
        //        {
        //            return;
        //        }
        //        autoFindAngle_A = angleTb_A.Text.ToDouble();
        //        SystemInfoPrint("写入自动找点预转动角度值：【" + autoFindAngle_A + "】【" + DateTime.Now.ToString() + "】\n");

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("请输入正确的格式");
        //        return;
        //    }
        //}

        //#endregion

        //#region 伺服电机L事件
        //private void PowerBtn_L_Click(object sender, EventArgs e)
        //{
        //    if (powerState_L)
        //        bpq.write_coil(powerAddress_L, false, 5);
        //    else
        //        bpq.write_coil(powerAddress_L, true, 5);
        //}

        //private void ShutdownBtn_L_MouseDown(object sender, MouseEventArgs e)
        //{
        //    if (!powerState_L)
        //    {
        //        MessageBox.Show("请开启伺服电机L");
        //    }
        //    else
        //    {
        //        shutdownBtn_L.BackColor = Color.Green;
        //        bpq.write_coil(shutdownAddress_L, true, 5);
        //    }
        //}

        //private void ShutdownBtn_L_MouseUp(object sender, MouseEventArgs e)
        //{
        //    if (!powerState_L)
        //    {
        //        MessageBox.Show("请开启伺服电机L");
        //    }
        //    else
        //    {
        //        shutdownBtn_L.BackColor = DefaultBackColor;
        //        bpq.write_coil(shutdownAddress_L, false, 5);
        //    }
        //}

        //private void OrignBtn_L_MouseDown(object sender, MouseEventArgs e)
        //{
        //    if (!powerState_L)
        //    {
        //        MessageBox.Show("请开启伺服电机L");
        //    }
        //    else
        //    {
        //        orignBtn_L.BackColor = Color.Green;
        //        bpq.write_coil(orignWriteAddress_L, true, 5);
        //    }
        //}

        //private void OrignBtn_L_MouseUp(object sender, MouseEventArgs e)
        //{
        //    if (!powerState_L)
        //    {
        //        MessageBox.Show("请开启伺服电机L");
        //    }
        //    else
        //    {
        //        orignBtn_L.BackColor = DefaultBackColor;
        //        bpq.write_coil(orignWriteAddress_L, false, 5);
        //    }
        //}

        //private void ForwardBtn_L_MouseDown(object sender, MouseEventArgs e)
        //{
        //    if (!powerState_L)
        //    {
        //        MessageBox.Show("请开启伺服电机L");
        //    }
        //    else
        //    {
        //        forwardBtn_L.BackColor = Color.Green;
        //        bpq.write_coil(forwardWriteAddress_L, true, 5);
        //    }
        //}

        //private void ForwardBtn_L_MouseUp(object sender, MouseEventArgs e)
        //{
        //    if (!powerState_L)
        //    {
        //        MessageBox.Show("请开启伺服电机L");
        //    }
        //    else
        //    {
        //        forwardBtn_L.BackColor = DefaultBackColor;
        //        bpq.write_coil(forwardWriteAddress_L, false, 5);
        //    }
        //}

        //private void NoForwardBtn_L_MouseDown(object sender, MouseEventArgs e)
        //{
        //    if (!powerState_L)
        //    {
        //        MessageBox.Show("请开启伺服电机L");
        //    }
        //    else
        //    {
        //        noForwardBtn_L.BackColor = Color.Green;
        //        bpq.write_coil(noForwardWriteAddress_L, true, 5);
        //    }
        //}

        //private void NoForwardBtn_L_MouseUp(object sender, MouseEventArgs e)
        //{
        //    if (!powerState_L)
        //    {
        //        MessageBox.Show("请开启伺服电机L");
        //    }
        //    else
        //    {
        //        noForwardBtn_L.BackColor = DefaultBackColor;
        //        bpq.write_coil(noForwardWriteAddress_L, false, 5);
        //    }
        //}

        //private void BackOrignBtn_L_Click(object sender, EventArgs e)
        //{
        //    if (!powerState_L)
        //    {
        //        MessageBox.Show("请开启伺服电机L");
        //    }
        //    else
        //    {
        //        bpq.write_coil(backOrignAddress_L, true, 5);
        //    }
        //}

        //private void AutoRunBtn_L_Click(object sender, EventArgs e)
        //{
        //    if (!powerState_L)
        //    {
        //        MessageBox.Show("请开启伺服电机L");
        //    }
        //    else
        //    {
        //        bpq.write_coil(autoRunAddress_L, true, 5);
        //    }
        //}

        //private void RadioBtn_L_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        uint val2 = (uint)(double.Parse(radioTb_L.Text) * 8000);
        //        if (val2 < 0 || val2 > 200000)
        //        {
        //            MessageBox.Show("请输入0-25 范围内的值");
        //            radioTb_L.Text = "";
        //        }
        //        else
        //        {

        //            Write_uint(radioAddress_L, val2, 5);
        //            SystemInfoPrint("写入：【" + radioAddress_L + "】【" + val2 + "】\n");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("请输入正确的格式");
        //        return;
        //    }
        //}

        //private void AngleBtn_L_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        autoFindAngle_L = angleTb_L.Text.ToDouble();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("请输入正确的格式");
        //        return;
        //    }
        //}
        //#endregion

        #endregion

        private void HslPumpOne8_Load(object sender, EventArgs e)
        {

        }
    }
}
