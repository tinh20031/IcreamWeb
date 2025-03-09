namespace IcreamShopApi.DTOs
{
	public class OrderDTO
	{
		public int OrderId { get; set; }
		public decimal TotalPrice { get; set; }
		public string Status { get; set; }
		public DateTime OrderDate { get; set; }
		public List<OrderDetailDTO> OrderDetails { get; set; }
	}

	public class OrderDetailDTO
	{
		public string IceCreamName { get; set; }
		public string ImageUrl { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }
	}

}
