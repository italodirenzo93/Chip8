using System;
using System.Collections.Generic;

namespace Chip8
{
    public class CPU
    {
        private byte[] Memory = new byte[4096];
        private byte[] V = new byte[16];    // CPU registers
        private ushort I = 0;
        //public ushort[] Stack = new ushort[24];
        private Stack<ushort> Stack = new Stack<ushort>(24);
        private byte DelayTimer;
        private byte SoundTimer;
        private byte Keyboard;
        
        private byte[] Display = new byte[64 * 32];

        public void ExecuteOpcode(ushort opcode)
        {
            var nibble = (ushort)(opcode & 0xF000);
            switch (nibble)
            {
                case 0x0000:
                    // Clear screen instruction
                    if (opcode == 0x00e0)
                    {
                        for (var i = 0; i < Display.Length; i++) Display[i] = 0;
                    }
                    // Return from subroutine instruction
                    else if (opcode == 0x00ee)
                    {
                        I = Stack.Pop();
                    }
                    // Unknown opcode
                    else
                    {
                        throw new NotSupportedException($"Unsupported Opcode: {opcode:X4}.");
                    }
                    break;
                // Jump to address
                case 0x1000:
                    I = (ushort)(opcode & 0x0FFF);
                    break;
                // Call subroutine
                case 0x2000:
                    Stack.Push(I);
                    I = (ushort) (opcode & 0x0FFF);
                    break;
                // Skips to the next instruction if VX equals NN
                case 0x3000:
                    if (V[(opcode & 0x0F00) >> 8] == (opcode & 0x00FF))
                        I += 2;
                    break;
                // Skips to the next instruction if VX does not equal NN
                case 0x4000:
                    if (V[(opcode & 0x0F00) >> 8] != (opcode & 0x00FF))
                        I += 2;
                    break;
                // Skips the next instruction if VX equals VY
                case 0x5000:
                    if (V[(opcode & 0x0F00) >> 8] != V[(opcode & 0x00F0) >> 4])
                        I += 2;
                    break;
                // Sets VX to NN
                case 0x6000:
                    V[(opcode & 0x0F00) >> 8] = (byte) (opcode & 0x00FF);
                    break;
                // Sets VX to NN
                case 0x7000:
                    V[(opcode & 0x0F00) >> 8] += (byte) (opcode & 0x00FF);
                    break;
                // Sets the value of VX equal to VY
                case 0x8000:
                    var vx = (opcode & 0x0F00) >> 8;
                    var vy = (opcode & 0x00F0) >> 8;
                    switch (opcode & 0x000F)
                    {
                        // Assignment
                        case 0:
                            V[vx] = V[vy];
                            break;
                        // OR
                        case 1:
                            V[vx] = (byte) (V[vx] | V[vy]);
                            break;
                        // AND
                        case 2:
                            V[vx] = (byte) (V[vx] & V[vy]);
                            break;
                        // XOR
                        case 3:
                            V[vx] = (byte) (V[vx] ^ V[vy]);
                            break;
                        // ADD
                        case 4:
                            V[15] = (byte) (V[vx] + V[vy] > 255 ? 1 : 0);
                            V[vx] = (byte) ((V[vx] + V[vy]) & 0x00FF);
                            break;
                        // SUBTRACT
                        case 5:
                            V[15] = (byte) (V[vx] > V[vy] ? 1 : 0);
                            V[vx] = (byte) ((V[vx] - V[vy]) & 0x00FF);
                            break;
                        default:
                            throw new NotSupportedException($"Unsupported Opcode: {opcode:X4}.");
                    }
                    break;
                default:
                    throw new NotSupportedException($"Unsupported Opcode: {opcode:X4}.");
            }
        }
    }
}