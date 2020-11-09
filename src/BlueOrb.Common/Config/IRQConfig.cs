using System;
using System.Collections.Generic;
using System.Text;

namespace BlueOrb.Base.Config
{
    public interface IRQConfig
    {
        string GetUniqueId();
        string Name { get; }
    }
}
