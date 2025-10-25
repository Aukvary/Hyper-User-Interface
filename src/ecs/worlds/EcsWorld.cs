using System.Collections.Generic;

namespace HUI;

public class EcsWorld
{
    private int _lastEntity;

    private Queue<int> _recycleEntities;

    private List<IEcsPool> _pools;
}
