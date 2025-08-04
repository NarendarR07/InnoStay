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
    public class PaymentTest
    {
        private readonly Mock<IInnoStayRepository> mockRepo;
        private PaymentController payment;

        public PaymentTest()
        {
            mockRepo = new Mock<IInnoStayRepository>();
            payment = new PaymentController(mockRepo.Object);
        }
        [Fact]
        public void GetPayments_Success()
        {
            var payments = new List<Payment> { new Payment { PaymentId = 1 } };
            mockRepo.Setup(r => r.GetPayments()).Returns(payments);

            var result = payment.GetPayments();
            var response = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<List<Payment>>(response.Value);
            Assert.Single(list);
        }

        [Fact]
        public void GetPayments_Exception()
        {
            mockRepo.Setup(r => r.GetPayments()).Throws(new Exception());

            var result = payment.GetPayments();
            var response = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, response.StatusCode);
        }

        [Fact]
        public void GetPaymentById_Success()
        {
            var pay = new Payment { PaymentId = 1 };
            mockRepo.Setup(r => r.GetPaymentById(1)).Returns(pay);

            var result = payment.GetPaymentById(1);
            var response = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<Payment>(response.Value);
            Assert.Equal(1, returned.PaymentId);
        }

        [Fact]
        public void GetPaymentById_NotFound()
        {
            mockRepo.Setup(r => r.GetPaymentById(1)).Returns((Payment)null);

            var result = payment.GetPaymentById(1);
            var response = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Payment not found", response.Value);
        }

        [Fact]
        public void GetPaymentsByUserId_Success()
        {
            var payments = new List<Payment> { new Payment { PaymentId = 1, BookingId = 10, Amouont = 100 } };
            mockRepo.Setup(r => r.GetPaymentsByUserId(1)).Returns(payments);

            var result = payment.GetPaymentsByUserId(1);
            var response = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<List<GetPaymentDTO>>(response.Value);
            Assert.Single(list);
        }

        [Fact]
        public void GetPaymentsByUserId_BadRequest()
        {
            var result = payment.GetPaymentsByUserId(0);
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid user ID", response.Value);
        }

        [Fact]
        public void GetPaymentsByUserId_NotFound()
        {
            mockRepo.Setup(r => r.GetPaymentsByUserId(1)).Returns(new List<Payment>());

            var result = payment.GetPaymentsByUserId(1);
            var response = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No payments found for this user", response.Value);
        }

        [Fact]
        public void AddPayment_Success()
        {
            var pay = new Payment();
            mockRepo.Setup(r => r.AddPayment(pay)).Returns(true);

            var result = payment.AddPayment(pay);
            var response = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Payment added successfully", response.Value);
        }

        [Fact]
        public void AddPayment_BadRequest()
        {
            var pay = new Payment();
            mockRepo.Setup(r => r.AddPayment(pay)).Returns(false);

            var result = payment.AddPayment(pay);
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error adding payment", response.Value);
        }

        [Fact]
        public void UpdatePaymentMethod_Success()
        {
            var dto = new PaymentUpdateDTO { PaymentId = 1, PaymentMethod = "Card" };
            mockRepo.Setup(r => r.UpdatePaymentMethod(1, "Card")).Returns(true);

            var result = payment.UpdatePaymentMethod(dto);
            var response = Assert.IsType<OkObjectResult>(result);
            Assert.True((bool)response.Value);
        }

        [Fact]
        public void UpdatePaymentMethod_BadRequest()
        {
            var dto = new PaymentUpdateDTO { PaymentId = 0, PaymentMethod = "" };

            var result = payment.UpdatePaymentMethod(dto);
            var response = Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void UpdatePaymentMethod_Failure()
        {
            var dto = new PaymentUpdateDTO { PaymentId = 1, PaymentMethod = "Card" };
            mockRepo.Setup(r => r.UpdatePaymentMethod(1, "Card")).Returns(false);

            var result = payment.UpdatePaymentMethod(dto);
            var response = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, response.StatusCode);
        }

        [Fact]
        public void UpdatePaymentStatus_Success()
        {
            mockRepo.Setup(r => r.UpdatePaymentStatus(1, "Completed")).Returns(true);

            var result = payment.UpdatePaymentStatus(1, "Completed");
            var response = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Payment status updated successfully", response.Value);
        }

        [Fact]
        public void UpdatePaymentStatus_BadRequest()
        {
            mockRepo.Setup(r => r.UpdatePaymentStatus(1, "Completed")).Returns(false);

            var result = payment.UpdatePaymentStatus(1, "Completed");
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error updating payment status", response.Value);
        }

        [Fact]
        public void DeletePayment_Success()
        {
            mockRepo.Setup(r => r.DeletePayment(1)).Returns(true);

            var result = payment.DeletePayment(1);
            var response = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Payment deleted successfully", response.Value);
        }

        [Fact]
        public void DeletePayment_BadRequest()
        {
            mockRepo.Setup(r => r.DeletePayment(1)).Returns(false);

            var result = payment.DeletePayment(1);
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error deleting payment", response.Value);
        }
    }
}
