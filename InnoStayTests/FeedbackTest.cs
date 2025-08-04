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
    public class FeedbackTest
    {
        private readonly Mock<IInnoStayRepository> mockRepo;
        private FeedbackController feedback;

        public FeedbackTest()
        {
            mockRepo = new Mock<IInnoStayRepository>();
            feedback = new FeedbackController(mockRepo.Object);
        }
        [Fact]
        public void GetFeedbacks_Success()
        {
            var list = new List<Feedback> { new Feedback { FeedBackId = 1 } };
            mockRepo.Setup(r => r.GetFeedbacks()).Returns(list);

            var result = feedback.GetFeedbacks();
            var response = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsAssignableFrom<List<Feedback>>(response.Value);
            Assert.Single(data);
        }

        [Fact]
        public void GetFeedbacks_Exception()
        {
            mockRepo.Setup(r => r.GetFeedbacks()).Throws(new Exception());

            var result = feedback.GetFeedbacks();
            var response = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, response.StatusCode);
        }

        [Fact]
        public void GetFeedbackById_Success()
        {
            var fb = new Feedback { FeedBackId = 1 };
            mockRepo.Setup(r => r.GetFeedbackById(1)).Returns(fb);

            var result = feedback.GetFeedbackById(1);
            var response = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<Feedback>(response.Value);
            Assert.Equal(1, returned.FeedBackId);
        }

        [Fact]
        public void GetFeedbackById_BadRequest()
        {
            var result = feedback.GetFeedbackById(0);
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Feedback ID should be greater than 0", response.Value);
        }

        [Fact]
        public void GetFeedbackById_NotFound()
        {
            mockRepo.Setup(r => r.GetFeedbackById(1)).Returns((Feedback)null);

            var result = feedback.GetFeedbackById(1);
            var response = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Feedback Not Found", response.Value);
        }

        [Fact]
        public void AddFeedback_Success()
        {
            var fb = new Feedback();
            mockRepo.Setup(r => r.AddFeedback(fb)).Returns(true);

            var result = feedback.AddFeedback(fb);
            var response = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Thanks for your Feeback!", response.Value);
        }

        [Fact]
        public void AddFeedback_BadRequest()
        {
            var fb = new Feedback();
            mockRepo.Setup(r => r.AddFeedback(fb)).Returns(false);

            var result = feedback.AddFeedback(fb);
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error adding feedback", response.Value);
        }

        [Fact]
        public void UpdateFeedback_Success()
        {
            mockRepo.Setup(r => r.UpdateFeedback(1, 5, "Good")).Returns(true);

            var result = feedback.UpdateFeedback(1, 5, "Good");
            var response = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Feedback updated successfully", response.Value);
        }

        [Fact]
        public void UpdateFeedback_BadRequest()
        {
            mockRepo.Setup(r => r.UpdateFeedback(1, 5, "Good")).Returns(false);

            var result = feedback.UpdateFeedback(1, 5, "Good");
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error updating feedback", response.Value);
        }

        [Fact]
        public void DeleteFeedback_Success()
        {
            mockRepo.Setup(r => r.DeleteFeedback(1)).Returns(true);

            var result = feedback.DeleteFeedback(1);
            var response = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Feedback deleted successfully", response.Value);
        }

        [Fact]
        public void DeleteFeedback_BadRequest()
        {
            mockRepo.Setup(r => r.DeleteFeedback(1)).Returns(false);

            var result = feedback.DeleteFeedback(1);
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error deleting feedback", response.Value);
        }
    }
}
