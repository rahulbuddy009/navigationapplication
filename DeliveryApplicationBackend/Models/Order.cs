using DeliveryApplicationBackend.Enums;

namespace DeliveryApplicationBackend.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string ItemDescription { get; set; }

        public int DeliveryPersonId { get; set; } // This links to the delivery person
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
