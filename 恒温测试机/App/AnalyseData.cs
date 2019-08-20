using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 恒温测试机.Utils;

namespace 恒温测试机.App
{
    /// <summary>
    /// 分析数据是否符合标准
    /// </summary>
    public class AnalyseData
    {
        /// <summary>
        /// 规定时间内，温度是否小于某个值
        /// </summary>
        /// <param name="second">规定时间内</param>
        /// <param name="flagTemp">阈值</param>
        /// <param name="data">传入数据</param>
        /// <returns></returns>
        public bool TempIsLower(double second,double flagTemp,DataTable data)
        {
            if (data == null)
            {
                return false;
            }
            data.Rows.RemoveAt(data.Rows.Count - 1);//移除末尾
            var startTime = data.Rows[1]["分析时间"].AsDateTime();
            var isFirstRow = true;
            foreach(DataRow row in data.Rows)
            {
                if (isFirstRow)
                {
                    isFirstRow = false;
                    continue;
                }
                var currentTime = row["分析时间"].AsDateTime();
                var diffSeconds = (currentTime - startTime).TotalSeconds;
                if (diffSeconds <= second) {
                    var tmVal = row["出水温度Tm"].AsDouble();
                    if (tmVal > flagTemp)
                    {
                        return false;
                    }
                }
                else
                {
                    break;
                }
            }
            return true;
        }

        /// <summary>
        /// 规定时间内，流量降至阈值，记录Qm5
        /// </summary>
        /// <param name="second">规定时间</param>
        /// <param name="flagValue">阈值</param>
        /// <param name="data">传入数据</param>
        /// <returns></returns>
        public double QmIsLower(double second,double flagValue,DataTable data)
        {
            if (data == null)
            {
                return 0;
            }
            data.Rows.RemoveAt(data.Rows.Count-1);//移除末尾
            //Log.Info("Test Log:"+data.Rows[1]["时间"].AsString());
            var startTime = data.Rows[1]["分析时间"].AsDateTime();
            var isFirstRow = true;
            foreach (DataRow row in data.Rows)
            {
                if (isFirstRow)
                {
                    isFirstRow = false;
                    continue;
                }
                var currentTime = row["分析时间"].AsDateTime();
                var diffSeconds = (currentTime - startTime).TotalSeconds;
                if (diffSeconds <= second)
                {
                    var qmVal = row["出水流量Qm"].AsDouble();
                    if (qmVal < flagValue)
                    {
                        var qm5Val = row["出水重量Qm5"].AsDouble();
                        return qm5Val;
                    }
                }
                else
                {
                    break;
                }
            }
            return 0;
        }

