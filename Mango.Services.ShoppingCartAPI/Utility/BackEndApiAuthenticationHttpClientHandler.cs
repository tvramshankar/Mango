using System;
using Microsoft.AspNetCore.Authentication;
using static System.Net.WebRequestMethods;

namespace Mango.Services.ShoppingCartAPI.Utility
{
	public class BackEndApiAuthenticationHttpClientHandler : DelegatingHandler
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		public BackEndApiAuthenticationHttpClientHandler(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
		{
			var token = await _httpContextAccessor.HttpContext!.GetTokenAsync("access_token"); //access_token is default from where the token will be defaulty saved in http request

			httpRequestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(httpRequestMessage, cancellationToken);
        }
	}
}

// to add / set the token captured from http request to new http request to product api