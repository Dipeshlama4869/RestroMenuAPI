namespace RestroMenu.Models
{
    public class CompanyMaster: BaseModel
    {
        [BsonElement("company_id")]
        public string CompanyId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("address")]
        public string Address { get; set; }

        [BsonElement("pan_no")]
        public string PanNo { get; set; }

        [BsonElement("city")]
        public string City { get; set; }

        [BsonElement("country")]
        public string Country { get; set; }

        [BsonElement("contact_person")]
        public string ContactPerson { get; set; }

        [BsonElement("pin_no")]
        public string PinNo { get; set; }

        [BsonElement("mobile")]
        public string Mobile { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("category")]
        public string Category { get; set; }

        [BsonElement("logo_url")]
        public string LogoUrl { get; set; }

        [BsonElement("theme_color")]
        public CompanyTheme ThemeColor { get; set; }

        [BsonElement("menu_items")]
        public List<MenuItem> MenuItems { get; set; }

        [BsonElement("units")]
        public List<Unit> Units { get; set; }

        [BsonElement("menu_item_groups")]
        public List<MenuItemGroup> MenuItemGroups { get; set; }

        [BsonElement("verified")]
        public string Verfied { get; set; }

        [BsonElement("user_logs")]
        public List<UserLog> UserLogs { get; set; }

        [BsonElement("app_secret")]
        public string AppSecret { get; set; }

        [BsonElement("api_key")]
        public string APIKey { get; set; }

        [BsonElement("m_categories")]
        public string[] MCategories { get; set; }

        [BsonElement("product_types")]
        public List<ProductType> ProductTypes { get; set; }

    }
}
