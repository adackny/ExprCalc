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
            byte bytecode = (byte)(Code[Counter].Value >> 8 & 0xF0);
            while (bytecode != PrimitiveOpcodes.HALT && Counter < Code.Count)
            {
                var instr = _instructions[bytecode];
                instr.Exec();
                bytecode = (byte)(Code[Counter].Value >> 8 & 0xF0);
            }
        }
    }

    public struct ByteCode
    {
        private readonly ushort _value;

        public ByteCode(ushort value) => _value = value;

        public ushort Value => _value;

        public static implicit operator ByteCode(ushort s) => new ByteCode(s);

        public override string ToString()
        {
            var opcodeVariant = (byte)((_value & 0xFF00) >> 8);
            var opcode = (byte)(opcodeVariant & 0xF0);
            var addr = (byte)(_value & 0x00FF);

            var opcodesType = typeof(Opcodes);
            var constFields = opcodesType.GetFields(
                BindingFlags.Public |
                BindingFlags.Static);

            var field = constFields.First(fld => (byte)fld.GetValue(null) == opcodeVariant);

            switch (opcode)
            {
                case PrimitiveOpcodes.LOAD:
                case PrimitiveOpcodes.STORE:
                case PrimitiveOpcodes.BRANCH:
                case PrimitiveOpcodes.CALL:
                    return $"<{field.Name}, {addr:X2}>";
                default:
                    return $"<{field.Name}>";
            }
        }
    }
}
