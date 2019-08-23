using HslCommunication;
using HslCommunication.ModBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 恒温测试机
{
    /// <summary>
    /// 用于初始化串口的各种参数
    /// </summary>
    /// 
    public struct COMconfig
    {
        //波特率
        public string botelv;
        //站号
        public string zhanhao;
        //数据位个数
        public string shujuwei;
        //停止位个数 
        public string tingzhiwei;
        //数据位从零开始读      
        public bool dataFromZero;
        //字符串反转
        public bool stringReverse;
        //串口名称
        public string COM_Name;
        //奇偶校验: 0:无校验 1:奇校验 2:偶校验
        public int checkInfo;


        //0:ABCD 
        //1:BADC
        //2:CDAB  这里的int类型用这个
        //3:DCBA 
        public int dataFrame;

    }
    public struct readRtuDataCMD
    {
        public string zijidizhi;//子机地址 1B
        public string gongnengma;//功能码 1B
        public string adress;//寄存器起始地址 2B
        public string readNum;//读取数量 2B
        public string CRC;// 2B
    }
    public class M_485Rtu
    {
        private ModbusRtu busRtuClient = null;
        public COMconfig config;
        public M_485Rtu(COMconfig c)
        {
            config = c;
        }
        public double short2float(short res, short pointnum)
        {
            double ans = res;
            while (pointnum != 0)
            {
                pointnum -= 1;
                ans = ans * 0.1;
            }
            return ans;
        }
        public short bytes2short(byte h, byte l)
        {
            short s = 0;   //一个16位整形变量，初值为 0000 0000 0000 0000            
            s = (short)(s ^ h);  //将b1赋给s的低8位
            s = (short)(s << 8);  //s的低8位移动到高8位
            s = (short)(s ^ l); //在b2赋给s的低8位
            return s;
        }
        //报文读取
        public string ReadFrame(string cmd)
        {
            string info = "";
            OperateResult<byte[]> read = busRtuClient.ReadBase(HslCommunication.Serial.SoftCRC16.CRC16(HslCommunication.BasicFramework.SoftBasic.HexStringToBytes(cmd)));
            if (read.IsSuccess)
            {
                info = "Result：" + HslCommunication.BasicFramework.SoftBasic.ByteToHexString(read.Content);
            }
            else
            {
                MessageBox.Show("Read Failed：" + read.ToMessageShowString());
            }
            return info;
        }
        public void disConnect()
        {
            busRtuClient.Close();

            //MessageBox.Show("串口已关闭");
        }
        public short read_short(string adreess, byte station)
        {
            busRtuClient.Station = station;
            // short读取
            OperateResult<short> result = busRtuClient.ReadInt16(adreess);
            if (result.IsSuccess) return result.Content;
            else return (short)-999;
        }

        public short[] read_short_batch(string adreess, int len, byte station)
        {
            busRtuClient.Station = station;
            short[] data = new short[len];
            OperateResult<byte[]> read = busRtuClient.Read(adreess, (ushort)len);
            // short批量读取
            if (!read.IsSuccess)
            {
                return null;
            }
            else
            {
                Console.WriteLine("length:" + read.Content.Length);
                for (int i = 0; i < read.Content.Length; i += 2)
                    data[i / 2] = bytes2short(read.Content[i], read.Content[i + 1]);
                return data;
            }

        }

        public void write_short(string adreess, short val, byte station)
        {
            busRtuClient.Station = station;
            // short写入
            OperateResult result = busRtuClient.Write(adreess, val);
            if (!result.IsSuccess) MessageBox.Show("short写入失败");
        }
        public bool read_coil(string address, byte station)
        {
            busRtuClient.Station = station;
            //线圈读取
            OperateResult<bool> result = busRtuClient.ReadCoil(address);
            if (result.IsSuccess)
                return result.Content;
            else
                return false;
        }
        public void write_coil(string adreess, bool val, byte station)
        {
            busRtuClient.Station = station;
            //写入线圈
            OperateResult result = busRtuClient.WriteCoil(adreess, val);
            if (!result.IsSuccess) MessageBox.Show("线圈写入失败");
        }
        // 读取float变量
        public float read_float(string adreess, byte station)
        {
            busRtuClient.Station = station;
            OperateResult<float> result = busRtuClient.ReadFloat(adreess);
            if (result.IsSuccess) return result.Content;
            else return (float)-999.0;
        }
        public void write_int(string adreess, int val, byte station)
        {
            //int写入
            busRtuClient.Station = station;
            OperateResult result = busRtuClient.Write(adreess, val);
            if (!result.IsSuccess) MessageBox.Show("int写入失败");
        }
        //读取int类型变量
        public int read_int(string adreess, byte station)
        {
            busRtuClient.Station = station;
            OperateResult<int> result = busRtuClient.ReadInt32(adreess);
            if (result.IsSuccess) return result.Content;
            else return (int)-999.0;
        }

        public void write_uint(string adreess, uint val, byte station)
        {
            //uint写入
            busRtuClient.Station = station;
            OperateResult result = busRtuClient.Write(adreess, val);
            if (!result.IsSuccess) MessageBox.Show("uint写入失败");
        }
        //读取uint类型变量
        public uint read_uint(string adreess, byte station)
        {
            busRtuClient.Station = station;
            OperateResult<uint> result = busRtuClient.ReadUInt32(adreess);
            if (result.IsSuccess) return result.Content;
            else return 0;
        }
        public double read_double(string adress, byte station)
        {
            busRtuClient.Station = station;
            // 读取double变量
            OperateResult<double> result = busRtuClient.ReadDouble(adress);
            if (result.IsSuccess) return result.Content;
            else return -999.0;
        }
        public void connect()
        {
            if (!int.TryParse(config.botelv, out int baudRate))
            {
                MessageBox.Show(DemoUtils.BaudRateInputWrong);
                return;
            }

            if (!int.TryParse(config.shujuwei, out int dataBits))
            {
                MessageBox.Show(DemoUtils.DataBitsInputWrong);
                return;
            }

            if (!int.TryParse(config.tingzhiwei, out int stopBits))
            {
                MessageBox.Show(DemoUtils.StopBitInputWrong);
                return;
            }


            if (!byte.TryParse(config.zhanhao, out byte station))
            {
                MessageBox.Show("Station input wrong！");
                return;
            }

            busRtuClient?.Close();
            busRtuClient = new ModbusRtu(station);

            busRtuClient.AddressStartWithZero = config.dataFromZero;

            busRtuClient.IsStringReverse = config.stringReverse;
            switch (config.dataFrame)
            {
                case 0: busRtuClient.DataFormat = HslCommunication.Core.DataFormat.ABCD; break;
                case 1: busRtuClient.DataFormat = HslCommunication.Core.DataFormat.BADC; break;
                case 2: busRtuClient.DataFormat = HslCommunication.Core.DataFormat.CDAB; break;
                case 3: busRtuClient.DataFormat = HslCommunication.Core.DataFormat.DCBA; break;
                default: busRtuClient.DataFormat = HslCommunication.Core.DataFormat.CDAB; break;//默认是CDAB 读取int
            }
            try
            {
                busRtuClient.SerialPortInni(sp =>
                {
                    sp.PortName = config.COM_Name;
                    sp.BaudRate = baudRate;
                    sp.DataBits = dataBits;
                    sp.StopBits = stopBits == 0 ? System.IO.Ports.StopBits.None : (stopBits == 1 ? System.IO.Ports.StopBits.One : System.IO.Ports.StopBits.Two);
                    sp.Parity = config.checkInfo == 0 ? System.IO.Ports.Parity.None : (config.checkInfo == 1 ? System.IO.Ports.Parity.Odd : System.IO.Ports.Parity.Even);
                });
                busRtuClient.Open();

                // button2.Enabled = true;
                // button1.Enabled = false;
                //panel2.Enabled = true;
                //userControlCurve1.ReadWriteNet = busRtuClient;
            }
            catch (Exception ex)
            {
                MessageBox.Show("串口打开失败!!");
                MessageBox.Show(ex.Message);
            }
        }
    }
}
