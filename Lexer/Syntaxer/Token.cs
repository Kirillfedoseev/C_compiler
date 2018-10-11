using System.Linq;

namespace Compiler.Syntaxing
{
    /// <summary>
    /// Storage class for tokens
    /// </summary>
    public class Token
    {
        public TokenType Type { get; set; }

        public int Line { get; set; }

        public int Position { get; set; }

        public string Value { get; set; }

        public Token(TokenType type, int line, int postion,string value = "")
        {
            Type = type;
            Line = line;
            Position = postion;
            Value = value;
        }

        //private string[] Display_nodes { get; set; } =
        //{
        //    "Identifier", "String", "Integer", "Sequence", "If", "Prtc",
        //    "Prts", "Prti", "While", "Assign", "Negate", "Not", "Multiply", "Divide", "Mod",
        //    "Add", "Subtract", "Less", "LessEqual", "Greater", "GreaterEqual", "Equal",
        //    "NotEqual", "And", "Or"
        //};


        static Dependency[] atr =
        {
            new Dependency("EOI", "Eof_input", TokenType.EOI, false, false, false, -1, NodeType.None),
            new Dependency("*", "Op_multiply", TokenType.Mul, false, true, false, 13, NodeType.Mul),
            new Dependency("/", "Op_divide", TokenType.Div, false, true, false, 13, NodeType.Div),
            new Dependency("%", "Op_mod", TokenType.Mod, false, true, false, 13, NodeType.Mod),
            new Dependency("+", "Op_add", TokenType.Add, false, true, false, 12, NodeType.Add),
            new Dependency("-", "Op_subtract", TokenType.Sub, false, true, false, 12, NodeType.Sub),
            new Dependency("-", "Op_negate", TokenType.Negate, false, false, true, 14, NodeType.Negate),
            new Dependency("!", "Op_not", TokenType.Not, false, false, true, 14, NodeType.Not),
            new Dependency("<", "Op_less", TokenType.Lss, false, true, false, 10, NodeType.Lss),
            new Dependency("<=", "Op_lessequal", TokenType.Leq, false, true, false, 10, NodeType.Leq),
            new Dependency(">", "Op_greater", TokenType.Gtr, false, true, false, 10, NodeType.Gtr),
            new Dependency(">=", "Op_greaterequal", TokenType.Geq, false, true, false, 10, NodeType.Geq),
            new Dependency("==", "Op_equal", TokenType.Eql, false, true, false, 9, NodeType.Eql),
            new Dependency("!=", "Op_notequal", TokenType.Neq, false, true, false, 9, NodeType.Neq),
            new Dependency("=", "Op_assign", TokenType.Assign, false, false, false, -1, NodeType.Assign),
            new Dependency("&&", "Op_and", TokenType.And, false, true, false, 5, NodeType.And),
            new Dependency("||", "Op_or", TokenType.Or, false, true, false, 4, NodeType.Or),
            new Dependency("if", "Keyword_if", TokenType.If, false, false, false, -1, NodeType.If),
            new Dependency("else", "Keyword_else", TokenType.Else, false, false, false, -1, NodeType.None),
            new Dependency("while", "Keyword_while", TokenType.While, false, false, false, -1, NodeType.While),
            new Dependency("print", "Keyword_print", TokenType.Print, false, false, false, -1, NodeType.None),
            new Dependency("putc", "Keyword_putc", TokenType.Putc, false, false, false, -1, NodeType.None),
            new Dependency("(", "LeftParen", TokenType.Lparen, false, false, false, -1, NodeType.None),
            new Dependency(")", "RightParen", TokenType.Rparen, false, false, false, -1, NodeType.None),
            new Dependency("{", "LeftBrace", TokenType.Lbrace, false, false, false, -1, NodeType.None),
            new Dependency("}", "RightBrace", TokenType.Rbrace, false, false, false, -1, NodeType.None),
            new Dependency(";", "Semicolon", TokenType.Semi, false, false, false, -1, NodeType.None),
            new Dependency(",", "Comma", TokenType.Comma, false, false, false, -1, NodeType.None),
            new Dependency("Ident", "Identifier", TokenType.Ident, false, false, false, -1, NodeType.Ident),
            new Dependency("Integer literal", "Integer", TokenType.Integer, false, false, false, -1,
                NodeType.Integer),
            new Dependency("String literal", "String", TokenType.String, false, false, false, -1, NodeType.String)
        };

        public static TokenType TypeByName(string name)
        {
            // return internal version of name

            return atr.First(n => n.enum_text == name).tok;
        }

        public static Dependency DependencyByType(TokenType type)
        {
            return atr.First(n => n.tok == type);
        }

        public override string ToString()
        {
            return "";

        }
    }
}