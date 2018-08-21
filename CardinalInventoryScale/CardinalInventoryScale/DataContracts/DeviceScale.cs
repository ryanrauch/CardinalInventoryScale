using System;
using System.Collections.Generic;
using System.Text;

namespace CardinalInventoryScale.DataContracts
{
    public class DeviceScale
    {
        public Guid DeviceScaleId { get; set; }
        public string DeviceName { get; set; }
        public DeviceScaleType DeviceScaleType { get; set; }
        public bool DebugMode { get; set; }
        public Int32 CalibrationConstant { get; set; }
        public string CalibrationUnits { get; set; }
        public string CalibrationUnitsLong { get; set; }
        public Int32 RefreshMilliseconds { get; set; }
        public Int32 StableThreshold { get; set; }
        public Int32 StableCount { get; set; }
        public Int32 ReadCountAverage { get; set; }
        public Int32 ReadCountMilliseconds { get; set; }
    }
}
