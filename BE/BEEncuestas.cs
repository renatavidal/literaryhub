using System;
using System.Collections.Generic;

namespace BE
{
    public enum SurveyQType { YesNo = 1, Rating = 2 }

    public class BESurvey
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }
        public List<BESurveyQuestion> Questions { get; set; } = new List<BESurveyQuestion>();
        public DateTime CreationDate { get; set; }
    }

    public class BESurveyQuestion
    {
        public int Id { get; set; }
        public int QIndex { get; set; }
        public SurveyQType QType { get; set; }
        public string Text { get; set; }
    }

    public class BESurveyQuestionStats
    {
        public int QuestionId { get; set; }
        public int QIndex { get; set; }
        public SurveyQType QType { get; set; }
        public string Text { get; set; }
        public int TotalAnswers { get; set; }
        public int YesCount { get; set; }
        public int NoCount { get; set; }
        public decimal? AvgRating { get; set; }
        public int C1 { get; set; }
        public int C2 { get; set; }
        public int C3 { get; set; }
        public int C4 { get; set; }
        public int C5 { get; set; }
    }
}
