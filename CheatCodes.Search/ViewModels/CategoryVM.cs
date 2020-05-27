using System.Collections.Generic;

namespace CheatCodes.Search.ViewModels
{
  public class CategoryBasicVM
  {
    public int Id { get; set; }
    public string Name { get; set; }

    public int? ParentId { get; set; }
  }

  public class CategoryNameTreeVM
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int? ParentId { get; set; }
    public IList<CategoryNameTreeVM> Childs { get; set; }

    static bool IsAngularAndHasChilds(object o)
    {
      return o is CategoryNameTreeVM c &&
             c is { Name: "Angular", Childs: {} };

    }
  }

  public class CategorySearchFiltersVM
  {
    public string CategoryNameFilter { get; set; }
    public bool CategoryNameFilterOr { get; set; }
    public bool CategoryNameFilterAnd { get; set; }
    public string CategoryName2Filter { get; set; }
  }
}