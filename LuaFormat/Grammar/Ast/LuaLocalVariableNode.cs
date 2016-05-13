using Irony.Ast;
using Irony.Parsing;
using Lua.LanguageService.Format;
using System.Collections.Generic;

namespace Lua.LanguageService.Grammar.Ast
{
    class LuaLocalVariableNode : AstNode, IAstNode
    {
        public ParseTreeNode Node
        {
            get;
            private set;
        }

        public AstNode Variables;

        public NodeType NodeType
        {
            get
            {
                return NodeType.LuaLocalVariable;
            }
        }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            Node = treeNode;
            Variables = AddChild("Variables", treeNode.ChildNodes[1]);
            AsString = "LuaLocalVariable";
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
