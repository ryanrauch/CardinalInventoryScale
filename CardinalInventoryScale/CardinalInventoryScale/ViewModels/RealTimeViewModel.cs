using CardinalInventoryScale.DataContracts;
using CardinalInventoryScale.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace CardinalInventoryScale.ViewModels
{
    public class RealTimeViewModel : ViewModelBase
    {
        private readonly IRequestService _requestService;
        private readonly INavigationService _navigationService;
        private readonly IWeightScaleCommunicationService _weightScaleCommunicationService;

        public RealTimeViewModel(
            IRequestService requestService,
            INavigationService navigationService,
            IWeightScaleCommunicationService weightScaleCommunicationService)
        {
            _requestService = requestService;
            _navigationService = navigationService;
            _weightScaleCommunicationService = weightScaleCommunicationService;
            _weightScaleCommunicationService.OnMessageReceived += _weightScaleCommunicationService_OnMessageReceived;
        }

        private DeviceScale _deviceScale { get; set; }
        public DeviceScale DeviceScale
        {
            get { return _deviceScale; }
            set
            {
                _deviceScale = value;
                RaisePropertyChanged(() => DeviceScale);
            }
        }

        private List<Int32> _measuredValues { get; set; } = new List<Int32>();
        public List<Int32> MeasuredValues
        {
            get { return _measuredValues; }
            set
            {
                _measuredValues = value;
                RaisePropertyChanged(() => MeasuredValues);
            }
        }

        private ObservableCollection<WeightScaleMessage> _messages { get; set; } = new ObservableCollection<WeightScaleMessage>();
        public ObservableCollection<WeightScaleMessage> Messages
        {
            get { return _messages; }
            set
            {
                _messages = value;
                RaisePropertyChanged(() => Messages);
            }
        }

        private void _weightScaleCommunicationService_OnMessageReceived(object sender, WeightScaleMessage e)
        {
            MeasuredValues.Add(e.MeasuredValue);
            Messages.Add(e);
        }

        public override void Initialize(object param)
        {
            base.Initialize(param);
            if(param is DeviceScale ds)
            {
                DeviceScale = ds;
            }
        }

        public override async Task OnAppearingAsync()
        {
            if(DeviceScale != null)
            {
                await _weightScaleCommunicationService.JoinGroup(DeviceScale.DeviceName);
            }
        }
    }
}
