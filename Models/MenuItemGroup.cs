namespace RestroMenu.Models
{
    public class MenuItemGroup
    {
        public string MenuItemGroupId { get; set; }
        public string GroupName { get; set; }
        public string Parent { get; set; }
        public string MGroup { get; set; }
        public int IsDiscontinued { get; set; }
    }
}
