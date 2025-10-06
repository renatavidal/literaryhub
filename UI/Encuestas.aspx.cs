using System;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using BLL;
using BE;

public partial class Encuestas : ReaderPage
{
    private int GetCurrentUserId()
    {
        var auth = Session["auth"] as UserSession;
        if (auth != null && auth.UserId > 0) return auth.UserId;
        object o = Session["UserId"];
        int uid; return (o != null && int.TryParse(o.ToString(), out uid)) ? uid : 0;
    }

    private static int GetUserIdFromSession()
    {
        var ctx = HttpContext.Current;
        if (ctx.Session == null) return 0;
        var auth = ctx.Session["auth"] as UserSession;
        if (auth != null && auth.UserId > 0) return auth.UserId;
        object o = ctx.Session["UserId"];
        int uid; return (o != null && int.TryParse(o.ToString(), out uid)) ? uid : 0;
    }

    public class ApiOk { public bool ok { get; set; } public string error { get; set; } public object survey { get; set; } }

    [WebMethod(EnableSession = true), ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static ApiOk GetNext()
    {
        int userId = GetUserIdFromSession();
        if (userId <= 0) return new ApiOk { ok = false, error = "no-login" };

        var bll = new BLLEncuesta();
        var s = bll.NextForUser(userId);
        if (s == null) return new ApiOk { ok = true, survey = null };

        return new ApiOk
        {
            ok = true,
            survey = new
            {
                id = s.Id,
                title = s.Title,
                questions = s.Questions.ConvertAll(q => new { id = q.Id, qIndex = q.QIndex, qType = (int)q.QType, text = q.Text })
            }
        };
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static void Create(string title, bool isActive,
    string q1Text, int? q1Type, string q2Text, int? q2Type, string q3Text, int? q3Type,
    string q4Text, int? q4Type, string q5Text, int? q5Type)
    {

        var s = new BESurvey { Title = title, IsActive = isActive };

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

    // Helper compatible con C# 5/6
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

    [WebMethod(EnableSession = true), ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static ApiOk Submit(int surveyId, byte? a1, byte? a2, byte? a3, byte? a4, byte? a5)
    {
        int userId = GetUserIdFromSession();
        if (userId <= 0) return new ApiOk { ok = false, error = "no-login" };

        try
        {
            var bll = new BLLEncuesta();
            bll.Submit(surveyId, userId, new byte?[] { a1, a2, a3, a4, a5 });
            return new ApiOk { ok = true };
        }
        catch (Exception ex)
        {
            return new ApiOk { ok = false, error = ex.Message };
        }
    }

    [WebMethod(EnableSession = true), ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static ApiOk Skip(int surveyId)
    {
        int userId = GetUserIdFromSession();
        if (userId <= 0) return new ApiOk { ok = false, error = "no-login" };
        try { new BLLEncuesta().Skip(surveyId, userId); return new ApiOk { ok = true }; }
        catch (Exception ex) { return new ApiOk { ok = false, error = ex.Message }; }
    }
}
