namespace FastTrade.Models.Users
{
    public record class Users
    {
        [Key]
        public int IND { get; set; }
        public string? UserName { get; set; }
        public string? UserNameAndSurname { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPhone { get; set; }
        public string? UserPassword { get; set; }
    }
}
