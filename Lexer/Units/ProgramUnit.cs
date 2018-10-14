using Newtonsoft.Json;
using System.Collections.Generic;

namespace Compiler.Units
{
    [JsonObject]
    public class ProgramUnit:Unit
    {

        [JsonProperty(Order = 2, PropertyName = "prog")]
        public List<Unit> Nested { get; set; }

        public ProgramUnit():base(UnitType.program)
        {
            Nested = new List<Unit>();
        }
    }
}
