using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HtmlAgilityPack;
using ZhiHuSpider.DataBase;
using MainContext;
using System.Threading;

namespace ZhiHuSpider.Business
{
    public static class CollectionBusiness
    {
        static string CollectionInfoBaseUrl = @"http://www.zhihu.com/collection/{0}";
        static string CollectionInfoAnswerUrl = @"http://www.zhihu.com/collection/{0}?page={1}";
        static List<CollectionInfo> infos;
        static int CIIndex = 0;
        public static void LoadCollectionIDsFormFile()
        {
            string filePath = System.Environment.CurrentDirectory + @"\CollectionID.txt";
            if (File.Exists(filePath))
            {
                string fileStr = File.ReadAllText(filePath);
                IEnumerable<CollectionInfo> infos = CollectionDB.GetAllCollectionInfos().ToList();
                foreach (var ci in infos)
                {
                    fileStr = fileStr.Replace(ci.CollectionID.ToString()+":"+ci.QuestionCount, "");
                }
                string[] collectionIds = fileStr.Replace(";", "|").Split('|');
                foreach (string id in collectionIds)
                {
                    if (!String.IsNullOrEmpty(id))
                    {
                        string[] ids = id.Split(':');
                        CollectionInfo ci = CollectionDB.GetCollectionInfoById(ids[0]);
                        if (ci != null)
                        {
                            ci.QuestionCount = int.Parse(ids[1]);
                            CollectionDB.SaveOrUpdateCollectionInfo(ci);
                        }
                        else
                        {
                            string url = string.Format(CollectionInfoBaseUrl, ids[0]);
                            GetCollectionInfoFormHtml(BusinessUtils.GetByUrl(url), ids[0]);
                        }
                    }
                }
            }
        }
        public static void GetCollectionInfoFormHtml(string html, string collectionId)
        {
            if (!String.IsNullOrWhiteSpace(html))
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                HtmlNode nameNode = doc.DocumentNode.SelectSingleNode(@"//h2[@id='zh-fav-head-title']");
                HtmlNodeCollection fllowerNodes = doc.DocumentNode.SelectNodes(@"//div[@class='zg-gray-normal']");
                HtmlNode creatorNode = doc.DocumentNode.SelectSingleNode(@"//h2[@class='zm-list-content-title']");
                string fllowercount = ((HtmlNode)fllowerNodes[2]).SelectSingleNode(@"a").InnerText.Replace("\n", "");
                CollectionInfo ci = new CollectionInfo()
                {
                    CollectionID = int.Parse(collectionId),
                    CollectionName = nameNode.InnerText.Replace("\n", ""),
                    CollectionUrl = string.Format(CollectionInfoBaseUrl, collectionId),
                    FllowersCount = int.Parse(fllowercount),
                    QuestionCount = 0,
                    CreatorName = creatorNode.InnerText.Replace("\n", ""),
                    ModefiedTime = DateTime.Now.ToString(),
                };
                CollectionDB.SaveOrUpdateCollectionInfo(ci);
                Console.WriteLine("收藏夹:" + ci.CollectionID + " " + ci.CollectionName + " 于" + ci.ModefiedTime + "保存完成");
                Thread.Sleep(1000 * 5);
            }
        }
        public static CollectionInfo CollInfo
        {
            get
            {
                lock (CollectionInfoBaseUrl)
                {
                    if (infos == null)
                    {
                        infos = CollectionDB.GetAllCollectionInfos().ToList();
                    }
                    return infos[CIIndex++];
                }
            }
        }
        public static void GetCollectionAnswerInfos(CollectionInfo ci)
        {
            if (CollectionDB.GetAnswerCountByCollectionId(ci.CollectionID.ToString()) != ci.QuestionCount)
            {
                int count = GetCollectionPageCount(ci);
                ci.QuestionCount = count;
                CollectionDB.SaveOrUpdateCollectionInfo(ci);
                for (int i = count; i >= 1; i--)
                {
                    string url = string.Format(CollectionInfoAnswerUrl, ci.CollectionID, i);
                    GetCollectionAnswerInfoFromHtml(BusinessUtils.GetByUrl(url), ci);
                    Thread.Sleep(1000 * 5);
                }
            }
        }
        public static void GetCollectionAnswerInfoFromHtml(string html, CollectionInfo ci)
        {
            if (!String.IsNullOrWhiteSpace(html))
            {
                try
                {
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(html);
                    HtmlNodeCollection answerNodes = doc.DocumentNode.SelectNodes(@"//div[@class='zm-item-rich-text js-collapse-body']");
                    foreach (HtmlNode answer in answerNodes)
                    {
                        string ids = answer.Attributes.FirstOrDefault(p => p.Name == "data-entry-url").Value;

                        string[] idsArr = ids.Split('/');
                        CollectionQuestionAndAnswer cqa = new CollectionQuestionAndAnswer()
                        {
                            CollectionID = ci.CollectionID,
                            QuestionID = int.Parse(idsArr[2]),
                            AnswerID = int.Parse(idsArr[4]),
                            ModefiedTime = DateTime.Now.ToString(),
                        };
                        CollectionDB.SaveOrUpdateCollectionAnswer(cqa);
                        Console.WriteLine("答案:" + cqa.AnswerID + " 于" + DateTime.Now.ToString() + "保存成功");
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
        public static int GetCollectionPageCount(CollectionInfo ci)
        {
            int pageCount = 0;
            string url = string.Format(CollectionInfoBaseUrl, ci.CollectionID);
            string res = BusinessUtils.GetByUrl(url);
            if (!String.IsNullOrWhiteSpace(res))
            {
                try
                {
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(res);
                    List<HtmlNode> rootNode = doc.DocumentNode.SelectNodes(@"//div[@class='zm-invite-pager']").ToList();
                    List<HtmlNode> nodes = rootNode[0].ChildNodes.ToList();
                    nodes.RemoveAll(p => p.InnerText.Replace("\n", "") == "");
                    if (nodes.Count == 7)
                    {
                        HtmlNode countNode = nodes[5];
                        pageCount = int.Parse(countNode.FirstChild.InnerText);
                    }
                    else
                    {
                        foreach (var node in nodes)
                        {
                            if (node.InnerHtml.Contains("下一页"))
                            {
                                HtmlNode countNode = nodes[nodes.IndexOf(node) - 1];
                                pageCount = int.Parse(countNode.FirstChild.InnerText);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return pageCount;
        }
    }
}
