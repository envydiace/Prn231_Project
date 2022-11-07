using Prm231_Project.Models;
using AutoMapper;

namespace Prm231_Project.DTO
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProductAddDTO, Product>();
            CreateMap<ProductEditDTO, Product>();

            CreateMap<Category, CategoryDTO>();

            CreateMap<Order, OrderDTO>()
                .ForMember(second => second.OrderDetails,
                map => map.MapFrom(
                    first => first.OrderDetails
                    ));

            CreateMap<Order, OrderAdminDTO>()
                .ForMember(second => second.EmployeeName,
                map => map.MapFrom(
                    first => first.Employee.LastName
                    ))
                .ForMember(second => second.CustomerName,
                map => map.MapFrom(
                    first => first.Customer.ContactName
                    ));

            CreateMap<OrderDetail, CartItemDTO>()
                .ForMember(second => second.ProductID,
                map => map.MapFrom(
                    first => first.ProductId
                    ))
                .ForMember(second => second.ProductName,
                map => map.MapFrom(
                    first => first.Product.ProductName
                    ));

            CreateMap<Product, ProductDTO>()
                .ForMember(second => second.CategoryName,
                map => map.MapFrom(
                    first => first.Category != null ? first.Category.CategoryName : null
                    ));

            CreateMap<Customer, CustomerDTO>()
                .ForMember(second => second.Email,
                map => map.MapFrom(
                    first => first.Accounts != null ? first.Accounts.FirstOrDefault().Email : null
                    ));


        }
    }
}
