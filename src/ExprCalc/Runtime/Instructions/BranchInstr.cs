using System;

namespace ExprCalc.Runtime.Instructions
{
    public class BranchInstr : Instruction
    {
        private const byte BR = 0xC0;
        private const byte BT = 0xC1;
        private const byte BF = 0xC2;

        public BranchInstr(Interpreter interpreter) : base(interpreter)
        {
        }

        public override void Exec()
        {
            var variant = _interpreter.Code[_interpreter.Counter];
            var address = Address2Bytes(_interpreter.Counter + 1);

            var cond = false;

            switch (variant)
            {
                case BR:
                    cond = true;
                    break;
                case BT:
                    cond = Pop<bool>();
                    break;
                case BF:
                    cond = !Pop<bool>();
                    break;
                default:
                    throw new Exception($"Error in branch. Unknown instruction: {variant:X2}");
            }

            if (cond)
                _interpreter.Counter = address;
            else
                _interpreter.Counter += 3;
        }
    }
}
