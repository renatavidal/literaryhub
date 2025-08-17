using System;

[Serializable]
public class UserSession
{
    public int UserId { get; set; }
    public string Email { get; set; }
    public bool EmailVerified { get; set; }
    public string[] Roles { get; set; }

    public UserSession()
    {
        // defaults (no auto-initializers)
        this.EmailVerified = false;
        this.Roles = new string[0];
    }

    public bool IsInRole(string role)
    {
        if (string.IsNullOrEmpty(role)) return false;
        var roles = this.Roles ?? new string[0];
        for (int i = 0; i < roles.Length; i++)
        {
            if (string.Equals(roles[i], role, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
}
