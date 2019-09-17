using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using 恒温测试机.UI;

namespace 恒温测试机
{
    static class Program
    {

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 注册控件示例，如果注册失败，你的控件仍然只能使用8个小时
            bool isSuccess = HslControls.Authorization.SetAuthorizationCode("d906733d-b7d9-477d-8773-c0d1a86ef7da");
            if (!isSuccess)
            {
                Console.WriteLine("注册失败");
            }
            else
            {
                Console.WriteLine("注册成功");
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }
    }
}
