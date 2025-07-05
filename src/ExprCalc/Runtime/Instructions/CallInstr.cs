using System.Collections.Generic;
using ExprCalc.Symbols;

namespace ExprCalc.Runtime.Instructions
{
    public class CallInstr : Instruction
    {
        public CallInstr(Interpreter interpreter) : base(interpreter) { }

        public override void Exec()
        {
            var addr = (byte)(_interpreter.Code[_interpreter.Counter].Value & 0x00FF);

            var fn = _interpreter.ConstPool[addr] as ExternalFunctionSymbol;
            var args = new List<object>();
            for (int i = 0; i < fn.Type.Args.Count; i++)
                args.Add(Pop<object>());
            var ret = fn.Call(args);
            _interpreter.Operands.Push(ret);

            _interpreter.Counter++;
        }
    }
}
