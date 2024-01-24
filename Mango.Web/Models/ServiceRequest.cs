using System;
using static Mango.Web.Utility.StaticDetails;
namespace Mango.Web.Models
{
	public class ServiceRequest
	{
		public ApiType ApiType { get; set; } = ApiType.GET;
		public string Url { get; set; } = string.Empty;
		public object? Data { get; set; }
		public string AccessToken { get; set; } = string.Empty;
	}
}

