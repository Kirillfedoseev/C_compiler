using Compiler.Syntaxer.Units;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Compiler.Units.SimpleUnits
{
    [JsonObject]
    abstract class SimpleUnit : Unit
    {

        [JsonProperty(Order = 2, PropertyName = "value")]
        public string Value { get; set; }

        public SimpleUnit(UnitType type, string value = "") : base(type)
        {
            Value = value;
        }
    }
}
