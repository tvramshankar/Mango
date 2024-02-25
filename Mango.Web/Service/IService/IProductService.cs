using System;
using Mango.Web.Models;
using Mango.Web.Models.DTO;
namespace Mango.Web.Service.IService;

public interface IProductService
{
    Task<ServiceResponce<object>> GetAllProductAsync();
    Task<ServiceResponce<object>> GetProductByIdAsync(Guid Id);
    Task<ServiceResponce<object>> CreateProductAsync(ProductPostDTO product);
    Task<ServiceResponce<object>> UpdateProductAsync(ProductsDTO product);
    Task<ServiceResponce<object>> DeleteProductAsync(Guid Id);
}

