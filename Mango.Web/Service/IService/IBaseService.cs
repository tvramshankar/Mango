using System;
using Mango.Web.Models;
namespace Mango.Web.Service.IService;

	public interface IBaseService
	{
		Task<ServiceResponce<object>> SendAsync(ServiceRequest serviceRequest, bool withBearer = true);
	}


