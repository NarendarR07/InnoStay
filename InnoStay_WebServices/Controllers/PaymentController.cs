using InnoStay_DAL;
using InnoStay_DAL.DTO;
using InnoStay_DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace InnoStay_WebServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        public IInnoStayRepository repo { get; set; }

        
        public PaymentController(IInnoStayRepository repository)
        {
            repo = repository;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("GetPayments")]
        public IActionResult GetPayments()
        {
            try
            {
                var payments = repo.GetPayments();
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [Authorize(Roles ="User,Admin")]
        [HttpGet("GetPaymentById/{paymentId:int}")]
        public IActionResult GetPaymentById(int paymentId)
        {
            var payment = repo.GetPaymentById(paymentId);
            if (payment == null)
                return NotFound("Payment not found");
            return Ok(payment);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("GetPaymentsByUserId/{userId:int}")]
        public IActionResult GetPaymentsByUserId(int userId)
        {
            if (userId <= 0)
                return BadRequest("Invalid user ID");

            try
            {
                var payments = repo.GetPaymentsByUserId(userId);
                if (payments == null || payments.Count == 0)
                    return NotFound("No payments found for this user");

                var paymentDTOs = payments.Select(p => new GetPaymentDTO
                {
                    PaymentId = p.PaymentId,
                    BookingId = p.BookingId,
                    Amount = p.Amouont, 
                    PaymentMethod = p.PaymentMethod,
                    PaymentStatus = p.PaymentStatus
                }).ToList();

                return Ok(paymentDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles = "User")]
        [HttpPost("AddPayment")]
        public IActionResult AddPayment([FromBody] Payment payment)
        {
            try
            {
                var result = repo.AddPayment(payment);
                if (result)
                    return Ok("Payment added successfully");
                else
                    return BadRequest("Error adding payment");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPut("UpdatePaymentMethod")]
        public IActionResult UpdatePaymentMethod([FromBody] PaymentUpdateDTO dto)
        {
            if (dto.PaymentId <= 0 || string.IsNullOrEmpty(dto.PaymentMethod))
                return BadRequest("Invalid input.");

            var success = repo.UpdatePaymentMethod(dto.PaymentId, dto.PaymentMethod);
            if (success)
                return Ok(true);
            return StatusCode(500, "Failed to update payment method.");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdatePaymentStatus")]
        public IActionResult UpdatePaymentStatus(int paymentId, string paymentStatus)
        {
            try
            {
                var result = repo.UpdatePaymentStatus(paymentId, paymentStatus);
                if (result)
                    return Ok("Payment status updated successfully");
                else
                    return BadRequest("Error updating payment status");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeletePayment")]
        public IActionResult DeletePayment(int paymentId)
        {
            try
            {
                var result = repo.DeletePayment(paymentId);
                if (result)
                    return Ok("Payment deleted successfully");
                else
                    return BadRequest("Error deleting payment");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
