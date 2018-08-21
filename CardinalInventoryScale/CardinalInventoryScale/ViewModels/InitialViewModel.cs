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
        // 20.0g weight = -7760
        // 138.0g iphone7 = -53667
        // 158.0g both = -61550
        // -53667 / 138.0 = -389

        private const Int32 CALIBRATION_CONSTANT = -389;
        private const Int32 REFRESH_WAIT_MS = 500;

        private readonly IRequestService _requestService;
        private readonly IWeightScale _weightScale;

        public InitialViewModel(
            IWeightScale weightScale,
            IRequestService requestService)
        {
            _weightScale = weightScale;
            _requestService = requestService;
        }

        private Int32 _weightValueRaw { get; set; }
        public Int32 WeightValueRaw
        {
            get { return _weightValueRaw; }
            set
            {
                _weightValueRaw = value;
                RaisePropertyChanged(() => WeightValueRaw);
                RaisePropertyChanged(() => WeightValueAdjusted);
                RaisePropertyChanged(() => WeightValueReference);
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

        public Int32 WeightValueAdjusted
        {
            get
            {
                return WeightValueRaw - WeightValueTare;
            }
        }

        public Int32 WeightValueReference
        {
            get
            {
                return WeightValueAdjusted / CALIBRATION_CONSTANT;
            }
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
        }

        public override async Task OnAppearingAsync()
        {
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
