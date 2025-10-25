using System.Reflection;
using System;
using System.Collections.Generic;

namespace HUI;

public class EcsPool<T> : IEcsPool 
    where T: struct
{
    private static BindingFlags _methodFindFlags =
        BindingFlags.Public |
        BindingFlags.NonPublic |
        BindingFlags.Instance;

    public readonly Type Type;
    public readonly EcsWorld World;

    public readonly short Id;

    private readonly RefAction<T> _resetHandler;
    private readonly RefAction<T, T> _copyHandler;

    private T[] _denseItems;
    private int[] _sparseItems;

    private int _itemsCount;

    private Queue<int> _recycleItems;

    object IEcsPool.this[int entity] 
    {
        get => Get(entity);

        set => Set(entity, (T)value);
    }

    public event Action<int, T> OnEntityAddEvent;
    public event Action<int, T> OnEntityRemoveEvent;

    public EcsPool(EcsWorld world, short id, int capacity)
    {
        Type = typeof(T);
        World = world;
        Id = id;

        _denseItems = new T[capacity];
        _sparseItems = new int[capacity];

        _recycleItems = new();

        if (typeof(IEcsAutoReset<T>).IsAssignableFrom(Type))
        {
            _resetHandler = Delegate
                .CreateDelegate(typeof(RefAction<T>), Type
                    .GetMethod(nameof(IEcsAutoReset<T>.AutoReset), _methodFindFlags)
                ) as RefAction<T>;
        }

        if (typeof(IEcsAutoCopy<T>).IsAssignableFrom(Type))
        {
            _copyHandler = Delegate
                .CreateDelegate(typeof(RefAction<T, T>), Type
                    .GetMethod(nameof(IEcsAutoCopy<T>.copy), _methodFindFlags)
                ) as RefAction<T, T>;
        }
    }

    public void AddRaw(int entity, object dataRaw = null)
    {
        if (_sparseItems[entity] != -1)
        {
            throw new Exception($"entity \"{entity}\" already has component {Type.Name}");
        }

        if (_recycleItems.Count > 0)
        {
            _sparseItems[entity] = _recycleItems.Dequeue();
            if (dataRaw == null)
                _resetHandler?.Invoke(ref _denseItems[_sparseItems[entity]]);
            OnEntityAddEvent?.Invoke(entity, _denseItems[_sparseItems[entity]]);
            return;
        }

        _sparseItems[entity] = _itemsCount++;
        _resetHandler(ref _denseItems[_sparseItems[entity]]);
        OnEntityAddEvent?.Invoke(entity, _denseItems[_sparseItems[entity]]);
    }

    public void Copy(int srcEntity, int dstEntity)
    {
        if (!Has(srcEntity))
            throw new Exception($"entity \"{srcEntity}\" hasnt compontnt \"{Type.Name}\"");

        if (!Has(dstEntity))
            AddRaw(dstEntity);

        if (_copyHandler != null)
            _copyHandler.Invoke(
                ref _denseItems[_sparseItems[srcEntity]],
                ref _denseItems[_sparseItems[dstEntity]]
            );
        else
            _denseItems[_sparseItems[dstEntity]] = _denseItems[_sparseItems[srcEntity]];
    }

    public void Remove(int entity)
    {
        if (!Has(entity)) return;

        T component = _denseItems[_sparseItems[entity]];
        _recycleItems.Enqueue(entity);
        _sparseItems[entity] = -1;


        OnEntityRemoveEvent?.Invoke(entity, component);
    }

    public bool Has(int entity)
        => entity < _itemsCount && _sparseItems[entity] != -1;

    public ref T Get(int entity)
    {
        if (!Has(entity))
            throw new Exception($"entity \"{entity}\" hasnt component \"{Type.Name}\"");

        return ref _denseItems[_sparseItems[entity]];
    }

    public void Set(int entity, T source)
    {
        if (!Has(entity))
            throw new Exception($"entity \"{entity}\" hasnt component \"{Type.Name}\"");

        _denseItems[_sparseItems[entity]] = source;
    }

    public void Resize(int newCapacity)
    {
        Array.Resize(ref _sparseItems, newCapacity);
        Array.Resize(ref _denseItems, newCapacity);
    }
}
