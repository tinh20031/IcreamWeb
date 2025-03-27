using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace IcreamShopApi.Models
{
    public class Address
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AddressId { get; set; }

        public int UserId { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }
   
        public string Ward { get; set; }   // Phường/Xã
        public string District { get; set; } // Quận/Huyện
        public string Province { get; set; } // Tỉnh/Thành phố

        public bool IsDefault { get; set; } // ịa chỉ mặc định

        // Khóa ngoại
        [JsonIgnore]
        public User User { get; set; }
    }
}