using InnoStay_DAL.DTO;
using InnoStay_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoStay_DAL
{
    public interface IInnoStayRepository
    {
        //User

        string ValidateCredentials(string email, string password);
        List<User> GetUsers();
        User GetUserById(int userId);
        bool AddUser(User user);
        bool UpdateUser(User user);
        bool DeleteUser(int userId);
        User GetUserByEmail(string email);

        //Booking
        List<BookingListDTO> GetAllBookingListItems();
        Booking GetBookingById(int bookingId);
        List<GetBookingDTO> GetBookingsByUser(int userId);
        bool AddBooking(Booking booking);
        bool UpdateBookingStatus(int bookingId, string bookingStatus);
        bool DeleteBooking(int bookingId);
        bool CancelBooking(int bookingId);


        //Room

        List<Room> GetRooms();
        Room GetAvailableRoom(string roomType, DateTime checkIn, DateTime checkOut);
        Room GetRoomById(int roomId);
        bool AddRoom(Room room);
        bool UpdateRoom(Room room);
        bool UpdateRoomPrice(int roomId, decimal newPrice);
        bool DeleteRoom(int roomId);


        //Payment
        List<Payment> GetPayments();
        Payment GetPaymentById(int paymentId);
        List<Payment> GetPaymentsByUserId(int userId);
        bool AddPayment(Payment payment);
        bool UpdatePaymentMethod(int paymentId, string paymentMethod);
        bool UpdatePaymentStatus(int paymentId, string paymentStatus);
        bool DeletePayment(int paymentId);


        //Notification
        List<Notification> GetNotifications();
        Notification GetNotificationById(int notificationId);
        bool AddNotification(Notification notification);
        bool MarkNotificationAsRead(int notificationId);
        bool DeleteNotification(int notificationId);


        //Feedback
        List<Feedback> GetFeedbacks();
        Feedback GetFeedbackById(int feedbackId);
        bool AddFeedback(Feedback feedback);
        bool UpdateFeedback(int feedbackId, int rating, string comments);
        bool DeleteFeedback(int feedbackId);
    }
}
