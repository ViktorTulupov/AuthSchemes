using AuthorizationService.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AuthorizationService.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<Settings> _settings;

        public AuthenticationService(HttpClient httpClient, IOptions<Settings> settings)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        //private Task<string> ExecuteRequest(HttpRequestMessage requestMessage)
        //{
        //    var response = _httpClient.SendAsync(requestMessage).Result;
        //    if (response.IsSuccessStatusCode)
        //    {
        //        var result = response.Content.ReadAsStringAsync().Result;
        //        return Task.FromResult(result);
        //    }

        //    return Task.FromResult(string.Empty);
        //}

        public async Task<User> Authentication(AuthenticationOptions options, AuthenticationParameters parameters)
        {
            string requestUri = $"{_settings.Value.AuthenticationServiceUrl}{options.MethodName}";
            var requestContent = new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(requestUri, requestContent).ConfigureAwait(false);
            var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonConvert.DeserializeObject<User>(jsonResponse);
        }

        //public Task<string> GetUserDataByToken(AuthenticationOptions options, string login, string tokenString)
        //{
        //    string requestUri = $"{_settings.Value.AuthenticationServiceUrl}{options.MethodName}?user={login}";
        //    var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
        //    requestMessage.Headers.Add("token", tokenString);
        //    return ExecuteRequest(requestMessage);
        //}
    }
}
