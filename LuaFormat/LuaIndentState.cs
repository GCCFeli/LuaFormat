using Irony.Ast;
using Irony.Parsing;
using Lua.LanguageService.Grammar.Ast;
using System.Collections.Generic;
using System.IO;

namespace Lua.LanguageService.Format
{
    class LuaIndentState
    {
        public Dictionary<int, bool> Indentation
        {
            get;
            private set;
        }

        public Dictionary<int, int> Unindentation
        {
            get;
            private set;
        }

        public string[] Lines
        {
            get;
            private set;
        }

        public LuaIndentState(string text)
        {
            Indentation = new Dictionary<int, bool>();
            Unindentation = new Dictionary<int, int>();

            List<string> lines = new List<string>();

            using (StringReader sr = new StringReader(text))
            {
                while (true)
                {
                    var line = sr.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    lines.Add(line);
                }
            }

            Lines = lines.ToArray();
        }

        public SourceLocation GetFirstLine(AstNode node)
        {
            var iAstNode = node as IAstNode;
            if (iAstNode != null)
            {
                return iAstNode.Node.Span.Location;
            }
            else
            {
                return node.Span.Location;
            }
        }

        private SourceLocation GetFirstLine(ParseTreeNode node)
        {
            return node.Span.Location;
        }

        public SourceLocation GetLastLine(AstNode node)
        {
            var iAstNode = node as IAstNode;
            if (iAstNode != null)
            {
                return GetLastLine(iAstNode.Node);
            }
            else
            {
                return node.Span.Location;
            }
        }

        private SourceLocation GetLastLine(ParseTreeNode node)
        {
            var token = node.FindLastToken();
            if (token != null)
            {
                return token.Location;
            }
            else
            {
                return node.Span.Location;
            }
        }

        public void Indent(SourceLocation startLocation, SourceLocation endLocation, AstNode parent)
        {
            string prefix = Lines[startLocation.Line].Substring(0, startLocation.Column);
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                startLocation.Line++;
            }

            // FIXME: some token's location is (0, 0), so we need this to avoid wrong indention
            if (startLocation.Line == 0)
            {
                return;
            }

            if (endLocation.Line < startLocation.Line)
            {
                return;
            }

            Indentation[startLocation.Line] = true;

            if (!Unindentation.ContainsKey(endLocation.Line + 1))
            {
                Unindentation[endLocation.Line + 1] = GetFirstLine(parent).Line;
            }
        }

        public int[] BuildDepthTable(ParseTreeNode root)
        {
            int currentDepth = 0;
            int lineCount = GetLastLine(root).Line + 1;
            int[] depthTable = new int[lineCount];

            // the line index start from 0
            for (int line = 0; line < lineCount; line++)
            {
                // restore depth
                if (Unindentation.ContainsKey(line))
                {
                    currentDepth = depthTable[Unindentation[line]];
                }

                // indent
                if (Indentation.ContainsKey(line))
                {
                    if (Indentation[line] == true)
                    {
                        currentDepth = currentDepth + 1;
                        depthTable[line] = currentDepth;
                    }
                    else
                    {
                        depthTable[line] = 0;
                    }
                }
                else
                {
                    depthTable[line] = currentDepth;
                }
            }

            return depthTable;
        }
    }
}
