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
    
    public static class OpcodesBuilder
    {
        public static ushort HALT() => 0x0000;

        public static ushort ADD() => 0x1100;
        public static ushort SUB() => 0x1200;
        public static ushort MUL() => 0x1300;
        public static ushort DIV() => 0x1400;
        public static ushort AND() => 0x1500;
        public static ushort OR() => 0x1600;

        public static ushort NEG() => 0x2100;
        public static ushort NOT() => 0x2200;

        public static ushort POP() => 0x9000;

        public static ushort LOAD(byte addr) => (ushort)(0xA100 | addr);
        public static ushort LOADA(byte addr) => (ushort)(0xA200 | addr);
        public static ushort LOADC(byte addr) => (ushort)(0xA300 | addr);

        public static ushort STORE(byte addr) => (ushort)(0xB000 | addr);

        public static ushort BR(byte addr) => (ushort)(0xC000 | addr);
        public static ushort BT(byte addr) => (ushort)(0xC100 | addr);
        public static ushort BF(byte addr) => (ushort)(0xC200 | addr);

        public static ushort CALL(byte addr) => (ushort)(0xD000 | addr);
    }
}
