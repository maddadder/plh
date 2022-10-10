using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Lib;
using Microsoft.AspNetCore.Components.Authorization;

namespace plhhoa.Services
{
    public class GameEntryService : BaseService
    {
        private readonly HttpClient _httpClient;
        private readonly AppSecrets _appSecrets;
        private readonly swaggerClient client;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private List<UserToken> _tokenCache;
        public GameEntryService(
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
        public async Task<IEnumerable<GameEntry>> List(string search, int limit, int skip)
        {
            try
            {
                return await client.GameEntryListAsync(search, limit, skip);
            }
            catch
            {
                return new List<GameEntry>();
            }
        }
        public async Task<GameEntry> Get(string id)
        {
            Guid g = new Guid(id);
            return await client.GameEntryGetByIdAsync(g);
        }
        public async Task Put(GameEntry gameEntry)
        {
            await AddAuthorizationHeader();
            GameEntryUpdateRequestCommand cmd = new GameEntryUpdateRequestCommand();
            cmd.Description = gameEntry.Description;
            cmd.Name = gameEntry.Name;
            cmd.Options = gameEntry.Options;
            cmd.Pid = gameEntry.Pid;
            await client.GameEntryUpdateAsync(cmd.Pid.ToString(), cmd);
        }
        
        public async Task Post(GameEntry gameEntry)
        {
            await AddAuthorizationHeader();
            GameEntryCreateRequestCommand cmd = new GameEntryCreateRequestCommand();
            cmd.Description = gameEntry.Description;
            cmd.Name = gameEntry.Name;
            cmd.Options = gameEntry.Options;
            await client.GameEntryPostAsync(cmd);
        }
        public async Task Delete(Guid Pid)
        {
            await AddAuthorizationHeader();
            await client.GameEntryDeleteAsync(Pid);
        }
    }
}
