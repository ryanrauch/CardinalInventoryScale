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
            _previousValue = 0;
            _stableCount = 0;
        }

        public const Int32 CALIBRATION_CONSTANT = -389;
        public const string CALIBRATION_UNIT = "g";
        public const string CALIBRATION_UNIT_LONG = "grams";
        private const Int32 REFRESH_WAIT_MS = 250;
        private const Int32 STABLE_COUNT = 2;
        private const Int32 STABLE_THRESHOLD = 2;

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

        private Int32 _previousValue { get; set; }
        private Int32 _stableCount { get; set; }
        
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

        public Int32 CalibrationConstant => CALIBRATION_CONSTANT;

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
            get { return WeightValueAfterTare / CALIBRATION_CONSTANT; }
        }

        public string WeightValueCalibratedString
        {
            get
            {
                return string.Format("{0}{1}", 
                                     WeightValueAfterTare / CALIBRATION_CONSTANT,
                                     CALIBRATION_UNIT);
            }
        }

        public bool WeightStable
        {
            get { return _stableCount > STABLE_COUNT; }
        }

        private async Task SetTareValue()
        {
            await RefreshWeightValue();
            WeightValueTare = WeightValueRaw;
        }

        private async Task RefreshWeightValue()
        {
            Int32 c = 5;
            Int32 avgw = 0;
            for (Int32 i = 0; i < c; ++i)
            {
                avgw += _weightScale.Read();
                await Task.Delay(c * 10);
            }
            WeightValueRaw = avgw / c;
            if (_previousValue + STABLE_THRESHOLD > WeightValueCalibrated
                && _previousValue - STABLE_THRESHOLD < WeightValueCalibrated)
            {
                _stableCount++;
                _previousValue = WeightValueCalibrated;
            }
            else
            {
                _stableCount = 0;
                _previousValue = WeightValueCalibrated;
            }
        }

        public override async Task OnAppearingAsync()
        {
            DeviceName = _weightScale.GetDeviceName();
            _weightScale.PowerOn();
            await Task.Delay(REFRESH_WAIT_MS);
            await SetTareValue();
            Device.StartTimer(TimeSpan.FromMilliseconds(REFRESH_WAIT_MS), () =>
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
