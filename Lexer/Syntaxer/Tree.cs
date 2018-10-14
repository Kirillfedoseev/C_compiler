using Compiler.Units;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Compiler.Syntaxing
{
    [JsonObject]

    public class Tree
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(Order = 1, PropertyName = "type")]
        public NodeType node_type;

        [JsonProperty(Order = 2, PropertyName = "left")]
        public Tree left;

        [JsonProperty(Order = 3, PropertyName = "right")]
        public Tree right;

        [JsonProperty(Order = 4, PropertyName = "value")]
        public string value;

        //leaf
        public Tree(NodeType node_type, string value)
        {
            this.node_type = node_type;
            this.value = value;
        }

        public Tree(NodeType node_type, Tree left, Tree right)
        {
            this.node_type = node_type;
            this.left = left;
            this.right = right;
        }


        //todo implement node type to string
        public string[] toString()
        {
            List<string> sb = new List<string>();
            if (this == null)
                sb.Add(";\n");
            else
            {
                sb.Add(string.Format(node_type.ToString(), "%-14s "));
                if (node_type == NodeType.Ident || node_type == NodeType.Integer ||
                    node_type == NodeType.String)
                {
                    sb.Add(value);
                }
                else
                {
                    sb.AddRange(left.toString());
                    sb.AddRange(right.toString());
                }
            }

            return sb.ToArray();
        }

    }



}
