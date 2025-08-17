public class UserSession
{
    public int UserId { get; set; }
    public string Email { get; set; }
    public bool EmailVerified { get; set; } = false;
    public string[] Roles { get; set; } = new string[0];

    public bool IsInRole(string role) =>
        Roles != null && System.Array.IndexOf(Roles, role) >= 0;
}
