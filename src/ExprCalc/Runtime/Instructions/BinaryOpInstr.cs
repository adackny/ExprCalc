using System;

namespace ExprCalc.Runtime.Instructions
{
    public class BinaryOpInstr : Instruction
    {
        private const byte ADD = 0x11;
        private const byte SUB = 0x12;
        private const byte MUL = 0x13;
        private const byte DIV = 0x14;
        private const byte AND = 0x15;
        private const byte OR = 0x16;

        public BinaryOpInstr(Interpreter interpreter) : base(interpreter) { }

        public override void Exec()
        {
            var variant = _interpreter.Code[_interpreter.Counter];

            switch (variant)
            {
                case ADD:
                    Push(Pop<decimal>() + Pop<decimal>());
                    break;
                case SUB:
                    Push(Pop<decimal>() - Pop<decimal>());
                    break;
                case MUL:
                    Push(Pop<decimal>() * Pop<decimal>());
                    break;
                case DIV:
                    Push(Pop<decimal>() / Pop<decimal>());
                    break;
                case AND:
                    Push(Pop<bool>() && Pop<bool>());
                    break;
                case OR:
                    Push(Pop<bool>() || Pop<bool>());
                    break;
                default:
                    throw new Exception($"Error in binary_op. Unknown instruction: {variant:X2}");
            }

            _interpreter.Counter++;
        }
    }
}
