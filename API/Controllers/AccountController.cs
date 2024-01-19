using API.Dtos;
using API.Errors;
using API.Extensions;
using AutoMapper;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers;

public class AccountController : BaseApiController
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IMapper _mapper;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
    }

    [HttpGet("emailexists")]
    public async Task<ActionResult<bool>> CheckEmailExistsAsync([FromQuery] string email)
    {
        return await _userManager.FindByEmailAsync(email) != null;
    }

    [HttpGet("address")]
    public async Task<ActionResult<AddressDto>> GetUserAddress(string email)
    {
        var user = await _userManager.FindUserByEmailWithAddressAsync(email);

        return _mapper.Map<Address, AddressDto>(user.Address);
    }

    [HttpPut("address")]
    public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto address, string email)
    {
        var user = await _userManager.FindUserByEmailWithAddressAsync(email);

        user.Address = _mapper.Map<AddressDto, Address>(address);

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded) return Ok(_mapper.Map<Address, AddressDto>(user.Address));

        return BadRequest("Problem updating the user");
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user == null) return Unauthorized(new ApiResponse(401));

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

        if (!result.Succeeded) return Unauthorized(new ApiResponse(401));

        return new UserDto
        {
            Email = user.Email,
            DisplayName = user.DisplayName
        };
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        var user = new AppUser
        {
            DisplayName = registerDto.DisplayName,
            Email = registerDto.Email,
            UserName = registerDto.Email
        };

        if (CheckEmailExistsAsync(registerDto.Email).Result.Value)
        {
            return new BadRequestObjectResult(
                new ApiValidationErrorResponse
                {
                    Errors = new[] { "Email address is in use" }
                });
        }

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded) return BadRequest(new ApiResponse(400));

        return new UserDto
        {
            DisplayName = user.DisplayName,
            Email = user.Email
        };
    }
}
