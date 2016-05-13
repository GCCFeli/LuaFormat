using Irony.Parsing;
using Lua.LanguageService.Format;
using System.Collections.Generic;

namespace Lua.LanguageService.Grammar.Ast
{
    interface IAstNode
    {
        ParseTreeNode Node
        {
            get;
        }

        NodeType NodeType
        {
            get;
        }

        void Indent(LuaIndentState indentState, Dictionary<SourceLocation, LuaTokenInfo> tokenMap);
    }
}
