using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace IcreamShopApi.Models
{
	public class Cart
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int CartId { get; set; }
		[Required]
		public int UserId { get; set; }
		public int IceCreamId { get; set; }
		public int Quantity { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		[JsonIgnore]
		[ForeignKey("UserId")]
		public User? User { get; set; } 
		[JsonIgnore]
		[ForeignKey("IceCreamId")]
		public IceCream? IceCream { get; set; }
	}

}
