using System;
using System.Runtime.CompilerServices;

namespace HUI;

public class EntityArray : IReadOnlyEntityArray
{
    public const int ChunkSize = sizeof(ulong);
    public const int Shift = 6; 
    public const int Mask = ChunkSize - 1;

    private ulong[] _source;

    public int Count => _source.Length >> Shift;

    public bool this[int index]
    {
        get => (_source[index >> Shift] & (1UL << (index & Mask))) != 0;

        set
        {
            if (value)
                _source[index >> Shift] |= 1UL << (index & Mask);
            else
                _source[index >> Shift] &= ~(1UL << (index & Mask));
        }
    }

    public EntityArray(int capacity)
    {
        _source = new ulong[(capacity + ChunkSize - 1) / ChunkSize];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Resize(int newCapacity)
        => Array.Resize(ref _source, newCapacity);
}
