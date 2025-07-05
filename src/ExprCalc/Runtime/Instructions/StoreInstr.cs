namespace ExprCalc.Runtime.Instructions
{
    public class StoreInstr : Instruction
    {
        public StoreInstr(Interpreter interpreter) : base(interpreter) { }

        public override void Exec()
        {
            var addr = _interpreter.Code[_interpreter.Counter + 1].Value;
            object value = Pop<object>();

            _interpreter.Memory[addr] = value;
            _interpreter.Counter += 2;
        }
    }
}


