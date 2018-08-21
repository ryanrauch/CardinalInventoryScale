using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CardinalInventoryScale.Services.Interfaces
{
    public class WeightScaleMessage
    {
        public Int32 MeasuredValue
        {
            get;
            set;
        }
    }

    public interface IWeightScaleCommunicationService
    {
        Task Connect();
        Task Send(WeightScaleMessage message, string deviceName);
        Task JoinGroup(string deviceName);
        event EventHandler<WeightScaleMessage> OnMessageReceived;

    }
}
