using System;
using System.Collections.Generic;

namespace ExprCalc.Symbols
{
    public abstract class Symbol
    {
        public Symbol(string name)
        {
            Name = name;
        }

        public string Name { get; protected set; }
    }

    public class ConstSymbol : Symbol
    {
        public ConstSymbol(string name, PrimitiveType type, object value) : base(name)
        {
            Type = type;
            Value = value;
        }

        public byte? ConstPoolIndex { get; set; }
        public PrimitiveType Type { get; }
        public object Value { get; }
    }

    public class VariableSymbol : Symbol
    {
        public VariableSymbol(string name, PrimitiveType type = null) : base(name)
        {
            Type = type;
        }

        public PrimitiveType Type { get; set; }
        public byte? Address { get; set; }
        public bool IsDefined { get; set; }
    }

    public class ExternalVariableSymbol : VariableSymbol
    {
        public ExternalVariableSymbol(string name, PrimitiveType type, object value) : base(name)
        {
            Type = type;
            Value = value;
            IsDefined = true;
        }

        public object Value { get; }
    }


    public class ExternalFunctionSymbol : Symbol
    {
        private readonly Func<List<object>, object> _func;

        public ExternalFunctionSymbol(string name, FunctionType funcType, Func<List<object>, object> func) : base(name)
        {
            Type = funcType;
            _func = func;
        }

        public byte? ConstPoolIndex { get; set; }
        public FunctionType Type { get; }

        public object Call(List<object> args) => _func.Invoke(args);
    }
}
