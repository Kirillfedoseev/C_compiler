using System.IO;
using Compiler.Lexing;
using Compiler.Syntaxing;

namespace Compiler
{
    class Program
    {

        static void Main(string[] args)
        {
            string code = File.ReadAllText("in.txt");

            // strip windows line endings out
            code = code.Replace("\r", "");

            Lexer lexer = new Lexer();
            string tokens = lexer.MakeAnalyze(code);

            Syntaxer syntaxer = new Syntaxer();
            string result = syntaxer.MakeAnalyze(tokens);

            File.WriteAllText("out.txt", result);
        }
    }
}

