using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 恒温测试机.App
{
    /// <summary>
        /// 快速傅立叶变换(Fast Fourier Transform)。 
        /// </summary>
    public class TWFFT
    {
        #region//调用该类的例子
        //res是原始的数据        
        //float[] ans = new float[res.ToArray().Length];
        //float[] R = new float[res.ToArray().Length];
        //    for (i = 0; i<res.ToArray().Length; i++)
        //        R[i] = (float) res[i];
        //ans = TWFFT.FFT_filter(R);
        //    for (i = 0; i<TWFFT.length; i++)            
        //        hslCurve1.AddCurveData("滤波数据", (float) ans[i]);

        #endregion
        #region//调用快速傅里叶的代码
        //length是傅里叶变换处理过后的数组大小
        public static int length = 0;
        //数组长度需要为2的n次方
        public static float[] FFT_filter(double[] res)
        {
            //a是实部、b是虚部

            float[] a = new float[res.Length];
            float[] b = new float[res.Length];
            List<float> ans = new List<float>();
            for (int j = 0; j < res.Length; j++)
            {
                //  Console.WriteLine("I:" + j);
                a[j] = (float)res[j];
                b[j] = 0.0f;
            }
            length = TWFFT.FFT(a, b);
            //length是傅里叶变换处理过后的数组大小
            for (int i = 100; i < length; i++)
            {
                a[i] = 0f;
                b[i] = 0f;
            }
            length = TWFFT.IFFT(a, b);
            for (int i = 0; i < length; i++)
            {
                //Console.WriteLine("{0}\t{1}\t{2}", i, a[i], b[i]);
                ans.Add((float)Math.Sqrt(a[i] * a[i] + b[i] * b[i]));
            }

            return ans.ToArray();
        }
        public static float[] FFT_filter(float[] res)
        {
            //a是实部、b是虚部

            float[] a = new float[res.Length];
            float[] b = new float[res.Length];
            List<float> ans = new List<float>();
            for (int j = 0; j < res.Length; j++)
            {
                //  Console.WriteLine("I:" + j);
                a[j] = (float)res[j];
                b[j] = 0.0f;
            }
            length = TWFFT.FFT(a, b);
            //length是傅里叶变换处理过后的数组大小
            for (int i = 100; i < length; i++)
            {
                if (i < length - 100 && i > 10)
                {
                    a[i] = 0f;
                    b[i] = 0f;
                }
            }
            length = TWFFT.IFFT(a, b);
            for (int i = 0; i < length; i++)
            {
                //Console.WriteLine("{0}\t{1}\t{2}", i, a[i], b[i]);
                ans.Add((float)Math.Sqrt(a[i] * a[i] + b[i] * b[i]));
            }

            return ans.ToArray();
        }
        #endregion

        #region//快速傅里叶变化的相关代码
        private static void bitrp(float[] xreal, float[] ximag, int n)
        {
            // 位反转置换 Bit-reversal Permutation
            int i, j, a, b, p;
            for (i = 1, p = 0; i < n; i *= 2)
            {
                p++;
            }
            for (i = 0; i < n; i++)
            {
                a = i;
                b = 0;
                for (j = 0; j < p; j++)
                {
                    b = b * 2 + a % 2;
                    a = a / 2;
                }
                if (b > i)
                {
                    float t = xreal[i];
                    xreal[i] = xreal[b];
                    xreal[b] = t;
                    t = ximag[i];
                    ximag[i] = ximag[b];
                    ximag[b] = t;
                }
            }
        }

        public static int FFT(float[] xreal, float[] ximag)
        {
            //n值为2的N次方
            int n = 2;
            while (n <= xreal.Length)
            {
                n *= 2;
            }
            n /= 2;
            // 快速傅立叶变换，将复数 x 变换后仍保存在 x 中，xreal, ximag 分别是 x 的实部和虚部
            float[] wreal = new float[n / 2];
            float[] wimag = new float[n / 2];
            float treal, timag, ureal, uimag, arg;
            int m, k, j, t, index1, index2;
            bitrp(xreal, ximag, n);
            // 计算 1 的前 n / 2 个 n 次方根的共轭复数 W'j = wreal [j] + i * wimag [j] , j = 0, 1, ... , n / 2 - 1
            arg = (float)(-2 * Math.PI / n);
            treal = (float)Math.Cos(arg);
            timag = (float)Math.Sin(arg);
            wreal[0] = 1.0f;
            wimag[0] = 0.0f;
            for (j = 1; j < n / 2; j++)
            {
                wreal[j] = wreal[j - 1] * treal - wimag[j - 1] * timag;
                wimag[j] = wreal[j - 1] * timag + wimag[j - 1] * treal;
            }
            for (m = 2; m <= n; m *= 2)
            {
                for (k = 0; k < n; k += m)
                {
                    for (j = 0; j < m / 2; j++)
                    {
                        index1 = k + j;
                        index2 = index1 + m / 2;
                        t = n * j / m;    // 旋转因子 w 的实部在 wreal [] 中的下标为 t
                        treal = wreal[t] * xreal[index2] - wimag[t] * ximag[index2];
                        timag = wreal[t] * ximag[index2] + wimag[t] * xreal[index2];
                        ureal = xreal[index1];
                        uimag = ximag[index1];
                        xreal[index1] = ureal + treal;
                        ximag[index1] = uimag + timag;
                        xreal[index2] = ureal - treal;
                        ximag[index2] = uimag - timag;
                    }
                }
            }
            return n;
        }
        public static int IFFT(float[] xreal, float[] ximag)
        {
            //n值为2的N次方
            int n = 2;
            while (n <= xreal.Length)
            {
                n *= 2;
            }
            n /= 2;
            // 快速傅立叶逆变换
            float[] wreal = new float[n / 2];
            float[] wimag = new float[n / 2];
            float treal, timag, ureal, uimag, arg;
            int m, k, j, t, index1, index2;
            bitrp(xreal, ximag, n);
            // 计算 1 的前 n / 2 个 n 次方根 Wj = wreal [j] + i * wimag [j] , j = 0, 1, ... , n / 2 - 1
            arg = (float)(2 * Math.PI / n);
            treal = (float)(Math.Cos(arg));
            timag = (float)(Math.Sin(arg));
            wreal[0] = 1.0f;
            wimag[0] = 0.0f;
            for (j = 1; j < n / 2; j++)
            {
                wreal[j] = wreal[j - 1] * treal - wimag[j - 1] * timag;
                wimag[j] = wreal[j - 1] * timag + wimag[j - 1] * treal;
            }
            for (m = 2; m <= n; m *= 2)
            {
                for (k = 0; k < n; k += m)
                {
                    for (j = 0; j < m / 2; j++)
                    {
                        index1 = k + j;
                        index2 = index1 + m / 2;
                        t = n * j / m;    // 旋转因子 w 的实部在 wreal [] 中的下标为 t
                        treal = wreal[t] * xreal[index2] - wimag[t] * ximag[index2];
                        timag = wreal[t] * ximag[index2] + wimag[t] * xreal[index2];
                        ureal = xreal[index1];
                        uimag = ximag[index1];
                        xreal[index1] = ureal + treal;
                        ximag[index1] = uimag + timag;
                        xreal[index2] = ureal - treal;
                        ximag[index2] = uimag - timag;
                    }
                }
            }
            for (j = 0; j < n; j++)
            {
                xreal[j] /= n;
                ximag[j] /= n;
            }
            return n;
        }
        #endregion
        private TWFFT()
        {

        }
    }
}
