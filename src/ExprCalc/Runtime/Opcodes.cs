namespace ExprCalc.Runtime
{
    public static class Opcodes
    {
        public const byte HALT = 0x00;

        public const byte ADD = 0x11;
        public const byte SUB = 0x12;
        public const byte MUL = 0x13;
        public const byte DIV = 0x14;
        public const byte AND = 0x15;
        public const byte OR = 0x16;

        public const byte NEG = 0x21;
        public const byte NOT = 0x22;

        public const byte POP = 0x90;

        public const byte LOAD = 0xA1;
        public const byte LOADA = 0xA2;
        public const byte LOADC = 0xA3;

        public const byte STORE = 0xB0;
        
        public const byte BR = 0xC0;
        public const byte BT = 0xC1;
        public const byte BF = 0xC2;

        public const byte CALL = 0xD0;
    }
}
