using System;

namespace Industrica.Network
{
    [Flags]
    public enum PortType
    {
        None = 0,
        Input = 1 << 0,
        Output = 1 << 1,
        InputAndOutput = Input | Output
    }
}
