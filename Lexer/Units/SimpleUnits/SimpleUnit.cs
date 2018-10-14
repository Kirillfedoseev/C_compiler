using Newtonsoft.Json;
namespace Compiler.Units.SimpleUnits
{
    [JsonObject]
    public abstract class SimpleUnit : Unit
    {

        [JsonProperty(Order = 2, PropertyName = "value")]
        public string Value { get; set; }

        public SimpleUnit(UnitType type, string value = "") : base(type)
        {
            Value = value;
        }
    }
}
