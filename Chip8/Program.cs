using System;
using System.IO;

namespace Chip8
{
    class Program
    {
        private static void Main(string[] args)
        {
            // Virtual CHIP-8 CPU
            var cpu = new CPU();

            // Open ROM file
            using var rom = File.OpenRead("roms/sample.ch8");
            Console.WriteLine($"File is {rom.Length} bytes long");

            // Read program
            using var reader = new BinaryReader(rom);
            while (rom.Position < rom.Length)
            {
                // CHIP-8 opcodes are 2 bytes long and stored in big-endian.
                var opcode = (ushort)((reader.ReadByte() << 8) | reader.ReadByte());
                try
                {
                    cpu.ExecuteOpcode(opcode);
                }
                catch (UnsupportedOpcodeException e)
                {
                    Console.WriteLine(e.Message); 
                }
            }
            Console.WriteLine("\nEnd of ROM file.");
        }
    }
}