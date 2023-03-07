namespace RestroMenu.Interfaces
{
    public interface ICompanyInfo
    {
        string Address { get; set; }
        string APIKey { get; set; }
        string AppSecret { get; set; }
        string CompanyID { get; set; }
        string CompanyName { get; set; }
        string Database { get; set; }
        string Division { get; set; }
        string Email { get; set; }
        string MasterCompany { get; set; }
        string PAN { get; set; }
        string Password { get; set; }
        int RequireHash { get; set; }
        string Secret { get; set; }
        string Server { get; set; }
        CompanyTheme ThemeColor { get; set; }
        string UserID { get; set; }
        string UserPassword { get; set; }
        int UserTokenExpiryTimeInMinutes { get; set; }
        string Phone { get; set; }
        string LogoUrl { get; set; }
        string fiscalid { get; set; }
    }
}
