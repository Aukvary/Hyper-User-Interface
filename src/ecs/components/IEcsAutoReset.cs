using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUI;

public interface IEcsAutoReset<T> where T: struct
{
    void AutoReset(ref T value);
}
