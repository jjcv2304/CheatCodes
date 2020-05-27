using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheatCodes.Search.DB;
using CheatCodes.Search.DB.Models;
using CheatCodes.Search.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CheatCodes.Search.Repositories
{
  public class CategoriesSearchRepository : ICategoriesSearchRepository
  {
    private readonly CheatCodesDbContext _context;

    public CategoriesSearchRepository(CheatCodesDbContext context)
    {
      _context = context;
    }

    public async Task<List<Category>> Test()
    {
      return await _context
        .Categories
        .Include(c => c.CategoryFields).ThenInclude(cf => cf.Field)
        .OrderBy(c => c.Id)
        .Take(10)
        .ToListAsync();
    }

    public async Task<List<CategoryBasicVM>> GetCategoriesByPartialNameAsync(string partialName)
    {
      
      var categories = await _context
        .Categories
        .Where(c => c.Name.ToLower().Contains(partialName.ToLower()))
        .Select(c => new CategoryBasicVM
        {
          Id = c.Id,
          Name = c.Name
        })
        .OrderBy(c => c.Id)
        .ToListAsync();

      return categories;
    }

    public async Task<List<CategoryBasicVM>> GetCategoriesByFiltersAsync(CategorySearchFiltersVM filtersVM)
    {
      Func<Category, bool> predicate;
      predicate = p => p.Name.ToLower().Contains(filtersVM.CategoryNameFilter.ToLower());

      if (filtersVM.CategoryNameFilterAnd)
      {
        var oldPredicate = predicate;
        predicate = p => oldPredicate(p) && p.Name.ToLower().Contains(filtersVM.CategoryName2Filter.ToLower());
      }

      if (filtersVM.CategoryNameFilterOr)
      {
        var oldPredicate = predicate;
        predicate = p => oldPredicate(p) || p.Name.ToLower().Contains(filtersVM.CategoryName2Filter.ToLower());
      }

      var result = _context
        .Categories
        .Where(predicate)
        .Select(c => new CategoryBasicVM
        {
          Id = c.Id,
          Name = c.Name,
          ParentId = c.ParentId
        })
        .OrderBy(c => c.Id);

      return await Task.FromResult(result.ToList());
    }

    public async Task<CategoryNameTreeVM> GetCategoriesSubTreeByRootId(int rootId)
    {
      var categories = await _context.Categories
        .FromSqlInterpolated($@"    
      WITH RECURSIVE ParentCategories (ID, Name, Description, ParentId)
       AS(
         SELECT ID, Name, Description, ParentId
         FROM Category
       Where Id = {rootId} 
       UNION ALL
       SELECT e.ID, e.Name, e.Description, e.ParentId
         FROM Category e
       INNER JOIN ParentCategories ON ParentCategories.ID = e.ParentId
         )
       SELECT * FROM ParentCategories")
        .Select(c => new CategoryNameTreeVM
        {
          Id = c.Id,
          Name = c.Name,
          Description = c.Description,
          ParentId = c.ParentId,
          Childs = new List<CategoryNameTreeVM>()
        })
        .ToListAsync();

      return BuildSubTree(rootId, categories);
    }

    private static CategoryNameTreeVM BuildSubTree(int rootId, List<CategoryNameTreeVM> items)
    {
      items.ForEach(i => i.Childs = items.Where(ch => ch.ParentId == i.Id).ToList());
      return items.Single(i => i.Id == rootId);
    }
  }
}