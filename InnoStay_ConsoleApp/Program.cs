using InnoStay_DAL;
using InnoStay_DAL.Models;

namespace InnoStay_ConsoleApp
{
    public class Program
    {
        public static InnoStayRepository Repository { get; set; }
        static Program()
        {
            Repository = new InnoStayRepository();
        }
        static void Main(string[] args)
        {

            //GetAllUsers();
            //GetUserById();
            //AddUser();
            //UpdateUser();
            //DeleteUser();


            //GetAllRooms();
            //GetRoomById();
            //AddRoom();
            //UpdateRoom();
            //DeleteRoom();


            //GetAllBookings();
            //GetBookingById();
            //AddBooking();
            //UpdateBookingStatus();
            //DeleteBooking();


            //GetAllPayments();
            //GetPaymentById();
            //AddPayment();
            //UpdatePaymentStatus();
            //DeletePayment();

            //GetAllNotifications();
            //GetNotificationById();
            //AddNotification();
            //MarkNotificationAsRead();
            //DeleteNotification();
        }
        public static void GetAllUsers()
        {
            List<User> users = Repository.GetUsers();
            if (users != null && users.Count > 0)
            {
                Console.WriteLine("User List:");
                foreach (var user in users)
                {
                    Console.WriteLine($"ID: {user.UserId}, Name: {user.FirstName} {user.LastName}, Email: {user.Email}");
                }
            }
            else
            {
                Console.WriteLine("No users found or an error occurred.");
            }
        }

        public static void GetUserById()
        {
            int userId = 1; // Assume a valid user ID
            User user = Repository.GetUserById(userId);
            if (user != null)
            {
                Console.WriteLine($"User Found: ID: {user.UserId}, Name: {user.FirstName} {user.LastName}, Email: {user.Email}");
            }
            else
            {
                Console.WriteLine("User not found or an error occurred.");
            }
        }

        public static void AddUser()
        {
            User newUser = new User
            {
                FirstName = "Alice",
                LastName = "Wonderland",
                Email = "alice@example.com",
                Password = "secure123",
                Role = "Admin"
            };
            bool isAdded = Repository.AddUser(newUser);
            if (isAdded)
            {
                Console.WriteLine("User added successfully.");
            }
            else
            {
                Console.WriteLine("Failed to add the user.");
            }
        }

        public static void UpdateUser()
        {
            User updatedUser = new User
            {
                UserId = 1,
                FirstName = "Alice",
                LastName = "W.",
                Email = "alice.new@example.com",
                Password = "newpass456",
                Role = "Admin"
            };
            bool result = Repository.UpdateUser(updatedUser);
            if (result)
            {
                Console.WriteLine("User updated successfully.");
            }
            else
            {
                Console.WriteLine("Failed to update user.");
            }
        }

        public static void DeleteUser()
        {
            int userId = 0; 
            bool result = Repository.DeleteUser(userId);
            if (result)
            {
                Console.WriteLine("User deleted successfully.");
            }
            else
            {
                Console.WriteLine("User could not be deleted (either does not exist or an error occurred).");
            }
        }
        public static void GetAllRooms()
        {
            List<Room> rooms = Repository.GetRooms();
            if (rooms != null && rooms.Count > 0)
            {
                Console.WriteLine("Room List:");
                foreach (var room in rooms)
                {
                    Console.WriteLine($"Room ID: {room.RoomId}, Room No: {room.RoomNumber}, Type: {room.RoomType}, Price: {room.PricePerNight}, Capacity: {room.Capacity}, Available: {room.IsAvailable}");
                }
            }
            else
            {
                Console.WriteLine("No rooms found or an error occurred.");
            }
        }

        public static void GetRoomById()
        {
            int roomId = 0; 
            Room room = Repository.GetRoomById(roomId);
            if (room != null)
            {
                Console.WriteLine($"Room Found: ID: {room.RoomId}, Number: {room.RoomNumber}, Type: {room.RoomType}");
            }
            else
            {
                Console.WriteLine("Room not found or an error occurred.");
            }
        }

