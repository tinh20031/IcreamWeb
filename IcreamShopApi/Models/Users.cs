using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace IcreamShopApi.Models
{
	public class User
	{
		[Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int UserId { get; set; }
		public string FullName { get; set; }
		public string Email { get; set; }
		public string PasswordHash { get; set; }
		public string PhoneNumber { get; set; }
		public string Address { get; set; }
		public string Role { get; set; } = "Customer";
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		// Liên kết với Orders và Reviews
		[JsonIgnore]
		public ICollection<Order> Orders { get; set; }
		[JsonIgnore]
		public ICollection<Review> Reviews { get; set; }
	}

}
