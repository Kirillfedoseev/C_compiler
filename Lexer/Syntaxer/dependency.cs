using Compiler.Units;

namespace Compiler.Syntaxing
{
        // Dependency: Ordered by Type, must remain in same order as TokenType enum
    public class Dependency
    {
        public string text, enum_text;
        public TokenType tok;
        public bool right_associative, is_binary, is_unary;
        public int precedence;
        public NodeType node_type;

        public Dependency(string text, string enum_text, TokenType tok, bool right_associative, bool is_binary,
            bool is_unary, int precedence, NodeType node_type)
        {
            this.text = text;
            this.enum_text = enum_text;
            this.tok = tok;
            this.right_associative = right_associative;
            this.is_binary = is_binary;
            this.is_unary = is_unary;
            this.precedence = precedence;
            this.node_type = node_type;
        }



    }
}