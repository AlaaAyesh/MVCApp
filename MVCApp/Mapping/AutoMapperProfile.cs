using AutoMapper;
using MVCApp.Models;
using MVCApp.ViewModels;

namespace MVCApp.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Product mappings
            CreateMap<Product, ProductViewModel>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.StockQuantity));

            CreateMap<ProductViewModel, Product>()
                .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.Stock));

            // Category mappings
            CreateMap<Category, CategoryViewModel>();
            CreateMap<CategoryViewModel, Category>();

            // Order mappings
            CreateMap<Order, OrderViewModel>()
                .ForMember(dest => dest.ShippingFullName, opt => opt.MapFrom(src => $"{src.ShippingFirstName} {src.ShippingLastName}"))
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));

            CreateMap<OrderViewModel, Order>();

            // OrderItem mappings
            CreateMap<OrderItem, OrderItemViewModel>();
            CreateMap<OrderItemViewModel, OrderItem>();

            // Cart mappings
            CreateMap<Product, CartItemViewModel>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.imageUrl, opt => opt.MapFrom(src => src.imageUrl))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.SalePrice ?? src.Price))
                .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.StockQuantity));
        }
    }
} 