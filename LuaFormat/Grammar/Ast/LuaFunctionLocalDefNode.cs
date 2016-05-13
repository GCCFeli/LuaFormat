using Irony.Ast;
using Irony.Parsing;

namespace Lua.LanguageService.Grammar.Ast
{
    class LuaFunctionLocalDefNode : LuaFunctionDefNodeBase
    {
        public AstNode Local;

        public override bool IsLocal
        {
            get
            {
                return true;
            }
        }

        public override void InitFunction(ParseTreeNode treeNode)
        {
            Local = AddChild("Local", treeNode.ChildNodes[0]);
            treeNode = treeNode.ChildNodes[1];
            base.InitFunction(treeNode);
            AsString = "<Local Function " + NameNode.AsString + ">";
        }
    }
}
