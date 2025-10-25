using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUI;

public interface IEcsPool
{
    object this[int entity] { get; set; }

    void Resize(int newCapacity);

    bool Has(int entity);

    void AddRaw(int entity, object dataRaw);
    void Copy(int srcEntity, int dstEntity);
    void Remove(int entity);
}
