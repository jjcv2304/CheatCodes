using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using Application.Utils;
using Application.Utils.Interfaces;
using Dapper;
using Domain;
using Dtos;

namespace Persistance
{
  public class CategoryQueryRepository : ICategoryQueryRepository
  {
    private readonly QueriesConnectionString _connectionString;

    public CategoryQueryRepository(QueriesConnectionString connectionString)
    {
      _connectionString = connectionString;
    }

    // public Category GetById(int id)
    // {
    //     using (SQLiteConnection connection = new SQLiteConnection(_connectionString.Value))
    //     {
    //         return connection.Query<Category>(
    //             "SELECT * FROM Category WHERE Id = @CategoryId",
    //             param: new { CategoryId = id }
    //
    //         ).FirstOrDefault();
    //     }
    // }
    public Category GetById(int id)
    {
      const string sql =
        @"SELECT cp.Id, cp.Name, cp.Description, cp.ParentId as endOfType, 
                         ch.Id , ch.Name, ch.Description, ch.ParentId 
                FROM Category ch
                left join Category cp on ch.ParentId=cp.Id
                where ch.Id=@CategoryId";

      using (var connection = new SQLiteConnection(_connectionString.Value))
      {
        var res = connection.Query<Category, Category, Category>(
          sql,
          (cp, ch) =>
          {
            ch.ParentCategory = cp;
            return ch;
          },
          new { CategoryId = id },
          splitOn: "endOfType"
        ).SingleOrDefault();

        return res;
      }
    }

    public IList<Category> GetByPartialName(string categoryName)
    {
      throw new NotImplementedException();
    }

    public IList<Category> GetByExactName(string categoryName)
    {
      throw new NotImplementedException();
    }

    public IList<Category> GetAllParents()
    {
      const string sql =
        @"SELECT cp.Id, cp.Name, cp.Description, cp.ParentId as endOfType, 
                         ch.Id , ch.Name, ch.Description, ch.ParentId as endOfType2,
                           null as Category, null as Field, cf.Value, '' as endOfType3,
                           f.Id, f.Name, f.Description
                FROM Category cp 
                left join Category ch on ch.ParentId=cp.Id
                left join CategoryField cf on cp.Id=cf.CategoryId
                left join Field F on cf.FieldId = F.Id
                where cp.ParentId is null";

      using (var connection = new SQLiteConnection(_connectionString.Value))
      {
        var res = connection.Query<Category, Category, CategoryField, Field, Category>(
            sql,
            (cp, ch, cf, f) =>
            {
              cp.ChildCategories = cp.ChildCategories ?? new List<Category>();
              if (ch.Id > 0) cp.ChildCategories.Add(ch);

              cp.CategoryFields = cp.CategoryFields ?? new List<CategoryField>();
              if (cf.Value != null)
              {
                cf.Field = f;
                cf.Category = cp;
                cp.CategoryFields.Add(cf);
              }

              return cp;
            },
            splitOn: "endOfType, endOfType2, endOfType3"
          ).GroupBy(cp => cp.Id)
          .Select(group =>
          {
            var categoryParent = group.First();
            if (categoryParent.ChildCategories.Any())
              categoryParent.ChildCategories =
                @group.Select(cp => cp.ChildCategories.Single()).Distinct().ToList();

            if (categoryParent.CategoryFields.Any())
              categoryParent.CategoryFields =
                group.Select(cp => cp.CategoryFields.Single()).Distinct().ToList();
            return categoryParent;
          }).ToList();

        return res;
      }
    }

    public IList<Category> GetAllChilds(int categoryParentId)
    {
      const string sql =
        @"SELECT c.Id, c.Name, c.Description, c.ParentId, c.ParentId as endOfType,
                         cp.Id, cp.Name, cp.Description, cp.ParentId as endOfType2,
                         ch.Id , ch.Name, ch.Description, ch.ParentId as endOfType3, 
                         null as Category, null as Field, cf.Value, '' as endOfType4,
                         f.Id, f.Name, f.Description
                FROM Category c
                left join Category cp on c.ParentId=cp.Id
                left join Category ch on ch.ParentId=c.Id
                left join CategoryField cf on c.Id=cf.CategoryId
                left join Field F on cf.FieldId = F.Id
                where c.ParentId = @categoryParentId";
      using (var connection = new SQLiteConnection(_connectionString.Value))
      {
        return connection.Query<Category, Category, Category, CategoryField, Field, Category>(
            sql,
            (c, cp, ch, cf, f) =>
            {
              c.ParentCategory = cp;
              c.ChildCategories = c.ChildCategories ?? new List<Category>();
              if (ch.Id > 0) c.ChildCategories.Add(ch);

              c.CategoryFields = c.CategoryFields ?? new List<CategoryField>();
              if (cf.Value != null)
              {
                cf.Field = f;
                cf.Category = c;
                c.CategoryFields.Add(cf);
              }

              return c;
            },
            new { categoryParentId },
            splitOn: "endOfType, endOfType2, endOfType3, endOfType4")
          .GroupBy(c => c.Id)
          .Select(group =>
          {
            var category = group.First();
            if (category.ChildCategories.Any())
              category.ChildCategories = group.Select(c => c.ChildCategories.Single()).ToList();
            if (category.CategoryFields.Any())
              category.CategoryFields = group.Select(c => c.CategoryFields.Single()).Distinct().ToList();
            return category;
          }).ToList();
      }
    }

