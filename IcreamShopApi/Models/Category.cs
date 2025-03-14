using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace IcreamShopApi.Models
{
	public class Category
	{
		[Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int CategoryId { get; set; }
		public string Name { get; set; }
		public string image {  get; set; }

		// Tránh lỗi vòng lặp JSON khi trả về danh sách IceCreams
		[JsonIgnore]
		public List<IceCream> IceCreams { get; set; } = new List<IceCream>();

	}
}
