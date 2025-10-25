using System.Collections.Generic;
using System;

namespace HUI;

public struct Config
{
    public int Entities;
    public int RecycledEntities;
    public int Pools;
    public int Filters;
    public int PoolDenseSize;
    public int PoolRecycledSize;
    public int EntityComponentsSize;

    internal const int EntitiesDefault = 512;
    internal const int RecycledEntitiesDefault = 512;
    internal const int PoolsDefault = 512;
    internal const int FiltersDefault = 512;
    internal const int PoolDenseSizeDefault = 512;
    internal const int PoolRecycledSizeDefault = 512;
    internal const int EntityComponentsSizeDefault = 8;
}

public static class RawEntityOffests
{
    public const int ComponentCount = 0;
    public const int Gen = 1;
    public const int Components = 2;
}

public class EcsWorld
{
    private short[] _entities;
    private int _entitiesItemSize;
    private int _entitiesCount;

    private Queue<int> _recycleEntities;

    private List<IEcsPool> _pools;
    private int _poolsDenseSize;
    private Dictionary<Type, IEcsPool> _poolHashs;

    private List<EcsFilter> _filtres;
    private Dictionary<int, EcsFilter> _filerHashes;

    private List<Mask> _masks;

    public EcsWorld()
    {
        _recycleEntities = new();
        int capacity = Config.EntitiesDefault;

        _entitiesItemSize = RawEntityOffests.Components + Config.EntityComponentsSizeDefault;
        _entities = new short[capacity * _entitiesItemSize];

        _recycleEntities = new(Config.RecycledEntitiesDefault);

        _pools = new(Config.PoolsDefault);
        _poolsDenseSize = Config.PoolDenseSizeDefault;
        _poolHashs = new(Config.PoolsDefault);

        _filtres = new(Config.FiltersDefault);
        _filerHashes = new(Config.FiltersDefault);

        _masks = new(64);
    }

    public int GetRawEntityOffset(int entity)
        => entity * _entitiesItemSize;


    public int NewEntity()
    {
        int newEntity;

        if (_recycleEntities.Count > 0)
        {
            newEntity = _recycleEntities.Dequeue();
            _entities[GetRawEntityOffset(newEntity) + RawEntityOffests.Gen] *= -1;
        }
        else
        {
            if (_entitiesCount * _entitiesItemSize == _entities.Length)
            {
                int newSize = _entitiesCount * 2;
                Array.Resize(ref _entities, newSize * _entitiesItemSize);

                foreach(var pool in _pools) 
                    pool.Resize(newSize);
            }
            newEntity = _entitiesCount++;
            _entities[GetRawEntityOffset(newEntity) + RawEntityOffests.Gen] = 1;

        }

        return newEntity;
    }

    public void KillEntity(int entity)
    {
        if (entity < 0 || entity >= _entitiesCount) return;

        int offset = GetRawEntityOffset(entity); 
        short componentsCount = _entities[offset + RawEntityOffests.ComponentCount];
        ref short gen = ref _entities[offset + RawEntityOffests.Gen];

        if (componentsCount > 0)
            foreach (var pool in _pools)
                pool.Remove(entity);

    }
}
