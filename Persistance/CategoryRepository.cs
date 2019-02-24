using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Application.Interfaces;
using Dapper;
using Domain;
using Persistance.Utils;

namespace Persistance
{
    public class CategoryRepository : RepositoryBase, ICategoryRepository
    {
        public CategoryRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public IEnumerable<Category> All()
        {
            return Connection.Query<Category>(
                "SELECT * FROM Category",
                transaction: Transaction
            ).ToList();
        }

        public Category Find(long id)
        {
            return Connection.Query<Category>(
                "SELECT * FROM Category WHERE CategoryId = @CategoryId",
                param: new {CategoryId = id},
                transaction: Transaction
            ).FirstOrDefault();
        }

        public void Remove(long id)
        {
            Connection.Execute(
                "DELETE FROM Category WHERE Id = @CategoryId",
                param: new {CategoryId = id},
                transaction: Transaction
            );
        }

        public void Remove(Category entity)
        {
            Remove(entity.Id);
        }

        public void Add(Category entity)
        {
            entity.Id = Connection.ExecuteScalar<int>(
                "INSERT INTO Category(Name) VALUES(@Name); SELECT SCOPE_IDENTITY()",
                param: new {Name = entity.Name},
                transaction: Transaction
            );
        }

        public void Update(Category entity)
        {
            Connection.Execute(
                "UPDATE Category SET Name = @Name WHERE Id = @CategoryId",
                param: new {Name = entity.Name, CategoryId = entity.Id},
                transaction: Transaction
            );
        }

        public Category FindByName(string name)
        {
            return Connection.Query<Category>(
                "SELECT * FROM Category WHERE Name = @Name",
                param: new {Name = name},
                transaction: Transaction
            ).FirstOrDefault();
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
                inner join CategoryField cf on cp.Id=cf.CategoryId
                inner join Field F on cf.FieldId = F.Id
                where cp.ParentId is null";

            var res = Connection.Query<Category, Category, CategoryField, Field, Category>(
                    sql,
                    (cp, ch, cf, f) =>
                    {
                        cp.ChildCategories = cp.ChildCategories ?? new List<Category>();
                        cp.ChildCategories.Add(ch);
                        cp.CategoryFields =cp.CategoryFields ?? new List<CategoryField>();
                        //cf.Category = cp;
                        cf.Field = f;
                        cf.Category = cp;
                        cp.CategoryFields.Add(cf);
                        
                        return cp;
                    },
                    splitOn: "endOfType, endOfType2, endOfType3",
                    transaction: Transaction
                ).GroupBy(cp => cp.Id)
                .Select(group =>
                {
                    var categoryParent = group.First();
                    categoryParent.ChildCategories = group.Select(cp => cp.ChildCategories.Single()).Distinct().ToList();
                    categoryParent.CategoryFields = group.Select(cp => cp.CategoryFields.Single()).Distinct().ToList();
                    return categoryParent;
                }).ToList();

            return res;
        }

        /*
         * public IList<Category> GetAllParents()
        {
            const string sql =
                @"SELECT cp.Id, cp.Name, cp.Description, cp.ParentId as endOfType, 
                         ch.Id , ch.Name, ch.Description, ch.ParentId as endOfType2,
                           null as Category, null as Field, cf.Value, '' as endOfType3,
                           f.Id, f.Name, f.Description
                FROM Category cp 
                left join Category ch on ch.ParentId=cp.Id
                inner join CategoryField cf on cp.Id=cf.CategoryId
                inner join Field F on cf.FieldId = F.Id
                where cp.ParentId is null";

            var res = Connection.Query<Category, Category, CategoryField, Field, Category>(
                    sql,
                    (cp, ch, cf, f) =>
                    {
                        cp.ChildCategories = cp.ChildCategories ?? new List<Category>();
                        cp.ChildCategories.Add(ch);
                        cp.CategoryFields =cp.CategoryFields ?? new List<CategoryField>();
                        //cf.Category = cp;
                        cf.Field = f;
                        cp.CategoryFields.Add(cf);
                        
                        return cp;
                    },
                    splitOn: "endOfType, endOfType2, endOfType3",
                    transaction: Transaction
                ).GroupBy(cp => cp.Id)
                .Select(group =>
                {
                    var categoryParent = group.First();
                    categoryParent.ChildCategories = group.Select(cp => cp.ChildCategories.Single()).ToList();
                    return categoryParent;
                }).ToList();

            return res;
        }
         * 
         */
        
        public IList<Category> GetAllChilds(int categoryParentId)
        {
            const string sql =
                @"SELECT c.Id, c.Name, c.Description, c.ParentId, c.ParentId as endOfType,
                         cp.Id, cp.Name, cp.Description, cp.ParentId as endOfType2,
                         ch.Id , ch.Name, ch.Description, ch.ParentId
                FROM Category c
                left join Category cp on c.ParentId=cp.Id
                left join Category ch on ch.ParentId=c.Id
                where c.ParentId = @categoryParentId";

            return Connection.Query<Category, Category, Category, Category>(
                    sql,
                    (c, cp, ch) =>
                    {
                        c.ParentCategory = cp;
                        c.ChildCategories = c.ChildCategories ?? new List<Category>();
                        if (ch.Id > 0)
                        {
                            c.ChildCategories.Add(ch);
                        }

                        return c;
                    },
                    param: new {categoryParentId = categoryParentId},
                    splitOn: "endOfType, endOfType2",
                    transaction: Transaction).GroupBy(c => c.Id)
                .Select(group =>
                {
                    var category = group.First();
                    if (category.ChildCategories.Any())
                        category.ChildCategories = group.Select(c => c.ChildCategories.Single()).ToList();
                    return category;
                }).ToList();
        }

        public IList<Category> GetSiblingsOf(int categoryId)
        {
            const string sql =
                @"SELECT c.Id, c.Name, c.Description, c.ParentId, c.ParentId as endOfType,
                         cp.Id, cp.Name, cp.Description, cp.ParentId as endOfType2,
                         ch.Id , ch.Name, ch.Description, ch.ParentId
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
              where cproot.Id = @categoryId";

            return Connection.Query<Category, Category, Category, Category>(
                    sql,
                    (c, cp, ch) =>
                    {
                        if (cp.Id > 0)
                        {
                            c.ParentCategory = cp;
                        }

                        c.ChildCategories = c.ChildCategories ?? new List<Category>();
                        if (ch.Id > 0)
                        {
                            c.ChildCategories.Add(ch);
                        }

                        return c;
                    },
                    param: new {categoryId = categoryId},
                    splitOn: "endOfType, endOfType2",
                    transaction: Transaction).GroupBy(c => c.Id)
                .Select(group =>
                {
                    var category = group.First();
                    if (category.ChildCategories.Any())
                        category.ChildCategories = group.Select(c => c.ChildCategories.Single()).ToList();
                    return category;
                }).ToList();
        }

    }
}