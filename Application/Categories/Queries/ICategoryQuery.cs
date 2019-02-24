using System.Collections.Generic;
using Application.Categories.Queries.ViewModels;
using NUnit.Framework;

namespace Application.Categories.Queries
{
    public interface ICategoryQuery
    {
        CategoryVM ById(long categoryId);
        List<CategoryVM> ByPartialName(string categoryName);
        List<CategoryVM> ByExactName(string categoryName);
        List<CategoryVM> All();
        List<CategoryVM> AllParents();
        List<CategoryVM> GetAllChilds(int categoryParentId);
        List<CategoryVM> GetSiblingsOf(int categoryChildId);
    }
}