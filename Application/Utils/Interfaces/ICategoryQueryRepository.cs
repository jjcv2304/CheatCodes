using System.Collections.Generic;
using Domain;
using Dtos;

namespace Application.Utils.Interfaces
{
  public interface ICategoryQueryRepository
  {
    IList<Category> GetByPartialName(string categoryName);

    IList<Category> GetByExactName(string categoryName);

    IList<Category> GetAllParents();

    IList<Category> GetAllChilds(int categoryParentId);

    IList<Category> GetSiblingsOf(int categoryChildId);

    Category GetById(int id);

    IList<CategoryTreeDto> ExportToJson();
    CategoryTreeDto ParentsCategoryTreeDtos(int rootCategoryId);
  }
}