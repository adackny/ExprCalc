using System;

namespace ExprCalc.Runtime.Instructions
{
    public class UnaryOpInstr : Instruction
    {
        public const byte NEG = 0x21;
        public const byte NOT = 0x22;

        public UnaryOpInstr(Interpreter interpreter) : base(interpreter) { }

        public override void Exec()
        {
            var variant = (byte)(_interpreter.Code[_interpreter.Counter].Value >> 8);

            switch (variant)
            {
                case NEG:
                    Push(-Pop<decimal>());
                    break;
                case NOT:
                    Push(Pop<decimal>() > 0);
                    break;
                default:
                    throw new Exception($"Error in unary_op. Unknown instruction: {variant:X2}");
            }

            _interpreter.Counter++;
        }
    }
}
