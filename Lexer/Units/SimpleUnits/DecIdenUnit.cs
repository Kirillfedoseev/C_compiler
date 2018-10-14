using Newtonsoft.Json;

namespace Compiler.Units.SimpleUnits
{
    [JsonObject]
    class DecIdenUnit : IdenUnit
    {
        [JsonProperty(Order = 3, PropertyName = "object_type")]
        public string Obj_Type { get; set; }

        public DecIdenUnit(string obj_Type = "", string value = "") : base(value)
        {
            Obj_Type = obj_Type;
        }
    }
}
