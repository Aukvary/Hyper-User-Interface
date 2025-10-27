namespace HUI;

public class Mask
{
    private readonly EcsWorld _world;

    private int[] _include;
    private byte _incCount;

    private int[] _exclude;
    private byte _excCount;

    private int _hash;

    public Mask(EcsWorld world)
    {
        _world = world;

        _include = new int[8];
        _exclude = new int[8];
    }

    public Mask Inc<T>() where T: struct
    {
        var id = _world.GetPool<T>().Id;
        _include[_incCount++] = id;
        return this;
    }

    public Mask Exc<T>() where T: struct
    {
        var id = _world.GetPool<T>().Id;
        _exclude[_excCount++] = id;
        return this;
    }

    public EcsFilter End(int capacity = 512)
    {
        throw new System.Exception();
    }
}
