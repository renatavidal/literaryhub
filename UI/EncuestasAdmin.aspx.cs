using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using BLL;
using BE;

public partial class EncuestasAdmin : Perm_AdminEncuestasPage
{
  

    public class ListResult { public object[] items { get; set; } }
    public class StatsResult { public object[] items { get; set; } }

    [WebMethod(EnableSession = true), ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static ListResult List()
    {
       
        var bll = new BLLEncuesta();
        var dt = bll.List(null);
        var items = dt.AsEnumerable().Select(r => new {
            id = r.Field<int>("Id"),
            title = r.Field<string>("Title"),
            isActive = r.Field<bool>("IsActive"),
            createdUtc = r.Field<DateTime>("CreatedUtc")
        }).ToArray();
        return new ListResult { items = items };
    }

    [WebMethod(EnableSession = true), ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static StatsResult Stats(int surveyId)
    {
        var stats = new BLLEncuesta().Stats(surveyId)
            .Select(s => new {
                questionId = s.QuestionId,
                qIndex = s.QIndex,
                qType = (int)s.QType,
                text = s.Text,
                total = s.TotalAnswers,
                yes = s.YesCount,
                no = s.NoCount,
                avg = s.AvgRating,
                c1 = s.C1,
                c2 = s.C2,
                c3 = s.C3,
                c4 = s.C4,
                c5 = s.C5
            }).ToArray();
        return new StatsResult { items = stats };
    }

    [WebMethod(EnableSession = true), ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static void Delete(int surveyId)
    {
        new BLLEncuesta().Delete(surveyId);
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static void Create(string title, bool isActive,
     string q1Text, int? q1Type, string q2Text, int? q2Type, string q3Text, int? q3Type,
     string q4Text, int? q4Type, string q5Text, int? q5Type, DateTime q7Text)
    {

        var s = new BESurvey { Title = title, IsActive = isActive, CreationDate = q7Text };

        AddQuestion(s, 1, q1Text, q1Type);
        AddQuestion(s, 2, q2Text, q2Type);
        AddQuestion(s, 3, q3Text, q3Type);
        AddQuestion(s, 4, q4Text, q4Type);
        AddQuestion(s, 5, q5Text, q5Type);

        int uid = 0;
        var auth = HttpContext.Current.Session["auth"] as UserSession;
        if (auth != null) uid = auth.UserId;

        new BLLEncuesta().Create(s, uid);
    }

    // helper compatible con C# 5/6
    private static void AddQuestion(BESurvey s, int qIndex, string text, int? type)
    {
        if (s == null) return;
        if (!string.IsNullOrWhiteSpace(text) && type.HasValue)
        {
            s.Questions.Add(new BESurveyQuestion
            {
                QIndex = qIndex,
                QType = (SurveyQType)type.Value, // 1=YesNo, 2=Rating
                Text = text
            });
        }
    }

}
