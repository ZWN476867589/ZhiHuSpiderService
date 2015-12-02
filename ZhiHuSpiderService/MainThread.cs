using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZhiHuSpider.Business;
using System.Threading;

namespace ZhiHuSpider
{
    public class MainThread
    {
        public string threadName = "";
        bool threadOn = false;
        int allCount = 0;
        public void GetQuestion()
        {
            while (threadOn)
            {
                string name = System.Threading.Thread.CurrentThread.Name;
                int pageCount = QuestionBusiness.PageCount;
                Console.WriteLine("线程:" + name + " 开始获取第" + pageCount + "页");
                QuestionBusiness.GetQuestionListByPage(pageCount);
                allCount++;
                Console.WriteLine("线程:" + name + " 获取第" + pageCount + "页完成");
                Console.WriteLine("线程共获取" + allCount + "页 " + DateTime.Now.ToString());
            }
        }
        public void GetQuestionInfo(int threadCount)
        {
            if (threadCount >= 1)
            {
                for (int i = 1; i <= threadCount; i++)
                {
                    Thread th = new Thread(new ThreadStart(GetQuestion));
                    th.Name = "thread" + i;
                    this.threadOn = true;
                    th.Start();
                }
            }
            else
            {

            }
        }
        public void GetCollectionDetail(int threadCount)
        {
            if (threadCount >= 1)
            {
                for (int i = 1; i <= threadCount; i++)
                {
                    Thread th = new Thread(new ThreadStart(GetCollection));
                    th.Name = "thread" + i;
                    this.threadOn = true;
                    th.Start();
                }
            }
            else
            {

            }
        }
        public void GetCollection()
        {
            while (threadOn)
            {
                string name = System.Threading.Thread.CurrentThread.Name;
                var m = CollectionBusiness.CollInfo;
                Console.WriteLine("线程:" + name + " 开始获取" + m.CollectionID+" "+m.CollectionName + "的答案列表");
                CollectionBusiness.GetCollectionAnswerInfos(m);
                allCount++;
                Console.WriteLine("线程:" + name + " 获取" + m.CollectionID +" "+m.CollectionName+ "的答案列表完成");
                Console.WriteLine("线程共获取" + allCount + "页答案 " + DateTime.Now.ToString());
            }
        }
    }
}
