using System;
using Mango.Web.Service.IService;
using NuGet.Common;

namespace Mango.Web.Service
{
	public class TokenProvider : ITokenProvider
	{
        private readonly IHttpContextAccessor _httpContextAccessor;
		public TokenProvider(IHttpContextAccessor httpContextAccessor)
		{
            _httpContextAccessor = httpContextAccessor;
		}

        public void ClearToken()
        {
            _httpContextAccessor.HttpContext!.Response.Cookies.Delete(Utility.StaticDetails.TokenCookie);
        }

        public string? GetToken()
        {
            string? token = null;
            bool? hasToken = _httpContextAccessor.HttpContext?.Request.Cookies.TryGetValue(Utility.StaticDetails.TokenCookie,
                out token);
            return hasToken is true ? token : null;
        }

        public void SetToken(string token)
        {
            _httpContextAccessor.HttpContext?.Response.Cookies.Append(Utility.StaticDetails.TokenCookie, token);
        }
    }
}

