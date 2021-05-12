using System.Collections.Generic;
using LiteDB;

namespace TGPToolchain.Models
{
    public class LDBItem
    {
        [BsonId]
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Prefab { get; set; } = null!;
        public int Grist { get; set; }
        public string? Strifekind { get; set; } = null!;
        public string? Weaponsprite { get; set; } = null!;
        public bool Custom { get; set; }
        public string? Icon { get; set; } = null!;
        public string? Description { get; set; } = null!;
        public int Speed { get; set; }
        public bool Spawn { get; set; }
        public List<string>? Aliases { get; set; }
        public List<string>? Tags { get; set; }
        public string? Prototyping { get; set; } = null!;
    }
}