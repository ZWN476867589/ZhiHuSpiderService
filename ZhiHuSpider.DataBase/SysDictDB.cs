using System;
using System.Linq;
using MainContext;

namespace ZhiHuSpider.DataBase
{
    public static class SysDictDB
    {
        public static bool SaveOrUpdateQuestionCount(int QuestionCount)
        {
            using (MainDataContext db = new MainDataContext())
            {
                SysDict dict = db.SysDicts.FirstOrDefault(p => p.DictName == "QuestionPageCount");
                if (dict == null)
                {
                    dict = new SysDict()
                    {
                        DictName = "QuestionPageCount",
                        DictValue = QuestionCount.ToString(),
                        DictCode = "QuestionPageCount",
                        Remark = "QuestionPageCount",
                        ModefyDate = DateTime.Now.ToString()
                    };
                    db.SysDicts.InsertOnSubmit(dict);
                    db.SubmitChanges();
                }
                else
                {
                    dict.DictValue = QuestionCount.ToString();
                    dict.ModefyDate = DateTime.Now.ToString();
                    db.SubmitChanges();
                }
            }
            return true;
        }
        public static int GetQuestionPageCount()
        {
            using (MainDataContext db = new MainDataContext())
            {
                SysDict dict = db.SysDicts.FirstOrDefault(p => p.DictName == "QuestionPageCount");
                return dict == null ? 0 : int.Parse(dict.DictValue) - db.QuestionInfos.Count()/20-2;
            }
        }
        public static void SaveOrUpdateDownLoadPageCount(int count)
        {
            using (MainDataContext db = new MainDataContext())
            {
                SysDict dict = db.SysDicts.FirstOrDefault(p => p.DictName == "DownLoadPageCount");
                if (dict == null)
                {
                    dict = new SysDict()
                    {
                        DictValue = count.ToString(),
                        DictName = "DownLoadPageCount",
                        DictCode = "DownLoadPageCount",
                        ModefyDate = DateTime.Now.ToString(),
                    };
                    db.SysDicts.InsertOnSubmit(dict);
                }
                else
                {
                    dict.DictValue = count.ToString();
                    dict.ModefyDate = DateTime.Now.ToString();
                }
                db.SubmitChanges();
            }
        }
        public static int GetDownLoadPageCount()
        {
            int count = 0;
            using (MainDataContext db = new MainDataContext())
            {
                SysDict dict = db.SysDicts.FirstOrDefault(p => p.DictName == "DownLoadPageCount");
                count = dict == null ? 0 : int.Parse(dict.DictValue);
            }
            return count;
        }
    }
}
