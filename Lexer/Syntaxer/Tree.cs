using System.Text;

namespace Compiler.Syntaxing
{
    public partial class Syntaxer
    {
        class Tree
        {
            public NodeType node_type;
            public Tree left;
            public Tree right;
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
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder("");
                if (this == null)
                    sb.Append(";\n");
                else
                {
                    sb.Append(string.Format(node_type.ToString(), "%-14s "));
                    if (node_type == NodeType.Ident || node_type == NodeType.Integer ||
                        node_type == NodeType.String)
                    {
                        sb.Append(value + "\n");
                    }
                    else
                    {
                        sb.Append("\n");
                        sb.Append(left.ToString());
                        sb.Append(right.ToString());
                    }
                }

                return sb.ToString();
            }

        }



    }
}