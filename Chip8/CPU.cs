using System;
using System.Collections.Generic;

namespace Chip8
{
    public class CPU
    {
        public byte[] Memory = new byte[4096];
        public byte[] Registers = new byte[16];
        public ushort I = 0;
        //public ushort[] Stack = new ushort[24];
        public Stack<ushort> Stack = new Stack<ushort>(24);
        public byte DelayTimer;
        public byte SoundTimer;
        public byte Keyboard;
        
        public byte[] Display = new byte[64 * 32];

        public void ExecuteOpcode(ushort opcode)
        {
            var nibble = (ushort)(opcode & 0xF000);
            switch (nibble)
            {
                case 0x0000:
                    // Clear screen instruction
                    if (opcode == 0x00e0)
                    {
                        Console.WriteLine("Clear screen");
                        for (var i = 0; i < Display.Length; i++) Display[i] = 0;
                    }
                    // Return from subroutine instruction
                    else if (opcode == 0x00ee)
                    {
                        Console.WriteLine("Return from subroutine");
                        I = Stack.Pop();
                    }
                    // Unknown opcode
                    else
                    {
                        throw new NotSupportedException($"Unsupported Opcode: {opcode:X4}.");
                    }
                    break;
                default:
                    throw new NotSupportedException($"Unsupported Opcode: {opcode:X4}.");
            }
        }
    }
}