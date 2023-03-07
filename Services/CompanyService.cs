using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using RestroMenu.Helpers;
using RestroMenu.Interfaces;
using RestroMenu.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RestroMenu.Services
{
    public class CompanyService
    {
        private readonly ICompanyMaster companyMasterDataAccess;
        private readonly DbHelper _dbHelper;
        private readonly EmailHelper _emailHelper;
        private readonly RandomGeneratorHelper _randomGeneratorHelper;
        private readonly IMongoCollection<CompanyMaster> _companyMasterCollection;
        public CompanyService(ICompanyMaster companyMasterDataAccess, DbHelper dbHelper, EmailHelper emailHelper, RandomGeneratorHelper randomGeneratorHelper)
        {
            _dbHelper = dbHelper;
            _emailHelper = emailHelper;
            _randomGeneratorHelper = randomGeneratorHelper;
        }

        public string Create(AddInputModel input, string host)
        {
            var secretPassword = _randomGeneratorHelper.RandomPassword();

            string password = _randomGeneratorHelper.RandomPassword();

            string hashPassword = MD5Helper.CreateMD5(password);

            CompanyTheme companyTheme = new CompanyTheme
            {
                Name = "green",
                PrimaryColor = "",
                SecondaryColor = "",
            };

            CompanyMaster companyMaster = new CompanyMaster
            {
                CompanyId = input.Email,
                Name = input.Name,
                PanNo = input.TaxNo,
                Country = input.Country,
                Address = input.Address,
                City = input.City,
                PinNo = input.PinNo,
                ContactPerson = input.ContactPerson,
                Mobile = input.Mobile,
                Email = input.Email,
                ThemeColor = input.Theme == null ? companyTheme : input.Theme,
                AppSecret = secretPassword
            };

            companyMaster.MenuItemGroups.Add(new MenuItemGroup
            {
                GroupName = "Kitchen",
                MenuItemGroupId = "G0001",
                MGroup = "G0001"
            });
            companyMaster.MenuItems.Add(new MenuItem()
            {
                MCode = "M0001",
                DescA = "Kitchen Item",
                IsVat = 1,
                MenuCode = "M0001",
                Parent = "G0001",
                ProductType = 10,
                Unit = "Plate",
                MCat = "Kitchen"
            });

            companyMaster.ProductTypes.Add(new ProductType
            {
                PTypeId = 10,
                PTypeName = "MenuItem"
            });
            companyMaster.ProductTypes.Add(new ProductType
            {
                PTypeId = 0,
                PTypeName = "InventoryItem"
            });

            companyMaster.MCategories = new string[] { "Kitchen", "Bar" };

            var verificationCode = companyMasterDataAccess.CreateCompany(companyMaster);

            string html = $@"<html>
                    <body>
                    <h1>Click the link to verify your company creation</h1>
                    </br><p> Your Login Credtial is </p >
                    <p> user: <span> <b> admin </b></span></p>
                    <p> password: <span><b>{password}</b></span></p >
                    <a href=""{host}/api/V1/VerifyCompany?id={verificationCode}&companyid ={input.Email}"">
                    Click to Confirm Register </a >
                    </body>
                    </html>";

            //emailService.Send("sagun.tamrakar@gmail.com", "sagun.tamrakar@gmail.com", "Company Registration", html);
            _emailHelper.SendEmail(input.Email, "Confirmation from Restaurant Order System", html, "");
            //if (ret == "") return "";
            return verificationCode;

        }

        public bool VerifyCreatedCompany(string verificationCode, string comapnyId)
        {
            var ret = companyMasterDataAccess.VerifyCreateCompany(verificationCode, comapnyId);
            return ret;
        }

        public (string token, string companyname) WebLogin(string companyEmail, string username, string password)
        {
            //get companyid from companyname
            string companyId = companyEmail;  //companyMasterDataAccess.GetCompanyIdFromCompanyName(companyName);
            //if (string.IsNullOrEmpty(companyId)) throw new Exception("Invalid Company");

            ICompanyInfo cinfo = companyMasterDataAccess.GetCompanyInfo(companyId);

            string companyName = cinfo.CompanyName;
            string appSecret = cinfo.AppSecret;

            TrnUser user = companyMasterDataAccess.LoginWebUser(companyId, username, password);

            string jwtToken = GenerateWebUserJwtKey(companyId, appSecret, user.Username);
            return ("bearer " + jwtToken, companyName);
        }

        private string GenerateWebUserJwtKey(string companyId, string appSecret, string username, double days = 1)
        {
            //var cinfo = companyDataAccess.GetCompanyInfo(companyID);

            var secret = "secretKeyis" + appSecret;

            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
            securityKey.KeyId = companyId;

            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name,username),
                    new Claim(ClaimTypes.Role,"Admin"),
                }),
                Issuer = "IMS Software",
                Expires = DateTime.UtcNow.AddDays(days),
                SigningCredentials = signingCredentials
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            token.Header.Add("app", "web");
            token.Payload.Remove("iss");
            token.Payload.Add("iss", "IMS Software");
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }




    }

    public class BaseInputModel
    {

        public string Email { get; set; }
        public string Name { get; set; }
        public string TaxNo { get; set; }
        public string Mobile { get; set; }
        public string ContactPerson { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PinNo { get; set; }
        public string LogoUrl { get; set; }
        public CompanyTheme Theme { get; set; }
    }

    public class AddInputModel : BaseInputModel
    {

    }
}
