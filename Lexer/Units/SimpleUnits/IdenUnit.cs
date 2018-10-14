using Newtonsoft.Json;

namespace Compiler.Units.SimpleUnits
{
    [JsonObject]
    public class IdenUnit : SimpleUnit
    {
        public IdenUnit(string value = "") : base(UnitType.var, value) { }
    }
}
