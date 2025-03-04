namespace IcreamShopApi.Models
{
	public class OrderDetail
	{
		public int OrderDetailId { get; set; }
		public int OrderId { get; set; }
		public int IceCreamId { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }

		// Khóa ngoại
		public Order Order { get; set; }
		public IceCream IceCream { get; set; }
	}

}
