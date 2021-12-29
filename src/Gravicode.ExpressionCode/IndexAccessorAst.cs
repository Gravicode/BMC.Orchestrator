// Decompiled with JetBrains decompiler
// Type: Gravicode.ExpressionCode.IndexAccessorAst
// Assembly: Gravicode.ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\Gravicode.ExpressionCode.dll

namespace Gravicode.ExpressionCode
{
  internal class IndexAccessorAst : ExpressionAst
  {
    public ExpressionAst left;
    public ExpressionAst index;

    public IndexAccessorAst(Token token, ExpressionAst left, ExpressionAst index)
      : base(token)
    {
      this.NodeType = AstNodeType.IndexAccessorPre;
      this.left = left;
      this.index = index;
    }
  }
}
