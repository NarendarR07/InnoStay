using InnoStay_DAL;
using InnoStay_DAL.DTO;
using InnoStay_DAL.Models;
using InnoStay_WebServices.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoStayTests
{
    public class BookingTest
    {
        private readonly Mock<IInnoStayRepository> mockRepo;
        private BookingController booking;

        public BookingTest()
        {
            mockRepo = new Mock<IInnoStayRepository>();
            booking = new BookingController(mockRepo.Object);
        }
        [Fact]
        public void GetAllBookingDetails_Success()
        {
            var bookingList = new List<BookingListDTO>
            {
                new BookingListDTO { BookingId = 1 },
                new BookingListDTO { BookingId = 2 }
            };

            mockRepo.Setup(r => r.GetAllBookingListItems()).Returns(bookingList);

            var result = booking.GetAllBookingDetails();

            var response = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<List<BookingListDTO>>(response.Value);
            Assert.Equal(2, list.Count);
        }

        [Fact]
        public void GetAllBookingDetails_Exception()
        {
            mockRepo.Setup(r => r.GetAllBookingListItems()).Throws(new Exception());

            var result = booking.GetAllBookingDetails();

            var response = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, response.StatusCode);
        }

        [Fact]
        public void GetBookingById_Success()
        {
            int id = 1;
            var bookingEntity = new Booking { BookingId = id, RoomId = 1, UserId = 1, CheckInDate = DateTime.Today, CheckOutDate = DateTime.Today.AddDays(1) };
            var room = new Room { RoomId = 1, RoomNumber = 101, RoomType = "Deluxe", PricePerNight = 100 };
            var user = new User { UserId = 1, FirstName = "John", LastName = "Doe" };

            mockRepo.Setup(r => r.GetBookingById(id)).Returns(bookingEntity);
            mockRepo.Setup(r => r.GetRoomById(1)).Returns(room);
            mockRepo.Setup(r => r.GetUserById(1)).Returns(user);

            var result = booking.GetBookingById(id);

            var response = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<BookingDetailsDTO>(response.Value);
            Assert.Equal(id, dto.BookingId);
            Assert.Equal(101, dto.RoomNumber);
            Assert.Equal("Deluxe", dto.RoomType);
            Assert.Equal("John Doe", dto.CustomerName);
        }

        [Fact]
        public void GetBookingById_BadRequest()
        {
            var result = booking.GetBookingById(0);
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid booking ID", response.Value);
        }

        [Fact]
        public void GetBookingById_NotFound()
        {
            mockRepo.Setup(r => r.GetBookingById(1)).Returns((Booking)null);

            var result = booking.GetBookingById(1);

            var response = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Booking not found", response.Value);
        }

        [Fact]
        public void GetBookingById_Exception()
        {
            mockRepo.Setup(r => r.GetBookingById(1)).Throws(new Exception());

            var result = booking.GetBookingById(1);

            var response = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, response.StatusCode);
        }

        [Fact]
        public void GetBookingsByUserId_Success()
        {
            var bookings = new List<GetBookingDTO>
            {
                new GetBookingDTO { BookingId = 1 },
                new GetBookingDTO { BookingId = 2 }
            };
            mockRepo.Setup(r => r.GetBookingsByUser(1)).Returns(bookings);

            var result = booking.GetBookingsByUserId(1);

            var response = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<List<GetBookingDTO>>(response.Value);
            Assert.Equal(2, list.Count);
        }

        [Fact]
        public void GetBookingsByUserId_BadRequest()
        {
            var result = booking.GetBookingsByUserId(0);
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid user ID", response.Value);
        }

        [Fact]
        public void GetBookingsByUserId_NotFound()
        {
            mockRepo.Setup(r => r.GetBookingsByUser(1)).Returns(new List<GetBookingDTO>());

            var result = booking.GetBookingsByUserId(1);

            var response = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No bookings found for this user", response.Value);
        }

        [Fact]
        public void AddBooking_Success()
        {
            var dto = new BookingDTO { UserID = 1, RoomType = "Deluxe", CheckInDate = DateTime.Today, CheckOutDate = DateTime.Today.AddDays(1) };
            var room = new Room { RoomId = 1, RoomNumber = 101, RoomType = "Deluxe" };

            mockRepo.Setup(r => r.GetAvailableRoom(dto.RoomType, dto.CheckInDate, dto.CheckOutDate)).Returns(room);
            mockRepo.Setup(r => r.AddBooking(It.IsAny<Booking>())).Returns(true);
            mockRepo.Setup(r => r.AddNotification(It.IsAny<Notification>())).Verifiable();

            var result = booking.AddBooking(dto);

            var response = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void AddBooking_NoRoomAvailable()
        {
            var dto = new BookingDTO { UserID = 1, RoomType = "Deluxe", CheckInDate = DateTime.Today, CheckOutDate = DateTime.Today.AddDays(1) };

            mockRepo.Setup(r => r.GetAvailableRoom(dto.RoomType, dto.CheckInDate, dto.CheckOutDate)).Returns((Room)null);

            var result = booking.AddBooking(dto);

            var response = Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void AddBooking_Exception()
        {
            var dto = new BookingDTO { UserID = 1, RoomType = "Deluxe", CheckInDate = DateTime.Today, CheckOutDate = DateTime.Today.AddDays(1) };

            mockRepo.Setup(r => r.GetAvailableRoom(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).Throws(new Exception());

            var result = booking.AddBooking(dto);

            var response = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, response.StatusCode);
        }

        [Fact]
        public void UpdateBooking_Success()
        {
            var bookingEntity = new Booking { BookingId = 1, BookingStatus = "Confirmed" };
            mockRepo.Setup(r => r.GetBookingById(1)).Returns(bookingEntity);
            mockRepo.Setup(r => r.UpdateBookingStatus(1, "Completed")).Returns(true);

            var result = booking.UpdateBooking(1, "Completed");

            var response = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Booking updated successfully", response.Value);
        }

        [Fact]
        public void UpdateBooking_NotFound()
        {
            mockRepo.Setup(r => r.GetBookingById(1)).Returns((Booking)null);

            var result = booking.UpdateBooking(1, "Completed");

            var response = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Booking 1 not found", response.Value);
        }

        [Fact]
        public void UpdateBooking_AlreadyCancelled()
        {
            var bookingEntity = new Booking { BookingId = 1, BookingStatus = "Cancelled" };
            mockRepo.Setup(r => r.GetBookingById(1)).Returns(bookingEntity);

            var result = booking.UpdateBooking(1, "Completed");

            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Cannot proceed — booking is already cancelled.", response.Value);
        }

        [Fact]
        public void UpdateBooking_UpdateFails()
        {

            var bookingEntity = new Booking { BookingId = 1, BookingStatus = "Confirmed" };
            mockRepo.Setup(r => r.GetBookingById(1)).Returns(bookingEntity);
            mockRepo.Setup(r => r.UpdateBookingStatus(1, "Completed")).Returns(false);

            var result = booking.UpdateBooking(1, "Completed");

            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error updating booking", response.Value);
        }

        [Fact]
        public void UpdateBooking_Exception()
        {

            mockRepo.Setup(r => r.GetBookingById(1)).Throws(new Exception("DB error"));

            var result = booking.UpdateBooking(1, "Completed");

            var response = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, response.StatusCode);
            Assert.Equal("DB error", response.Value);
        }


        [Fact]
        public void DeleteBooking_Success()
        {
            mockRepo.Setup(r => r.DeleteBooking(1)).Returns(true);

            var result = booking.DeleteBooking(1);

            var response = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Booking deleted successfully", response.Value);
        }

        [Fact]
        public void DeleteBooking_BadRequest()
        {
            mockRepo.Setup(r => r.DeleteBooking(1)).Returns(false);

            var result = booking.DeleteBooking(1);

            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error deleting booking", response.Value);
        }

        [Fact]
        public void DeleteBooking_Exception()
        {
            mockRepo.Setup(r => r.DeleteBooking(1)).Throws(new Exception());

            var result = booking.DeleteBooking(1);

            var response = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, response.StatusCode);
        }
    }
}
