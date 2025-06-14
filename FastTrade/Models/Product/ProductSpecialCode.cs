namespace FastTrade.Models.Product
{
    public class ProductSpecialCode
    {
        [Key]
        public int IND { get; set; }
        public int STOCKIND { get; set; }
        public string? NAME { get; set; }
        public string? CODE { get; set; }
    }
}
