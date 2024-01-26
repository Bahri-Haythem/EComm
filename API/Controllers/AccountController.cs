using API.Dtos;
using API.Errors;
using API.Extensions;
using AutoMapper;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class AccountController : ControllerBase
{
    public static void AddRoutes(WebApplication app)
    {
        app.MapGet("/api/Account/emailexists", async (
            [FromServices] UserManager<AppUser> _userManager,
            string email) =>
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }).WithTags("Account");

        app.MapGet("/api/Account/address", async (
            [FromServices] UserManager<AppUser> _userManager,
            [FromServices] IMapper _mapper,
            string email) =>
        {
            var user = await _userManager.FindUserByEmailWithAddressAsync(email);
            if (user == null)
                return Results.Empty;
            return Results.Ok(_mapper.Map<Address, AddressDto>(user.Address));
        }).WithTags("Account");

        app.MapPut("/api/Account/address", async (
            [FromServices] UserManager<AppUser> _userManager,
            [FromServices] IMapper _mapper,
            [FromBody] AddressDto address,
            string email) =>
        {
            var user = await _userManager.FindUserByEmailWithAddressAsync(email);

            user.Address = _mapper.Map<AddressDto, Address>(address);

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded) return Results.Ok(_mapper.Map<Address, AddressDto>(user.Address));

            return Results.BadRequest("Problem updating the user");
        }).WithTags("Account");

        app.MapPost("/api/Account/login", async (
            [FromServices] UserManager<AppUser> _userManager,
            [FromServices] IMapper _mapper,
            [FromServices] SignInManager<AppUser> _signInManager,
            [FromBody] LoginDto loginDto) =>
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null) return Results.Unauthorized();

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded) return Results.Unauthorized(); //new ApiResponse(401)

            return Results.Ok(new UserDto
            {
                Email = user.Email,
                DisplayName = user.DisplayName
            });
        }).WithTags("Account");

        app.MapPost("/api/Account/register", async (
            [FromServices] UserManager<AppUser> _userManager,
            [FromServices] IMapper _mapper,
            [FromServices] SignInManager<AppUser> _signInManager,
            [FromBody] RegisterDto registerDto) =>
        {
            var user = new AppUser
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.Email
            };

            var emailExists = _userManager.FindByEmailAsync(registerDto.Email) != null;

            if (emailExists)
            {
                return Results.BadRequest(
                    new ApiValidationErrorResponse
                    {
                        Errors = new[] { "Email address is in use" }
                    });
            }

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded) return Results.BadRequest(new ApiResponse(400));

            return Results.Ok(new UserDto
            {
                DisplayName = user.DisplayName,
                Email = user.Email
            });
        }).WithTags("Account");
    }
}
