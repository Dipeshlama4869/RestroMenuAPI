namespace RestroMenu.Services
{
    public class MenuItemService
    {
        //private readonly IMenuItemsDataAccess menuItemDataAccess;
        //private readonly AWSuploadService awsUploadService;
        //private string cdnUrl = @"https://d3361ux20movyf.cloudfront.net";
        //public MenuItemService(IMenuItemsDataAccess menuItemDataAccess, AWSuploadService awsUploadService)
        //{
        //    this.menuItemDataAccess = menuItemDataAccess;
        //    this.awsUploadService = awsUploadService;
        //}


        //public async Task<string> AddMenuItem(MenuItemEntryModel menuItemModel)
        //{
        //    var ret = await menuItemDataAccess.AddMenuItem(menuItemModel);
        //    return ret;
        //}
        //public async Task<string> AddMenuItemWithImage(MenuItemEntryModel menuItemModel, Stream fileStream)
        //{
        //    string companyid = menuItemModel.companyId;

        //    var ret = await menuItemDataAccess.AddMenuItem(menuItemModel);
        //    if (!string.IsNullOrEmpty(ret))
        //    {
        //        string keyname = $"{companyid}_{ret}_0.png";
        //        await awsUploadService.UploadToAwsStream(keyname, fileStream);
        //    }
        //    return ret;
        //}
        //public async Task<bool> UpdateMenuItem(MenuItemEntryModel menuItemEntryModel)
        //{
        //    var ret = await menuItemDataAccess.UpdateMenuItem(menuItemEntryModel);
        //    return ret;
        //}
    }
}
