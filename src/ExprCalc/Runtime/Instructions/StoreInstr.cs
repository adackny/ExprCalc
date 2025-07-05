namespace ExprCalc.Runtime.Instructions
{
    public class StoreInstr : Instruction
    {
        public StoreInstr(Interpreter interpreter) : base(interpreter) { }

        public override void Exec()
        {
            var addr = (byte)(_interpreter.Code[_interpreter.Counter].Value & 0x00FF);
            object value = Pop<object>();

            _interpreter.Memory[addr] = value;
            _interpreter.Counter++;
        }
    }
}


