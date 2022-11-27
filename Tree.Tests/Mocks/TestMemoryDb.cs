﻿using System.Runtime.InteropServices;

namespace Tree.Tests.Mocks;

public unsafe class TestMemoryDb : IDb, IDisposable
{
    private const int FileNumber = 13;
    
    private byte* _memory;
    public int Size { get; }
    public int Position { get; private set; }

    public TestMemoryDb(int size)
    {
        Size = size;
        _memory = (byte*)NativeMemory.Alloc((UIntPtr)size);
    }

    public ReadOnlySpan<byte> Read(long id)
    {
        var (position, lenght, file) = Id.Decode(id);

        if (file != FileNumber)
            throw new ArgumentException($"Wrong file: {file}");

        return new ReadOnlySpan<byte>(_memory + position, lenght);
    }

    public long Write(ReadOnlySpan<byte> payload)
    {
        var length = payload.Length;
        payload.CopyTo(new Span<byte>(_memory + Position, length));
        var key= Id.Encode(Position, length, FileNumber);
        
        Position += length;

        return key;
    }

    public void Dispose()
    {
        NativeMemory.Free(_memory);
        _memory = default;
    }
}