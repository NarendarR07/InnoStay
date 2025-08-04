using InnoStay_DAL;
using InnoStay_DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InnoStay_WebServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        public IInnoStayRepository repo { get; set; }
        public FeedbackController(IInnoStayRepository repository)
        {
            repo = repository;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("GetFeedbacks")]
        public IActionResult GetFeedbacks()
        {
            List<Feedback> feedbacks = new List<Feedback>();
            try
            {
                feedbacks = repo.GetFeedbacks();
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // Get feedback by ID
        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        [Route("GetFeedbackById")]
        public IActionResult GetFeedbackById(int feedbackId)
        {
            Feedback feedback = new Feedback();
            try
            {
                if (feedbackId > 0)
                {
                    feedback = repo.GetFeedbackById(feedbackId);
                    if (feedback != null)
                    {
                        return Ok(feedback);
                    }
                    else
                    {
                        return NotFound("Feedback Not Found");
                    }
                }
                else
                {
                    return BadRequest("Feedback ID should be greater than 0");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // Add feedback
        [Authorize(Roles = "User")]
        [HttpPost]
        [Route("AddFeedback")]
        public IActionResult AddFeedback([FromBody] Feedback feedback)
        {
            bool result = false;
            try
            {
                result = repo.AddFeedback(feedback);
                if (result)
                {
                    return Ok("Thanks for your Feeback!");
                }
                else
                {
                    return BadRequest("Error adding feedback");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // Update feedback (rating and comments)
        [Authorize(Roles = "User")]
        [HttpPut]
        [Route("UpdateFeedback")]
        public IActionResult UpdateFeedback(int feedbackId, int rating, string comments)
        {
            bool result = false;
            try
            {
                result = repo.UpdateFeedback(feedbackId, rating, comments);
                if (result)
                {
                    return Ok("Feedback updated successfully");
                }
                else
                {
                    return BadRequest("Error updating feedback");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // Delete feedback
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("DeleteFeedback")]
        public IActionResult DeleteFeedback(int feedbackId)
        {
            bool result = false;
            try
            {
                result = repo.DeleteFeedback(feedbackId);
                if (result)
                {
                    return Ok("Feedback deleted successfully");
                }
                else
                {
                    return BadRequest("Error deleting feedback");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
