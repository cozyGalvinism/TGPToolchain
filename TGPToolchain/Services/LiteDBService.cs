using System;
using System.Collections.Generic;
using LiteDB;
using TGPToolchain.Models;

namespace TGPToolchain.Services
{
    public class LiteDBService : IDataService, IDisposable
    {
        private LiteDatabase? _ldb;
        private ILiteCollection<LDBItem> _itemCollection = null!;
        private ILiteCollection<LDBRecipe> _recipeCollection = null!;
        private string _lastError;
        public string LastError => _lastError;

        public LiteDBService(string databasePath)
        {
            _lastError = "";
            InitializeDatabase(databasePath);
        }

        public void InitializeDatabase(string databasePath)
        {
            _ldb?.Dispose();
            _ldb = new LiteDatabase(new ConnectionString
            {
                Filename = databasePath,
                Collation = new Collation("en-US/None")
            });

            _itemCollection = _ldb.GetCollection<LDBItem>();
            _itemCollection.EnsureIndex(k => k.Tags);
            _itemCollection.EnsureIndex(k => k.Grist);
            _itemCollection.EnsureIndex(k => k.Strifekind);

            _recipeCollection = _ldb.GetCollection<LDBRecipe>();
            _recipeCollection.EnsureIndex(k => k.Method);
            _recipeCollection.EnsureIndex(k => k.ItemA);
            _recipeCollection.EnsureIndex(k => k.ItemB);
        }

        public IEnumerable<LDBItem> GetItems() => _itemCollection.FindAll();

        public IEnumerable<LDBItem> FindItems(string query)
        {
            return _itemCollection.Find(i =>
                i.Code.Contains(query) ||
                i.Name.Contains(query) ||
                i.Prefab.Contains(query) ||
                i.Icon!.Contains(query) ||
                i.Weaponsprite!.Contains(query) ||
                i.Strifekind!.Contains(query) ||
                i.Description!.Contains(query) ||
                i.Tags!.Contains(query) ||
                i.Aliases!.Contains(query));
        }

        public bool CreateItem(LDBItem item)
        {
            var existingItem = _itemCollection.FindById(item.Code);
            if (existingItem != null)
            {
                _lastError = "An item already exists with this ID!";
                return false;
            }
            _itemCollection.Insert(item);
            return true;
        }

        public bool EditItem(LDBItem item)
        {
            var existingItem = _itemCollection.FindById(item.Code);
            if (existingItem == null)
            {
                _lastError = "This item does not exist!";
                return false;
            }
            _itemCollection.Update(item);
            return true;
        }

        public bool DeleteItem(string code)
        {
            var item = _itemCollection.FindById(code);
            if (item == null)
            {
                _lastError = "This item does not exist!";
                return false;
            }
            _itemCollection.Delete(code);
            return true;
        }

        public LDBItem? GetItem(string code)
        {
            var item = _itemCollection.FindById(code);
            return item;
        }

        public IEnumerable<LDBRecipe> GetRecipes()
        {
            return _recipeCollection.FindAll();
        }

        public IEnumerable<LDBRecipe> FindRecipes(string query)
        {
            if (string.IsNullOrEmpty(query)) return GetRecipes();
            return _recipeCollection.Find(r =>
                r.ItemA.Contains(query) || r.ItemB.Contains(query) || r.Result.Code.Contains(query) ||
                r.Result.Code.Contains(query) ||
                r.Result.Name.Contains(query) ||
                r.Result.Prefab.Contains(query) ||
                r.Result.Icon!.Contains(query) ||
                r.Result.Weaponsprite!.Contains(query) ||
                r.Result.Strifekind!.Contains(query) ||
                r.Result.Description!.Contains(query) ||
                r.Result.Tags!.Contains(query) ||
                r.Result.Aliases!.Contains(query));
        }

        public bool DoesRecipeExist(string itemA, LDBRecipe.Methods method, string itemB)
        {
            return _recipeCollection.Exists(r =>
                (r.ItemA == itemA && r.ItemB == itemB || r.ItemA == itemB && r.ItemB == itemA) && r.Method == method);
        }

        public bool CreateRecipe(LDBRecipe recipe)
        {
            var exists = recipe.Id != null && _recipeCollection.FindById(recipe.Id) != null ||
                         DoesRecipeExist(recipe.ItemA, recipe.Method, recipe.ItemB);
            if (exists)
            {
                _lastError = "A recipe like that already exists!";
                return false;
            }
            _recipeCollection.Insert(recipe);
            return true;
        }

        public bool EditRecipe(LDBRecipe recipe)
        {
            // Recipe has no ID
            if (recipe.Id == null)
            {
                _lastError = "The recipe you're editing has no ID!";
                return false;
            }
            // Find recipe by ID
            var existingRecipe = _recipeCollection.FindById(recipe.Id);
            // No recipe exists to edit
            if (existingRecipe == null)
            {
                _lastError = "This recipe does not exist!";
                return false;
            }
            // A recipe like this already exists
            if (DoesRecipeExist(recipe.ItemA, recipe.Method, recipe.ItemB))
            {
                _lastError = "A recipe like that already exists!";
                return false;
            }
            _recipeCollection.Update(recipe);
            return true;
        }

        public bool DeleteRecipe(LDBRecipe recipe)
        {
            var existingRecipe = _recipeCollection.FindById(recipe.Id);
            if (existingRecipe == null)
            {
                _lastError = "This recipe does not exist!";
                return false;
            }
            _recipeCollection.Delete(recipe.Id);
            return true;
        }

        public LDBRecipe? GetRecipe(string id)
        {
            ObjectId objectId = new(id);
            return _recipeCollection.FindById(objectId);
        }

        public void Dispose()
        {
            _ldb?.Dispose();
        }
    }
}