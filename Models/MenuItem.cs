namespace RestroMenu.Models
{
    public class MenuItem: BaseModel
    {
        [BsonElement("m_code")]
        public string MCode { get; set; }

        [BsonElement("menu_code")]
        public string MenuCode { get; set; }

        [BsonElement("desc_asc")]
        public string DescA { get; set; }

        [BsonElement("unit")]
        public string Unit { get; set; }

        [BsonElement("is_vat")]
        public int IsVat { get; set; }  //0 to 9 is inventory item, 11 above is service item, 1= raw material or non-selling item

        [BsonElement("product_type")]
        public int ProductType { get; set; }

        [BsonElement("parent")]
        public string Parent { get; set; }//MenuItemGroup Id

        [BsonElement("m_cat")]
        public string MCat { get; set; }//Main Categories

        [BsonElement("sub_cat")]
        public string SubCat { get; set; }// Sub Categories

        [BsonElement("image_url")]
        public string ImageUrl { get; set; }

        [BsonElement("rate")]
        public double Rate { get; set; }

        [BsonElement("food_prep_time")]
        public double FoodPrepTime { get; set; }

        [BsonElement("discontinued")]
        public int Discontinued { get; set; }

        [BsonElement("discontinue_date")]
        public DateTime? DiscontinueDate { get; set; }

        [BsonElement("discontinue_period")]
        public int? DiscontinuePeriod { get; set; }

        [BsonElement("brand")]
        public string Brand { get; set; }

        [BsonElement("season")]
        public string Season { get; set; }

        [BsonElement("tags")]
        public string[] Tags { get; set; }

        [BsonElement("variant_master")]
        public List<ProductVariant> VariantMaster { get; set; }

        [BsonElement("variant_detail")]
        public List<VariantLine> VariantDetail { get; set; }


        [BsonElement("composite_detail")]
        public List<CompositeLine> CompositeDetail { get; set; }




    }
}
