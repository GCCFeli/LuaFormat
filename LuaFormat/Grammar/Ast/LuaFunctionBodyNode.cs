using Irony.Ast;
using Irony.Parsing;
using Lua.LanguageService.Format;
using System.Collections.Generic;

namespace Lua.LanguageService.Grammar.Ast
{
    class LuaFunctionBodyNode : StatementListNode, IAstNode
    {
        public ParseTreeNode Node
        {
            get;
            private set;
        }

        public NodeType NodeType
        {
            get
            {
                return NodeType.LuaChunk;
            }
        }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            Node = treeNode;
            AsString = "LuaFunctionBody";
        }

        public void Indent(LuaIndentState indentState, Dictionary<SourceLocation, LuaTokenInfo> tokenMap)
        {
            if (ChildNodes[0].ChildNodes.Count > 0)
            {
                var startLocation = indentState.GetFirstLine(ChildNodes[0].ChildNodes[0]);
                var endLocation = indentState.GetLastLine(ChildNodes[0].ChildNodes[ChildNodes[0].ChildNodes.Count - 1]);
                indentState.Indent(startLocation, endLocation, Parent);
            }
        }
    }
}
