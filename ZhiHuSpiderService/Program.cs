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
            Console.WriteLine("1.更新问题总页数:P");
            Console.WriteLine("2.开始获取问题:Q");
            Console.WriteLine("3.开始获取收藏夹答案列表:A");
            Console.WriteLine("4.退出:EXIT");
            string consoleCode = Console.ReadLine().ToLower().Trim();
            while (consoleCode != "exit")
            {
                if (consoleCode == "p")
                {
                    QuestionBusiness.RefreshQuestionPageCount();
                }
                if (consoleCode == "q")
                {
                    Console.WriteLine("输入线程数量");
                    string threadCount = Console.ReadLine().ToLower().Trim();
                    int threadCountDefault = 5;
                    int.TryParse(threadCount, out threadCountDefault);
                    MainThread mainThread = new MainThread();
                    mainThread.GetQuestionInfo(threadCountDefault);
                }
                if (consoleCode == "a")
                {
                    Console.WriteLine("输入线程数量");
                    string threadCount = Console.ReadLine().ToLower().Trim();
                    int threadCountDefault = 5;
                    int.TryParse(threadCount, out threadCountDefault);
                    MainThread mainThread = new MainThread();
                    mainThread.GetCollectionDetail(threadCountDefault);
                }
                if (consoleCode == "m")
                {
                    MongoBusiness.CollectionBusiness.ConvertCollectionInfoToMongoDB();
                }
                else
                {
                    Console.WriteLine("未知命令...\r\n请重新输入...");
                    //CollectionBusiness.LoadCollectionIDsFormFile();
                }
                consoleCode = Console.ReadLine();
            }
        }
    }
}
