using System;

namespace Chip8
{
    public class UnsupportedOpcodeException : Exception
    {
        public UnsupportedOpcodeException(ushort opcode)
            : base($"Unsupported Opcode: {opcode:X4}.")
        {
            
        }
    }
}