        public static void AddRoom()
        {
            Room newRoom = new Room
            {
                RoomNumber = 101,
                RoomType = "Suite",
                PricePerNight = 150.0m,
                Capacity = 2,
                IsAvailable = true
            };

            bool isAdded = Repository.AddRoom(newRoom);
            if (isAdded)
            {
                Console.WriteLine("Room added successfully.");
            }
            else
            {
                Console.WriteLine("Failed to add the room.");
            }
        }

        public static void UpdateRoom()
        {
            Room updatedRoom = new Room
            {
                RoomId = 1, 
                RoomNumber = 401,
                RoomType = "Executive",
                PricePerNight = 350.0m,
                Capacity = 4,
                IsAvailable = false
            };
            bool result = Repository.UpdateRoom(updatedRoom);
            if (result)
            {
                Console.WriteLine("Room updated successfully.");
            }
            else
            {
                Console.WriteLine("Failed to update room.");
            }
        }

        public static void DeleteRoom()
        {
            int roomId = 0; 
            bool result = Repository.DeleteRoom(roomId);
            if (result)
            {
                Console.WriteLine("Room deleted successfully.");
            }
            else
            {
                Console.WriteLine("Room could not be deleted (either does not exist or an error occurred).");
            }
        }
        //public static void GetAllBookings()
        //{
        //    List<Booking> bookings = Repository.GetBookings();
        //    if (bookings != null && bookings.Count > 0)
        //    {
        //        Console.WriteLine("Booking List:");
        //        foreach (var booking in bookings)
        //        {
        //            Console.WriteLine($"Booking ID: {booking.BookingId}, User ID: {booking.UserId}, Room ID: {booking.RoomId}, Status: {booking.BookingStatus}");
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine("No bookings found or an error occurred.");
        //    }
        //}

        public static void GetBookingById()
        {
            int bookingId = 0; 
            Booking booking = Repository.GetBookingById(bookingId);
            if (booking != null)
            {
                Console.WriteLine($"Booking Found: ID: {booking.BookingId}, User ID: {booking.UserId}, Status: {booking.BookingStatus}");
            }
            else
            {
                Console.WriteLine("Booking not found or an error occurred.");
            }
        }

        public static void AddBooking()
        {
            Booking newBooking = new Booking
            {
                UserId = 0,
                RoomId = 0,
                CheckInDate = DateTime.MaxValue,
                CheckOutDate = DateTime.MaxValue,
                BookingStatus = "Confirmed"
            };
            bool isAdded = Repository.AddBooking(newBooking);
            if (isAdded)
            {
                Console.WriteLine("Booking added successfully.");
            }
            else
            {
                Console.WriteLine("Failed to add the booking.");
            }
        }

        public static void UpdateBookingStatus()
        {
            int bookingId = 1;
            string newStatus = "Cancelled";
            bool result = Repository.UpdateBookingStatus(bookingId, newStatus);
            if (result)
            {
                Console.WriteLine("Booking status updated successfully.");
            }
            else
            {
                Console.WriteLine("Failed to update booking status.");
            }
        }

        public static void DeleteBooking()
        {
            int bookingId = 0; 
            bool result = Repository.DeleteBooking(bookingId);
            if (result)
            {
                Console.WriteLine("Booking deleted successfully.");
            }
            else
            {
                Console.WriteLine("Booking could not be deleted (either does not exist or an error occurred).");
            }
        }
        public static void GetAllPayments()
        {
            List<Payment> payments = Repository.GetPayments();
            if (payments != null && payments.Count > 0)
            {
                Console.WriteLine("Payment List:");
                foreach (var payment in payments)
                {
                    Console.WriteLine($"Payment ID: {payment.PaymentId}, Booking ID: {payment.BookingId}, Amount: {payment.Amouont}, Status: {payment.PaymentStatus}");
                }
            }
            else
            {
                Console.WriteLine("No payments found or an error occurred.");
            }
        }

