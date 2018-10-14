using Compiler.Syntaxer.Units;
using Newtonsoft.Json;

namespace Compiler.Units.SimpleUnits
{
    [JsonObject]
    class NumUnit :SimpleUnit
    {
        public NumUnit(string value = "") : base(UnitType.num, value) { }

    }
}
