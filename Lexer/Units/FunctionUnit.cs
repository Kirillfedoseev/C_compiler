using Newtonsoft.Json;
using System.Collections.Generic;
using Compiler.Units.SimpleUnits;

namespace Compiler.Units
{
    [JsonObject]
    public class FunctionUnit:Unit
    {

        [JsonProperty(Order = 2, PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(Order = 3, PropertyName = "return")]
        public string Return { get; set; }

        [JsonProperty(Order = 4, PropertyName = "args")]
        public List<IdenUnit> Args { get; set; }

        [JsonProperty(Order = 5, PropertyName = "body")]
        public List<Unit> Body { get; set; }

        public FunctionUnit():base(UnitType.program)
        {
            Args = new List<IdenUnit>();
            Body = new List<Unit>();
        }
    }
}
