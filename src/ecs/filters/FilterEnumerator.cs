using System;
using System.Collections;
using System.Collections.Generic;

namespace HUI;

public struct FilterEnumerator : IDisposable, IEnumerator<bool>, IEnumerable<bool>
{
    readonly private EcsFilter _filter;
    readonly private IReadOnlyEntityArray _entities;
    private int _index;

    public object Current => _entities[_index];

    bool IEnumerator<bool>.Current => _entities[_index];

    public FilterEnumerator(EcsFilter filter)
    {
        _filter = filter;
        _entities = _filter.Entities;
        _index = -1;

        _filter.Lock();
    }

    public bool MoveNext()
        => _index++ < _entities.Count;

    public void Reset()
        => _index = -1;

    public IEnumerator GetEnumerator()
        => this;

    public void Dispose()
        => _filter.Unlock();

    IEnumerator<bool> IEnumerable<bool>.GetEnumerator()
        => this;
}
