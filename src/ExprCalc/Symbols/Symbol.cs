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

        public short Address { get; set; } = -1;
        public PrimitiveType Type { get; }
        public object Value { get; }
    }

    public class VariableSymbol : Symbol
    {
        public VariableSymbol(string name, bool isDefined = false) : base(name)
        {
            Name = name;
            IsDefined = isDefined;
        }

        public PrimitiveType Type { get; set; }
        public short Address { get; set; } = -1;
        public bool IsDefined { get; set; }
    }

    public class ExternalVariableSymbol : VariableSymbol
    {
        public ExternalVariableSymbol(string name, PrimitiveType type, object value) : base(name, true)
        {
            Type = type;
            Value = value;
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

        public short Address { get; set; } = -1;
        public FunctionType Type { get; }

        public object Call(List<object> args) => _func.Invoke(args);
    }
}
