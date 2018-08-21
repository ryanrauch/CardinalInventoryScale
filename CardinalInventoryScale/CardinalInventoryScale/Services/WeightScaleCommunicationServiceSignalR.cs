using CardinalInventoryScale.Services.Interfaces;
using CardinalInventoryScale.WebSocketImplementation;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CardinalInventoryScale.Services
{
    public class WeightScaleCommunicationServiceSignalR : IWeightScaleCommunicationService
    {
        private readonly HubConnection _connection;
        private readonly IHubProxy _proxy;

        public event EventHandler<WeightScaleMessage> OnMessageReceived;

        public WeightScaleCommunicationServiceSignalR()
        {
            _connection = new HubConnection(Constants.SignalRUrl);
            _proxy = _connection.CreateHubProxy("chatGroupHub");
            _proxy.On("GetMessage", (Int32 mv) => OnMessageReceived(this, new WeightScaleMessage
            {
                MeasuredValue = mv
            }));
        }

        #region IWeightScaleCommunicationService implementation

        public async Task Connect()
        {

            var http = new Microsoft.AspNet.SignalR.Client.Http.DefaultHttpClient();
            //var transports = new List<IClientTransport>()
            //                                                        {
            //                                                            new WebSocketTransportLayer(http),
            //                                                            new ServerSentEventsTransport(http),
            //                                                            new LongPollingTransport(http)
            //                                                        };
            /// Preparando la conexion
            //await _connection.Start(new AutoTransport(http, transports));
            await _connection.Start(new WebSocketTransportLayer(http));
        }

        public async Task Send(WeightScaleMessage message, string deviceName)
        {
            if (_connection.State == ConnectionState.Disconnected)
            {
                await Connect();
            }
            await _proxy.Invoke("SendMessage", message.MeasuredValue, deviceName);
        }

        public async Task JoinGroup(string deviceName)
        {
            if (_connection.State == ConnectionState.Disconnected)
            {
                await Connect();
            }
            await _proxy.Invoke("JoinGroup", deviceName);
        }

        #endregion
    }
}
