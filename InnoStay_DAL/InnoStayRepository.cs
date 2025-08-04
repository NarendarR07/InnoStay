using InnoStay_DAL.DTO;
using InnoStay_DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoStay_DAL
{
    public class InnoStayRepository : IInnoStayRepository
    {
        public InnoStayDbContext Context { get; set; }
        public InnoStayRepository()
        {
            Context = new InnoStayDbContext();
        }

        public User GetUserByEmail(string email)
        {
            using (var context = new InnoStayDbContext())
            {
                return context.Users.FirstOrDefault(u => u.Email == email);
            }
        }

        public string ValidateCredentials(string email, string password)
        {
            try
            {
                using (var context = new InnoStayDbContext())
                {
                    
                    var user = context.Users
                        .FirstOrDefault(u => u.Email == email && u.Password == password);

                    if (user != null)
                    {
                        return user.Role; 
                    }
                    else
                    {
                        return "Invalid credentials";
                    }
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Login validation error: {ex.Message}");
                return "Invalid credentials";
            }
        }
        public List<User> GetUsers()
        {
            List<User> users = new List<User>();
            try
            {
                users = Context.Users.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                users = null;
            }
            return users;
        }

        public User GetUserById(int userId)
        {
            User user = null;
            try
            {
                user = Context.Users.Find(userId);
            }
            catch (Exception ex)
            {
                user = null;
            }
            return user;
        }

        public bool AddUser(User user)
        {
            bool status = false;
            try
            {
                user.CreatedAt = DateTime.Now;
                Context.Users.Add(user);
                Context.SaveChanges();
                status = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                status = false;
            }
            return status;
        }

        public bool UpdateUser(User user)
        {
            bool status = false;
            try
            {
                var existing = Context.Users.Find(user.UserId);
                if (existing != null)
                {
                    existing.FirstName = user.FirstName;
                    existing.LastName = user.LastName;
                    existing.Email = user.Email;
                    existing.Password = user.Password;
                    existing.Role = user.Role;
                    Context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }

        public bool DeleteUser(int userId)
        {
            bool status = false;
            try
            {
                var user = Context.Users.Find(userId);
                if (user != null)
                {
                    Context.Users.Remove(user);
                    Context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }

        //Room Table
        public List<Room> GetRooms()
        {
            List<Room> rooms = new List<Room>();
            try
            {
                rooms = Context.Rooms.ToList();
            }
            catch (Exception ex)
            {
                rooms = null;
            }
            return rooms;
        }

        public Room GetRoomById(int roomId)
        {
            Room room = null;
            try
            {
                room = Context.Rooms.Find(roomId);
            }
            catch (Exception ex)
            {
                room = null;
            }
            return room;
        }

        public Room GetAvailableRoom(string roomType, DateTime checkIn, DateTime checkOut)
        {
            Room availableRoom = null;
            try
            {
                availableRoom = Context.Rooms
                    .Where(r => r.RoomType == roomType && r.IsAvailable)
                    .Where(r => !Context.Bookings.Any(b =>
                        b.RoomId == r.RoomId &&
                        b.BookingStatus == "Confirmed" &&
                        !(b.CheckOutDate < checkIn || b.CheckInDate > checkOut)
                    ))
                    .OrderBy(r => r.RoomNumber)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);
                }
            }

            return availableRoom;
        }



        public bool AddRoom(Room room)
        {
            bool status = false;
            try
            {
                Context.Rooms.Add(room);
                Context.SaveChanges();
                status = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while adding room: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
                }
                status = false;
            }
            return status;
        }

        public bool UpdateRoom(Room room)
        {
            bool status = false;
            try
            {
                var existing = Context.Rooms.Find(room.RoomId);
                if (existing != null)
                {
                    existing.RoomNumber = room.RoomNumber;
                    existing.RoomType = room.RoomType;
                    existing.PricePerNight = room.PricePerNight;
                    existing.Capacity = room.Capacity;
                    existing.IsAvailable = room.IsAvailable;
                    Context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }

        public bool UpdateRoomPrice(int roomId, decimal newPrice)
        {
            bool success = false;
            try
            {
                var room = Context.Rooms.Find(roomId);
                if (room != null)
                {
                    room.PricePerNight = newPrice;
                    Context.SaveChanges();
                    success = true;
                }
            }
            catch (Exception)
            {
                success = false;
            }
            return success;
        }

        public bool DeleteRoom(int roomId)
        {
            bool status = false;
            try
            {
                var room = Context.Rooms.Find(roomId);
                if (room != null)
                {
                    Context.Rooms.Remove(room);
                    Context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }

        //Booking Table

        public List<BookingListDTO> GetAllBookingListItems()
        {
            try
            {
                return (from b in Context.Bookings
                        join r in Context.Rooms on b.RoomId equals r.RoomId
                        join u in Context.Users on b.UserId equals u.UserId   
                        select new BookingListDTO
                        {
                            BookingId = b.BookingId,
                            UserName = u.FirstName + " " + u.LastName,       
                            RoomNumber = r.RoomNumber,
                            RoomType = r.RoomType,
                            CheckInDate = b.CheckInDate.Value,
                            CheckOutDate = b.CheckOutDate.Value,
                            BookingStatus = b.BookingStatus
                        }).ToList();
            }
            catch
            {
                return null;
            }
        }



        public Booking GetBookingById(int bookingId)
        {
            Booking booking = null;
            try
            {
                booking = Context.Bookings.Find(bookingId);
            }
            catch (Exception ex)
            {
                booking = null;
            }
            return booking;
        }


        public List<GetBookingDTO> GetBookingsByUser(int userId)
        {
            var bookings = new List<GetBookingDTO>();
            try
            {
                bookings = (from b in Context.Bookings
                            join r in Context.Rooms on b.RoomId equals r.RoomId
                            where b.UserId == userId
                            select new GetBookingDTO
                            {
                                BookingId = b.BookingId,
                                RoomNumber = r.RoomNumber ?? 0,
                                RoomType = r.RoomType,
                                CheckInDate = b.CheckInDate,
                                CheckOutDate = b.CheckOutDate,
                                Status = b.BookingStatus
                            }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return bookings;
        }


        public bool AddBooking(Booking booking)
        {
            bool status = false;
            try
            {
                Context.Bookings.Add(booking);
                Context.SaveChanges();
                status = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);
                }
                status = false;
            }
            return status;
        }

        public bool UpdateBookingStatus(int bookingId, string bookingStatus)
        {
            bool status = false;
            try
            {
                var booking = Context.Bookings.Find(bookingId);
                if (booking != null)
                {
                    booking.BookingStatus = bookingStatus;
                    Context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }

        public bool DeleteBooking(int bookingId)
        {
            bool status = false;
            try
            {
                var booking = Context.Bookings.Find(bookingId);
                if (booking != null)
                {
                    Context.Bookings.Remove(booking);
                    Context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }
        public bool CancelBooking(int bookingId)
        {
            bool status = false;
            try
            {
                var booking = Context.Bookings.Find(bookingId);
                if (booking != null)
                {
                    booking.BookingStatus = "Cancelled";
                    Context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }

        //Payment Table
        public List<Payment> GetPayments()
        {
            List<Payment> payments = new List<Payment>();
            try
            {
                payments = Context.Payments.ToList();
            }
            catch (Exception ex)
            {
                payments = null;
            }
            return payments;
        }

        public Payment GetPaymentById(int paymentId)
        {
            Payment payment = null;
            try
            {
                payment = Context.Payments.Find(paymentId);
            }
            catch (Exception ex)
            {
                payment = null;
            }
            return payment;
        }

        public List<Payment> GetPaymentsByUserId(int userId)
        {
            return Context.Payments
                .Where(p => p.Booking.UserId == userId)
                .ToList();
        }


        public bool AddPayment(Payment payment)
        {
            bool status = false;
            try
            {
                Context.Payments.Add(payment);
                Context.SaveChanges();
                status = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);
                }
                status = false;
            }
            return status;
        }

        public bool UpdatePaymentMethod(int paymentId, string paymentMethod)
        {
            bool status = false;
            try
            {
                var payment = Context.Payments.Find(paymentId);
                if (payment != null)
                {
                    payment.PaymentMethod = paymentMethod;
                    Context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }

        public bool UpdatePaymentStatus(int paymentId, string paymentStatus)
        {
            bool status = false;
            try
            {
                var payment = Context.Payments.Find(paymentId);
                if (payment != null)
                {
                    payment.PaymentStatus = paymentStatus;
                    Context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }

        public bool DeletePayment(int paymentId)
        {
            bool status = false;
            try
            {
                var payment = Context.Payments.Find(paymentId);
                if (payment != null)
                {
                    Context.Payments.Remove(payment);
                    Context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }

        //Feedback Table
        public List<Feedback> GetFeedbacks()
        {
            List<Feedback> feedbacks = new List<Feedback>();
            try
            {
                feedbacks = Context.Feedbacks
                                    .Include(f => f.User)
                                    .Include(f => f.Room)
                                    .ToList();
            }
            catch (Exception ex)
            {
                feedbacks = null;
            }
            return feedbacks;
        }

        public Feedback GetFeedbackById(int feedbackId)
        {
            Feedback feedback = null;
            try
            {
                feedback = Context.Feedbacks.Find(feedbackId);
            }
            catch (Exception ex)
            {
                feedback = null;
            }
            return feedback;
        }

        public bool AddFeedback(Feedback feedback)
        {
            bool status = false;
            try
            {
                Context.Feedbacks.Add(feedback);
                Context.SaveChanges();
                status = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);
                }
                status = false;
            }
            return status;
        }

        public bool UpdateFeedback(int feedbackId, int rating, string comments)
        {
            bool status = false;
            try
            {
                var feedback = Context.Feedbacks.Find(feedbackId);
                if (feedback != null)
                {
                    feedback.Rating = rating;
                    feedback.Comments = comments;
                    Context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }

        public bool DeleteFeedback(int feedbackId)
        {
            bool status = false;
            try
            {
                var feedback = Context.Feedbacks.Find(feedbackId);
                if (feedback != null)
                {
                    Context.Feedbacks.Remove(feedback);
                    Context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }

        //Notifications Table
        public List<Notification> GetNotifications()
        {
            List<Notification> notifications = new List<Notification>();
            try
            {
                notifications = Context.Notifications.ToList();
            }
            catch (Exception ex)
            {
                notifications = null;
            }
            return notifications;
        }

        public Notification GetNotificationById(int notificationId)
        {
            Notification notification = null;
            try
            {
                notification = Context.Notifications.Find(notificationId);
            }
            catch (Exception ex)
            {
                notification = null;
            }
            return notification;
        }

        public bool AddNotification(Notification notification)
        {
            bool status = false;
            try
            {
                Context.Notifications.Add(notification);
                Context.SaveChanges();
                status = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);
                }
                status = false;
            }
            return status;
        }

        public bool MarkNotificationAsRead(int notificationId)
        {
            bool status = false;
            try
            {
                var notification = Context.Notifications.Find(notificationId);
                if (notification != null)
                {
                    notification.IsRead = true;
                    Context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }

        public bool DeleteNotification(int notificationId)
        {
            bool status = false;
            try
            {
                var notification = Context.Notifications.Find(notificationId);
                if (notification != null)
                {
                    Context.Notifications.Remove(notification);
                    Context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }

    }
}
