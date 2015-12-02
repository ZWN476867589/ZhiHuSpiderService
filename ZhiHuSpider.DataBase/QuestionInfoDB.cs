using System;
using System.Collections.Generic;
using System.Linq;
using MainContext;

namespace ZhiHuSpider.DataBase
{
    public static class QuestionInfoDB
    {
        public static bool SaveOrUpdateQuestionInfo(QuestionInfo qi)
        {
            bool result = false;
            try
            {
                using (MainDataContext db = new MainDataContext())
                {
                    QuestionInfo Info = db.QuestionInfos.FirstOrDefault(p => p.QuestionID == qi.QuestionID);
                    if (Info == null)
                    {
                        db.QuestionInfos.InsertOnSubmit(qi);
                    }
                    else
                    {
                        Info = qi;
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
        public static bool SaveOrUpdateQuestionInfoList(IEnumerable<QuestionInfo> InfoList)
        {
            bool result = false;
            try
            {
                if (InfoList != null)
                {
                    using (MainDataContext db = new MainDataContext())
                    {
                        db.QuestionInfos.InsertAllOnSubmit(InfoList);
                        db.SubmitChanges();
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public static bool DeleteQuestionInfo(QuestionInfo qi)
        {
            bool result = false;
            try
            {
                if (qi != null)
                {
                    using (MainDataContext db = new MainDataContext())
                    {
                        QuestionInfo Info = db.QuestionInfos.FirstOrDefault(p => p.QuestionID == qi.QuestionID);
                        if (Info != null)
                        {
                            db.QuestionInfos.DeleteOnSubmit(Info);
                        }
                        db.SubmitChanges();
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public static bool DeleteQuestionInfoById(int questionId)
        {
            bool result = false;
            try
            {
                using (MainDataContext db = new MainDataContext())
                {
                    QuestionInfo Info = db.QuestionInfos.FirstOrDefault(p => p.QuestionID == questionId);
                    if (Info != null)
                    {
                        db.QuestionInfos.DeleteOnSubmit(Info);
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
        public static bool DeleteQuestionInfoList(IEnumerable<QuestionInfo> InfoList)
        {
            bool result = false;
            try
            {
                if (InfoList != null)
                {
                    using (MainDataContext db = new MainDataContext())
                    {
                        db.QuestionInfos.DeleteAllOnSubmit(InfoList);
                        db.SubmitChanges();
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public static QuestionInfo FindQuestionInfoById(int questionId)
        {
            QuestionInfo qi = null;
            try
            {
                using (MainDataContext db = new MainDataContext())
                {
                    qi = db.QuestionInfos.FirstOrDefault(p => p.QuestionID == questionId);
                }
            }
            catch (Exception ex)
            {
            }
            return qi;
        }
    }
}
