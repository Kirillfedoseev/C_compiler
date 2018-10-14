using Newtonsoft.Json;

namespace Compiler.Units.SimpleUnits
{
    [JsonObject]
    class IncludeUnit : SimpleUnit
    {
        public IncludeUnit(string value = "") : base(UnitType.include, value) { }
    }
}
