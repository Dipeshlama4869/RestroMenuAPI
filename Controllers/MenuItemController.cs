using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Protocol;
using RestroMenu.Helpers;
using RestroMenu.Services;

namespace RestroMenu.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly DbHelper _dbHelper;
        private readonly IMongoCollection<MenuItem> _menuItemCollection;
        private readonly AWSUploadService _awsUploadService;
        private string cdnUrl = @"https://d3361ux20movyf.cloudfront.net";

        public MenuItemController(DbHelper dbHelper, AWSUploadService awsUploadService)
        {
            _dbHelper = dbHelper;
            _menuItemCollection = _dbHelper.GetCollection<MenuItem>();
            _awsUploadService = awsUploadService;
        }

        public class AddInputModel
        {
            public string companyId { get; set; }
            public MenuItem menuItem { get; set; }
        }

    }
}
