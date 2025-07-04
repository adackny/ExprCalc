using System;
using System.Collections.Generic;
using System.Linq;

namespace ExprCalc.Symbols
{
    public interface IType : IEquatable<IType>
    {
        string Name { get; }
    }
    
    public class PrimitiveType : Symbol, IType
    {
        public PrimitiveType(string name) : base(name) { }

        public bool Equals(IType other) => Name == other.Name;
    }

    public class FunctionType : Symbol, IType
    {
        public FunctionType(List<PrimitiveType> args, PrimitiveType ret) : base(null)
        {
            Ret = ret;
            Args = args;
            Name = $"<fn({string.Join(", ", args.Select(a => a.Name))})->{ret.Name}>";
        }

        public PrimitiveType Ret { get; }
        public List<PrimitiveType> Args { get; }

        public bool Equals(IType other) => Name == other.Name;
    }
}
