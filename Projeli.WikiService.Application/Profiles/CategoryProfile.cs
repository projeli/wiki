using AutoMapper;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Application.Profiles;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryDto>();
        CreateMap<CategoryDto, Category>();
    }
}