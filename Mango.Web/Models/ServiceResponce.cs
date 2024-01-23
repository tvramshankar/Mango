using System;
namespace Mango.Web.Models
{
	public class ServiceResponce<T>
	{
		public T? Data { get; set; }
		public string Message { get; set; } = string.Empty;
		public bool IsSuccess { get; set; } = true;
	}
}

