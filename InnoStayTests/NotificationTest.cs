using InnoStay_DAL;
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
    public class NotificationTest
    {
        private readonly Mock<IInnoStayRepository> mockRepo;
        private NotificationController notification;
        public NotificationTest()
        {
            mockRepo = new Mock<IInnoStayRepository>();
            notification = new NotificationController(mockRepo.Object);
        }
        [Fact]
        public void GetNotifications_Success()
        {
            var list = new List<Notification> { new Notification { NotificationId = 1 } };
            mockRepo.Setup(r => r.GetNotifications()).Returns(list);

            var result = notification.GetNotifications();
            var response = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsAssignableFrom<List<Notification>>(response.Value);
            Assert.Single(data);
        }

        [Fact]
        public void GetNotifications_Exception()
        {
            mockRepo.Setup(r => r.GetNotifications()).Throws(new Exception());

            var result = notification.GetNotifications();
            var response = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, response.StatusCode);
        }

        [Fact]
        public void GetNotificationById_Success()
        {
            var n = new Notification { NotificationId = 1 };
            mockRepo.Setup(r => r.GetNotificationById(1)).Returns(n);

            var result = notification.GetNotificationById(1);
            var response = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<Notification>(response.Value);
            Assert.Equal(1, returned.NotificationId);
        }

        [Fact]
        public void GetNotificationById_BadRequest()
        {
            var result = notification.GetNotificationById(0);
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid notification ID", response.Value);
        }

        [Fact]
        public void GetNotificationById_NotFound()
        {
            mockRepo.Setup(r => r.GetNotificationById(1)).Returns((Notification)null);

            var result = notification.GetNotificationById(1);
            var response = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Notification not found", response.Value);
        }

        [Fact]
        public void GetNotificationsByUser_Success()
        {
            var list = new List<Notification> {
                new Notification { NotificationId = 1, UserId = 2 },
                new Notification { NotificationId = 2, UserId = 2 },
                new Notification { NotificationId = 3, UserId = 3 }
            };
            mockRepo.Setup(r => r.GetNotifications()).Returns(list);

            var result = notification.GetNotificationsByUser(2);
            var response = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsAssignableFrom<List<Notification>>(response.Value);
            Assert.Equal(2, data.Count);
        }

        [Fact]
        public void GetNotificationsByUser_Exception()
        {
            mockRepo.Setup(r => r.GetNotifications()).Throws(new Exception());

            var result = notification.GetNotificationsByUser(2);
            var response = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, response.StatusCode);
        }

        [Fact]
        public void AddNotification_Success()
        {
            var n = new Notification();
            mockRepo.Setup(r => r.AddNotification(n)).Returns(true);

            var result = notification.AddNotification(n);
            var response = Assert.IsType<CreatedAtActionResult>(result);
            var created = Assert.IsType<Notification>(response.Value);
        }

        [Fact]
        public void AddNotification_BadRequest()
        {
            var n = new Notification();
            mockRepo.Setup(r => r.AddNotification(n)).Returns(false);

            var result = notification.AddNotification(n);
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error adding notification", response.Value);
        }

        [Fact]
        public void MarkNotificationAsRead_Success()
        {
            mockRepo.Setup(r => r.MarkNotificationAsRead(1)).Returns(true);

            var result = notification.MarkNotificationAsRead(1);
            var response = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Notification marked as read successfully", response.Value);
        }

        [Fact]
        public void MarkNotificationAsRead_BadRequest()
        {
            mockRepo.Setup(r => r.MarkNotificationAsRead(1)).Returns(false);

            var result = notification.MarkNotificationAsRead(1);
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error updating notification", response.Value);
        }

        [Fact]
        public void DeleteNotification_Success()
        {
            mockRepo.Setup(r => r.DeleteNotification(1)).Returns(true);

            var result = notification.DeleteNotification(1);
            var response = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Notification deleted successfully", response.Value);
        }

        [Fact]
        public void DeleteNotification_BadRequest()
        {
            mockRepo.Setup(r => r.DeleteNotification(1)).Returns(false);

            var result = notification.DeleteNotification(1);
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error deleting notification", response.Value);
        }
    }
}
