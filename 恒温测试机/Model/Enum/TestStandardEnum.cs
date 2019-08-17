﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 恒温测试机.Utils;

namespace 恒温测试机.Model.Enum
{
    public enum TestStandardEnum
    {
        [EnumDescription("EN1111-2017")]
        default1711 = 1,
        [EnumDescription("灵敏度流程")]
        sensitivityProcess=2,
        [EnumDescription("QB/T 2806-2017标准")]
        default2806 = 3,
        [EnumDescription("自定义")]
        blank = 10
    }
}
