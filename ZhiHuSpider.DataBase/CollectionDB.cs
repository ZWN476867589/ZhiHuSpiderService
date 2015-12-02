using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MainContext;

namespace ZhiHuSpider.DataBase
{
    public static class CollectionDB
    {
        public static bool SaveOrUpdateCollectionInfo(CollectionInfo ci)
        {
            bool result = false;
            try
            {
                using (MainDataContext db = new MainDataContext())
                {
                    CollectionInfo info = db.CollectionInfos.FirstOrDefault(p => p.CollectionID == ci.CollectionID);
                    if (info == null)
                    {
                        db.CollectionInfos.InsertOnSubmit(ci);
                    }
                    else
                    {
                        info.QuestionCount = ci.QuestionCount;
                        info.CollectionName = ci.CollectionName;
                        info.CollectionUrl = ci.CollectionUrl;
                        info.CreatorHash = ci.CreatorHash;
                        info.CreatorName = ci.CreatorName;
                        info.ModefiedTime = DateTime.Now.ToString();
                    }
                    db.SubmitChanges();
                    result = true;
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public static IEnumerable<CollectionInfo> GetAllCollectionInfos()
        {
            using (MainDataContext db = new MainDataContext())
            {
                var res = db.CollectionInfos.Select(p => p).ToList();
                return res;
            }
        }
        public static bool SaveOrUpdateCollectionAnswer(CollectionQuestionAndAnswer cqa)
        {
            bool result = false;
            try
            {
                using (MainDataContext db = new MainDataContext())
                {
                    CollectionQuestionAndAnswer info = db.CollectionQuestionAndAnswers.FirstOrDefault(p => p.AnswerID == cqa.AnswerID);
                    if (info == null)
                    {
                        db.CollectionQuestionAndAnswers.InsertOnSubmit(cqa);
                    }
                    else
                    {
                        info = cqa;
                        info.ModefiedTime = DateTime.Now.ToString();
                    }
                    db.SubmitChanges();
                    result = true;
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public static int GetAnswerCountByCollectionId(string CollectionId)
        {
            using (MainDataContext db = new MainDataContext())
            {
                var res= db.CollectionQuestionAndAnswers.Count(p => p.CollectionID.ToString() == CollectionId);
                db.Dispose();
                return res;
            }
        }
        public static CollectionInfo GetCollectionInfoById(string CollectionId)
        {
            using (MainDataContext db = new MainDataContext())
            {
                var res =  db.CollectionInfos.FirstOrDefault(p => p.CollectionID.ToString() == CollectionId);
                db.Dispose();
                return res;
            }
        }
    }
}
