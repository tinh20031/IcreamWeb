using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace IcreamShopApi.Models
{
	public class Review
	{
		[Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ReviewId { get; set; }
		public int UserId { get; set; }
		public int IceCreamId { get; set; }
		public int Rating { get; set; }
		public string Comment { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		// Khóa ngoại
		[JsonIgnore]
		public User User { get; set; }
		[JsonIgnore]
		public IceCream IceCream { get; set; }
	}

}
