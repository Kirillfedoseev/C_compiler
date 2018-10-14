using Compiler.Syntaxer.Units;
using Newtonsoft.Json;

namespace Compiler.Units.SimpleUnits
{
    [JsonObject]
    class IdenUnit : SimpleUnit
    {
        public IdenUnit(string value = "") : base(UnitType.var, value) { }
    }
}
