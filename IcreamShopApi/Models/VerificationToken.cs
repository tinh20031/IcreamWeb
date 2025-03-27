namespace IcreamShopApi.Models
{
    public class VerificationToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public virtual User User { get; set; } // Liên kết với User
    }
}
