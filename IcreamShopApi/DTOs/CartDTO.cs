namespace IcreamShopApi.DTOs
{
	public class CartDTO
	{
		public int CartId { get; set; }
		public int UserId { get; set; }
		public int IceCreamId { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }
		public DateTime CreatedAt { get; set; }
		public string IceCreamName { get; set; }
		public string Image { get; set; } 
	}
}
