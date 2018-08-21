using System;

namespace CardinalInventoryScale.WebSocketImplementation
{
    public interface IConnectionHolder
    {
        void OnError(Exception ex);
        void AddHeader(string key, string value);
    }
}