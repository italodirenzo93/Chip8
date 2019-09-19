using System;
using System.IO;

namespace Chip8
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var rom = File.OpenRead("roms/IBM Logo.ch8"))
            {
                Console.WriteLine($"File is {rom.Length} bytes long");

                using (var reader = new BinaryReader(rom))
                {
                    while (rom.Position < rom.Length)
                    {
                        // CHIP-8 opcodes are 2 bytes long and stored in big-endian.
                        var opcode = (ushort)((reader.ReadByte() << 8) | reader.ReadByte());
                        Console.WriteLine($"OPCODE is {opcode:X4}.");
                    }
                    Console.WriteLine("\nEnd of ROM file.");
                }// using BinaryReader
            }// using FileStream
        }
    }
}