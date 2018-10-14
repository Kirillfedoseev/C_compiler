using Newtonsoft.Json;

namespace Compiler.Units.SimpleUnits
{
    [JsonObject]
    class BinaryOpsUnit :Unit
    {


        [JsonProperty(Order = 2, PropertyName = "operator")]
        private string op;

        [JsonProperty(Order = 3, PropertyName = "left")]
        private SimpleUnit left;

        [JsonProperty(Order = 4, PropertyName = "right")]
        private SimpleUnit right;

        public BinaryOpsUnit(string op, SimpleUnit left, SimpleUnit right) : base(UnitType.expression)
        {
            this.op = op;
            this.left = left;
            this.right = right;
        }
    }
}
