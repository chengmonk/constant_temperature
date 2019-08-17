using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 恒温测试机.Model
{
    public class Model_2806
    {
        /// <summary>
        /// 冷水压力
        /// </summary>
        public string Pc { get; set; }

        /// <summary>
        /// 冷水温度
        /// </summary>
        public string Tc { get; set; }

        /// <summary>
        /// 热水压力
        /// </summary>
        public string Ph { get; set; }

        /// <summary>
        /// 热水温度
        /// </summary>
        public string Th { get; set; }

        /// <summary>
        /// 混合水流量
        /// </summary>
        public string Qm { get; set; }

        /// <summary>
        /// 混合水温度
        /// </summary>
        public string Tm { get; set; }


    }
}
