using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Lib;
using Microsoft.AspNetCore.Components.Authorization;

namespace plhhoa.Services
{
    public class UserMessageService : BaseService
    {
        private readonly HttpClient _httpClient;
        private readonly AppSecrets _appSecrets;
        private readonly swaggerClient client;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private List<UserToken> _tokenCache;
        public UserMessageService(
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
        public async Task<IEnumerable<UserMessage>> List(string search)
        {
            try
            {
                if(await AddAuthorizationHeader())
                    return await client.UserMessageListAsync(search, null, null);
                return new List<UserMessage>();
            }
            catch
            {
                return new List<UserMessage>();
            }
        }
        public async Task<UserMessage> Get(string id)
        {
            Guid g = new Guid(id);
            await AddAuthorizationHeader();
            return await client.UserMessageGetByIdAsync(g);
        }
        public async Task Put(UserMessage userMessage)
        {
            await AddAuthorizationHeader();
            UserMessageUpdateRequestCommand cmd = new UserMessageUpdateRequestCommand();
            cmd.Body = userMessage.Body;
            cmd.To = userMessage.To;
            cmd.From = userMessage.From;
            cmd.ApiVersion = userMessage.ApiVersion;
            cmd.Pid = userMessage.Pid;
            await client.UserMessageUpdateAsync(cmd.Pid.ToString(), cmd);
        }
        
        public async Task Post(UserMessage userMessage)
        {
            await AddAuthorizationHeader();
            UserMessageCreateRequestCommand cmd = new UserMessageCreateRequestCommand();
            cmd.Body = userMessage.Body;
            cmd.To = userMessage.To;
            cmd.From = userMessage.From;
            cmd.ApiVersion = userMessage.ApiVersion;
            await client.UserMesssagePostAsync(cmd);
        }
        public async Task Delete(Guid Pid)
        {
            await AddAuthorizationHeader();
            await client.UserMessageDeleteAsync(Pid);
        }
    }
}
