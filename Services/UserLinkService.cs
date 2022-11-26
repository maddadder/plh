using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Lib;
using Microsoft.AspNetCore.Components.Authorization;

namespace plhhoa.Services
{
    public class UserLinkService : BaseService
    {
        private readonly HttpClient _httpClient;
        private readonly AppSecrets _appSecrets;
        private readonly swaggerClient client;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private List<UserToken> _tokenCache;
        public UserLinkService(
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
            client = new swaggerClient(appSecrets.swaggerClient,_httpClient);
        }
        public async Task<IEnumerable<UserLink>> List(string search, int? limit, int? skip)
        {
            return await client.UserLinkListAsync(search, limit, skip);
        }
        public async Task<UserLink> Get(string id)
        {
            Guid g = new Guid(id);
            return await client.UserLinkGetByIdAsync(g);
        }
        public async Task Put(UserLinkOverride link)
        {
            await AddAuthorizationHeader();
            UserLinkUpdateRequestCommand cmd = new UserLinkUpdateRequestCommand();
            cmd.Content = link.Content;
            cmd.Href = link.Href;
            cmd.ImgHref = link.ImgHref;
            cmd.ImgContent = link.ImgContent;
            cmd.Category = link.Category;
            cmd.Pid = link.Pid;
            cmd.Target = link.Target;
            await client.UserLinkUpdateAsync(cmd.Pid.ToString(), cmd);
        }
        public async Task Post(UserLinkOverride link)
        {
            await AddAuthorizationHeader();
            UserLinkCreateRequestCommand cmd = new UserLinkCreateRequestCommand();
            cmd.Content = link.Content;
            cmd.Href = link.Href;
            cmd.ImgHref = link.ImgHref;
            cmd.ImgContent = link.ImgContent;
            cmd.Category = link.Category;
            cmd.Target = link.Target;
            await client.UserLinkPostAsync(cmd);
        }
        public async Task Delete(Guid Pid)
        {
            await AddAuthorizationHeader();
            await client.UserLinkDeleteAsync(Pid);
        }
    }
}
