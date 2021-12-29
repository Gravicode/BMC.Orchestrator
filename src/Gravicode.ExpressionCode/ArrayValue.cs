// Decompiled with JetBrains decompiler
// Type: Gravicode.ExpressionCode.ArrayValue
// Assembly: Gravicode.ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\Gravicode.ExpressionCode.dll

using System.Collections;
using System.Text;

namespace Gravicode.ExpressionCode
{
  internal class ArrayValue : ArrayList
  {
    public static readonly ArrayValue Empty = new ArrayValue();

    public ArrayValue()
    {
    }

    public ArrayValue(ICollection c)
      : base(c)
    {
    }

    public ArrayValue(int capacity)
      : base(capacity)
    {
    }

    public override string ToString() => ArrayValue.ToString((ICollection) this);

    internal static string ToString(ICollection collection)
    {
      StringBuilder stringBuilder = new StringBuilder();
      bool flag = false;
      stringBuilder.Append('[');
      foreach (object obj in (IEnumerable) collection)
      {
        if (flag)
          stringBuilder.Append(',');
        stringBuilder.Append(obj.ToString());
        flag = true;
      }
      stringBuilder.Append(']');
      return stringBuilder.ToString();
    }
  }
}
