using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExprCalc.Runtime.Instructions;

namespace ExprCalc.Runtime
{
    public class Interpreter
    {
        private readonly Dictionary<byte, Instruction> _instructions;

        public Interpreter()
        {
            _instructions = new Dictionary<byte, Instruction>()
            {
                { PrimitiveOpcodes.UNARY_OP, new UnaryOpInstr(this) },
                { PrimitiveOpcodes.BINARY_OP, new BinaryOpInstr(this) },
                { PrimitiveOpcodes.POP, new PopInstr(this) },
                { PrimitiveOpcodes.BRANCH, new BranchInstr(this) },
                { PrimitiveOpcodes.LOAD, new LoadInstr(this) },
                { PrimitiveOpcodes.STORE, new StoreInstr(this) },
                { PrimitiveOpcodes.CALL, new CallInstr(this) },
            };
        }

        public int Counter { get; set; }
        public List<ByteCode> Code { get; private set; }
        public Stack<object> Operands { get; private set; }
        public List<object> Memory { get; private set; }
        public List<object> ConstPool { get; private set; }

        public void Load(List<ByteCode> code, List<object> memory, List<object> constPool)
        {
            Code = code;
            Operands = new Stack<object>();
            Memory = memory;
            ConstPool = constPool;
            Counter = 0;
        }

        public void Run()
        {
            byte bytecode = (byte)(Code[Counter].Value & 0xF0);
            while (bytecode != PrimitiveOpcodes.HALT && Counter < Code.Count)
            {
                var instr = _instructions[bytecode];
                instr.Exec();
                bytecode = (byte)(Code[Counter].Value & 0xF0);
            }
        }
    }

    public struct ByteCode
    {
        public readonly bool _isOpcode;
        private readonly byte value;

        public ByteCode(bool isOpcode, byte value)
        {
            _isOpcode = isOpcode;
            this.value = value;
        }

        public byte Value => value;

        public override string ToString()
        {
            if (_isOpcode)
            {
                var opcodesType = typeof(Opcodes);
                var constFields = opcodesType.GetFields(
                    BindingFlags.Public |
                    BindingFlags.Static);

                var byteValue = value;

                var field = constFields.First(fld => (byte)fld.GetValue(null) == byteValue);
                return field.Name;
            }
            return value.ToString();
        }
    }
}
