using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Lib;
using Microsoft.AspNetCore.Components.Authorization;

namespace plhhoa.Services
{
    public class UserProfileService : BaseService
    {
        private readonly AppSecrets _appSecrets;
        private readonly HttpClient _httpClient;
        private readonly swaggerClient client;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private List<UserToken> _tokenCache;
        
        public UserProfileService(
            List<UserToken> tokenCache,
            HttpClient httpClient, 
            AppSecrets appSecrets,
            AuthenticationStateProvider authenticationStateProvider
            ) : base(tokenCache, httpClient, appSecrets, authenticationStateProvider)
        {
            _tokenCache = tokenCache;
            _authenticationStateProvider = authenticationStateProvider;
            _httpClient = httpClient;
            _appSecrets = appSecrets;
            client = new swaggerClient("https://plhhoa-couchclient.leenet.link",_httpClient);
        }
        
        public async Task<IEnumerable<UserProfile>> GetUserProfileAsync(string search)
        {
            
            try
            {
                if(await AddAuthorizationHeader())
                    return await client.UserProfileListAsync(search, null, null);
                return new List<UserProfile>();
            }
            catch
            {
                return new List<UserProfile>();
            }
        }
        public async Task<UserProfile> Get(string id)
        {
            Guid g = new Guid(id);
            await AddAuthorizationHeader();
            return await client.UserProfileGetByIdAsync(g);
        }
        public async Task Put(UserProfile userMessage)
        {
            await AddAuthorizationHeader();
            UserProfileUpdateRequestCommand cmd = new UserProfileUpdateRequestCommand();
            cmd.FirstName = userMessage.FirstName;
            cmd.LastName = userMessage.LastName;
            cmd.Email = userMessage.Email;
            cmd.Password = userMessage.Password;
            cmd.Pid = userMessage.Pid;
            await client.UserProfileUpdateAsync(cmd.Pid.ToString(), cmd);
        }
        
        public async Task Post(UserProfile userMessage)
        {
            await AddAuthorizationHeader();
            UserProfileCreateRequestCommand cmd = new UserProfileCreateRequestCommand();
            cmd.FirstName = userMessage.FirstName;
            cmd.LastName = userMessage.LastName;
            cmd.Email = userMessage.Email;
            cmd.Password = userMessage.Password;
            await client.UserProfilePostAsync(cmd);
        }
        public async Task Delete(Guid Pid)
        {
            await AddAuthorizationHeader();
            await client.UserProfileDeleteAsync(Pid);
        }
    }
}
