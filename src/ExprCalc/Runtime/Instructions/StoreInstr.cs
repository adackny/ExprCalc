namespace ExprCalc.Runtime.Instructions
{
    public class StoreInstr : Instruction
    {
        public StoreInstr(Interpreter interpreter) : base(interpreter) { }

        public override void Exec()
        {
            var addr = Address2Bytes(_interpreter.Counter + 1);
            object value = Pop<object>();

            _interpreter.Memory[addr] = value;
            _interpreter.Counter += 3;
        }
    }
}


