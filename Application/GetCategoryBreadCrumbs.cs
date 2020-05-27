using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Utils;
using Application.Utils.Interfaces;
using Dtos;

namespace Application
{
  public sealed class GetCategoryBreadCrumbs : IQuery<CategoryBreadCrumbsDto>
  {
    public int RootCategoryId { get; }

    public GetCategoryBreadCrumbs(int rootCategoryId)
    {
      RootCategoryId = rootCategoryId;
    }

    public sealed class GetCategoryBreadCrumbsQueryHandler : IQueryHandler<GetCategoryBreadCrumbs, CategoryBreadCrumbsDto>
    {
      private readonly ICategoryQueryRepository _categoryQueryRepository;

      public GetCategoryBreadCrumbsQueryHandler(ICategoryQueryRepository categoryQueryRepository)
      {
        _categoryQueryRepository = categoryQueryRepository;
      }

      public CategoryBreadCrumbsDto Handle(GetCategoryBreadCrumbs query)
      {
        var categoryRoot = _categoryQueryRepository.ParentsCategoryTreeDtos(query.RootCategoryId);
        CategoryBreadCrumbsDto categoryBreadCrumbsDto = MapService.Map(categoryRoot);
        return categoryBreadCrumbsDto;
      }
    }
  }
}
