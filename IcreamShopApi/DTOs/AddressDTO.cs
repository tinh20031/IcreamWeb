namespace IcreamShopApi.DTOs
{
    public class AddressDTO
    {
        public int AddressId { get; set; }
        public int UserId { get; set; }
        public string Street { get; set; }
        public string Ward { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public bool IsDefault { get; set; }

        // Tương thích với giao diện cũ
        public string City => District;
        public string State => Province;
      
    }
}