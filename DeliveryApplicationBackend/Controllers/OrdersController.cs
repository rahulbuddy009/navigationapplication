using DeliveryApplicationBackend.Enums;

using DeliveryApplicationBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DeliveryApplicationBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<OrderHub> _hubContext;

        public OrdersController(ApplicationDbContext context, IHubContext<OrderHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // Get orders assigned to logged-in delivery person
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            // TEMP: hardcoded user ID for testing
            var userId = 1;

            var orders = await _context.Orders
                .Where(o => o.DeliveryPersonId == userId)
                .ToListAsync();

            return Ok(orders);
        }

        // Swipe-to-complete endpoint with SignalR notification
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] OrderStatusUpdateDto dto)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            order.Status = dto.NewStatus.ToString(); // Assuming Status is a string column
            await _context.SaveChangesAsync();

            // Notify delivery group via SignalR
            await _hubContext.Clients
                .Group($"delivery-{order.DeliveryPersonId}")
                .SendAsync("OrderStatusUpdated", order);

            return Ok(order);
        }

        // DTO for status update
        public record OrderStatusUpdateDto(OrderStatus NewStatus);
    }
}
