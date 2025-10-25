using System.Collections.Generic;

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

    private Dictionary<Type, EcsPool> _poolHashs;

    public EcsWorld()
    {
        _recycleEntities = new();
        int capacity = Config.EntitiesDefault;

        _entitiesItemSize = RawEntityOffests.Components + Config.EntityComponentsSizeDefault;
        _entities = new short[capacity * _entitiesItemSize];

        _recycleEntities = new(Config.RecycledEntitiesDefault);

        _pools = new(Config.PoolsDefault);
        _poolHashs = new(Config.PoolsDefault)
    }

    public int NewEntity()
    {
    }
}
