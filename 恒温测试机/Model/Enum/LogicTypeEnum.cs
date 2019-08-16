using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 恒温测试机.Utils;

namespace 恒温测试机.Model.Enum
{
    /// <summary>
    /// 子界面操作选择枚举
    /// </summary>
    public enum LogicTypeEnum
    {
        [EnumDescription("安全性测试")]
        safeTest = 1,
        [EnumDescription("压力变化测试")]
        PressureTest = 2,
        [EnumDescription("降温测试")]
        CoolTest = 3,
        [EnumDescription("温度稳定性测试")]
        TemTest = 4,
        [EnumDescription("流量减少测试")]
        FlowTest = 5,
        [EnumDescription("灵敏度测试")]
        SensitivityTest=6,
        [EnumDescription("保真度测试")]
        FidelityTest=7,
        [EnumDescription("出水温度稳定性测试")]
        TmSteadyTest=8,
        [EnumDescription("升温测试")]
        HeatTest=9,
        [EnumDescription("最高限温测试")]
        MaxHeatTest=10
    }
}
