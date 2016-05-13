using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;
using Lua.LanguageService.Format;
using System.Collections.Generic;

namespace Lua.LanguageService.Grammar.Ast
{
    class LuaFunctionCallNode : AstNode, IAstNode
    {
        public ParseTreeNode Node
        {
            get;
            private set;
        }

        public AstNode TargetRef;

        public AstNode Arguments;

        public AstNode RefrerenceElement;

        private string _targetName;

        public NodeType NodeType
        {
            get
            {
                return NodeType.LuaFunctionCall;
            }
        }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            Node = treeNode;
            if (treeNode.ChildNodes.Count >= 2)
            {
                ParseTreeNode targetNode = treeNode.ChildNodes[0];
                ParseTreeNode argumentsNode = treeNode.ChildNodes[1];
                if (treeNode.ChildNodes.Count == 4)
                {
                    RefrerenceElement = AddChild("Reference", treeNode.ChildNodes[0]);
                    targetNode = treeNode.ChildNodes[2];
                    argumentsNode = treeNode.ChildNodes[3];
                }
                TargetRef = AddChild("Target", targetNode);
                Arguments = AddChild("Args", argumentsNode);
                string text = "";
                if (targetNode.Term.Name == "identifier")
                {
                    text += targetNode.FindTokenAndGetText();
                }
                else
                {
                    text += TargetRef.AsString;
                }
                if (targetNode.Term.FlagIsSet(TermFlags.IsOperator))
                {
                    text += targetNode.Term.Name;
                }
                _targetName = text;
                AsString = "Call " + _targetName;
            }
        }

        public void Indent(LuaIndentState indentState, Dictionary<SourceLocation, LuaTokenInfo> tokenMap)
        {
            if (Arguments != null && Arguments.ChildNodes.Count > 0)
            {
                var startLocation = indentState.GetFirstLine(Arguments.ChildNodes[0]);
                var endLocation = indentState.GetLastLine(Arguments.ChildNodes[Arguments.ChildNodes.Count - 1]);
                indentState.Indent(startLocation, endLocation, this);
            }
        }
    }
}
