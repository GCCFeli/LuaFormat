using Irony.Parsing;

namespace Lua.LanguageService.Grammar.Ast
{
    class LuaFunctionDefNode : LuaFunctionDefNodeBase
    {
        public override bool IsLocal
        {
            get
            {
                return false;
            }
        }

        public override void InitFunction(ParseTreeNode treeNode)
        {
            base.InitFunction(treeNode);
            AsString = "<Function " + NameNode.AsString + ">";
        }
    }
}
