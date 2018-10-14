using System;
using System.IO;
using Compiler.Units.SimpleUnits;
using Newtonsoft.Json;

namespace Compiler
{
    class Program
    {

        static void Main(string[] args)
        {
            //string code = File.ReadAllText("in.txt");

            //// strip windows line endings out
            //code = code.Replace("\r", "");

            //Lexing.Lexer lexer = new Lexing.Lexer();
            //List<Token> tokens = lexer.Scan(code);

            //Syntaxer syntaxer = new Syntaxer();
            //string result = syntaxer.MakeAnalyze(tokens);


            BinaryOpsUnit unit = new BinaryOpsUnit("*", new NumUnit("123"), new IdenUnit("a"));
            var jset = new JsonSerializerSettings();
            jset.Formatting = Formatting.Indented;
            string s = JsonConvert.SerializeObject(unit, jset);

            File.WriteAllText("out.txt", s);

        }
    }
}

