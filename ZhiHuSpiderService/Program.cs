using System;
using System.Collections.Generic;
using System.Linq;
using ZhiHuSpider.Business;
using System.Text;
using System.ServiceProcess;

namespace ZhiHuSpider.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("请输入命令开启功能");
            Console.WriteLine("1.更新问题总页数:P。");
            Console.WriteLine("2.开始获取问题:Q");
            Console.WriteLine("3.退出:EXIT");
            string consoleCode = Console.ReadLine().ToLower().Trim();
            while (consoleCode != "exit")
            {
                if (consoleCode == "p")
                {
                    QuestionBusiness.RefreshQuestionPageCount();
                }
                if (consoleCode == "q")
                {
                    MainThread mainThread = new MainThread();
                    mainThread.GetQuestionInfo(5);
                }
                if (consoleCode == "a")
                {
                    MainThread mainThread = new MainThread();
                    mainThread.GetCollectionDetail(2);
                }
                else
                {
                    CollectionBusiness.LoadCollectionIDsFormFile();
                }
                consoleCode = Console.ReadLine();
            }
        }
    }
}
