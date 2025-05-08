using DeliveryApplicationBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;

using System.Collections.Generic;
using System.Threading.Tasks;
using SkiaSharp;

namespace DeliveryApplicationBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PickupController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PickupController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Pickup
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PickupLocation>>> GetAllPickups()
        {
            return await _context.PickupLocations.ToListAsync();
        }

        // GET: api/Pickup/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PickupLocation>> GetPickupById(int id)
        {
            var pickup = await _context.PickupLocations.FindAsync(id);

            if (pickup == null)
                return NotFound();

            return pickup;
        }

        //// POST: api/Pickup
        //[HttpPost]
        //public async Task<IActionResult> PostPickup([FromBody] PickupLocation pickup)
        //{
        //    if (pickup == null)
        //        return BadRequest("Pickup location data is required.");

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    try
        //    {
        //        await _context.PickupLocations.AddAsync(pickup);
        //        await _context.SaveChangesAsync();

        //        return CreatedAtAction(nameof(GetPickupById), new { id = pickup.Id }, pickup);
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        return StatusCode(500, $"Database error: {ex.Message}");
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> PostPickup([FromBody] PickupLocation pickup)
        {
            if (pickup == null)
                return BadRequest("Pickup location data is required.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Generate the unique pickup code
                pickup.PickupCode = GenerateUniqueCode(pickup);

                await _context.PickupLocations.AddAsync(pickup);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPickupById), new { id = pickup.Id }, pickup);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }

        private string GenerateUniqueCode(PickupLocation pickup)
        {
            string countryCode = GetCode(pickup.Country);
            string stateCode = GetCode(pickup.State);
            string cityCode = GetCode(pickup.City);
            string geoCode = GenerateSimpleGeoCode(pickup.Latitude.Value, pickup.Longitude.Value);

            return $"{countryCode}-{stateCode}-{cityCode}-{geoCode}";
        }

        private string GetCode(string value)
        {
            return new string((value ?? "").ToUpper().Where(char.IsLetter).Take(2).ToArray());
        }

        private string GenerateSimpleGeoCode(double latitude, double longitude)
        {
            string lat = Math.Round(latitude, 2).ToString("0.00").Replace(".", "");
            string lng = Math.Round(longitude, 2).ToString("0.00").Replace(".", "");
            return $"{lat}{lng}";
        }


        [HttpGet("map-url/{pickupCode}")]
        public IActionResult GetGoogleMapUrlFromPickupCode(string pickupCode)
        {
            if (string.IsNullOrWhiteSpace(pickupCode))
                return BadRequest("Invalid pickup code format.");

            try
            {
                // Expecting pickupCode like: IN-HA-YA-30157728
                var segments = pickupCode.Split('-');
                if (segments.Length < 2)
                    return BadRequest("Pickup code format is incorrect.");

                string geoPart = segments.Last(); // Take last segment: 30157728

                if (geoPart.Length != 8 || !geoPart.All(char.IsDigit))
                    return BadRequest("Geolocation part must be 8 digits.");

                string latStr = geoPart.Substring(0, 4); // "3015"
                string lngStr = geoPart.Substring(4, 4); // "7728"

                double latitude = double.Parse(latStr.Insert(2, "."));  // "30.15"
                double longitude = double.Parse(lngStr.Insert(2, ".")); // "77.28"

                string googleMapUrl = $"https://www.google.com/maps?q={latitude},{longitude}";

                return Ok(new
                {
                    Latitude = latitude,
                    Longitude = longitude,
                    GoogleMapsUrl = googleMapUrl
                });
            }
            catch
            {
                return BadRequest("Unable to decode location from pickup code.");
            }
        }


        [HttpGet("qrcode/{pickupCode}")]
        public IActionResult GetQRCodePng(string pickupCode)
        {
            if (string.IsNullOrWhiteSpace(pickupCode))
                return BadRequest("Pickup code is required.");

            try
            {
                using (var qrGenerator = new QRCodeGenerator())
                using (var qrCodeData = qrGenerator.CreateQrCode(pickupCode, QRCodeGenerator.ECCLevel.Q))
                {
                    // Use QRCoder's built-in PNG generator
                    var pngCode = new PngByteQRCode(qrCodeData);
                    byte[] qrCodeBytes = pngCode.GetGraphic(20);

                    return File(qrCodeBytes, "image/png");
                }
            }
            catch (Exception ex)
            {
                // Log the actual error (ex.Message) for debugging
                return StatusCode(500, $"QR Generation Failed: {ex.Message}");
            }
        }






        // PUT: api/Pickup/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePickup(int id, [FromBody] PickupLocation updatedPickup)
        {
            if (id != updatedPickup.Id)
                return BadRequest("ID mismatch.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingPickup = await _context.PickupLocations.FindAsync(id);
            if (existingPickup == null)
                return NotFound();

            // Update fields
            existingPickup.Address = updatedPickup.Address;
            existingPickup.City = updatedPickup.City;
            existingPickup.State = updatedPickup.State;
            existingPickup.Pincode = updatedPickup.Pincode;
            existingPickup.Country = updatedPickup.Country;
            existingPickup.Landmark = updatedPickup.Landmark;
            existingPickup.Latitude = updatedPickup.Latitude;
            existingPickup.Longitude = updatedPickup.Longitude;

            try
            {
                _context.PickupLocations.Update(existingPickup);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }

        // DELETE: api/Pickup/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePickup(int id)
        {
            var pickup = await _context.PickupLocations.FindAsync(id);
            if (pickup == null)
                return NotFound();

            try
            {
                _context.PickupLocations.Remove(pickup);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }
    }
}
