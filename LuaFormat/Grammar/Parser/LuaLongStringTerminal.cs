using Irony.Parsing;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lua.LanguageService.Grammar.Parser
{
    class LuaLongStringTerminal : Terminal
    {
        public string StartSymbol = "[";

        public LuaLongStringTerminal(string name) : base(name, TokenCategory.Content)
        {
        }

        public override void Init(GrammarData grammarData)
        {
            base.Init(grammarData);
            base.SetFlag(TermFlags.IsMultiline);
            if (EditorInfo == null)
            {
                EditorInfo = new TokenEditorInfo(TokenType.String, TokenColor.String, TokenTriggers.None);
            }
        }

        public override Token TryMatch(ParsingContext context, ISourceStream source)
        {
            Token token;
            if (context.VsLineScanState.Value != 0)
            {
                byte tokenSubType = context.VsLineScanState.TokenSubType;
                token = CompleteMatch(context, source, tokenSubType);
            }
            else
            {
                byte level = 0;
                if (!BeginMatch(context, source, ref level))
                {
                    return null;
                }
                token = CompleteMatch(context, source, level);
            }
            if (token != null)
            {
                return token;
            }
            if (context.Mode == ParseMode.VsLineScan)
            {
                return CreateIncompleteToken(context, source);
            }
            return source.CreateErrorToken("Unclosed comment block", new object[0]);
        }

        private Token CreateIncompleteToken(ParsingContext context, ISourceStream source)
        {
            source.PreviewPosition = source.Text.Length;
            Token expr_1D = source.CreateToken(base.OutputTerminal);
            expr_1D.Flags |= TokenFlags.IsIncomplete;
            context.VsLineScanState.TerminalIndex = MultilineIndex;
            return expr_1D;
        }

        private bool BeginMatch(ParsingContext context, ISourceStream source, ref byte level)
        {
            if (!source.MatchSymbol(StartSymbol, !base.Grammar.CaseSensitive))
            {
                return false;
            }
            Match match = Regex.Match(source.Text.Substring(source.PreviewPosition + StartSymbol.Length), "^(=*)\\[");
            if (match.Value != string.Empty)
            {
                level = (byte)match.Groups[1].Value.Length;
                return true;
            }
            return false;
        }

        private Token CompleteMatch(ParsingContext context, ISourceStream source, byte level)
        {
            foreach (Match match in Regex.Matches(source.Text.Substring(source.PreviewPosition), "\\](=*)\\]"))
            {
                if (match.Groups[1].Value.Length == (int)level)
                {
                    source.PreviewPosition += match.Index + match.Length;
                    Token result;
                    if (context.VsLineScanState.Value != 0)
                    {
                        SourceLocation location = default(SourceLocation);
                        location.Position = 0;
                        string text = source.Text.Substring(0, source.PreviewPosition);
                        context.VsLineScanState.Value = 0;
                        result = new Token(this, location, text, null);
                        return result;
                    }
                    result = source.CreateToken(base.OutputTerminal);
                    return result;
                }
            }
            context.VsLineScanState.TerminalIndex = MultilineIndex;
            context.VsLineScanState.TokenSubType = level;
            return null;
        }

        public override IList<string> GetFirsts()
        {
            return new string[]
            {
                StartSymbol
            };
        }
    }
}
