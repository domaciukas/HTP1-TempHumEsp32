using System;
using System.Diagnostics;
using System.Threading;
using DevBot9.Protocols.Homie;

namespace HTP1_TempHumEsp32
{

    public class Program
    {
        private const string BrokerIp = "192.168.1.44";
        private const string WifiSsid = "";
        private const string WifiPassword = "";
        public static void Main()
        {
            var networkProvider = new NetworkProvider();
            var isConnectedToNetwork = networkProvider.ConnectToNetwork(WifiSsid, WifiPassword);

            var tempHumProvider = new TempHumProvider();
            var tempHumConsumer = new TempHumConsumer(BrokerIp);

            if (isConnectedToNetwork) {
                DeviceFactory.Initialize();
                tempHumProvider.Initialize();
                
                tempHumConsumer.MqttClientGuid = Guid.NewGuid().ToString();
                tempHumConsumer.TempHumProvider = tempHumProvider;
                tempHumConsumer.Initialize();

                Thread.Sleep(-1);
            } else {
                Debug.WriteLine("Exiting...");
            }
        }
    }
}
