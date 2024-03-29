using System;
using Mango.Web.Models;
using Newtonsoft.Json;
using Mango.Web.Service.IService;
using System.Net.Http;

namespace Mango.Web.Service
{
	public class BaseService : IBaseService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ITokenProvider _tokenProvider;
        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
		{
			_httpClientFactory = httpClientFactory;
			_tokenProvider = tokenProvider;
		}
		public async Task<ServiceResponce<object>> SendAsync(ServiceRequest serviceRequest, bool withBearer = true)
		{
			HttpClient client = _httpClientFactory.CreateClient("MangoAPI");
			HttpRequestMessage message = new HttpRequestMessage();
			if(serviceRequest.ContentType == Utility.StaticDetails.ContentType.MultipartFormData)
			{
				message.Headers.Add("Accept", "*/*");
			}
			else
			{
                message.Headers.Add("Accept", "application/json");
            }
            //token
            if (withBearer)
			{
				var token = _tokenProvider.GetToken();
				message.Headers.Add("Authorization", $"Bearer {token}");
			}
			message.RequestUri = new Uri(serviceRequest.Url);

			if(serviceRequest.ContentType == Utility.StaticDetails.ContentType.MultipartFormData)
			{
				var content = new MultipartFormDataContent();
				foreach(var prop in serviceRequest.Data!.GetType().GetProperties())
				{
					var value = prop.GetValue(serviceRequest.Data);
					if(value is FormFile)
					{
						var file = (FormFile)value;
						if(file != null)
						{
							content.Add(new StreamContent(file.OpenReadStream()), prop.Name, file.FileName);
						}
					}
					else
					{
                        content.Add(new StringContent(value == null ? "" : value.ToString()!),prop.Name);
                    }
				}
				message.Content = content;
			}
			else
			{
                if (serviceRequest.Data is not null)
                {
                    message.Content = new StringContent(JsonConvert
                        .SerializeObject(serviceRequest.Data), System.Text.Encoding.UTF8, "application/json");
                }
            }


			HttpResponseMessage? httpResponseMessage = null;
			switch(serviceRequest.ApiType)
			{
				case Utility.StaticDetails.ApiType.POST:
					message.Method = HttpMethod.Post;
					break;
                case Utility.StaticDetails.ApiType.DELETE:
                    message.Method = HttpMethod.Delete;
					break;
                case Utility.StaticDetails.ApiType.PUT:
                    message.Method = HttpMethod.Post;
					break;
                default:
                    message.Method = HttpMethod.Get;
					break;
            }
			httpResponseMessage = await client.SendAsync(message);
			try
			{
				switch(httpResponseMessage.StatusCode)
				{
					case System.Net.HttpStatusCode.NotFound:
						return new() { IsSuccess = false, Message = "Not found" };
					case System.Net.HttpStatusCode.Forbidden:
						return new() { IsSuccess = false, Message = "Access Denied" };
					case System.Net.HttpStatusCode.Unauthorized:
						return new() { IsSuccess = false, Message = "Unauthorized" };
					case System.Net.HttpStatusCode.InternalServerError:
						return new() { IsSuccess = false, Message = "Internal Server Error" };
					default:
						var apiContent = await httpResponseMessage.Content.ReadAsStringAsync();
						var apiResponce = JsonConvert.DeserializeObject<ServiceResponce<object>>(apiContent);
						return apiResponce!;
				}
            }
			catch(Exception ex)
			{
				return new() { IsSuccess = false, Message = ex.Message };
			}

        }

    }
}

