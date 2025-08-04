using InnoStay_DAL;
using InnoStay_DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InnoStay_WebServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        public IInnoStayRepository repo { get; set; }
        public RoomController(IInnoStayRepository repository)
        {
            repo = repository;
        }
        [Authorize(Roles ="User,Admin")]
        [HttpGet]
        [Route("GetRooms")]
        public IActionResult GetRooms()
        {
            List<InnoStay_DAL.Models.Room> rooms = new List<InnoStay_DAL.Models.Room>();
            try
            {
                rooms = repo.GetRooms();
                return Ok(rooms); 
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // Get room by ID
        [Authorize(Roles = "User,Admin")]
        [HttpGet("GetRoomById")]
        public IActionResult GetRoomById(int roomId)
        {
            if (roomId <= 0)
                return BadRequest("Room ID should be greater than 0");

            try
            {
                var room = repo.GetRoomById(roomId);
                if (room != null)
                    return Ok(room);
                else
                    return NotFound("Room Not Found");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        //Add room
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("AddRoom")]
        public IActionResult AddRoom([FromBody] InnoStay_DAL.Models.Room room)
        {
            bool result = false;
            try
            {
                result = repo.AddRoom(room);
                if (result)
                {
                    return Ok(new { message = "Room added successfully" }); // Success response
                }
                else
                {
                    return BadRequest("Error adding room"); // Error response
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); // Internal Server Error
            }
        }

        // Update room details
        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("UpdateRoom")]
        public IActionResult UpdateRoom([FromBody] InnoStay_DAL.Models.Room room)
        {
            bool result = false;
            try
            {
                result = repo.UpdateRoom(room);
                if (result)
                {
                    return Ok("Room updated successfully"); // Success response
                }
                else
                {
                    return BadRequest("Error updating room"); // Error response
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); // Internal Server Error
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateRoomPrice")]
        public IActionResult UpdateRoomPrice(int roomId, decimal newPrice)
        {
            if (roomId <= 0)
                return BadRequest("Invalid room ID");

            bool ok = repo.UpdateRoomPrice(roomId, newPrice);
            if (ok)
            {
                
                var updatedRoom = repo.GetRoomById(roomId);
                return Ok(updatedRoom);
            }
            return BadRequest("Error updating room price");
        }


        // Delete room by ID
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("DeleteRoom")]
        public IActionResult DeleteRoom(int roomId)
        {
            bool result = false;
            try
            {
                result = repo.DeleteRoom(roomId);
                if (result)
                {
                    return Ok("Room deleted successfully");
                }
                else
                {
                    return BadRequest("Error deleting room"); 
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