        public static void GetPaymentById()
        {
            int paymentId = 1; 
            Payment payment = Repository.GetPaymentById(paymentId);
            if (payment != null)
            {
                Console.WriteLine($"Payment Found: ID: {payment.PaymentId}, Booking ID: {payment.BookingId}, Amount: {payment.Amouont}, Status: {payment.PaymentStatus}");
            }
            else
            {
                Console.WriteLine("Payment not found or an error occurred.");
            }
        }

        public static void AddPayment()
        {
            Payment newPayment = new Payment
            {
                BookingId = 0,
                Amouont = 600.00m,
                PaymentMethod = "Cash",
                PaymentStatus = "Completed"
            };
            bool isAdded = Repository.AddPayment(newPayment);
            if (isAdded)
            {
                Console.WriteLine("Payment added successfully.");
            }
            else
            {
                Console.WriteLine("Failed to add the payment.");
            }
        }

        public static void UpdatePaymentStatus()
        {
            int paymentId = 1;
            string newStatus = "Failed";
            bool result = Repository.UpdatePaymentStatus(paymentId, newStatus);
            if (result)
            {
                Console.WriteLine("Payment status updated successfully.");
            }
            else
            {
                Console.WriteLine("Failed to update payment status.");
            }
        }

        public static void DeletePayment()
        {
            int paymentId = 0;
            bool result = Repository.DeletePayment(paymentId);
            if (result)
            {
                Console.WriteLine("Payment deleted successfully.");
            }
            else
            {
                Console.WriteLine("Payment could not be deleted (either does not exist or an error occurred).");
            }
        }
        public static void GetAllNotifications()
        {
            List<Notification> notifications = Repository.GetNotifications();
            if (notifications != null && notifications.Count > 0)
            {
                Console.WriteLine("Notification List:");
                foreach (var notification in notifications)
                {
                    Console.WriteLine($"ID: {notification.NotificationId}, User ID: {notification.UserId}, Message: {notification.Message}, IsRead: {notification.IsRead}, SentAt: {notification.SendAt}");
                }
            }
            else
            {
                Console.WriteLine("No notifications found or an error occurred.");
            }
        }

        public static void GetNotificationById()
        {
            int notificationId = 1; 
            Notification notification = Repository.GetNotificationById(notificationId);
            if (notification != null)
            {
                Console.WriteLine($"Notification Found: ID: {notification.NotificationId}, User ID: {notification.UserId}, Message: {notification.Message}, IsRead: {notification.IsRead}, SentAt: {notification.SendAt}");
            }
            else
            {
                Console.WriteLine("Notification not found or an error occurred.");
            }
        }

        public static void AddNotification()
        {
            Notification newNotification = new Notification
            {
                UserId = 1, 
                Message = "Your booking is confirmed!",
                IsRead = false,
                SendAt = DateTime.Now
            };

            bool isAdded = Repository.AddNotification(newNotification);
            if (isAdded)
            {
                Console.WriteLine("Notification added successfully.");
            }
            else
            {
                Console.WriteLine("Failed to add the notification.");
            }
        }

        public static void MarkNotificationAsRead()
        {
            int notificationId = 1;
            bool result = Repository.MarkNotificationAsRead(notificationId);
            if (result)
            {
                Console.WriteLine("Notification marked as read successfully.");
            }
            else
            {
                Console.WriteLine("Failed to mark the notification as read.");
            }
        }

        public static void DeleteNotification()
        {
            int notificationId = 1;
            bool result = Repository.DeleteNotification(notificationId);
            if (result)
            {
                Console.WriteLine("Notification deleted successfully.");
            }
            else
            {
                Console.WriteLine("Notification could not be deleted (either does not exist or an error occurred).");
            }
        }


    }
}
