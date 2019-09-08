using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 恒温测试机.Model
{
    public class Model_1111
    {
        #region 安全性测试
        /// <summary>
        /// 冷水压力
        /// </summary>
        public double Pc { get; set; }

        /// <summary>
        /// 冷水温度
        /// </summary>
        public double Tc { get; set; }

        /// <summary>
        /// 热水压力
        /// </summary>
        public double Ph { get; set; }

        /// <summary>
        /// 热水温度
        /// </summary>
        public double Th { get; set; }

        /// <summary>
        /// 混合水流量
        /// </summary>
        public double Qm { get; set; }

        /// <summary>
        /// 混合水温度
        /// </summary>
        public double Tm { get; set; }

        /// <summary>
        /// 冷水压力降为0，Qm5
        /// </summary>
        public double A_1_Qc { get; set; }

        /// <summary>
        /// 冷水压力降为0，出水温度
        /// </summary>
        public double A_1_Tc { get; set; }

        /// <summary>
        /// 冷水压力降为0，与初始出水温度之差
        /// </summary>
        public double A_1_Tcdiff { get; set; }

        /// <summary>
        /// 冷水压力恢复，Qm5
        /// </summary>
        public double A_2_Qc { get; set; }

        
        /// <summary>
        /// 压力回至原始压力，出水温度
        /// </summary>
        public double A_3_Tm { get; set; }

        /// <summary>
        /// 压力回至原始压力，与初始出水温度之差
        /// </summary>
        public double A_3_Tmdiff { get; set; }

        #endregion

        #region 稳定性测试

        #region 热水压力
        /// <summary>
        /// 热水降压，出水温度
        /// </summary>
        public double H_1_Tm { get; set; }

        /// <summary>
        /// 热水降压，高于初始出水温度3℃时间
        /// </summary>
        public double H_1_3 { get; set; }

        /// <summary>
        /// 热水降压，与初始出水温度之差
        /// </summary>
        public double H_1_Tmdiff { get; set; }

        /// <summary>
        /// 热水降压恢复，出水温度
        /// </summary>
        public double H_2_Tm { get; set; }

        /// <summary>
        /// 热水降压回复，与初始出水温度之差
        /// </summary>
        public double H_2_Tmdiff { get; set; }

        #endregion

        #region 冷水压力
        /// <summary>
        /// 冷水降压，出水温度
        /// </summary>
        public double C_1_Tm { get; set; }

        /// <summary>
        /// 冷水降压，高于初始出水温度3℃时间
        /// </summary>
        public double C_1_3 { get; set; }

        /// <summary>
        /// 冷水降压，与初始出水温度之差
        /// </summary>
        public double C_1_Tmdiff { get; set; }

        /// <summary>
        /// 冷水降压恢复，出水温度
        /// </summary>
        public double C_2_Tm { get; set; }

        /// <summary>
        /// 冷水降压回复，与初始出水温度之差
        /// </summary>
        public double C_2_Tmdiff { get; set; }

        #endregion

        #endregion

        #region 降温测试
        public double Cool_1_Tm { get; set; }
        public double Cool_1_3 { get; set; }
        public double Cool_1_Tmdiff { get; set; }
        public double Cool_2_Tm { get; set; }
        public double Cool_2_Tmdiff { get; set; }
        #endregion

        #region 温度稳定性
        public double Steady_1_Tm { get; set; }
        public double Steady_1_Tmdiff { get; set; }
        public double Steady_2_Tm { get; set; }
        public double Steady_2_Tmdiff { get; set; }
        public double Steady_2_3 { get; set; }

        public double Steady_3_Tm { get; set; }
        public double Steady_3_Tmdiff { get; set; }
        public double Steady_4_Tm { get; set; }
        public double Steady_4_Tmdiff { get; set; }
        public double Steady_4_3 { get; set; }
        #endregion

        #region 流量减少
        public double FLow_1_Qm { get; set; }
        public double FLow_1_Tm { get; set; }
        public double FLow_1_Tmdiff { get; set; }
        public double FLow_2_Tm { get; set; }
        public double FLow_2_Tmdiff { get; set; }
        #endregion
    }
}
