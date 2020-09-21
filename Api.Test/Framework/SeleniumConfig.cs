using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Test.Framework
{
  public static class SeleniumConfig
  {
    private static string _baseUrl = "http://localhost3214/";
    public static string LoginUrl = "https://localhost:5002/Account/Login";
    public static string CardsUrl = _baseUrl + "categoryList/0";
    public static string SearchUrl = _baseUrl + "categorySearch";
    
    public static string CardsSwaggerUrl = "https://localhost:44326/swagger/index.html";
    public static string SearchSwaggerUrl =  "http://localhost:57243/swagger/index.html";
  }
}
