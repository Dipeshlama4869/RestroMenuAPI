namespace RestroMenu.Interfaces
{
    public interface ICompanyMaster: IAuthCompanyInfo
    {
        string CreateCompany(CompanyMaster companyMaster);
        bool VerifyCreateCompany(string verificationcode, string companyId);
        bool IsCompanyExists(string email);
        Task<List<MenuItem>> GetMenuItemsAsync(string companyid);
        TrnUser AddWebUser(WebUserEntryModel userEntry, string companyId);
        TrnUser LoginWebUser(string companyId, string username, string password);
        string GetCompanyIdFromCompanyName(string companyName);
        Task<string> WebForgotPassword(string companyId, string user);
        string GetWebForgotPasswordLog(string id);
        Task<bool> ChangeWebForgotPasswordAsync(WebForgottenPasswordSetting passwordChangeEntry);
        Task<bool> ChangeWebPasswordAsync(WebForgottenPasswordSetting changePasswordEntry);
        string GetCompanyAPIkey(string companyId);
    }
}
