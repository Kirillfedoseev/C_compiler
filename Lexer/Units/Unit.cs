using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Compiler.Units
{
    [JsonObject]
    public abstract class Unit
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(Order = 1, PropertyName = "type")]
        public UnitType Type { get; set; } 

        public Unit(UnitType type)
        {
            Type = type;            
        }
    }
}
