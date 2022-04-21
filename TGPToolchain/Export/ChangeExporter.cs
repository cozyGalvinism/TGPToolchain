using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiteDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TGPToolchain.Models;
using TGPToolchain.Services;
using TGPToolchain.Helpers;
using JsonSerializer = LiteDB.JsonSerializer;

namespace TGPToolchain.Export
{
    public class ChangeExporter
    {
        public static void Import(string path, IDataService dataService)
        {
            
        }

        public static void Export(string path, string name, string version, string author, List<LDBItem> addedItems,
            List<LDBItem> changedItems, List<string> deletedItems, List<LDBRecipe> addedRecipes,
            List<LDBRecipe> changedRecipes, List<string> deletedRecipes)
        {
            JObject exportObject = new();
            JObject itemsObject = new();
            JObject recipesObject = new();
            var addedItemsArray = LiteDbEntitiesToJson(addedItems.DistinctLast(i => i.Code));
            var changedItemsArray = LiteDbEntitiesToJson(changedItems.DistinctLast(i => i.Code));
            var deletedItemsArray = JArray.FromObject(deletedItems.DistinctLast(i => i));
            itemsObject.Add("added", addedItemsArray);
            itemsObject.Add("changed", changedItemsArray);
            itemsObject.Add("deleted", deletedItemsArray);
            // var addedRecipesArray = LiteDbEntitiesToJson(addedRecipes.DistinctLast(i => i.Id));
            // var changedRecipesArray = LiteDbEntitiesToJson(changedRecipes.DistinctLast(i => i.Id));
            // var deletedRecipesArray = JArray.FromObject(deletedRecipes.DistinctLast(i => i));
            // recipesObject.Add("added", addedRecipesArray);
            // recipesObject.Add("changed", changedRecipesArray);
            // recipesObject.Add("deleted", deletedRecipesArray);
            
            exportObject.Add("name", name);
            exportObject.Add("version", version);
            exportObject.Add("author", author);
            exportObject.Add("items", itemsObject);
            exportObject.Add("recipes", recipesObject);
            
            File.WriteAllText(path, exportObject.ToString(Formatting.Indented));
        }

        private static JArray LiteDbEntitiesToJson<T>(IEnumerable<T> entities)
        {
            JArray entityArray = new();
            foreach (var entity in entities)
            {
                var doc = BsonMapper.Global.ToDocument(entity);
                var jsonStr = JsonSerializer.Serialize(doc);
                JObject jsonObject = JObject.Parse(jsonStr);
                entityArray.Add(jsonObject);
            }

            return entityArray;
        }

        private static List<T> JsonToLiteDbEntities<T>(JArray entities)
        {
            return (from JObject? jsonObject in entities
                select jsonObject.ToString()
                into jsonStr
                select (BsonDocument) JsonSerializer.Deserialize(jsonStr)
                into doc
                select BsonMapper.Global.ToObject<T>(doc)).ToList();
        }

        private static void DeleteEntitiesFromDatabase<TEntity>(IEnumerable<string> deletedEntities,
            ILiteCollection<TEntity> entityCollection)
        {
            foreach (var deletedEntity in deletedEntities) entityCollection.Delete(deletedEntity);
        }
    }
}