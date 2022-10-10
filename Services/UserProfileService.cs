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
    }
}
