namespace ExprCalc.Runtime.Instructions
{
    public abstract class Instruction
    {
        protected readonly Interpreter _interpreter;

        public Instruction(Interpreter interpreter)
        {
            _interpreter = interpreter;
        }

        public abstract void Exec();

        protected T Pop<T>() => (T)_interpreter.Operands.Pop();
        protected void Push<T>(T v) => _interpreter.Operands.Push(v);

        protected short Address2Bytes(int codeAddr)
        {
            return (short) (_interpreter.Code[codeAddr] << 8 | _interpreter.Code[codeAddr + 1]);
        }
    }
}
