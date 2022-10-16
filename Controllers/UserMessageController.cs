using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using plhhoa;
using plhhoa.Services;
using Lib;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class UserMessageController : ControllerBase
{
    private readonly UserMessageService UserMessageService;
    public UserMessageController(UserMessageService userMessageService)
    {
        this.UserMessageService = userMessageService;
    }

    

    [HttpPost]
    public async Task<IActionResult> Post([FromForm] UserMessage request)
    {
        if (!string.IsNullOrEmpty(request.To) && !string.IsNullOrEmpty(request.From) && !string.IsNullOrEmpty(request.Body) && !string.IsNullOrEmpty(request.ApiVersion))
        {
            await UserMessageService.PostTwilio(request);
            return Ok();
        }
        else 
        {
            return UnprocessableEntity();  
        }
    }
}