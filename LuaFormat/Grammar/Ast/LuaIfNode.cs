using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;
using Lua.LanguageService.Format;
using System.Collections.Generic;

namespace Lua.LanguageService.Grammar.Ast
{
    class LuaIfNode : AstNode, IAstNode
    {
        public ParseTreeNode Node
        {
            get;
            private set;
        }

        public AstNode Conditions;

        public AstNode IfTrue;

        public AstNode IfFalse;

        public NodeType NodeType
        {
            get
            {
                return NodeType.LuaIf;
            }
        }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            Node = treeNode;
            Conditions = AddChild("Conditions", treeNode.ChildNodes[1]);
            IfTrue = AddChild("IfTrue", treeNode.ChildNodes[3]);
            foreach (ParseTreeNode current in treeNode.ChildNodes)
            {
                if (current.ToString() == "ElseIfClause*" && current.ChildNodes.Count > 0)
                {
                    foreach (ParseTreeNode current2 in current.ChildNodes)
                    {
                        AddChild("ElseIf", current2);
                    }
                }
                if (current.ToString() == "ElseClause" && current.ChildNodes.Count > 0)
                {
                    IfFalse = AddChild("IfFalse", current.ChildNodes[1]);
                }
            }
        }

        public void Indent(LuaIndentState indentState, Dictionary<SourceLocation, LuaTokenInfo> tokenMap)
        {
            {
                var startLocation = indentState.GetFirstLine(Conditions);
                var endLocation = indentState.GetLastLine(Conditions);
                indentState.Indent(startLocation, endLocation, this);
            }

            {
                var startLocation = indentState.GetFirstLine(IfTrue);
                var endLocation = indentState.GetLastLine(IfTrue);
                indentState.Indent(startLocation, endLocation, this);
            }

            if (IfFalse != null)
            {
                var startLocation = indentState.GetFirstLine(IfFalse);
                var endLocation = indentState.GetLastLine(IfFalse);
                indentState.Indent(startLocation, endLocation, this);
            }
        }
    }
}
