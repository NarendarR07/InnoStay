using InnoStay_DAL;
using InnoStay_DAL.DTO;
using InnoStay_DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace InnoStay_WebServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        public IInnoStayRepository repo { get; set; }
        public BookingController(IInnoStayRepository repository)
        {
            repo = repository;
        }
        
        [Authorize(Roles ="Admin")]
        [HttpGet]
        [Route("GetAllBookingDetails")]
        public IActionResult GetAllBookingDetails()
        {
            try
            {
                var items = repo.GetAllBookingListItems();
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }




        [Authorize(Roles = "User,Admin")]
        [HttpGet("GetBookingById/{bookingId:int}")]
        public IActionResult GetBookingById(int bookingId)
        {
            if (bookingId <= 0)
                return BadRequest("Invalid booking ID");

            try
            {
                
                Booking bookingEntity = repo.GetBookingById(bookingId);
                if (bookingEntity == null)
                    return NotFound("Booking not found");

                if (bookingEntity.RoomId == null)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                                      "Booking has no associated RoomId");

                Room roomEntity = repo.GetRoomById(bookingEntity.RoomId.Value);
                if (roomEntity == null)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                                      "Room not found for this booking");
                User userEntity = repo.GetUserById(bookingEntity.UserId.Value);
                if (userEntity == null)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                                       "User not found for this booking");

                int nights = (bookingEntity.CheckOutDate.Value
                              - bookingEntity.CheckInDate.Value).Days;
                decimal? totalPrice = nights * roomEntity.PricePerNight;

                var dto = new BookingDetailsDTO
                {
                    BookingId = bookingEntity.BookingId,
                    RoomNumber = roomEntity.RoomNumber,
                    RoomType = roomEntity.RoomType,
                    CheckInDate = bookingEntity.CheckInDate.Value,
                    CheckOutDate = bookingEntity.CheckOutDate.Value,
                    Price = totalPrice,
                    BookingStatus = bookingEntity.BookingStatus,
                    CustomerName = $"{userEntity.FirstName} {userEntity.LastName}"
                };


                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }



        // Get all bookings by User ID
        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        [Route("GetBookingsByUserId")]
        public IActionResult GetBookingsByUserId(int userId)
        {
            try
            {
                if (userId > 0)
                {
                    var bookings = repo.GetBookingsByUser(userId);
                    if (bookings != null && bookings.Any())
                    {
                        return Ok(bookings);
                    }
                    else
                    {
                        return NotFound("No bookings found for this user");
                    }
                }
                else
                {
                    return BadRequest("Invalid user ID");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles = "User")]
        [HttpPost("AddBooking")]
        public IActionResult AddBooking([FromBody] BookingDTO bookingObj)
        {
            try
            {

                var today = DateTime.UtcNow.Date;
                if (bookingObj.CheckInDate.Date < today)
                    return BadRequest("Check-in date cannot be in the past.");
                if (bookingObj.CheckOutDate <= bookingObj.CheckInDate)
                    return BadRequest("Check-out date must be after check-in date.");

                var availableRoom = repo.GetAvailableRoom(
                    bookingObj.RoomType,
                    bookingObj.CheckInDate,
                    bookingObj.CheckOutDate
                );
                if (availableRoom == null)
                    return BadRequest("No rooms available for the selected type and dates.");

                var booking = new Booking
                {
                    UserId = bookingObj.UserID,
                    RoomId = availableRoom.RoomId,
                    CheckInDate = bookingObj.CheckInDate,
                    CheckOutDate = bookingObj.CheckOutDate,
                    BookingStatus = "Pending"
                };

                if (!repo.AddBooking(booking))
                    return StatusCode(500, "Booking failed. Please try again later.");

                var note = new Notification
                {
                    UserId = bookingObj.UserID,
                    Message = $"Almost there—booking #{booking.BookingId} will be confirmed once we receive your payment details. My Bookings => Confirm Payment",
                    IsRead = false
                };
                repo.AddNotification(note);

                return Ok(new
                {
                    bookingId = booking.BookingId,
                    roomNumber = availableRoom.RoomNumber,
                    roomType = availableRoom.RoomType,
                    checkIn = bookingObj.CheckInDate,
                    checkOut = bookingObj.CheckOutDate,
                    message = "Booking created! Please complete payment to confirm."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }



        // Update a booking
        [Authorize(Roles = "Admin,User")]
        [HttpPut]
        [Route("UpdateBooking")]
        public IActionResult UpdateBooking(int bookingId, string bookingStatus)
        {
            try
            {
                
                var existing = repo.GetBookingById(bookingId);
                if (existing == null)
                    return NotFound($"Booking {bookingId} not found");

                
                if (existing.BookingStatus == "Cancelled")
                    return BadRequest("Cannot proceed — booking is already cancelled.");
         
                bool result = repo.UpdateBookingStatus(bookingId, bookingStatus);
                if (result)
                    return Ok("Booking updated successfully");
                else
                    return BadRequest("Error updating booking");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }




        // Delete a booking
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("DeleteBooking")]
        public IActionResult DeleteBooking(int bookingId)
        {
            try
            {
                bool result = repo.DeleteBooking(bookingId);
                if (result)
                {
                    return Ok("Booking deleted successfully");
                }
                else
                {
                    return BadRequest("Error deleting booking");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //Cancel a booking
        [Authorize(Roles = "User")]
        [HttpPut("CancelBooking")]
        public IActionResult CancelBooking(int bookingId)
        {
            try
            {
                
                var booking = repo.GetBookingById(bookingId);
                if (booking == null)
                    return NotFound("Booking not found");

                
                var now = DateTime.UtcNow;
                var start = booking.CheckInDate.Value.Date;
                if (now.Date >= start)
                    return BadRequest("Cancellation window has closed.");

                
                bool result = repo.CancelBooking(bookingId);
                if (result)
                    return Ok("Booking cancelled successfully");
                else
                    return BadRequest("Error cancelling booking");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


    }
}
