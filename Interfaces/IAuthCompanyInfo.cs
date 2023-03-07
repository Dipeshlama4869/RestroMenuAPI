namespace RestroMenu.Interfaces
{
    public interface IAuthCompanyInfo
    {
        ICompanyInfo GetCompanyInfo(string companyId, bool refresh = false);
    }
}
