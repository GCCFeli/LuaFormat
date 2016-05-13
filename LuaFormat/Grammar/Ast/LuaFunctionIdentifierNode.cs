using Irony.Ast;
using Irony.Parsing;
using Lua.LanguageService.Format;
using System.Collections.Generic;

namespace Lua.LanguageService.Grammar.Ast
{
    class LuaFunctionIdentifierNode : AstNode, IAstNode
    {
        public ParseTreeNode Node
        {
            get;
            private set;
        }

        private static int anonID;

        private string name = "";

        private string[] fullname;

        public List<string> NameSpace = new List<string>();

        public AstNode Name;

        public bool IsAnonymous;

        public string[] FullName
        {
            get
            {
                return fullname;
            }
        }

        public NodeType NodeType
        {
            get
            {
                return NodeType.LuaFuncIdentifier;
            }
        }

        internal LuaFunctionIdentifierNode InitAnonymous()
        {
            name = "anonfunc" + LuaFunctionIdentifierNode.anonID++;
            AsString = name;
            IsAnonymous = true;
            return this;
        }

        public override void Init(ParsingContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            Node = treeNode;
            ParseTreeNode parseTreeNode = null;
            ParseTreeNode parseTreeNode2 = null;
            foreach (ParseTreeNode current in treeNode.ChildNodes)
            {
                if (current.Term.Name == "identifier including namespace")
                {
                    parseTreeNode = current;
                }
                if (current.Term.Name == "colon call")
                {
                    parseTreeNode2 = current;
                }
            }
            if (parseTreeNode2.ChildNodes.Count > 1)
            {
                Name = AddChild("Name", parseTreeNode2.ChildNodes[1]);
                for (int i = 0; i < parseTreeNode.ChildNodes.Count; i++)
                {
                    NameSpace.Add(parseTreeNode.ChildNodes[i].Token.Text);
                }
            }
            else if (parseTreeNode.ChildNodes.Count > 0)
            {
                Name = AddChild("Name", parseTreeNode.ChildNodes[parseTreeNode.ChildNodes.Count - 1]);
                for (int j = 0; j < parseTreeNode.ChildNodes.Count - 1; j++)
                {
                    NameSpace.Add(parseTreeNode.ChildNodes[j].Token.Text);
                }
            }
            fullname = new string[NameSpace.Count + 1];
            for (int k = 0; k < NameSpace.Count; k++)
            {
                name += NameSpace[k];
                if (k < NameSpace.Count - 1)
                {
                    name += ".";
                }
                fullname[k] = NameSpace[k];
            }
            if (NameSpace.Count > 0)
            {
                name = name + ":" + Name.AsString;
            }
            else
            {
                name = Name.AsString;
            }
            fullname[NameSpace.Count] = Name.AsString;
            AsString = name;
        }

        public void Indent(LuaIndentState indentState, Dictionary<SourceLocation, LuaTokenInfo> tokenMap)
        {
            // no indent needed
        }
    }
}
