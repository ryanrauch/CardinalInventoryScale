using System;
using System.Collections.Generic;
using System.Text;

namespace CardinalInventoryScale.Services.Interfaces
{
    public interface IWeightScale
    {
        InputAndGainOption InputAndGainSelection { get; set; }

        void PowerDown();
        void PowerOn();
        Int32 Read();
        string GetDeviceName();
    }

    public enum InputAndGainOption : int
    {
        A128 = 1,
        B32 = 2,
        A64 = 3
    }
}
