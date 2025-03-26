using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
		[JsonIgnore]
		public Order Order { get; set; }
		[JsonIgnore]
		public IceCream IceCream { get; set; }
	}

}
