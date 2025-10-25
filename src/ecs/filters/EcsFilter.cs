using System;
using System.Collections.Generic;

namespace HUI;

public class EcsFilter 
{
    public readonly EcsWorld World;

    private EntityArray _denseEntities;

    private Queue<KeyValuePair<int, bool>> _delayedEntities;
    private int _lockedCount;

    public IReadOnlyEntityArray Entities => _denseEntities;

    public EcsFilter(EcsWorld world, int baseCapacity)
    {
        World = world;

        _denseEntities = new(baseCapacity);   
    }

    public void Add(int entity)
    {
        if (_denseEntities[entity]) return;
        if (_lockedCount > 0)
        {
            _delayedEntities.Enqueue(new(entity, true));
            return;
        }

        _denseEntities[entity] = true;
    }

    public void Remove(int entity)
    {
        if (!_denseEntities[entity]) return;
        if (_lockedCount > 0)
        {
            _delayedEntities.Enqueue(new(entity, false));
            return;
        }

        _denseEntities[entity] = false;
    }

    public FilterEnumerator GetEnumerator()
        => new(this);

    public void Lock()
        => _lockedCount++; 

    public void Unlock()
    {
        _lockedCount--;

        if (_lockedCount > 0) return;

        while(_delayedEntities.Count > 0)
        {
            var pair = _delayedEntities.Dequeue();
            _denseEntities[pair.Key] = pair.Value;
        }
    }
}
