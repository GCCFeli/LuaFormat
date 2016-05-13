using Irony.Ast;
using Irony.Parsing;
using Lua.LanguageService.Format;
using System.Collections.Generic;

namespace Lua.LanguageService.Grammar.Ast
{
    class LuaExpressionListNode : ExpressionListNode, IAstNode
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
                return NodeType.LuaExpressionList;
            }
        }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            Node = treeNode;
            AsString = "LuaExprList";
        }

        public void Indent(LuaIndentState indentState, Dictionary<SourceLocation, LuaTokenInfo> tokenMap)
        {
            if (ChildNodes.Count > 0)
            {
                var startLocation = indentState.GetFirstLine(ChildNodes[0]);
                var endLocation = indentState.GetLastLine(ChildNodes[ChildNodes.Count - 1]);
                indentState.Indent(startLocation, endLocation, Parent);
            }
        }
    }
}
