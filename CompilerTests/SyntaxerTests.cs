using Compiler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LexerTests
{
    [TestClass]
    class SyntaxerTests
    {

        private Lexer _lexical;
        private Syntaxer _syntaxer;


        public SyntaxerTests()
        {
            _lexical = new Lexer();
            _syntaxer = new Syntaxer();
        }

        /// <summary>
        /// Метод, которые убирает пробелы и переносы строки, чтобы они не влияли на исход теста
        /// </summary>
        /// <param name="expected">ожидаемый выход</param>
        /// <param name="actual">действительный выход</param>
        /// <returns></returns>
        public bool AreEqual(String expected, String actual)
        {
            expected = expected.Replace(" ", "").Replace("\n", "");
            actual = actual.Replace(" ", "").Replace("\n", "");
            return expected.Equals(actual);
        }

        [TestMethod]
        public void TestCase1()
        {
            String in_ = ""; //исходный код
            String expected = ""; //предполагаемый ответ
            String lexems = _lexical.MakeAnalyze(in_); //сначала исходный код делится на лексемы
            String actual = _syntaxer.MakeAnalyze(lexems);//потом лексемы превращаются в program tree
            Assert.IsTrue(AreEqual(expected, actual));//проверка на совпадение
        }

        [TestMethod]
        public void MainTest()
        {
            String in_ = "sum = lambda(a, b) {\n" +
                "  a + b;\n" +
                "};\n" +
                "print(sum(1, 2));";
            String expected = "{\n" +
                "  type: \"prog\",\n" +
                "  prog: [\n" +
                "    {\n" +
                "      type: \"assign\",\n" +
                "      operator: \"=\",\n" +
                "      left: { type: \"var\", value: \"sum\" },\n" +
                "      right: {\n" +
                "        type: \"lambda\",\n" +
                "        vars: [ \"a\", \"b\" ],\n" +
                "        body: {\n" +
                "          type: \"binary\",\n" +
                "          operator: \"+\",\n" +
                "          left: { type: \"var\", value: \"a\" },\n" +
                "          right: { type: \"var\", value: \"b\" }\n" +
                "        }\n" +
                "      }\n" +
                "    },\n" +
                "    {\n" +
                "      type: \"call\",\n" +
                "      func: { type: \"var\", value: \"print\" },\n" +
                "      args: [{\n" +
                "        type: \"call\",\n" +
                "        func: { type: \"var\", value: \"sum\" },\n" +
                "        args: [ { type: \"num\", value: 1 },\n" +
                "                { type: \"num\", value: 2 } ]\n" +
                "      }]\n" +
                "    }\n" +
                "  ]\n" +
                "}";
            String lexems = _lexical.MakeAnalyze(in_);
            String actual = _syntaxer.MakeAnalyze(lexems);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void NumbersTest()
        {
            String in_ = "123.5";
            String expected = "{ type: \"num\", value: 123.5 }";
            String lexems = _lexical.MakeAnalyze(in_);
            String actual = _syntaxer.MakeAnalyze(lexems);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void StringTest()
        {
            String in_ = "\"Hello World!\"";
            String expected = "{ type: \"char[]\", value: \"Hello World!\" }";
            String lexems = _lexical.MakeAnalyze(in_);
            String actual = _syntaxer.MakeAnalyze(lexems);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void BooleanTest()
        {
            String in_ = "true\n" +
                "false";
            String expected = "{ type: \"bool\", value: true }\n" +
                "{ type: \"bool\", value: false }";
            String lexems = _lexical.MakeAnalyze(in_);
            String actual = _syntaxer.MakeAnalyze(lexems);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void IdentifierTest()
        {
            String in_ = "foo";
            String expected = "{ type: \"var\", value: \"foo\" }";
            String lexems = _lexical.MakeAnalyze(in_);
            String actual = _syntaxer.MakeAnalyze(lexems);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void FunctionTest()
        {
            String in_ = "lambda (x) 10   # or\n" +
                "λ (x) 10";
            String expected = "{\n" +
                "  type: \"lambda\",\n" +
                "  vars: [ \"x\" ],\n" +
                "  body: { type: \"num\", value: 10 }\n" +
                "}";
            String lexems = _lexical.MakeAnalyze(in_);
            String actual = _syntaxer.MakeAnalyze(lexems);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void FunctionCallTest()
        {
            String in_ = "foo(a, 1)";
            String expected = "{\n" +
                "  \"type\": \"call\",\n" +
                "  \"func\": { \"type\": \"var\", \"value\": \"foo\" },\n" +
                "  \"args\": [\n" +
                "    { \"type\": \"var\", \"value\": \"a\" },\n" +
                "    { \"type\": \"num\", \"value\": 1 }\n" +
                "  ]\n" +
                "}";
            String lexems = _lexical.MakeAnalyze(in_);
            String actual = _syntaxer.MakeAnalyze(lexems);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void ElseTest()
        {
            String in_ = "if foo then bar else baz";
            String expected = "{\n" +
                "  \"type\": \"if\",\n" +
                "  \"cond\": { \"type\": \"var\", \"value\": \"foo\" },\n" +
                "  \"then\": { \"type\": \"var\", \"value\": \"bar\" },\n" +
                "  \"else\": { \"type\": \"var\", \"value\": \"baz\" }\n" +
                "}";
            String lexems = _lexical.MakeAnalyze(in_);
            String actual = _syntaxer.MakeAnalyze(lexems);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void IfConditionalsTest()
        {
            String in_ = "if foo then bar";
            String expected = "{\n" +
                "  \"type\": \"if\",\n" +
                "  \"cond\": { \"type\": \"var\", \"value\": \"foo\" },\n" +
                "  \"then\": { \"type\": \"var\", \"value\": \"bar\" }\n" +
                "}";
            String lexems = _lexical.MakeAnalyze(in_);
            String actual = _syntaxer.MakeAnalyze(lexems);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void WhileCycleTest()
        {
            String in_ = "while(foo)\n" +
                "{\n" +
                "  bar;\n" +
                "}";
            String expected = "{\n" +
                "  \"type\": \"while\",\n" +
                "  \"cond\": { \"type\": \"var\", \"value\": \"foo\" },\n" +
                "  \"body\": { \"type\": \"var\", \"value\": \"bar\" }\n" +
                "}";
            String lexems = _lexical.MakeAnalyze(in_);
            String actual = _syntaxer.MakeAnalyze(lexems);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void AssignmentTest()
        {
            String in_ = "a = 10";
            String expected = "{\n" +
                "  \"type\": \"assign\",\n" +
                "  \"operator\": \"=\",\n" +
                "  \"left\": { \"type\": \"var\", \"value\": \"a\" },\n" +
                "  \"right\": { \"type\": \"num\", \"value\": 10 }\n" +
                "}";
            String lexems = _lexical.MakeAnalyze(in_);
            String actual = _syntaxer.MakeAnalyze(lexems);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void BinaryExpressionTest()
        {
            String in_ = "x + y * z";
            String expected = "{\n" +
                "  \"type\": \"binary\",\n" +
                "  \"operator\": \"+\",\n" +
                "  \"left\": { \"type\": \"var\", \"value\": \"x\" },\n" +
                "  \"right\": {\n" +
                "    \"type\": \"binary\",\n" +
                "    \"operator\": \"*\",\n" +
                "    \"left\": { \"type\": \"var\", \"value\": \"y\" },\n" +
                "    \"right\": { \"type\": \"var\", \"value\": \"z\" }\n" +
                "  }\n" +
                "}";
            String lexems = _lexical.MakeAnalyze(in_);
            String actual = _syntaxer.MakeAnalyze(lexems);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void SequenceTest()
        {
            String in_ = "{\n" +
                "  a = 5;\n" +
                "  b = a * 2;\n" +
                "  a + b;\n" +
                "}";
            String expected = "{\n" +
                "  \"type\": \"prog\",\n" +
                "  \"prog\": [\n" +
                "    {\n" +
                "      \"type\": \"assign\",\n" +
                "      \"operator\": \"=\",\n" +
                "      \"left\": { \"type\": \"var\", \"value\": \"a\" },\n" +
                "      \"right\": { \"type\": \"num\", \"value\": 5 }\n" +
                "    },\n" +
                "    {\n" +
                "      \"type\": \"assign\",\n" +
                "      \"operator\": \"=\",\n" +
                "      \"left\": { \"type\": \"var\", \"value\": \"b\" },\n" +
                "      \"right\": {\n" +
                "        \"type\": \"binary\",\n" +
                "        \"operator\": \"*\",\n" +
                "        \"left\": { \"type\": \"var\", \"value\": \"a\" },\n" +
                "        \"right\": { \"type\": \"num\", \"value\": 2 }\n" +
                "      }\n" +
                "    },\n" +
                "    {\n" +
                "      \"type\": \"binary\",\n" +
                "      \"operator\": \"+\",\n" +
                "      \"left\": { \"type\": \"var\", \"value\": \"a\" },\n" +
                "      \"right\": { \"type\": \"var\", \"value\": \"b\" }\n" +
                "    }\n" +
                "  ]\n" +
                "}";
            String lexems = _lexical.MakeAnalyze(in_);
            String actual = _syntaxer.MakeAnalyze(lexems);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void BlockScopedVariablesTest()
        {
            String in_ = "let (a = 10, b = a * 10) {\n" +
                "  a + b;\n" +
                "}";
            String expected = "{\n" +
                "  \"type\": \"let\",\n" +
                "  \"vars\": [\n" +
                "    {\n" +
                "      \"name\": \"a\",\n" +
                "      \"def\": { \"type\": \"num\", \"value\": 10 }\n" +
                "    },\n" +
                "    {\n" +
                "      \"name\": \"b\",\n" +
                "      \"def\": {\n" +
                "        \"type\": \"binary\",\n" +
                "        \"operator\": \"*\",\n" +
                "        \"left\": { \"type\": \"var\", \"value\": \"a\" },\n" +
                "        \"right\": { \"type\": \"num\", \"value\": 10 }\n" +
                "      }\n" +
                "    }\n" +
                "  ],\n" +
                "  \"body\": {\n" +
                "    \"type\": \"binary\",\n" +
                "    \"operator\": \"+\",\n" +
                "    \"left\": { \"type\": \"var\", \"value\": \"a\" },\n" +
                "    \"right\": { \"type\": \"var\", \"value\": \"b\" }\n" +
                "  }\n" +
                "}";
            String lexems = _lexical.MakeAnalyze(in_);
            String actual = _syntaxer.MakeAnalyze(lexems);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void IncludeTest()
        {
            String in_ = "#include <stdio.h>";
            String expected = "{ type: \"include\", value: stdio.h }";
            String lexems = _lexical.MakeAnalyze(in_);
            String actual = _syntaxer.MakeAnalyze(lexems);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void Include2Test()
        {
            String in_ = "#include \"func.c\"";
            String expected = "{ type: \"include\", value: func.c }";
            String lexems = _lexical.MakeAnalyze(in_);
            String actual = _syntaxer.MakeAnalyze(lexems);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void Test()
        {
            String in_ = "";
            String expected = "";
            String lexems = _lexical.MakeAnalyze(in_);
            String actual = _syntaxer.MakeAnalyze(lexems);
            Assert.IsTrue(AreEqual(expected, actual));
        }


    }
}
