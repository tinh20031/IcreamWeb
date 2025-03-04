using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
		public User User { get; set; }
		public IceCream IceCream { get; set; }
	}

}
