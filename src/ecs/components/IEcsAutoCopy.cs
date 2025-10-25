using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUI;

public interface IEcsAutoCopy<T>
{
    void copy(ref T src, ref T dst);
}
