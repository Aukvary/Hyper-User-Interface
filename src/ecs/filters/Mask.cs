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
}
