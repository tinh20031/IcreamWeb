namespace IcreamShopApi.Models
{
	public class Cart
	{
		public int CartId { get; set; }
		public int UserId { get; set; }
		public int IceCreamId { get; set; }
		public int Quantity { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		// Khóa ngoại
		public User User { get; set; }
		public IceCream IceCream { get; set; }
	}

}
