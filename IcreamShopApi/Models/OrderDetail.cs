using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IcreamShopApi.Models
{
	public class OrderDetail
	{
		[Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
