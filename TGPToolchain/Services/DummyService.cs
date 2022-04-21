using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TGPToolchain.Models;

namespace TGPToolchain.Services
{
    public class DummyService : IDataService
    {
        public string LastError => "";
        public IEnumerable<LDBItem> GetItems()
        {
            return new[]
            {
                new LDBItem
                {
                    Code = "abcdefgh",
                    Name = "Random Item",
                    Aliases = new List<string>()
                    {
                        "Multiple",
                        "Aliases"
                    },
                    Custom = true,
                    Description = "A random item",
                    Grist = 10,
                    Icon = "icon",
                    Prefab = "someprefab",
                    Prototyping = "",
                    Spawn = true,
                    Speed = 1,
                    Weaponsprite = "",
                    Tags = new List<string>()
                },
                new LDBItem
                {
                    Code = "aaaaaaaa",
                    Name = "Random Item 2",
                    Aliases = new List<string>(),
                    Custom = true,
                    Description = "A random item 2",
                    Grist = 10,
                    Icon = "icon",
                    Prefab = "someprefab",
                    Prototyping = "",
                    Spawn = true,
                    Speed = 1,
                    Weaponsprite = "",
                    Tags = new List<string>()
                    {
                        "Multiple",
                        "Tags"
                    }
                }
            };
        }

        public IEnumerable<LDBItem> FindItems(string query)
        {
            return GetItems();
        }

        public bool CreateItem(LDBItem item)
        {
            return true;
        }

        public bool EditItem(LDBItem item)
        {
            return true;
        }

        public bool DeleteItem(string code)
        {
            return true;
        }

        public LDBItem? GetItem(string code)
        {
            return GetItems().FirstOrDefault();
        }

        public IEnumerable<LDBRecipe> GetRecipes()
        {
            return new[]
            {
                new LDBRecipe()
                {
                    ItemA = "abcdefgh",
                    ItemB = "abcdefgh",
                    Method = LDBRecipe.Methods.AND,
                    Result = GetItems().FirstOrDefault()!
                }
            };
        }

        public IEnumerable<LDBRecipe> FindRecipes(string query)
        {
            return GetRecipes();
        }

        public bool CreateRecipe(LDBRecipe recipe)
        {
            return true;
        }

        public bool EditRecipe(LDBRecipe oldRecipe, LDBRecipe recipe)
        {
            return true;
        }

        public bool DeleteRecipe(LDBRecipe recipe)
        {
            return true;
        }

        public LDBRecipe? GetRecipe(string id)
        {
            return GetRecipes().FirstOrDefault();
        }
    }
}