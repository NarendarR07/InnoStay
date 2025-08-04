using InnoStay_DAL;
using InnoStay_DAL.DTO;
using InnoStay_DAL.Models;
using InnoStay_WebServices.Controllers;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace InnoStayTests
{
    public class UserControllerTests
    {
        private readonly Mock<IInnoStayRepository> mockRepo;
        private readonly Mock<IConfiguration> mockConfig;
        private readonly UserController user;

        public UserControllerTests()
        {
            mockRepo = new Mock<IInnoStayRepository>();
            mockConfig = new Mock<IConfiguration>();
            user = new UserController(mockRepo.Object, mockConfig.Object);
        }

        [Fact]
        public void GetUserDetailsById_Success()
        {
            mockRepo.Setup(r => r.GetUserById(1)).Returns(new User { UserId = 1 });

            var result = user.GetUserDetailsById(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            var u = Assert.IsType<User>(ok.Value);
            Assert.Equal(1, u.UserId);
        }

        [Fact]
        public void GetUserDetailsById_BadRequest()
        {
            mockRepo.Setup(r => r.GetUserById(1)).Returns((User)null);

            var result = user.GetUserDetailsById(1);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User Not Found", bad.Value);
        }

        [Fact]
        public void GetUserDetailsById_NotFound()
        {
            var result = user.GetUserDetailsById(-1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User Id Should be greater than 0", notFound.Value);
        }

        [Fact]
        public void GetUserDetailsById_Exception()
        {
            mockRepo.Setup(r => r.GetUserById(1)).Throws(new Exception());

            var result = user.GetUserDetailsById(1);

            var ex = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, ex.StatusCode);
        }

        [Fact]
        public void AddCustomer_Success()
        {
            var newUser = new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "pass",
                Role = "Customer"
            };

            mockRepo.Setup(r => r.AddUser(newUser)).Returns(true);

            var result = user.AddCustomer(newUser);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Customer added", ok.Value);
        }

        [Fact]
        public void AddCustomer_BadRequest()
        {
            var incomplete = new User(); 

            var result = user.AddCustomer(incomplete);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("All fields are required.", bad.Value);
        }

        [Fact]
        public void AddCustomer_ServerError()
        {
            var newUser = new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "pass",
                Role = "Customer"
            };

            mockRepo.Setup(r => r.AddUser(newUser)).Returns(false);

            var result = user.AddCustomer(newUser);

            var error = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, error.StatusCode);
        }
        [Fact]
        public void ValidateUserCredentials_InvalidCredentials()
        {
            var login = new LoginRequest { Email = "wrong@example.com", Password = "badpass" };
            mockRepo.Setup(r => r.ValidateCredentials(login.Email, login.Password)).Returns(string.Empty);

            var result = user.ValidateUserCredentials(login);

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);

            var json = JsonSerializer.Serialize(unauthorized.Value);
            using var doc = JsonDocument.Parse(json);
            var error = doc.RootElement.GetProperty("error").GetString();

            Assert.Equal("Invalid credentials", error);
        }



        [Fact]
        public void ValidateUserCredentials_Exception()
        {
            var login = new LoginRequest { Email = "test@example.com", Password = "pass" };
            mockRepo.Setup(r => r.ValidateCredentials(login.Email, login.Password)).Throws(new Exception());

            var result = user.ValidateUserCredentials(login);

            var ex = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, ex.StatusCode);
        }
        public void UpdateUserDetails_Success()
        {
            var u = new User();
            mockRepo.Setup(r => r.UpdateUser(u)).Returns(true);

            var result = user.UpdateUserDetails(u);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.True((bool)ok.Value);
        }

        [Fact]
        public void UpdateUserDetails_Exception()
        {
            var u = new User();
            mockRepo.Setup(r => r.UpdateUser(u)).Throws(new Exception());

            var result = user.UpdateUserDetails(u);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.False((bool)ok.Value);
        }

        [Fact]
        public void SignUp_Success()
        {
            var signup = new SignUpDTO
            {
                FirstName = "Mark",
                LastName = "Smith",
                Email = "mark@example.com",
                Password = "pass"
            };

            mockRepo.Setup(r => r.AddUser(It.IsAny<User>())).Returns(true);

            var result = user.SignUp(signup);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User registered successfully", ok.Value);
        }

        [Fact]
        public void SignUp_BadRequest()
        {
            var signup = new SignUpDTO
            {
                FirstName = "",
                LastName = "Smith",
                Email = "mark@example.com",
                Password = "pass"
            };

            var result = user.SignUp(signup);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("All fields are required.", bad.Value);
        }

        [Fact]
        public void SignUp_ServerError()
        {
            var signup = new SignUpDTO
            {
                FirstName = "Mark",
                LastName = "Smith",
                Email = "mark@example.com",
                Password = "pass"
            };

            mockRepo.Setup(r => r.AddUser(It.IsAny<User>())).Returns(false);

            var result = user.SignUp(signup);

            var error = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, error.StatusCode);
        }

        [Fact]
        public void DeleteUser_Success()
        {
            mockRepo.Setup(r => r.DeleteUser(1)).Returns(true);

            var result = user.DeleteUser(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.True((bool)ok.Value);
        }

        [Fact]
        public void DeleteUser_Exception()
        {
            mockRepo.Setup(r => r.DeleteUser(1)).Throws(new Exception());

            var result = user.DeleteUser(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.False((bool)ok.Value);
        }

        [Fact]
        public void GetAllUsers_Success()
        {
            var users = new List<User> { new User { UserId = 1 } };
            mockRepo.Setup(r => r.GetUsers()).Returns(users);

            var result = user.GetAllUsers();

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<List<User>>(ok.Value);
            Assert.Single(list);
        }

        [Fact]
        public void GetAllUsers_Exception()
        {
            mockRepo.Setup(r => r.GetUsers()).Throws(new Exception());

            var result = user.GetAllUsers();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Null(ok.Value);
        }
    }
}