        /// <summary>
        /// 温度是否在阈值区间
        /// </summary>
        /// <param name="flagTemp">阈值</param>
        /// <param name="regionTemp">区间大小</param>
        /// <param name="data">传入数据</param>
        /// <returns></returns>
        public bool TmIsBetween(double flagTemp,double regionTemp,DataTable data)
        {
            if (data == null)
            {
                return false;
            }
            data.Rows.RemoveAt(data.Rows.Count - 1);//移除末尾
            var isFirstRow = true;
            foreach (DataRow row in data.Rows)
            {
                if (isFirstRow)
                {
                    isFirstRow = false;
                    continue;
                }
                var tmVal= row["出水温度Tm"].AsDouble();
                if (tmVal < flagTemp - regionTemp || tmVal > flagTemp + regionTemp)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 设定时间后，出水温度偏差在某区间内
        /// </summary>
        /// <param name="second"></param>
        /// <param name="tempRegion"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool TmDeviationIsBetween(double second,double tempRegion,DataTable data)
        {
            if (data == null)
            {
                return false;
            }
            data.Rows.RemoveAt(data.Rows.Count - 1);//移除末尾
            var isFirstRow = true;
            var isAfterSecond = false;
            var startTime = data.Rows[1]["分析时间"].AsDateTime();
            double maxTm = 0;
            double minTm = 0;
            foreach(DataRow row in data.Rows)
            {
                if (isFirstRow)
                {
                    isFirstRow = false;
                    continue;
                }
                var currentTime = row["分析时间"].AsDateTime();
                var diffSeconds = (currentTime - startTime).TotalSeconds;
                if (diffSeconds <= second)
                {
                    continue;
                }
                else
                {
                    var tmVal = row["出水温度Tm"].AsDouble();
                    if (isAfterSecond == false)
                    {
                        minTm = tmVal;
                        maxTm = tmVal;
                        isAfterSecond = true;
                    }
                    else
                    {
                        minTm = tmVal < minTm ? tmVal : minTm;
                        maxTm = tmVal > maxTm ? tmVal : maxTm;
                    }
                }
            }
            return maxTm - minTm <= 2 ? true : false;
        }

        /// <summary>
        /// 设定时间后，出水温度与所设定的温度的偏差在某区间内
        /// </summary>
        /// <param name="second"></param>
        /// <param name="tempFlag"></param>
        /// <param name="tempRegion"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool TmDeviationIsBetweenFlagRegion(double second,double tempFlag,double tempRegion,DataTable data)
        {
            if (data == null)
            {
                return false;
            }
            data.Rows.RemoveAt(data.Rows.Count - 1);//移除末尾
            var isFirstRow = true;
            var startTime = data.Rows[1]["分析时间"].AsDateTime();
            foreach (DataRow row in data.Rows)
            {
                if (isFirstRow)
                {
                    isFirstRow = false;
                    continue;
                }
                var currentTime = row["分析时间"].AsDateTime();
                var diffSeconds = (currentTime - startTime).TotalSeconds;
                if (diffSeconds <= second)
                {
                    continue;
                }
                else
                {
                    var tmVal = row["出水温度Tm"].AsDouble();
                    if (Math.Abs(tmVal - tempFlag) > tempRegion)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 出水温度规定时间内，超过阈值的时间
        /// </summary>
        /// <param name="second"></param>
        /// <param name="tempFlag"></param>
        /// <returns></returns>
        public double TmOverFlagRegion(double second,double tempFlag, DataTable data)
        {
            if (data == null)
            {
                return 0;
            }
            data.Rows.RemoveAt(data.Rows.Count - 1);//移除末尾
            var isFirstRow = true;
            var isOverFlag = false;
            var startTime = data.Rows[1]["分析时间"].AsDateTime();
            var lastOverTime = new DateTime();
            double overSec = 0;
            foreach (DataRow row in data.Rows)
            {
                if (isFirstRow)
                {
                    isFirstRow = false;
                    continue;
                }
                var currentTime = row["分析时间"].AsDateTime();
                var diffSeconds = (currentTime - startTime).TotalSeconds;
                if (diffSeconds <= second)
                {
                    var tmVal = row["出水温度Tm"].AsDouble();
                    if (tmVal > tempFlag)
                    {
                        if (isOverFlag == false)
                        {
                            lastOverTime = currentTime;
                            isOverFlag = true;
                        }
                    }
                    else
                    {
                        if (isOverFlag)
                        {
                            overSec += (currentTime - lastOverTime).TotalSeconds;
                            isOverFlag = false;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
            if (isOverFlag)
            {
                overSec += second - (lastOverTime - startTime).TotalSeconds;
            }

            return overSec;
        }

        /// <summary>
        /// 出水温度规定时间内，小于阈值的时间
        /// </summary>
        /// <param name="second"></param>
        /// <param name="tempFlag"></param>
        /// <returns></returns>
        public double TmBelowFlagRegion(double second, double tempFlag,DataTable data)
        {
            if (data == null)
            {
                return 0;
            }
            data.Rows.RemoveAt(data.Rows.Count - 1);//移除末尾
            var isFirstRow = true;
            var isBelowFlag = false;
            var startTime = data.Rows[1]["分析时间"].AsDateTime();
            var lastBelowTime = new DateTime();
            double belowSec = 0;
            foreach (DataRow row in data.Rows)
            {
                if (isFirstRow)
                {
                    isFirstRow = false;
                    continue;
                }
                var currentTime = row["分析时间"].AsDateTime();
                var diffSeconds = (currentTime - startTime).TotalSeconds;
                if (diffSeconds <= second)
                {
                    var tmVal = row["出水温度Tm"].AsDouble();
                    if (tmVal < tempFlag)
                    {
                        if (isBelowFlag == false)
                        {
                            lastBelowTime = currentTime;
                            isBelowFlag = true;
                        }
                    }
                    else
                    {
                        if (isBelowFlag)
                        {
                            belowSec += (currentTime - lastBelowTime).TotalSeconds;
                            isBelowFlag = false;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
            if (isBelowFlag)
            {
                belowSec += second - (lastBelowTime - startTime).TotalSeconds;
            }

            return belowSec;
        }

        /// <summary>
        /// 出水流量是否降低50%
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool QmBelowHalf(DataTable data)
        {
            if (data == null)
            {
                return false;
            }
            //data.Rows.RemoveAt(data.Rows.Count-1);//移除末尾
            var startQm = data.Rows[1]["出水流量"].AsDouble();
            var endQm = data.Rows[data.Rows.Count-2]["出水流量"].AsDouble();
            if (endQm * 2 <= startQm)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
