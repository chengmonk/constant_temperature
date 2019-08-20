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
using 恒温测试机.Utils;

namespace 恒温测试机.UI
{
    public partial class FormConnectValueSetting : Form
    {
        private FormMain formMain;
        private byte station = 1;

        System.Timers.Timer monitorTimer;            //监控M寄存器定时器
        System.Timers.Timer monitorDTimer;            //监控D寄存器定时器

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

        private string radioAddress_A = "4296";
        private uint radioValue_A = 0;
        private string angleAddress_A = "5432";
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

        private string radioAddress_L = "4298";
        private uint radioValue_L = 0;
        private string angleAddress_L = "5434";
        private int angleValue_L = 0;

        #endregion

        public FormConnectValueSetting(FormMain formMain)
        {
            InitializeComponent();
            InitTimer();
            this.formMain = formMain;
            timeAngleDt = new DataTable();
            timeAngleDt.Columns.Add("时间", typeof(string));
            timeAngleDt.Columns.Add("角度", typeof(double));
        }


        private void InitTimer()
        {
            monitorTimer = new System.Timers.Timer(1000);
            monitorTimer.Elapsed += (o, a) =>
            {
                MonitorActive();
            };//到达时间的时候执行事件；
            monitorTimer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；
            monitorTimer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；

            monitorDTimer = new System.Timers.Timer(1000);
            monitorDTimer.Elapsed += (o, a) =>
            {
                MonitorDActive();
            };//到达时间的时候执行事件；
            monitorDTimer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；
            monitorDTimer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
        }

        #region 委托

        private delegate void MonitorActiveDelegate();
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
                    #region 伺服电机A
                    powerState_A = formMain.bpq.read_coil(powerAddress_A, 5);
                    forwardState_A = formMain.bpq.read_coil(forwardReadAddress_A, 5);
                    noForwadState_A = formMain.bpq.read_coil(noForwardReadAddress_A, 5);
                    orignState_A = formMain.bpq.read_coil(orignReadAddress_A, 5);

                    powerBtn_A.BackColor = powerState_A ? Color.Green : DefaultBackColor;
                    powerBtn_A.Text = powerState_A ? "伺服开" : "伺服关";
                    forwardBtn_A.BackColor = forwardState_A ? Color.Green : DefaultBackColor;
                    noForwardBtn_A.BackColor = noForwadState_A ? Color.Green : DefaultBackColor;
                    orignBtn_A.BackColor = orignState_A ? Color.Green : DefaultBackColor;
                    #endregion

                    #region 伺服电机L
                    powerState_L = formMain.bpq.read_coil(powerAddress_L, 5);
                    forwardState_L = formMain.bpq.read_coil(forwardReadAddress_L, 5);
                    noForwadState_L = formMain.bpq.read_coil(noForwardReadAddress_L, 5);
                    orignState_L = formMain.bpq.read_coil(orignReadAddress_L, 5);

