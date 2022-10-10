using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using plhhoa.Services;
using Lib;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class UserLinkFileController : ControllerBase
{
    private readonly UserLinkService UserLinkService;
    public UserLinkFileController(UserLinkService userLinkService)
    {
        this.UserLinkService = userLinkService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<string>> Index()
    {
        return Ok(new List<string>(){"file1.txt","file2.txt"});
    }

    [HttpGet("{userLinkId}")]
    public async Task<ActionResult> Get(string userLinkId)
    {
        if (!string.IsNullOrEmpty(userLinkId))
        {
            // allow user to enter in a file extension to help identify the file type
            if(userLinkId.EndsWith(".jpeg"))
                userLinkId = userLinkId.Replace(".jpeg","");
            var row = await UserLinkService.Get(userLinkId);
            var link = new UserLinkOverride(){
                __T = row.__T,
                Category = row.Category,
                Content = row.Content,
                Created = row.Created,
                Href = row.Href,
                ImgContent = row.ImgContent,
                ImgHref = row.ImgHref,
                Modified = row.Modified,
                Pid = row.Pid,
                Target = row.Target,
            };
            if(link.ImgContent != null)
                return new FileContentResult(link.ImgContent, "image/jpg;base64");
            return NotFound();
        }
        return NotFound();
    }
} 