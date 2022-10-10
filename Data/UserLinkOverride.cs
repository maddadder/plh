using System;
using plhhoa;

namespace Lib
{
    public partial class UserLinkOverride : UserLink
    {
        [Newtonsoft.Json.JsonProperty("WebsiteLink", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.RegularExpression(@"^(?:http(s)?:\/\/)[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$",ErrorMessage ="Please provide a valid URL, e.g. https://amazon.com")]
        public string WebsiteLink 
        { 
            get{
                return this.Href;
            } 
            set{
                this.Href = value;
            }
        }

        [Newtonsoft.Json.JsonProperty("WebsiteImgLink", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.RegularExpression(@"^(?:http(s)?:\/\/)[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$",ErrorMessage ="Please provide a valid URL, e.g. https://amazon.com")]
        public string WebsiteImgLink
        {
            get{
                return this.ImgHref;
            } 
            set{
                this.ImgHref = value;
            }
        }
        
        public string ImgContentBase64 { 
            get
            {
                if(this.ImgContent == null)
                    return null;
                return string.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(this.ImgContent));
            } 
        }
    }
}
