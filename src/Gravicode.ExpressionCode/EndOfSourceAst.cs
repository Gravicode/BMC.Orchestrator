// Decompiled with JetBrains decompiler
// Type: Gravicode.ExpressionCode.EndOfSourceAst
// Assembly: Gravicode.ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\Gravicode.ExpressionCode.dll

namespace Gravicode.ExpressionCode
{
  internal class EndOfSourceAst : StatementAst
  {
    public EndOfSourceAst(Token token)
      : base(token)
    {
      this.NodeType = AstNodeType.EndOfSource;
    }
  }
}
