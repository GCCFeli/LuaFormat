using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;
using Lua.LanguageService.Format;
using System.Collections.Generic;

namespace Lua.LanguageService.Grammar.Ast
{
    abstract class LuaFunctionDefNodeBase : AstNode, IAstNode
    {
        public ParseTreeNode Node
        {
            get;
            private set;
        }

        public AstNode NameNode;

        public AstNode Parameters;

        public AstNode Body;

        private Token BeginRegionToken;

        private Token EndRegionToken;

        public ParseTree CurrentParseTree
        {
            get;
            private set;
        }

        public abstract bool IsLocal
        {
            get;
        }

        public NodeType NodeType
        {
            get
            {
                return NodeType.LuaFunctionDef;
            }
        }

        public virtual void InitFunction(ParseTreeNode treeNode)
        {
            int num = treeNode.ChildNodes.Count - 1;
            ParseTreeNode parseTreeNode = treeNode.ChildNodes[num - 2];
            if (parseTreeNode != null && parseTreeNode.Token != null && parseTreeNode.Token.KeyTerm != null)
            {
                NameNode = new LuaFunctionIdentifierNode().InitAnonymous();
            }
            else
            {
                NameNode = AddChild("Name", parseTreeNode);
            }
            Parameters = AddChild("Parameters", treeNode.ChildNodes[num - 1]);
            Body = AddChild("Body", treeNode.ChildNodes[num]);
            AsString = "<Function " + NameNode.AsString + ">";
            LuaParamListNode luaParmListNode = Parameters as LuaParamListNode;
            if (luaParmListNode != null)
            {
                BeginRegionToken = luaParmListNode.EndToken;
                return;
            }
            BeginRegionToken = treeNode.ChildNodes[num - 1].FindToken();
        }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            Node = treeNode;
            InitFunction(treeNode);
            CurrentParseTree = context.CurrentParseTree;
            EndRegionToken = context.CurrentToken;
        }

        public void Indent(LuaIndentState indentState, Dictionary<SourceLocation, LuaTokenInfo> tokenMap)
        {
            // no indent needed
        }
    }
}
