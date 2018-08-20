using CardinalInventoryScale.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

        private Int32 _weightValue { get; set; }
        public Int32 WeightValue
        {
            get { return _weightValue; }
            set
            {
                _weightValue = value;
                RaisePropertyChanged(() => WeightValue);
            }
        }

        public override Task OnAppearingAsync()
        {
            _weightScale.PowerOn();
            WeightValue = _weightScale.Read();
            return Task.CompletedTask;
        }
    }
}
