namespace FastTrade.Models.Product
{
    public class Product
    {
        [Key]
        public int IND { get; set; }
        public string? GUID { get; set; }
        public string? CODE { get; set; }
        public string? NAME { get; set; }
        public int ENVANTER { get; set; }
        [Column(TypeName = "money")]
        public decimal PRICE { get; set; }
        [Column(TypeName = "money")]
        public decimal PURCHASEPRICE { get; set; }
        public bool? IsEnabled { get; set; }
        public bool? IsPurchase { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
