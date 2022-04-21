using System.Collections;
using System.Collections.Generic;
using TGPToolchain.Models;

namespace TGPToolchain.Services
{
    public interface IDataService
    {
        string LastError { get; }
        IEnumerable<LDBItem> GetItems();
        IEnumerable<LDBItem> FindItems(string query);
        bool CreateItem(LDBItem item);
        bool EditItem(LDBItem item);
        bool DeleteItem(string code);
        LDBItem? GetItem(string code);
        IEnumerable<LDBRecipe> GetRecipes();
        IEnumerable<LDBRecipe> FindRecipes(string query);
        bool CreateRecipe(LDBRecipe recipe);
        bool EditRecipe(LDBRecipe oldRecipe, LDBRecipe recipe);
        bool DeleteRecipe(LDBRecipe recipe);
    }
}