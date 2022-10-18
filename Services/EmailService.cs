using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Lib;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace plhhoa.Services
{
    public class EmailService : BaseService
    {
        private readonly HttpClient _httpClient;
        private readonly AppSecrets _appSecrets;
        private readonly swaggerClient client;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private List<UserToken> _tokenCache;
        private readonly UserProfileService UserProfileService;
        public EmailService(
            UserProfileService userProfileService,
            List<UserToken> tokenCache,
            HttpClient httpClient, 
            AppSecrets appSecrets,
            AuthenticationStateProvider authenticationStateProvider
            ) : base(tokenCache, httpClient, appSecrets, authenticationStateProvider)
        {
            UserProfileService = userProfileService;
            _tokenCache = tokenCache;
            _authenticationStateProvider = authenticationStateProvider;
            _httpClient = httpClient;
            _appSecrets = appSecrets;
            client = new swaggerClient("https://plhhoa-couchclient.leenet.link",_httpClient);
        }
        public async Task SendEmailVerification(string To, string PreferredUsername)
        {
            string token = GenerateToken(PreferredUsername);
            string body = $"If you requested this verification, please go to the following URL to confirm that you are authorized to use this email address: {_appSecrets.ApplicationUrl}/codeconfirm?jwt={token}";

            await SendEmail(To, "Email Verification", body);
        }
        public async Task SendEmail(string To, string Subject, string Body)
        {
            int port = 25;
            using (var client = new System.Net.Mail.SmtpClient(_appSecrets.SmtpHost, port))
            {
                client.Credentials = new System.Net.NetworkCredential(_appSecrets.SmtpUserName, _appSecrets.SmtpPassword);
                client.EnableSsl = true;
                var mailMessage = new System.Net.Mail.MailMessage(_appSecrets.SmtpFromEmail, To);
                mailMessage.Subject = Subject;
                mailMessage.Body = Body;
                await client.SendMailAsync(mailMessage);
            }
        }
        public async Task SendEmailToSubscribers(string TwilioFrom, string TwilioBody)
        {
            
            //then send notifications
            var userProfiles = await UserProfileService.GetUserProfilesViaServiceAccount();
            int port = 25;
            using (var client = new System.Net.Mail.SmtpClient(_appSecrets.SmtpHost, port))
            {
                client.Credentials = new System.Net.NetworkCredential(_appSecrets.SmtpUserName, _appSecrets.SmtpPassword);
                client.EnableSsl = true;
                foreach(var userProfile in userProfiles.Where(x => x.ReceiveEmailNotificationFromSms && x.EmailIsVerified))
                {
                    string token = GenerateToken(userProfile.PreferredUsername);
                    var subject = $"A user has requested to Contact Us from {_appSecrets.ApplicationUrl}";
                    string body = 
$@"Message From: {TwilioFrom}
Message Contents: {TwilioBody}

You received the above message because you have 'Receive Email Notifications' turned on. 


To unsubscribe from these messages click here: 
{_appSecrets.ApplicationUrl}/unsubscribe?jwt={token}";
                    var mailMessage = new System.Net.Mail.MailMessage(_appSecrets.SmtpFromEmail, userProfile.Email);
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    await client.SendMailAsync(mailMessage);
                    
                }
            }	
        }
        public string GenerateToken(string PreferredUsername)
        {
            var mySecret = _appSecrets.UserProfilePassword;
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

            var myIssuer = _appSecrets.ApplicationUrl;
            var myAudience = _appSecrets.ApplicationUrl;

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, PreferredUsername),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                //Issuer = myIssuer, //set if multi-tenant
                //Audience = myAudience, //set if multi-tenant
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var result = tokenHandler.WriteToken(token);
            return result;
        }
        public SecurityToken ValidateCurrentToken(string token)
        {
            var mySecret = _appSecrets.UserProfilePassword;
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

            var myIssuer = _appSecrets.ApplicationUrl;
            var myAudience = _appSecrets.ApplicationUrl;

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false, //set to true if multi-tenant
                    ValidateAudience = false, //set to true if multi-tenant
                    //ValidIssuer = myIssuer, //set if multi-tenant
                    //ValidAudience = myAudience, //set if multi-tenant
                    IssuerSigningKey = mySecurityKey
                }, out SecurityToken validatedToken);
                return validatedToken;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            
        }
        public string GetUserIdFromClaim(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            var stringClaimValue = securityToken.Claims.First(claim => claim.Type == "nameid").Value;
            return stringClaimValue;
        }
    }
}
