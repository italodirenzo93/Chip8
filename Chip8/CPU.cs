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
                {
                    var vx = (opcode & 0x0F00) >> 8;
                    var vy = (opcode & 0x00F0) >> 4;
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
                        // BITSHIFT RIGHT BY 1
                        case 6:
                            V[15] = (byte) (V[vx] & 0x0001);
                            V[vx] = (byte) (V[vx] >> 1);
                            break;
                        // SET VX TO VY - VX. VF is set to 0 when there's a borrow, and 1 when there isn't
                        case 7:
                            V[15] = (byte) (V[vy] > V[vx] ? 1 : 0);
                            V[vx] = (byte) ((V[vy] - V[vx]) & 0x00FF);
                            break;
                        // BITSHIFT LEFT BY 1
                        case 14:
                            V[15] = (byte) ((V[vx] & 0x80) == 0x80 ? 1 : 0);
                            V[vx] = (byte) (V[vx] << 1);
                            break;
                        default:
                            throw new NotSupportedException($"Unsupported Opcode: {opcode:X4}.");
                    }
                }
                    break;
                // Skip the next instruction if VX doesn't equal VY
                case 0x9000:
                {
                    var vx = (opcode & 0x0F00) >> 8;
                    var vy = (opcode & 0x00F0) >> 4;
                    if (vx != vy)
                        I += 2;
                }
                    break;
                default:
                    throw new NotSupportedException($"Unsupported Opcode: {opcode:X4}.");
            }
        }
    }
}