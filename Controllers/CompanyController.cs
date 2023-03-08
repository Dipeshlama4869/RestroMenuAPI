using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RestroMenu.Interfaces;

namespace RestroMenu.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyMaster companyMasterData;
        private readonly DbHelper _dbHelper;
        private readonly EmailHelper _emailHelper;
        private readonly RandomGeneratorHelper _randomGeneratorHelper;
        private readonly IMongoCollection<CompanyMaster> _companyMasterCollection;

        public CompanyController(DbHelper dbHelper, RandomGeneratorHelper randomGeneratorHelper, EmailHelper emailHelper)
        {
            _dbHelper = dbHelper;
            _emailHelper = emailHelper;
            _randomGeneratorHelper = randomGeneratorHelper;
            _companyMasterCollection = _dbHelper.GetCollection<CompanyMaster>();
        }

    }
}
