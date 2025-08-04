using InnoStay_DAL;
using InnoStay_DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InnoStay_WebServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        public IInnoStayRepository repo{ get; set; }
        public NotificationController(IInnoStayRepository repository)
        {
            repo = repository;
        }
        [Authorize(Roles = "Admin,User  ")]
        [HttpGet]
        [Route("GetNotifications")]
        public IActionResult GetNotifications()
        {
            List<Notification> result = new List<Notification>();
            try
            {
                result = repo.GetNotifications();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // Get notification by ID
        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        [Route("GetNotificationById")]
        public IActionResult GetNotificationById(int notificationId)
        {
            Notification result = new Notification();
            try
            {
                if (notificationId > 0)
                {
                    result = repo.GetNotificationById(notificationId);
                    if (result != null)
                    {
                        return Ok(result);
                    }
                    else
                    {
                        return NotFound("Notification not found");
                    }
                }
                else
                {
                    return BadRequest("Invalid notification ID");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        [Route("GetNotificationsByUser")]
        public IActionResult GetNotificationsByUser(int userId)
        {
            try
            {
                var all = repo.GetNotifications();
                var filtered = all?.Where(n => n.UserId == userId).ToList();
                return Ok(filtered);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // Add a new notification
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("AddNotification")]
        public IActionResult AddNotification(Notification notification)
        {
            try
            {
                bool result = repo.AddNotification(notification);
                if (result)
                {
                    return CreatedAtAction(nameof(GetNotificationById), new { notificationId = notification.NotificationId }, notification);
                }
                else
                {
                    return BadRequest("Error adding notification");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // Mark notification as read
        [Authorize(Roles = "User,Admin")]
        [HttpPut]
        [Route("MarkAsRead")]
        public IActionResult MarkNotificationAsRead(int notificationId)
        {
            try
            {
                bool result = repo.MarkNotificationAsRead(notificationId);
                if (result)
                {
                    return Ok("Notification marked as read successfully");
                }
                else
                {
                    return BadRequest("Error updating notification");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // Delete notification
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("DeleteNotification")]
        public IActionResult DeleteNotification(int notificationId)
        {
            try
            {
                bool result = repo.DeleteNotification(notificationId);
                if (result)
                {
                    return Ok("Notification deleted successfully");
                }
                else
                {
                    return BadRequest("Error deleting notification");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