    public IList<Category> GetSiblingsOf(int categoryId)
    {
      const string sql =
        @"SELECT c.Id, c.Name, c.Description, c.ParentId, c.ParentId as endOfType,
                         cp.Id, cp.Name, cp.Description, cp.ParentId as endOfType2,
                         ch.Id , ch.Name, ch.Description, ch.ParentId  as endOfType3, 
                         null as Category, null as Field, cf.Value, '' as endOfType4,
                         f.Id, f.Name, f.Description
                    FROM Category cproot
                    inner join Category cgproot on (CASE
                                                     WHEN cproot.ParentId is null
                                                       THEN cproot.ParentId is null
                                                     Else cgproot.Id = cproot.ParentId
                                                    END)
                    inner join Category c on (CASE
                                                   WHEN cproot.ParentId is null
                                                     THEN c.ParentId is null
                                                   Else c.ParentId = cproot.ParentId
                                                   END)
                   left join Category cp on c.ParentId = cp.Id
                   left join Category ch on ch.ParentId = c.Id
                   left join CategoryField cf on c.Id=cf.CategoryId
                   left join Field F on cf.FieldId = F.Id
              where cproot.Id = @categoryId";

      using (var connection = new SQLiteConnection(_connectionString.Value))
      {
        return connection.Query<Category, Category, Category, CategoryField, Field, Category>(
            sql,
            (c, cp, ch, cf, f) =>
            {
              if (cp.Id > 0) c.ParentCategory = cp;

              c.ChildCategories = c.ChildCategories ?? new List<Category>();
              if (ch.Id > 0) c.ChildCategories.Add(ch);

              c.CategoryFields = c.CategoryFields ?? new List<CategoryField>();
              cf.Field = f;
              cf.Category = c;
              c.CategoryFields.Add(cf);

              return c;
            },
            new { categoryId },
            splitOn: "endOfType, endOfType2, endOfType3, endOfType4")
          .GroupBy(c => c.Id)
          .Select(group =>
          {
            var category = group.First();
            if (category.ChildCategories.Any())
              category.ChildCategories = group.Select(c => c.ChildCategories.Single()).ToList();
            category.CategoryFields = group.Select(c => c.CategoryFields.Single()).Distinct().ToList();
            return category;
          }).ToList();
      }
    }

    public IList<CategoryTreeDto> ExportToJson()
    {
      using (var connection = new SQLiteConnection(_connectionString.Value))
      {
        var flatResult = connection.Query<CategoryTreeDto>(@"select * from Category");
        return BuildTree(flatResult.ToList());
      }
    }

    public CategoryTreeDto ParentsCategoryTreeDtos(int rootCategoryId)
    {
      using (var connection = new SQLiteConnection(_connectionString.Value))
      {
        var flatResult = connection.Query<CategoryTreeDto>(@$"
            ;with category_tree as
            (
               select id,name, parentId
               from Category
               where id = {rootCategoryId}
               union all
               select C.id, C.name, C.parentId
               from Category c
               join category_tree p on C.id = P.parentId
                AND C.id<>C.parentId 
            ) 

            select ct2.id, ct2.name, ct2.ParentId
              from category_tree ct2
              union
              select c.id, c.name, c.ParentId
              from category_tree ct
              inner join Category c on ct.ParentId=c.Id"
        );
        return BuildTreeSingleRoot(flatResult.ToList());
      }
    }

    private static List<CategoryTreeDto> BuildTree(List<CategoryTreeDto> items)
    {
      items.ForEach(i => i.ChildCategoryDtos = items.Where(ch => ch.ParentId == i.Id).ToList());
      return items.Where(i => i.ParentId == null).ToList();
    }
    private static CategoryTreeDto BuildTreeSingleRoot(List<CategoryTreeDto> items)
    {
      items.ForEach(i => i.ChildCategoryDtos = items.Where(ch => ch.ParentId == i.Id).ToList());
      return items.Single(i => i.ParentId == null);
    }

    /*
     ;with category_tree as
       (
       select id,name, parentid
       from Category
       where id = 464 
       union all
       select C.id, C.name, C.parentid
       from Category c
       join category_tree p on C.id = P.parentid  
       AND C.id<>C.parentid 
       ) 
       
       select c.id, c.name, c.ParentId
       from category_tree ct inner join Category c on ct.ParentId=c.Id
     */

    /*
     WITH RECURSIVE MyTree AS (
       SELECT * FROM Category WHERE id =rootId
       UNION ALL
       SELECT m.* FROM Category AS m JOIN MyTree AS t ON m.ParentId = t.Id
       )
       SELECT * FROM MyTree;
     */
  }
}