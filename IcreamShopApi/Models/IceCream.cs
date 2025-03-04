using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IcreamShopApi.Models
{
	public class IceCream
	{
		[Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int IceCreamId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal Price { get; set; }
		public int Stock { get; set; }
		public string ImageUrl { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		// Liên kết với OrderDetails và Reviews
		public ICollection<OrderDetail> OrderDetails { get; set; }
		public ICollection<Review> Reviews { get; set; }
	}

}
