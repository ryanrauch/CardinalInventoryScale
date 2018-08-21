using CardinalInventoryScale.DataContracts;
using CardinalInventoryScale.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CardinalInventoryScale.ViewModels
{
    public class InitialViewModel : ViewModelBase
    {
        private readonly IRequestService _requestService;
        private readonly IWeightScale _weightScale;

        public InitialViewModel(
            IWeightScale weightScale,
            IRequestService requestService)
        {
            _weightScale = weightScale;
            _requestService = requestService;
        }

        private Int32 _currentStableCount { get; set; } = 0;
        private Int32 _previousValue { get; set; } = 0;

        private DeviceScale _deviceScale { get; set; }

        public bool DeviceSettingsInitialized
        {
            get { return _deviceScale != null; }
        }

        private string _deviceName { get; set; } = string.Empty;
        public string DeviceName
        {
            get { return _deviceName; }
            set
            {
                _deviceName = value;
                RaisePropertyChanged(() => DeviceName);
            }
        }

        private Int32 _refreshMilliseconds { get; set; } = 250;
        public Int32 RefreshMilliseconds
        {
            get { return _refreshMilliseconds; }
            set
            {
                _refreshMilliseconds = value;
                RaisePropertyChanged(() => RefreshMilliseconds);
            }
        }

        private Int32 _stableThreshold { get; set; } = 2;
        public Int32 StableThreshold
        {
            get { return _stableThreshold; }
            set
            {
                _stableThreshold = value;
                RaisePropertyChanged(() => StableThreshold);
            }
        }

        private Int32 _stableCount { get; set; } = 2;
        public Int32 StableCount
        {
            get { return _stableCount; }
            set
            {
                _stableCount = value;
                RaisePropertyChanged(() => StableCount);
            }
        }

        private Int32 _readCountAverage { get; set; } = 5;
        public Int32 ReadCountAverage
        {
            get { return _readCountAverage; }
            set
            {
                _readCountAverage = value;
                RaisePropertyChanged(() => ReadCountAverage);
            }
        }

        private Int32 _readCountMilliseconds { get; set; } = 10;
        public Int32 ReadCountMilliseconds
        {
            get { return _readCountMilliseconds; }
            set
            {
                _readCountMilliseconds = value;
                RaisePropertyChanged(() => ReadCountMilliseconds);
            }
        }

        private Int32 _weightValueRaw { get; set; }
        public Int32 WeightValueRaw
        {
            get { return _weightValueRaw; }
            set
            {
                _weightValueRaw = value;
                RaisePropertyChanged(() => WeightValueRaw);
                RaisePropertyChanged(() => WeightValueAfterTare);
                RaisePropertyChanged(() => WeightValueCalibrated);
                RaisePropertyChanged(() => WeightValueCalibratedString);
                RaisePropertyChanged(() => WeightStable);
            }
        }

        private Int32 _calibrationConstant { get; set; } = -3089;
        public Int32 CalibrationConstant
        {
            get { return _calibrationConstant; }
            set
            {
                _calibrationConstant = value;
                RaisePropertyChanged(() => CalibrationConstant);
            }
        }

        private string _calibrationUnit { get; set; } = "g";
        public string CalibrationUnit
        {
            get { return _calibrationUnit; }
            set
            {
                _calibrationUnit = value;
                RaisePropertyChanged(() => CalibrationUnit);
            }
        }

        private string _calibrationUnitLong { get; set; } = "grams";
        public string CalibrationUnitLong
        {
            get { return _calibrationUnitLong; }
            set
            {
                _calibrationUnitLong = value;
                RaisePropertyChanged(() => CalibrationUnitLong);
            }
        }

        private Int32 _weightValueTare { get; set; }
        public Int32 WeightValueTare
        {
            get { return _weightValueTare; }
            set
            {
                _weightValueTare = value;
                RaisePropertyChanged(() => WeightValueTare);
            }
        }

        public Int32 WeightValueAfterTare
        {
            get { return WeightValueRaw - WeightValueTare; }
        }

        public Int32 WeightValueCalibrated
        {
            get { return WeightValueAfterTare / CalibrationConstant; }
        }

        public string WeightValueCalibratedString
        {
            get
            {
                return string.Format("{0}{1}", 
                                     WeightValueAfterTare / CalibrationConstant,
                                     CalibrationUnit);
            }
        }

        public bool WeightStable
        {
            get { return _currentStableCount > StableCount; }
        }

        private async Task SetTareValue()
        {
            await RefreshWeightValue();
            WeightValueTare = WeightValueRaw;
        }

        private async Task RefreshWeightValue()
        {
            Int32 c = ReadCountAverage;
            Int32 d = c * ReadCountMilliseconds;
            Int32 avgw = 0;
            for (Int32 i = 0; i < c; ++i)
            {
                avgw += _weightScale.Read();
                await Task.Delay(d);
            }
            WeightValueRaw = avgw / c;
            if (_previousValue + StableThreshold > WeightValueCalibrated
                && _previousValue - StableThreshold < WeightValueCalibrated)
            {
                _currentStableCount++;
                _previousValue = WeightValueCalibrated;
            }
            else
            {
                _currentStableCount = 0;
                _previousValue = WeightValueCalibrated;
            }
        }

        private async Task ConfigureDeviceSettings(string deviceName)
        {
            var devices = await _requestService.GetAsync<List<DeviceScale>>("DeviceScales");
            if(devices == null || devices.Count == 0)
            {
                return;
            }
            var ds = devices.FirstOrDefault(d => d.DeviceName.Equals(deviceName, StringComparison.OrdinalIgnoreCase));
            if(ds != null)
            {
                _deviceScale = ds;
                ConfigureDeviceParameters(ds);
                RaisePropertyChanged(() => DeviceSettingsInitialized);
            }
        }

        private void ConfigureDeviceParameters(DeviceScale ds)
        {
            CalibrationConstant = ds.CalibrationConstant;
            CalibrationUnit = ds.CalibrationUnits;
            CalibrationUnitLong = ds.CalibrationUnitsLong;
            RefreshMilliseconds = ds.RefreshMilliseconds;
            StableThreshold = ds.StableThreshold;
            StableCount = ds.StableCount;
            ReadCountAverage = ds.ReadCountAverage;
            ReadCountMilliseconds = ds.ReadCountMilliseconds;
        }

        public override async Task OnAppearingAsync()
        {
            DeviceName = _weightScale.GetDeviceName();
            await ConfigureDeviceSettings(DeviceName);

            _weightScale.PowerOn();
            await Task.Delay(RefreshMilliseconds);
            await SetTareValue();
            Device.StartTimer(TimeSpan.FromMilliseconds(RefreshMilliseconds), () =>
            {
                Device.BeginInvokeOnMainThread(async () => await RefreshWeightValue());
                return true;
            });
        }
    }
}

// Actual Readings
// 20.0g weight = -7760
// 138.0g iphone7 = -53667
// 158.0g both = -61550
// -53667 / 138.0 = -389
