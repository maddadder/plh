using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using plhhoa;
using plhhoa.Services;
using Lib;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Twilio.Security;
using Microsoft.AspNetCore.Http;

[ApiController]
[Route("[controller]")]
public class UserMessageController : ControllerBase
{
    private readonly UserMessageService UserMessageService;
    private readonly UserProfileService UserProfileService;
    private readonly AppSecrets _appSecrets;
    private readonly EmailService EmailService;
    public UserMessageController(
        EmailService emailService,
        UserProfileService userProfileService,
        UserMessageService userMessageService, 
        AppSecrets appSecrets)
    {
        this.EmailService = emailService;
        this.UserProfileService = userProfileService;
        this.UserMessageService = userMessageService;
        _appSecrets = appSecrets;
    }

    

    [HttpPost]
    public async Task<IActionResult> Post([FromForm] UserMessage request)
    {
        if (
            !string.IsNullOrEmpty(request.To) && 
            !string.IsNullOrEmpty(request.From) && 
            !string.IsNullOrEmpty(request.Body) && 
            !string.IsNullOrEmpty(request.ApiVersion) && 
            Request.Headers.ContainsKey("x-twilio-signature") && 
            IsValidRequest(Request)
        )
        {
            //first save to database
            await UserMessageService.PostTwilio(request);
            await EmailService.SendEmailToSubscribers(request.From, request.Body);
            return Ok();
        }
        else 
        {
            return UnprocessableEntity();  
        }
    }
    private bool IsValidRequest(HttpRequest request) 
    {
        var _requestValidator = new RequestValidator(_appSecrets.TwilioAuthToken);
        var requestUrl = RequestRawUrl(request);
        var parameters = ToDictionary(request.Form);
        var signature = request.Headers["x-twilio-signature"];
        return _requestValidator.Validate(requestUrl, parameters, signature);
    }

    private string RequestRawUrl(HttpRequest request)
    {
        var rawUrl = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
        return rawUrl;
    }

    private IDictionary<string, string> ToDictionary(IFormCollection collection)
    {
        return collection.Keys
            .Select(key => new { Key = key, Value = collection[key] })
            .ToDictionary(p => p.Key, p => p.Value.ToString());
    }
}