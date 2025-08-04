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
    public class RoomTest
    {
        private readonly Mock<IInnoStayRepository> mockRepo;
        private RoomController room;

        public RoomTest()
        {
            mockRepo = new Mock<IInnoStayRepository>();
            room = new RoomController(mockRepo.Object);
        }
        [Fact]
        public void GetRooms_Success()
        {
            var rooms = new List<Room> { new Room { RoomId = 1 } };
            mockRepo.Setup(r => r.GetRooms()).Returns(rooms);

            var result = room.GetRooms();
            var response = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<List<Room>>(response.Value);
            Assert.Single(list);
        }

        [Fact]
        public void GetRooms_Exception()
        {
            mockRepo.Setup(r => r.GetRooms()).Throws(new Exception());

            var result = room.GetRooms();
            var response = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, response.StatusCode);
        }

        [Fact]
        public void GetRoomById_Success()
        {
            var r = new Room { RoomId = 1 };
            mockRepo.Setup(x => x.GetRoomById(1)).Returns(r);

            var result = room.GetRoomById(1);
            var response = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<Room>(response.Value);
            Assert.Equal(1, returned.RoomId);
        }

        [Fact]
        public void GetRoomById_BadRequest()
        {
            var result = room.GetRoomById(0);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Room ID should be greater than 0", badRequest.Value);
        }

        [Fact]
        public void GetRoomById_NotFound()
        {
            mockRepo.Setup(x => x.GetRoomById(1)).Returns((Room)null);

            var result = room.GetRoomById(1);
            var response = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Room Not Found", response.Value);
        }

        [Fact]
        public void AddRoom_Success()
        {
            var r = new Room();
            mockRepo.Setup(x => x.AddRoom(r)).Returns(true);

            var result = room.AddRoom(r);
            var response = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void AddRoom_BadRequest()
        {
            var r = new Room();
            mockRepo.Setup(x => x.AddRoom(r)).Returns(false);

            var result = room.AddRoom(r);
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error adding room", response.Value);
        }

        [Fact]
        public void UpdateRoom_Success()
        {
            var r = new Room();
            mockRepo.Setup(x => x.UpdateRoom(r)).Returns(true);

            var result = room.UpdateRoom(r);
            var response = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Room updated successfully", response.Value);
        }

        [Fact]
        public void UpdateRoom_BadRequest()
        {
            var r = new Room();
            mockRepo.Setup(x => x.UpdateRoom(r)).Returns(false);

            var result = room.UpdateRoom(r);
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error updating room", response.Value);
        }

        [Fact]
        public void UpdateRoomPrice_Success()
        {
            var r = new Room { RoomId = 1, PricePerNight = 200 };
            mockRepo.Setup(x => x.UpdateRoomPrice(1, 200)).Returns(true);
            mockRepo.Setup(x => x.GetRoomById(1)).Returns(r);

            var result = room.UpdateRoomPrice(1, 200);
            var response = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<Room>(response.Value);
            Assert.Equal(1, returned.RoomId);
        }

        [Fact]
        public void UpdateRoomPrice_BadRequest_Id()
        {
            var result = room.UpdateRoomPrice(0, 100);
            var response = Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void UpdateRoomPrice_BadRequest_Fail()
        {
            mockRepo.Setup(x => x.UpdateRoomPrice(1, 200)).Returns(false);

            var result = room.UpdateRoomPrice(1, 200);
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error updating room price", response.Value);
        }

        [Fact]
        public void DeleteRoom_Success()
        {
            mockRepo.Setup(x => x.DeleteRoom(1)).Returns(true);

            var result = room.DeleteRoom(1);
            var response = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Room deleted successfully", response.Value);
        }

        [Fact]
        public void DeleteRoom_BadRequest()
        {
            mockRepo.Setup(x => x.DeleteRoom(1)).Returns(false);

            var result = room.DeleteRoom(1);
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error deleting room", response.Value);
        }
    }
}
