using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hkzx.db
{
    public class DataTable
    {
        public string tbody { get; set; }//
        public int Id { get; set; }//
        public string CommName { get; set; }//专委会名称
        public int UserNum { get; set; }//委员数
        public int FinalNum { get; set; }//计划及总结
        public int AllNum { get; set; }//全体会议
        public int DirectorNum { get; set; }//主任会议
        public int ViewNum { get; set; }//视察
        public int StudyNum { get; set; }//学习考察
        public int SurveyNum { get; set; }//一般调研
        public int SurveyAdvise { get; set; }//调研(建议案)
        public int TogetherNum { get; set; }//同心桥
        public int SpeakNum { get; set; }//大会发言(上台)
        public int WriteNum { get; set; }//大会发言(书面)
        public int OpinNum { get; set; }//提案数
        public string OpinScale { get; set; }//提案参与比例
        public int PopNum { get; set; }//社情民意数
        public string PopScale { get; set; }//社情民意参与比例
    }
}