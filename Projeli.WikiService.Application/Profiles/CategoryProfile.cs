using AutoMapper;
using Projeli.WikiService.Application.Dtos;
using Projeli.WikiService.Application.Models.Requests;
using Projeli.WikiService.Application.Models.Responses;
using Projeli.WikiService.Domain.Models;

namespace Projeli.WikiService.Application.Profiles;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryDto>();
        CreateMap<CategoryDto, Category>();

        CreateMap<CategoryDto, SimpleCategoryResponse>();

        CreateMap<CreateCategoryRequest, CategoryDto>();
        CreateMap<UpdateCategoryRequest, CategoryDto>();
    }
}