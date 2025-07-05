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
    }
}
