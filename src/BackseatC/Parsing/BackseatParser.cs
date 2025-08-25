using Silverfly;
using Silverfly.Lexing.IgnoreMatcher.Comments;
using Silverfly.Lexing.Matcher;
using Silverfly.Lexing.NameAdvancers;
using Silverfly.Parselets;

namespace BackseatC.Parsing;

public class BackseatParser : Parser
{
    protected override void InitLexer(LexerConfig lexer)
    {
        lexer.IgnoreCasing = false;
        lexer.AddKeywords("function", "Function", "true", "false", "and", "or", "not", "if", "else", "loop", "break", "continue", "while", "do", 
                          "for", "mutable", "const", "nothing", "return");
        lexer.AddSymbols("==", "!=", ">", "<", "<=", ">=");
        lexer.AddSymbols("/*", "*/", "//", "\"", "~>", "+", "-", "*", "/", ";");

        lexer.UseNameAdvancer(new CStyleNameAdvancer());
        lexer.MatchString("\"", "\"");

        //Todo: replace NumberMatcher with custom algorithm to match with backseat number rules
        lexer.AddMatcher(new NumberMatcher(allowHex: false, allowBin: false, floatingPointSymbol: ".", seperatorSymbol: "'"));

        lexer.IgnoreWhitespace();
        lexer.Ignore(new SingleLineCommentIgnoreMatcher("//"));
    }

    protected override void InitParser(ParserDefinition def)
    {
        def.AddCommonLiterals();
        def.AddArithmeticOperators();

        def.Register("(", new CallParselet(def.PrecedenceLevels.GetPrecedence("Call")));
        def.Register(PredefinedSymbols.Name, new NameParselet());

        def.Block(PredefinedSymbols.SOF, PredefinedSymbols.EOF, ";");

        def.Block("{", "}", PredefinedSymbols.EOL);
    }
}
