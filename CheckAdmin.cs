namespace KR123
{
    public class CheckAdmin
    {
        public string Login { get; set; }
        public bool IsAdmin { get; }
        public string Status => IsAdmin ? "Admin" : "User";
        public CheckAdmin(string login, bool isAdmin)
        {
            Login = login.Trim();
            IsAdmin = isAdmin;
        }
    }
}
