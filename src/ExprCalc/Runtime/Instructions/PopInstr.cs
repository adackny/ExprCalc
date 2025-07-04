namespace ExprCalc.Runtime.Instructions
{
    public class PopInstr : Instruction
    {
        public PopInstr(Interpreter interpreter) : base(interpreter) { }

        public override void Exec()
        {
            _interpreter.Operands.Pop();
            _interpreter.Counter++;
        }
    }
}
