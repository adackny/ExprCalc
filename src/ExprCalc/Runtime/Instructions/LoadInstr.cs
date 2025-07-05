using System;

namespace ExprCalc.Runtime.Instructions
{
    public class LoadInstr : Instruction
    {
        private const byte LOAD = 0xA1;
        private const byte LOADA = 0xA2;
        private const byte LOADC = 0xA3;

        public LoadInstr(Interpreter interpreter) : base(interpreter) { }

        public override void Exec()
        {
            var variant = (byte)(_interpreter.Code[_interpreter.Counter].Value >> 8);
            var addr = (byte)(_interpreter.Code[_interpreter.Counter].Value & 0x00FF);
            object value = null;

            switch (variant)
            {
                case LOAD:
                    value = _interpreter.Memory[addr];
                    break;
                case LOADA:
                    value = addr;
                    break;
                case LOADC:
                    value = _interpreter.ConstPool[addr];
                    break;
                default:
                    throw new Exception($"Error in load. Unknown instruction: {variant:X2}");
            }

            _interpreter.Operands.Push(value);
            _interpreter.Counter++;
        }
    }
}


