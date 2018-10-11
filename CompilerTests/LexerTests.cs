using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Compiler;


namespace LexerTests
{
    [TestClass]
    public class LexerTests
    {


        private Lexer _lexical;

        
        public LexerTests()
        {
            _lexical = new Lexer();
        }

        public bool AreEqual(String expected, String actual)
        {
            Console.WriteLine(actual);
            expected = expected.Replace(" ", "").Replace("\n", "");
            actual = actual.Replace(" ", "").Replace("\n", "");
            return expected.Equals(actual);
        }

        [TestMethod]
        public void TestCase1()
        {
             String in_ =
                "/*\n" +
                "  Hello world\n" +
                " */\n" +
                "print(\"Hello, World!\\n\");";
            String expected =
                "4 1 Keyword_print\n" +
                "4 6 LeftParen\n" +
                "4 7 String \"Hello, World!\\n\"\n" +
                "4 24 RightParen\n" +
                "4 25 Semicolon\n" +
                "5 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void TestCase2()
        {
            String in_ =
                "/*\n" +
                "  Show Ident and Integers\n" +
                " */\n" +
                "phoenix_number = 142857;\n" +
                "print(phoenix_number, \"\\n\");";
            String expected =
                "4 1 Identifier phoenix_number\n" +
                "4 16 Op_assign\n" +
                "4 18 Integer 142857\n" +
                "4 24 Semicolon\n" +
                "5 1 Keyword_print\n" +
                "5 6 LeftParen\n" +
                "5 7 Identifier phoenix_number\n" +
                "5 21 Comma\n" +
                "5 23 String \"\\n\"\n" +
                "5 27 RightParen\n" +
                "5 28 Semicolon\n" +
                "6 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void TestCase3()
        {
            String in_ =
                "/*\n" +
                "  All lexical tokens - not syntactically correct, but that will\n" +
                "  have to wait until syntax analysis\n" +
                " */\n" +
                "/* Print   */  print    /* Sub     */  -\n" +
                "/* Putc    */  putc     /* Lss     */  <\n" +
                "/* If      */  if       /* Gtr     */  >\n" +
                "/* Else    */  else     /* Leq     */  <=\n" +
                "/* While   */  while    /* Geq     */  >=\n" +
                "/* Lbrace  */  {        /* Eq      */  ==\n" +
                "/* Rbrace  */  }        /* Neq     */  !=\n" +
                "/* Lparen  */  (        /* And     */  &&\n" +
                "/* Rparen  */  )        /* Or      */  ||\n" +
                "/* Uminus  */  -        /* Semi    */  ;\n" +
                "/* Not     */  !        /* Comma   */  ,\n" +
                "/* Mul     */  *        /* Assign  */  =\n" +
                "/* Div     */  /        /* Integer */  42\n" +
                "/* Mod     */  %        /* String  */  \"String literal\"\n" +
                "/* Add     */  +        /* Ident   */  variable_name\n" +
                "/* character literal */  '\\n'\n" +
                "/* character literal */  '\\\\'\n" +
                @"/* character literal */  ' '";
            String expected =
                "5 16 Keyword_print\n" +
                "5 40 Op_subtract\n" +
                "6 16 Keyword_putc\n" +
                "6 40 Op_less\n" +
                "7 16 Keyword_if\n" +
                "7 40 Op_greater\n" +
                "8 16 Keyword_else\n" +
                "8 40 Op_lessequal\n" +
                "9 16 Keyword_while\n" +
                "9 40 Op_greaterequal\n" +
                "10 16 LeftBrace\n" +
                "10 40 Op_equal\n" +
                "11 16 RightBrace\n" +
                "11 40 Op_notequal\n" +
                "12 16 LeftParen\n" +
                "12 40 Op_and\n" +
                "13 16 RightParen\n" +
                "13 40 Op_or\n" +
                "14 16 Op_subtract\n" +
                "14 40 Semicolon\n" +
                "15 16 Op_not\n" +
                "15 40 Comma\n" +
                "16 16 Op_multiply\n" +
                "16 40 Op_assign\n" +
                "17 16 Op_divide\n" +
                "17 40 Integer 42\n" +
                "18 16 Op_mod\n" +
                "18 40 String \"String literal\"\n" +
                "19 16 Op_add\n" +
                "19 40 Identifier variable_name\n" +
                "20 26 Char '\\n'\n" +
                "21 26 Char '\\\\'\n" +
                "22 26 Char ' '\n" +
                "23 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void TestCase4()
        {
            String in_ =
                "/*** test printing, embedded \\n and comments with lots of '*' ***/\n" +
                "print(42);\n" +
                "print(\"\\nHello World\\nGood Bye\\nok\\n\");\n" +
                "print(\"Print a slash n - \\\\n.\\n\");";
            String expected =
                "2 1 Keyword_print\n" +
                "2 6 LeftParen\n" +
                "2 7 Integer 42\n" +
                "2 9 RightParen\n" +
                "2 10 Semicolon\n" +
                "3 1 Keyword_print\n" +
                "3 6 LeftParen\n" +
                "3 7 String \"\\nHello World\\nGood Bye\\nok\\n\"\n" +
                "3 38 RightParen\n" +
                "3 39 Semicolon\n" +
                "4 1 Keyword_print\n" +
                "4 6 LeftParen\n" +
                @"4 7 String ""Print a slash n - \\n.\n""
" +
                "4 33 RightParen\n" +
                "4 34 Semicolon\n" +
                "5 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        // Operators testing

        [TestMethod]
        public void AssignmentTest()
        {
            String in_ = "a = b;";
            String expected =
                "1 1 Identifier a\n" +
                "1 3 Op_assign\n" +
                "1 5 Identifier b\n" +
                "1 6 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void AdditionTest()
        {
            String in_ = "a+b;";
            String expected =
                "1 1 Identifier a\n" +
                "1 2 Op_add\n" +
                "1 3 Identifier b\n" +
                "1 4 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void ModTest()
        {
            String in_ = "a%b;";
            String expected =
                "1 1 Identifier a\n" +
                "1 2 Op_mod\n" +
                "1 3 Identifier b\n" +
                "1 4 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void PrefixIncrementTest()
        {
            String in_ = "++a;";
            String expected =
                "1 1 Op_pre_inc\n" +
                "1 3 Identifier a\n" +
                "1 4 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void SuffixIncrementTest()
        {
            String in_ = "a++;";
            String expected =
                "1 1 Identifier a\n" +
                "1 2 Op_suf_inc\n" +
                "1 4 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void PrefixDecrementTest()
        {
            String in_ = "--a;";
            String expected =
                "1 1 Op_pre_dec\n" +
                "1 3 Identifier a\n" +
                "1 4 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void SuffixDecrementTest()
        {
            String in_ = "a--;";
            String expected =
                "1 1 Identifier a\n" +
                "1 2 Op_suf_dec\n" +
                "1 4 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void BitwiseNotTest()
        {
            String in_ = "~a;";
            String expected =
                "1 1 Op_bit_not\n" +
                "1 2 Identifier a\n" +
                "1 3 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void BitwiseAndTest()
        {
            String in_ = "a & b;";
            String expected =
                "1 1 Identifier a\n" +
                "1 3 Op_bit_and\n" +
                "1 5 Identifier b\n" +
                "1 6 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }


        [TestMethod]
        public void BitwiseOrTest()
        {
            String in_ = "a|b;";
            String expected =
                "1 1 Identifier a\n" +
                "1 2 Op_bit_or\n" +
                "1 3 Identifier b\n" +
                "1 4 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void BitwiseXorTest()
        {
            String in_ = "a^b;";
            String expected =
                "1 1 Identifier a\n" +
                "1 2 Op_bit_xor\n" +
                "1 3 Identifier b\n" +
                "1 4 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void BitwiseLeftShiftTest()
        {
            String in_ = "a<<b;";
            String expected =
                "1 1 Identifier a\n" +
                "1 2 Op_bit_left_shift\n" +
                "1 4 Identifier b\n" +
                "1 5 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void BitwiseRightShiftTest()
        {
            String in_ = "a>>b;";
            String expected =
                "1 1 Identifier a\n" +
                "1 2 Op_bit_right_shift\n" +
                "1 4 Identifier b\n" +
                "1 5 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void AdditionAssignmentTest()
        {
            String in_ = "a+=b;";
            String expected =
                "1 1 Identifier a\n" +
                "1 2 Op_add_assign\n" +
                "1 4 Identifier b\n" +
                "1 5 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void SubtractionAssignmentTest()
        {
            String in_ = "a-=b;";
            String expected =
                "1 1 Identifier a\n" +
                "1 2 Op_sub_assign\n" +
                "1 4 Identifier b\n" +
                "1 5 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void MultiplicationAssignmentTest()
        {
            String in_ = "a*=b;";
            String expected =
                "1 1 Identifier a\n" +
                "1 2 Op_multiply_assign\n" +
                "1 4 Identifier b\n" +
                "1 5 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void BitwiseAndAssignmentTest()
        {
            String in_ = "a&=b;";
            String expected =
                "1 1 Identifier a\n" +
                "1 2 Op_bit_and_assign\n" +
                "1 4 Identifier b\n" +
                "1 5 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void BitwiseOrAssignmentTest()
        {
            String in_ = "a|=b;";
            String expected =
                "1 1 Identifier a\n" +
                "1 2 Op_bit_or_assign\n" +
                "1 4 Identifier b\n" +
                "1 5 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void BitwiseXorAssignmentTest()
        {
            String in_ = "a^=b;";
            String expected =
                "1 1 Identifier a\n" +
                "1 2 Op_bit_xor_assign\n" +
                "1 4 Identifier b\n" +
                "1 5 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void BitwiseLeftShiftAssignmentTest()
        {
            String in_ = "a<<=b;";
            String expected =
                "1 1 Identifier a\n" +
                "1 2 Op_bit_left_shift_assign\n" +
                "1 5 Identifier b\n" +
                "1 6 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void BitwiseRightShiftAssignmentTest()
        {
            String in_ = "a>>=b;";
            String expected =
                "1 1 Identifier a\n" +
                "1 2 Op_bit_right_shift_assign\n" +
                "1 5 Identifier b\n" +
                "1 6 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void PointerTest()
        {
            String in_ = "*a;";
            String expected =
                "1 1 Op_pointer\n" +
                "1 2 Identifier a\n" +
                "1 3 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void ReferenceTest()
        {
            String in_ = "&a;";
            String expected =
                "1 1 Op_reference\n" +
                "1 2 Identifier a\n" +
                "1 3 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void StructureReferenceTest()
        {
            String in_ = "a.b;";
            String expected =
                "1 1 Identifier a\n" +
                "1 2 Op_struct_reference\n" +
                "1 3 Identifier b\n" +
                "1 4 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        // Symbols testing

        [TestMethod]
        public void SquareBracketsTest()
        {
            String in_ = "Arr[20];";
            String expected =
                "1 1 Identifier Arr\n" +
                "1 4 LeftSquareBracket\n" +
                "1 5 Integer 20\n" +
                "1 7 RightSquareBracket\n" +
                "1 8 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void IncludeTest()
        {
            String in_ = "#include <header_file>;";
            String expected =
                "1 1 Include header_file\n" +
                "1 23 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        // Keywords testing

        [TestMethod]
        public void AutoKeywordTest()
        {
            String in_ = "auto;";
            String expected =
                "1 1 Keyword_auto\n" +
                "1 5 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void DoubleKeywordTest()
        {
            String in_ = "double;";
            String expected =
                "1 1 Keyword_double\n" +
                "1 7 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void IntKeywordTest()
        {
            String in_ = "int;";
            String expected =
                "1 1 Keyword_int\n" +
                "1 4 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void StructKeywordTest()
        {
            String in_ = "struct;";
            String expected =
                "1 1 Keyword_struct\n" +
                "1 7 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void BreakKeywordTest()
        {
            String in_ = "break;";
            String expected =
                "1 1 Keyword_break\n" +
                "1 6 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void ElseKeywordTest()
        {
            String in_ = "else";
            String expected =
                "1 1 Keyword_else\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void LongKeywordTest()
        {
            String in_ = "long a;";
            String expected =
                "1 1 Keyword_long\n" +
                "1 6 Identifier a\n" +
                "1 7 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void SwitchKeywordTest()
        {
            String in_ = "switch;";
            String expected =
                "1 1 Keyword_switch\n" +
                "1 7 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void CaseKeywordTest()
        {
            String in_ = "case;";
            String expected =
                "1 1 Keyword_case\n" +
                "1 5 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void EnumKeywordTest()
        {
            String in_ = "enum;";
            String expected =
                "1 1 Keyword_enum\n" +
                "1 5 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void RegisterKeywordTest()
        {
            String in_ = "register;";
            String expected =
                "1 1 Keyword_register\n" +
                "1 9 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void TypedefKeywordTest()
        {
            String in_ = "typedef;";
            String expected =
                "1 1 Keyword_typedef\n" +
                "1 8 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void CharKeywordTest()
        {
            String in_ = "char a;";
            String expected =
                "1 1 Keyword_char\n" +
                "1 6 Identifier a" +
                "1 7 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void ExternKeywordTest()
        {
            String in_ = "extern;";
            String expected =
                "1 1 Keyword_extern\n" +
                "1 7 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void ReturnKeywordTest()
        {
            String in_ = "return;";
            String expected =
                "1 1 Keyword_return\n" +
                "1 7 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void UnionKeywordTest()
        {
            String in_ = "union;";
            String expected =
                "1 1 Keyword_union\n" +
                "1 6 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void ContinueKeywordTest()
        {
            String in_ = "continue;";
            String expected =
                "1 1 Keyword_continue\n" +
                "1 9 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void ForKeywordTest()
        {
            String in_ = "for;";
            String expected =
                "1 1 Keyword_for\n" +
                "1 4 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void SignedKeywordTest()
        {
            String in_ = "signed;";
            String expected =
                "1 1 Keyword_signed\n" +
                "1 7 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void VoidKeywordTest()
        {
            String in_ = "void;";
            String expected =
                "1 1 Keyword_void\n" +
                "1 5 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void DoKeywordTest()
        {
            String in_ = "do;";
            String expected =
                "1 1 Keyword_do\n" +
                "1 3 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void IfKeywordTest()
        {
            String in_ = "if";
            String expected =
                "1 1 Keyword_if\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void StaticKeywordTest()
        {
            String in_ = "static;";
            String expected =
                "1 1 Keyword_static\n" +
                "1 7 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void WhileKeywordTest()
        {
            String in_ = "while";
            String expected =
                "1 1 Keyword_while\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void DefaultKeywordTest()
        {
            String in_ = "default;";
            String expected =
                "1 1 Keyword_default\n" +
                "1 8 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void GotoKeywordTest()
        {
            String in_ = "goto;";
            String expected =
                "1 1 Keyword_goto\n" +
                "1 5 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void SizeofKeywordTest()
        {
            String in_ = "sizeof";
            String expected =
                "1 1 Keyword_sizeof\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void VolatileKeywordTest()
        {
            String in_ = "volatile;";
            String expected =
                "1 1 Keyword_volatile\n" +
                "1 9 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void ConstKeywordTest()
        {
            String in_ = "const;";
            String expected =
                "1 1 Keyword_const\n" +
                "1 6 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void FloatKeywordTest()
        {
            String in_ = "float;";
            String expected =
                "1 1 Keyword_float\n" +
                "1 6 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void ShortKeywordTest()
        {
            String in_ = "short;";
            String expected =
                "1 1 Keyword_short\n" +
                "1 6 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void UnsignedKeywordTest()
        {
            String in_ = "unsigned;";
            String expected =
                "1 1 Keyword_unsigned\n" +
                "1 9 Semicolon\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        // Data type testing

        [TestMethod]
        public void IntegerTest()
        {
            String in_ = "12";
            String expected =
                "1 1 Integer 12\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void CharacterTest()
        {
            String in_ = "\'A\'";
            String expected =
                "1 1 Char 'A'" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

        [TestMethod]
        public void FloatingTest()
        {
            String in_ = "15.786";
            String expected =
                "1 1 Float 15.786\n" +
                "2 1 End_of_input";
            String actual;
            actual = _lexical.MakeAnalyze(in_);
            Assert.IsTrue(AreEqual(expected, actual));
        }

    }
}
