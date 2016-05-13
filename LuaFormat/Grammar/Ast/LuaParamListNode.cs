using Irony.Ast;
using Irony.Parsing;
using Lua.LanguageService.Format;
using System.Collections.Generic;

namespace Lua.LanguageService.Grammar.Ast
{
    class LuaParamListNode : AstNode, IAstNode
    {
        public ParseTreeNode Node
        {
            get;
            private set;
        }

        public Token EndToken;

        public NodeType NodeType
        {
            get
            {
                return NodeType.LuaParmList;
            }
        }

        private static void RecursiveChildTraversal(List<ParseTreeNode> leafNodes, ParseTreeNode node)
        {
            if (node.ChildNodes.Count == 0)
            {
                leafNodes.Add(node);
                return;
            }
            foreach (ParseTreeNode current in node.ChildNodes)
            {
                LuaParamListNode.RecursiveChildTraversal(leafNodes, current);
            }
        }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            Node = treeNode;
            EndToken = context.CurrentToken;
            List<ParseTreeNode> list = new List<ParseTreeNode>();
            if (treeNode.ChildNodes.Count > 0)
            {
                LuaParamListNode.RecursiveChildTraversal(list, treeNode);
            }
            AsString = " (";
            int num = 0;
            foreach (ParseTreeNode current in list)
            {
                AddChild("Parameter", current);
                AsString += ((num == 0) ? current.ToString() : (", " + current));
                num++;
            }
            AsString += ")";
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