                    powerBtn_L.BackColor = powerState_L ? Color.Green : DefaultBackColor;
                    powerBtn_L.Text = powerState_L ? "伺服开" : "伺服关";
                    forwardBtn_L.BackColor = forwardState_L ? Color.Green : DefaultBackColor;
                    noForwardBtn_L.BackColor = noForwadState_L ? Color.Green : DefaultBackColor;
                    orignBtn_L.BackColor = orignState_L ? Color.Green : DefaultBackColor;
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Log.Error("监控M寄存器异常：" + ex.ToString());
                return;
            }
        }

        private delegate void MonitorDActiveDelegate();
        private void MonitorDActive()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    MonitorDActiveDelegate monitorActiveDelegate = MonitorDActive;
                    this.Invoke(monitorActiveDelegate);
                }
                else
                {
                    #region 伺服电机A
                    radioValue_A = formMain.bpq.read_uint(radioAddress_A, 5);   //转速  读取 uint
                    angleValue_A = formMain.bpq.read_int(angleAddress_A, 5);    //角度  读取 int

                    var temp1 = (radioValue_A * 0.0001);
                    var temp2 = (angleValue_A * 0.0001);
                    radioLb_A.Text = "" + temp1;
                    angelLb_A.Text = "" + temp2;

                    if (isAutoFindAngle)
                    {
                        timeAngleDt.Rows.Add(
                            DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss:fff"),
                            temp2);
                    }
                    #endregion

                    #region 伺服电机L
                    radioValue_L = formMain.bpq.read_uint(radioAddress_L, 5);   //转速  读取 uint
                    angleValue_L = formMain.bpq.read_int(angleAddress_L, 5);    //角度  读取 int

                    var temp3 = (radioValue_L * 0.0001);
                    var temp4 = (angleValue_L * 0.0001);
                    radioLb_L.Text = "" + temp3;
                    angelLb_L.Text = "" + temp4;
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Log.Error("监控D寄存器异常：" + ex.ToString());
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
                    outInfoTb.AppendText(msg);
                    outInfoTb.AppendText("\n");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return;
            }
        }
        #endregion

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
            var val=formMain.Read(textBox2.Text, station);
            SystemInfoPrint("读取：【" + textBox2.Text + "】【" + val + "】\n");
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
                formMain.Write(textBox2.Text, val3, station);
                SystemInfoPrint("写入：【" + textBox2.Text + "】【" + val3 + "】\n");
            }
        }

        #endregion

        #region 伺服电机A 按钮事件
        /// <summary>
        /// 伺服开关
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PowerSwitchBtn_A_Click(object sender, EventArgs e)
        {
            if (powerState_A)
            {
                formMain.bpq.write_coil(powerAddress_A, false, 5);
                isAutoFindAngle = false;
            }
            else
                formMain.bpq.write_coil(powerAddress_A, true, 5);
        }

        private void ForwardBtn_A_Click(object sender, EventArgs e)
        {
            if (!powerState_A)
            {
                MessageBox.Show("请开启伺服电机A");
            }
            else if (isAutoFindAngle)
            {
                return;
            }
            else
            {
                if(forwardState_A)
                    formMain.bpq.write_coil(forwardWriteAddress_A, false, 5);
                else
                    formMain.bpq.write_coil(forwardWriteAddress_A, true, 5);
            }
        }

        private void NoForwardBtn_A_Click(object sender, EventArgs e)
        {
            if (!powerState_A)
            {
                MessageBox.Show("请开启伺服电机A");
            }
            else if (isAutoFindAngle)
            {
                return;
            }
            else
            {
                if(noForwadState_A)
                    formMain.bpq.write_coil(noForwardWriteAddress_A, false, 5);
                else
                    formMain.bpq.write_coil(noForwardWriteAddress_A, true, 5);
            }
        }

        private void OrignBtn_A_MouseClick(object sender, MouseEventArgs e)
        {
            if (!powerState_A)
            {
                MessageBox.Show("请开启伺服电机A");
            }
            else if (isAutoFindAngle)
            {
                return;
            }
            else
            {
                if(orignState_A)
                    formMain.bpq.write_coil(orignWriteAddress_A, true, 5);
                else
                    formMain.bpq.write_coil(orignWriteAddress_A, false, 5);
            }
        }

        private void ShutdownBtn_A_MouseDown(object sender, MouseEventArgs e)
        {
            if (!powerState_A)
            {
                MessageBox.Show("请开启伺服电机A");
            }
            else
            {
                formMain.bpq.write_coil(shutdownAddress_A, true, 5);
                isAutoFindAngle = false;
            }
        }

        private void ShutdownBtn_A_MouseUp(object sender, MouseEventArgs e)
        {
            if (!powerState_A)
            {
                MessageBox.Show("请开启伺服电机A");
            }
            else
            {
                formMain.bpq.write_coil(shutdownAddress_A, false, 5);
                isAutoFindAngle = false;
            }
        }

        private void BackOrignBtn_Click(object sender, EventArgs e)
        {
            if (!powerState_A)
            {
                MessageBox.Show("请开启伺服电机A");
            }
            else if (isAutoFindAngle)
            {
                return;
            }
            else
            {
                formMain.bpq.write_coil(backOrignAddress_A, true, 5);
            }
        }

        private void AutoRunBtn_Click(object sender, EventArgs e)
        {
            if (!powerState_A)
            {
                MessageBox.Show("请开启伺服电机A");
            }
            else if (isAutoFindAngle)
            {
                return;
            }
            else
            {
                formMain.bpq.write_coil(autoRunAddress_A, true, 5);
            }
        }

        private void RadioBtn_A_Click(object sender, EventArgs e)
        {
            int val2 = Convert.ToInt32(radioTb_A.Text) * 10000;

            if (val2 < 0 || val2 > 200000)
            {
                MessageBox.Show("请输入0-20 范围内的值");
                radioTb_A.Text = "";
            }
            else if (isAutoFindAngle)
            {
                return;
            }
            else
            {
                short val3 = Convert.ToInt16("" + val2);
                formMain.Write(radioAddress_A, val3, 5);
                SystemInfoPrint("写入：【" + radioAddress_A + "】【" + val3 + "】\n");
            }
        }

        private void AngleBtn_A_Click(object sender, EventArgs e)
        {
            try
            {
                if (isAutoFindAngle)
                {
                    return;
                }
                autoFindAngle_A = angleTb_A.Text.ToDouble();
                SystemInfoPrint("写入自动找点预转动角度值：【" + autoFindAngle_A + "】【" + DateTime.Now.ToString() + "】\n");

            }
            catch (Exception ex)
            {
                MessageBox.Show("请输入正确的格式");
                return;
            }
        }

        #endregion

        #region 伺服电机L 按钮事件
        /// <summary>
        /// 伺服开关
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PowerSwitchBtn_L_Click(object sender, EventArgs e)
        {
            if (powerState_L)
                formMain.bpq.write_coil(powerAddress_L, false, 5);
            else
                formMain.bpq.write_coil(powerAddress_L, true, 5);
        }

        private void ForwardBtn_L_MouseClick(object sender, MouseEventArgs e)
        {
            if (!powerState_L)
            {
                MessageBox.Show("请开启伺服电机L");
            }
            else
            {
                if (forwardState_L)
                    formMain.bpq.write_coil(forwardWriteAddress_L, false, 5);
                else
                    formMain.bpq.write_coil(forwardWriteAddress_L, true, 5);
            }
        }

        private void NoForwardBtn_L_Click(object sender, EventArgs e)
        {
            if (!powerState_L)
            {
                MessageBox.Show("请开启伺服电机L");
            }
            else
            {
                if (noForwadState_L)
                    formMain.bpq.write_coil(noForwardWriteAddress_L, false, 5);
                else
                    formMain.bpq.write_coil(noForwardWriteAddress_L, true, 5);
            }
        }

        private void OrignBtn_L_MouseClick(object sender, MouseEventArgs e)
        {
            if (!powerState_L)
            {
                MessageBox.Show("请开启伺服电机L");
            }
            else
            {
                if (orignState_L)
                    formMain.bpq.write_coil(orignWriteAddress_L, true, 5);
                else
                    formMain.bpq.write_coil(orignWriteAddress_L, false, 5);
            }
        }

        private void ShutdownBtn_L_MouseDown(object sender, MouseEventArgs e)
        {
            if (!powerState_L)
            {
                MessageBox.Show("请开启伺服电机L");
            }
            else
            {
                formMain.bpq.write_coil(shutdownAddress_L, true, 5);
            }
        }

        private void ShutdownBtn_L_MouseUp(object sender, MouseEventArgs e)
        {
            if (!powerState_L)
            {
                MessageBox.Show("请开启伺服电机L");
            }
            else
            {
                formMain.bpq.write_coil(shutdownAddress_L, false, 5);
            }
        }


        private void BackOrignBtn_L_Click(object sender, EventArgs e)
        {
            if (!powerState_L)
            {
                MessageBox.Show("请开启伺服电机L");
            }
            else
            {
                formMain.bpq.write_coil(backOrignAddress_L, true, 5);
            }
        }

        private void AutoRunBtn_L_Click(object sender, EventArgs e)
        {
            if (!powerState_L)
            {
                MessageBox.Show("请开启伺服电机L");
            }
            else
            {
                formMain.bpq.write_coil(autoRunAddress_L, true, 5);
            }
        }
        private void AngleBtn_L_Click(object sender, EventArgs e)
        {
            try
            {
                autoFindAngle_L = angleTb_L.Text.ToDouble();
            }
            catch (Exception ex)
            {
                MessageBox.Show("请输入正确的格式");
                return;
            }
        }

        private void RadioBtn_L_Click(object sender, EventArgs e)
        {
            int val2 = Convert.ToInt32(radioTb_L.Text) * 10000;

            if (val2 < 0 || val2 > 200000)
            {
                MessageBox.Show("请输入0-20 范围内的值");
                radioTb_L.Text = "";
            }
            else
            {
                short val3 = Convert.ToInt16("" + val2);
                formMain.Write(radioAddress_L, val3, 5);
                SystemInfoPrint("写入：【" + radioAddress_L + "】【" + val3 + "】\n");
            }
        }

        #endregion

        #region 自动找点

        private void AutoFind_A_Click(object sender, EventArgs e)
        {
            try
            {
                if (!powerState_A)
                {
                    MessageBox.Show("请开启伺服电机A");
                }
                if (autoFindAngle_A == 0)
                {
                    MessageBox.Show("请先设置自动找点，电机预转动的角度");
                    return;
                }
                if (MessageBox.Show("请确认是否设置好原点、转速等相关参数", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    timeAngleDt.Clear();
                    formMain.ElectDt.Clear();
                    SystemInfoPrint("【"+DateTime.Now.ToString()+"】"+"开始找点");
                    isAutoFindAngle = true;
                    formMain.electDataFlag = true;
                    System.Threading.Thread.Sleep(100);
                    //正传
                    formMain.bpq.write_coil(forwardWriteAddress_A, true, 5);
                    SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "开始正传...");
                    while (isAutoFindAngle && (angleValue_A<=autoFindAngle_A))
                    {
                        //等待角度达到 预设角度
                    }
                    formMain.bpq.write_coil(forwardWriteAddress_A, false, 5);
                    SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "结束正传...");
                    if (isAutoFindAngle == false)
                    {
                        SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "当前已手动停止找点...");
                        timeAngleDt.Clear();
                        formMain.ElectDt.Clear();
                        return;
                    }
                    //反传
                    formMain.bpq.write_coil(noForwardWriteAddress_A, true, 5);
                    SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "开始反传...");
                    while (isAutoFindAngle && (angleValue_A == 0))
                    {
                        //等待角度达到 原点
                    }
                    formMain.bpq.write_coil(noForwardWriteAddress_A, false, 5);
                    SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "结束反传...");
                    if (isAutoFindAngle == false)
                    {
                        SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "当前已手动停止找点...");
                        timeAngleDt.Clear();
                        formMain.ElectDt.Clear();
                        return;
                    }

                    SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "数据采集完毕，开始分析...");
                    AnalyseElect();
                    if (tempAngleDict.Count > 0)
                    {
                        SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "采集的点位如下...");
                        foreach(var dic in tempAngleDict)
                        {
                            SystemInfoPrint("【" + DateTime.Now.ToString() + "】" + "温度：" + dic.Key + "-----> 角度：" + dic.Value);
                        }
                    }
                }
                else
                {
                    return;
                }
            }
            catch(Exception ex)
            {
                Log.Error("自动找点异常："+ex.ToString());
                return;
            }
            finally
            {
                isAutoFindAngle = false;
                formMain.electDataFlag = false;
                
            }
        }

        /// <summary>
        /// 分析温度与角度对应关系
        /// </summary>
        private void AnalyseElect()
        {
            tmTimeDict = new Dictionary<double, DateTime>();
            timeAngleDict = new Dictionary<DateTime, double>();
            tempAngleDict = new Dictionary<double, double>();
            foreach (DataRow tmRow in formMain.ElectDt.Rows)
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

        private void Button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "文档|*.csv";
            fileDialog.InitialDirectory = Application.StartupPath;
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                DataTableUtils.DataTableToCsvT(timeAngleDt, fileDialog.FileName);
                MessageBox.Show("保存成功!");
                
            }
            fileDialog.Dispose();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "文档|*.csv";
            fileDialog.InitialDirectory = Application.StartupPath;
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                DataTableUtils.DataTableToCsvT(formMain.ElectDt, fileDialog.FileName);
                MessageBox.Show("保存成功!");
            }
            fileDialog.Dispose();
        }

        #endregion

    }
}
