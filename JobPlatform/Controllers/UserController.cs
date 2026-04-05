using JobPlatformBackend.API.Contracts.User.Shared;
using JobPlatformBackend.API.Contracts.User.Update;
using JobPlatformBackend.Business.src.Services.Abstractions;
using JobPlatformBackend.Contracts.Contracts.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

namespace JobPlatformBackend.API.Controllers
{
	[ApiController]
	[Route("api/v1/Users")]

	public class UserController(IUserService userService,ICloudinaryService cloudinaryService):ControllerBase
	{
		
		private readonly IUserService _userService=userService;
		private readonly ICloudinaryService _cloudinaryService=cloudinaryService;

		[HttpGet]

		public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUser([FromQuery] QueryOptions queryOptions)
		{
			var users = await _userService.GetAllUserAsync(queryOptions);
			return Ok(users);
		}

		[HttpGet("profile")]
		[Authorize]
		public async Task<ActionResult> GetUserProfileAsync()
		{
		 
			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

			if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
			{
				return Unauthorized();
			}

			var user = await _userService.GetUserByIdAsync(userId);
			if (user == null)
				return NotFound();

			return Ok(user);
		}


		[HttpGet("{id}")]
		public async Task<IActionResult>GetUserById(int id)
		{
			var user=await _userService.GetUserByIdAsync(id);
			return Ok(user);
		}

		[HttpGet("Email/{email}")]
		public async Task<ActionResult<UserDto>> GetUserByEmail(string email)
		{
			var user = await _userService.GetUserByEmailAsync(email);
			if (user == null)
			{
				return NotFound("Not found user ");
			}
			return Ok(user);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult<bool>>DeleteUserByIdAsync(int id)
		{
			var user = await _userService.GetUserByIdAsync(id);

			if (user == null)
				return NotFound("not found this user");

			return await _userService.DeleteUserByIdAsync(id);

		}
		[HttpPut("{id}")]
		public async Task<ActionResult> UpdateUser(int id,UpdateUserRequest request)
		{
			var updateUser = await _userService.UpdateUserAsync(id, request);
			return Ok(updateUser);
		}


		[HttpPost("upload-Profile-Picture")]
		[Authorize]
		public async Task<ActionResult> UploadProfilePicture(IFormFile file)
		{
			if (file == null || file.Length == 0)
			{
				return BadRequest("No file uploaded.");
			}
			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
			if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
			{
				return Unauthorized();
			}

			var uploadResult = await _cloudinaryService.UploadImageAsync(file,"UserProfilePhotot");
			if (uploadResult == null ||uploadResult.Error!=null)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "Error uploading image.");
			}
			var imageUrl= uploadResult.SecureUrl.ToString();
			var publicId=uploadResult.PublicId;

			try {

				var success = await _userService.UpdateUserProfilePictureAsync(userId, uploadResult.Url.ToString());
				if (!success) throw new Exception("Database save failed");
				return Ok(new { Message = "Profile picture updated successfully.", ImageUrl = uploadResult.Url });
			}
			catch (Exception)
			{
				// 3. التراجع اليدوي (Rollback) في السحابة لأن الداتا بيس فشلت
				await _cloudinaryService.DeleteImageAsync(publicId);
				return StatusCode(500, "Database error, image upload reverted.");
			}

		}

	}
}
