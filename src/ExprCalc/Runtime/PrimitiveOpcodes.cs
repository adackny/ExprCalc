namespace ExprCalc.Runtime
{
    public static class PrimitiveOpcodes
    {
        public const byte HALT = 0x00;

        public const byte BINARY_OP = 0x10;
        public const byte UNARY_OP = 0x20;

        public const byte POP = 0x90;
        public const byte LOAD = 0xA0;
        public const byte STORE = 0xB0;
        public const byte BRANCH = 0xC0;
        public const byte CALL = 0xD0;
    }
}
