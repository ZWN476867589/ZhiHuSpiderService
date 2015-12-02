using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using ZhiHuSpider.DataBase;
using MainContext;
using System.Threading;

namespace ZhiHuSpider.Business
{
    public static class QuestionBusiness
    {
        static string questionPageCountUrl = "http://www.zhihu.com/topic/19776749/questions";
        static string questionPageUrl = "http://www.zhihu.com/topic/19776749/questions/?page={0}";
        static int DownLoadCount = 0;
        static int pagecount = 0;
        public static void RefreshQuestionPageCount()
        {
            Console.WriteLine("更新问题总页数于" + DateTime.Now.ToString() + "开始");
            string result = BusinessUtils.GetByUrl(questionPageCountUrl);
            if (!String.IsNullOrWhiteSpace(result))
            {
                try
                {
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(result);
                    List<HtmlNode> rootNode = doc.DocumentNode.SelectNodes(@"//div[@class='zm-invite-pager']").ToList();
                    int count = 0;
                    List<HtmlNode> nodes = rootNode[0].ChildNodes.ToList();
                    nodes.RemoveAll(p => p.InnerText.Replace("\n", "") == "");
                    if (nodes.Count == 7)
                    {
                        HtmlNode countNode = nodes[5];
                        count = int.Parse(countNode.FirstChild.InnerText);
                    }
                    else
                    {
                        foreach (var node in nodes)
                        {
                            if (node.InnerHtml.Contains("下一页"))
                            {
                                HtmlNode countNode = nodes[nodes.IndexOf(node) - 1];
                                count = int.Parse(countNode.FirstChild.InnerText);
                            }
                        }
                    }
                    SysDictDB.SaveOrUpdateQuestionCount(count);
                    Console.WriteLine("更新问题总页数于" + DateTime.Now.ToString() + "成功，目前问题总页数为" + count);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("更新问题总页数于" + DateTime.Now.ToString() + "失败，失败原因:" + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("更新问题总页数于" + DateTime.Now.ToString() + "失败，失败原因:没有获取到数据。");
            }
        }
        public static int GetQuestionPageCount()
        {
            pagecount = SysDictDB.GetQuestionPageCount();
            return pagecount;
        }
        public static bool GetQuestionList()
        {
            bool result = false;
            DownLoadCount = SysDictDB.GetDownLoadPageCount();
            pagecount = GetQuestionPageCount();
            try
            {
                for (int pageNo = pagecount; pageNo >= 1; pageNo--)
                {
                    GetQuestionListByPage(pageNo);
                }
                result = true;
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public static bool GetQuestionListByPage(int pageNo)
        {
            bool result = false;
            string url = String.Format(questionPageUrl, pageNo);
            GetQuestionInfoFormHtml(BusinessUtils.GetByUrl(url));
            return result;
        }
        public static void GetQuestionInfoFormHtml(string HtmlStr)
        {
            if (!String.IsNullOrEmpty(HtmlStr))
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(HtmlStr);
                HtmlNodeCollection questionItems = doc.DocumentNode.SelectNodes(@"//div[@class='feed-item feed-item-hook question-item']");
                if (questionItems != null && questionItems.Count > 0)
                {
                    foreach (HtmlNode node in questionItems)
                    {
                        try
                        {
                            HtmlNode subTopic = node.SelectNodes(@"div[@class='subtopic']//a").ToList()[0];
                            HtmlNode titleNode = node.SelectNodes(@"h2[@class='question-item-title']").ToList()[0];
                            string subtopicId = "";
                            string timeStamp = "";
                            string title = "";
                            string questionId = "";
                            subtopicId = subTopic.Attributes.FirstOrDefault(p => p.Name == "href").Value;
                            string[] topic = subtopicId.Split('/');
                            subtopicId = topic[2];
                            timeStamp = titleNode.SelectNodes(@"span").ToList()[0].Attributes.FirstOrDefault(p => p.Name == "data-timestamp").Value;
                            questionId = titleNode.SelectNodes(@"a").ToList()[0].Attributes.FirstOrDefault(p => p.Name == "href").Value;
                            title = titleNode.SelectNodes(@"a").ToList()[0].InnerText;
                            string[] question = questionId.Split('/');
                            foreach (var j in question)
                            {
                                questionId = question[2];
                            }
                            QuestionInfo qi = new QuestionInfo();
                            qi.QuestionID = int.Parse(questionId);
                            qi.QuestionTitle = title;
                            qi.QuestionTimeStamp = long.Parse(timeStamp);
                            qi.BelongsTopic = subtopicId;
                            qi.ModefiedTime = DateTime.Now.ToString();
                            qi.QuestionUrl = @"http://www.zhihu.com/question/" + qi.QuestionID;
                            if (QuestionInfoDB.SaveOrUpdateQuestionInfo(qi))
                            {
                                Console.WriteLine("问题：" + qi.QuestionID + " " + qi.QuestionTitle + " 于 " + qi.ModefiedTime + " 保存完成");
                            }
                            else
                            {
                                Console.WriteLine("问题：" + qi.QuestionID + " " + qi.QuestionTitle + " 于 " + qi.ModefiedTime + " 保存失败");
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    DownLoadCount += 1;
                    if (DownLoadCount % 10 == 0)
                    {
                        SysDictDB.SaveOrUpdateDownLoadPageCount(DownLoadCount);
                    }
                    Thread.Sleep(1000 * 5);
                }
            }
        }
        public static int PageCount
        {
            get
            {
                lock (questionPageCountUrl)
                {
                    if (pagecount == 0)
                    {
                        GetQuestionPageCount();
                    }
                    return --pagecount;
                }
            }
        }
    }
}
