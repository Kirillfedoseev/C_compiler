using Newtonsoft.Json;

namespace Compiler.Units.SimpleUnits
{
    [JsonObject]
    class StringUnit : SimpleUnit
    {
        public StringUnit(string value = "") : base(UnitType.String, value) { }
    }
}
