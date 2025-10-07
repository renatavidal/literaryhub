using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BE;
using DAL;

namespace MPP
{
    public class MPPEncuesta
    {
        private readonly Acceso _datos = new Acceso();

        public int Create(BESurvey s, int createdBy)
        {
            var h = new Hashtable {
                {"@Title", s.Title}, {"@IsActive", s.IsActive}, {"@CreatedBy", createdBy},
                {"@Q1Text", s.Questions.Count>0 ? (object)s.Questions[0].Text : DBNull.Value},
                {"@Q1Type", s.Questions.Count>0 ? (object)(int)s.Questions[0].QType : DBNull.Value},
                {"@Q2Text", s.Questions.Count>1 ? (object)s.Questions[1].Text : DBNull.Value},
                {"@Q2Type", s.Questions.Count>1 ? (object)(int)s.Questions[1].QType : DBNull.Value},
                {"@Q3Text", s.Questions.Count>2 ? (object)s.Questions[2].Text : DBNull.Value},
                {"@Q3Type", s.Questions.Count>2 ? (object)(int)s.Questions[2].QType : DBNull.Value},
                {"@Q4Text", s.Questions.Count>3 ? (object)s.Questions[3].Text : DBNull.Value},
                {"@Q4Type", s.Questions.Count>3 ? (object)(int)s.Questions[3].QType : DBNull.Value},
                {"@Q5Text", s.Questions.Count>4 ? (object)s.Questions[4].Text : DBNull.Value},
                {"@Q5Type", s.Questions.Count>4 ? (object)(int)s.Questions[4].QType : DBNull.Value},
                {"@q7Text", s.CreationDate }
            };
            var dt = _datos.Leer("usp_Survey_Create", h);
            return (dt.Rows.Count > 0) ? Convert.ToInt32(dt.Rows[0]["SurveyId"]) : 0;
        }

        public void Delete(int surveyId) =>
            _datos.Escribir("usp_Survey_Delete", new Hashtable { { "@SurveyId", surveyId } });

        public DataTable List(bool? onlyActive)
        {
            var h = new Hashtable();
            if (onlyActive.HasValue) h["@OnlyActive"] = onlyActive.Value;
            return _datos.Leer("usp_Survey_List", h);
        }

        public BESurvey Get(int surveyId)
        {
            var head = _datos.Leer("usp_Survey_Get", new Hashtable { { "@SurveyId", surveyId } });
            if (head.Rows.Count == 0) return null;
            var s = new BESurvey
            {
                Id = Convert.ToInt32(head.Rows[0]["Id"]),
                Title = Convert.ToString(head.Rows[0]["Title"]),
                IsActive = Convert.ToBoolean(head.Rows[0]["IsActive"])
            };
            var qdt = _datos.Leer("usp_Survey_GetQuestions", new Hashtable { { "@SurveyId", surveyId } });
            foreach (DataRow r in qdt.Rows)
                s.Questions.Add(new BESurveyQuestion
                {
                    Id = Convert.ToInt32(r["Id"]),
                    QIndex = Convert.ToInt32(r["QIndex"]),
                    QType = (SurveyQType)Convert.ToInt32(r["QType"]),
                    Text = Convert.ToString(r["Text"])
                });
            return s;
        }

        public BESurvey NextForUser(int userId)
        {
            var dt = _datos.Leer("usp_Survey_NextIdForUser", new Hashtable { { "@UserId", userId } });
            if (dt.Rows.Count == 0) return null;
            int id = Convert.ToInt32(dt.Rows[0][0]);
            return id > 0 ? Get(id) : null;
        }

        public void SubmitAnswers(int surveyId, int userId, byte?[] answers)
        {
            var h = new Hashtable {
                {"@SurveyId", surveyId}, {"@UserId", userId},
                {"@A1", answers.Length>0 ? (object)answers[0] ?? DBNull.Value : DBNull.Value},
                {"@A2", answers.Length>1 ? (object)answers[1] ?? DBNull.Value : DBNull.Value},
                {"@A3", answers.Length>2 ? (object)answers[2] ?? DBNull.Value : DBNull.Value},
                {"@A4", answers.Length>3 ? (object)answers[3] ?? DBNull.Value : DBNull.Value},
                {"@A5", answers.Length>4 ? (object)answers[4] ?? DBNull.Value : DBNull.Value}
            };
            _datos.Escribir("usp_Survey_SubmitAnswers", h);
        }

        public void OptOut(int surveyId, int userId) =>
            _datos.Escribir("usp_Survey_OptOut", new Hashtable { { "@SurveyId", surveyId }, { "@UserId", userId } });

        public List<BESurveyQuestionStats> GetStats(int surveyId)
        {
            var dt = _datos.Leer("usp_Survey_Stats", new Hashtable { { "@SurveyId", surveyId } });
            var list = new List<BESurveyQuestionStats>();
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new BESurveyQuestionStats
                {
                    QuestionId = Convert.ToInt32(r["QuestionId"]),
                    QIndex = Convert.ToInt32(r["QIndex"]),
                    QType = (SurveyQType)Convert.ToInt32(r["QType"]),
                    Text = Convert.ToString(r["Text"]),
                    TotalAnswers = Convert.ToInt32(r["TotalAnswers"]),
                    YesCount = Convert.ToInt32(r["YesCount"]),
                    NoCount = Convert.ToInt32(r["NoCount"]),
                    AvgRating = r.IsNull("AvgRating") ? (decimal?)null : Convert.ToDecimal(r["AvgRating"]),
                    C1 = Convert.ToInt32(r["C1"]),
                    C2 = Convert.ToInt32(r["C2"]),
                    C3 = Convert.ToInt32(r["C3"]),
                    C4 = Convert.ToInt32(r["C4"]),
                    C5 = Convert.ToInt32(r["C5"])
                });
            }
            return list;
        }
    }
}
