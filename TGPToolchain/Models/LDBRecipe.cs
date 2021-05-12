using LiteDB;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
#pragma warning disable 8618

namespace TGPToolchain.Models
{
    public class LDBRecipe
    {
        public enum Methods
        {
            AND,
            OR
        }
    
        [BsonId]
        public ObjectId Id { get; set; }
        public LDBItem Result { get; set; }
        public string ItemA { get; set; }
        public string ItemB { get; set; }
        public Methods Method { get; set; }
    }
}