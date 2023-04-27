//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PostmarkDotNet.Model;
using RESTFulSense.Controllers;
using Tarteeb.Api.Brokers.Storages;
using Tarteeb.Api.Models.DTO;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Services.Processings.Users;
using Tarteeb.Api.UtilityService;
using Tarteeb.Api.UtilityService.Helpers;

namespace Tarteeb.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UsersController : RESTFulController
    {
        private readonly IUserProcessingService userProcessingService;
        private readonly StorageBroker storageBroker;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public UsersController(IUserProcessingService userProcessingService, StorageBroker storageBroker, IConfiguration configuration, IEmailService emailService)
        {
            this.userProcessingService = userProcessingService;
            this.storageBroker = storageBroker;
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpGet("{userId}")]
        public async ValueTask<ActionResult<Guid>> VerifyUserByIdAsync(Guid userId)
        {
            Guid verifiedId = await this.userProcessingService.VerifyUserByIdAsync(userId);

            return Ok(verifiedId);
        }

        [HttpPost]
        public async ValueTask<ActionResult<Guid>> ActivateUserByIdAsync([FromBody] Guid userId)
        {
            Guid activatedId = await this.userProcessingService.ActivateUserByIdAsync(userId);

            return Ok(activatedId);
        }

        [HttpPost("send-reset-email/{email}")]
        public async Task<IActionResult> SendEmail(string email)
        {
            var user = await storageBroker.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
            {
                return NotFound(new
                {
                    StatusCode=404,
                    Message="Email Doesn't Exist"
                });
            }
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var emailToken= Convert.ToBase64String(tokenBytes);
            user.ResetPasswordToken = emailToken;
            user.ResetPasswordExpiry=DateTimeOffset.UtcNow.AddMinutes(15);
            string from = _configuration["EmailSettings:Form"];
            var emailModel = new EmailModel(email,"Reset Password!!",EmailBody.EmailStringBody(email,emailToken));
            _emailService.SendEmail(emailModel);
            storageBroker.Entry(user).State=EntityState.Modified;
            await storageBroker.SaveChangesAsync();
            return Ok(new
            {
                StatusCode = 200,
                Message="Email Sent!"
            });
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var newToken = resetPasswordDto.EmailToken.Replace("", "+");
            var user= await storageBroker.Users.AsNoTracking().FirstOrDefaultAsync(a=>a.Email == resetPasswordDto.Email);

            if (user == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Email Doesn't Exist"
                });
            }

            var tokenCode = user.ResetPasswordToken;
            DateTimeOffset emailTokenExpiry = user.ResetPasswordExpiry;
            if(tokenCode != resetPasswordDto.EmailToken|| emailTokenExpiry < DateTimeOffset.Now)
            {
                return BadRequest(new
                {
                    StatusCode =400,
                    Message="Invalid Reset Link"
                });
            }
            var passwordHasher = new PasswordHasher();
            user.Password = passwordHasher.HashPassword(resetPasswordDto.NewPassword);
            storageBroker.Entry(user).State= EntityState.Modified;
            await storageBroker.SaveChangesAsync();
            return Ok(new
            {
                StatusCode = 200,
                Message = "Password Reset Successfull!!"
            });
        }
    }
}
