﻿using LoggingWithSerilog.Models;
using LoggingWithSerilog.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoggingWithSerilog.Controllers;
[Route("api/[controller]")]
[ApiController]
public class MailController : ControllerBase
{
    private readonly IMailService mailService;
    public MailController(IMailService mailService)
    {
        this.mailService = mailService;
    }
    [HttpPost("send")]
    public async Task<IActionResult> SendMail([FromForm] MailRequest request)
    {
        try
        {
            await mailService.SendEmailAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {
            throw;
        }

    }


    [HttpPost("welcome")]
    public async Task<IActionResult> SendWelcomeMail([FromForm] WelcomeRequest request)
    {
        try
        {
            await mailService.SendWelcomeEmailAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {

            throw;
        }
    }
}
