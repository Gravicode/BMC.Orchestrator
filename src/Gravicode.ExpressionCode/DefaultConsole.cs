﻿// Decompiled with JetBrains decompiler
// Type: Gravicode.ExpressionCode.DefaultConsole
// Assembly: Gravicode.ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\Gravicode.ExpressionCode.dll

using System.Diagnostics;

namespace Gravicode.ExpressionCode
{
  internal class DefaultConsole : IConsole
  {
    public void Cls()
    {
    }

    public void Locate(int row, int col)
    {
    }

    public void Print(string s) => Trace.WriteLine(s);
  }
}
