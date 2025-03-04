using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IcreamShopApi.Models
{
	public class Order
	{
		[Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int OrderId { get; set; }
		public int UserId { get; set; }
		public decimal TotalPrice { get; set; }
		public string Status { get; set; } = "Pending";
		public DateTime OrderDate { get; set; } = DateTime.Now;

		// Khóa ngoại
		public User User { get; set; }
		public ICollection<OrderDetail> OrderDetails { get; set; }
	}

}
