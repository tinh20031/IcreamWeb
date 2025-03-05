using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace IcreamShopApi.Models
{
	public class IceCream
	{
		[Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int IceCreamId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		[Range(0.01, double.MaxValue, ErrorMessage = "giá phải lớn hơn không")]
		public decimal Price { get; set; }
		[Range(0, int.MaxValue, ErrorMessage = "số lượng kho phải lớn hơn 0 ")]
		public int Stock { get; set; }
		public string ImageUrl { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		[ForeignKey("Category")]
		[Required]
		public int CategoryId { get; set; }
		public virtual Category Category { get; set; }

		// Liên kết với OrderDetails và Reviews
		[JsonIgnore]
		public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
		[JsonIgnore]
		public ICollection<Review> Reviews { get; set; } = new List<Review>();

	}

}
