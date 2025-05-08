using System.ComponentModel.DataAnnotations;

namespace DeliveryApplicationBackend.Models
{
    public class PickupLocation
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Landmark { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Pincode { get; set; }
        public string Country { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public string PickupCode { get; set; }
    }

}
