
namespace Compiler.Lexer
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

        public override string ToString()
        {
            return Type == TokenType.Integer || 
                   Type == TokenType.Identifier ||
                   Type == TokenType.Include || 
                   Type == TokenType.Char ||
                   Type == TokenType.Float
                ? $"{Line,-5}  {Position,-5}   {Type.ToString(),-14}     {Value}"
                : (Type == TokenType.String
                    ? $"{Line,-5}  {Position,-5}   {Type.ToString(),-14}     \"{Value.Replace("\n", "\\n")}\""
                    : $"{Line,-5}  {Position,-5}   {Type.ToString(),-14}");
        }
    }
}