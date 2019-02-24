using System.Collections.Generic;
using System.Linq;
using Application.Categories.Queries.ViewModels;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore.Extensions.Internal;
using NUnit.Framework;

namespace Application.Categories.Queries
{
    public class CategoryQuery : ICategoryQuery
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryQuery(IUnitOfWork unitOfWork)
        {
            _categoryRepository = unitOfWork.CategoryRepository;
        }
        
        public List<CategoryVM> ByExactName(string categoryName)
        {
            var category = _categoryRepository.GetByExactName(categoryName);

            var categoryVM = MapService.Map(category);

            return categoryVM;
        }
        
        public List<CategoryVM> ByPartialName(string categoryName)
        {
            var category = _categoryRepository.GetByPartialName(categoryName);

            var categoryVM = MapService.Map(category);

            return categoryVM;
        }
        
        public CategoryVM ById(long categoryId)
        {
            var category = _categoryRepository.Find(categoryId);

            var categoryVM = MapService.Map(category);

            return categoryVM;
        }

        public List<CategoryVM> All()
        {
            var categories = _categoryRepository.All();

            var categoriesVM = MapService.Map(categories);

            return categoriesVM;
        }
        public List<CategoryVM> AllParents()
        {
            var categories = _categoryRepository.GetAllParents();

            var categoriesVM = MapService.Map(categories);

            return categoriesVM;
        }

        public List<CategoryVM> GetAllChilds(int categoryParentId)
        {
            var categories = _categoryRepository.GetAllChilds(categoryParentId);

            var categoriesVM = MapService.Map(categories);

            return categoriesVM;
        }
        public List<CategoryVM> GetSiblingsOf(int categoryChildId)
        {
            var categories = _categoryRepository.GetSiblingsOf(categoryChildId);

            var categoriesVM = MapService.Map(categories);

            return categoriesVM;
        }
        
    }
}