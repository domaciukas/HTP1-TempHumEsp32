using System;
using System.Diagnostics;
using System.Threading;
using DevBot9.Protocols.Homie;
using nanoFramework.UI;

namespace HTP1_TempHumEsp32
{

    public class Program
    {
        private const string BrokerIp = "192.168.1.43";
        private const string WifiSsid = "Telia-2.4G-Greitas-5FD576";
        private const string WifiPassword = "Y49T9VCR9UH";
        public static void Main()
        {
            var networkProvider = new NetworkProvider();
            var isConnectedToNetwork = networkProvider.ConnectToNetwork(WifiSsid, WifiPassword);

            Bitmap fullScreenBitmap = DisplayControl.FullScreen;
            fullScreenBitmap.Clear();

            Font tempHumFont = Resource.GetFont(Resource.FontResources.temperatureFont);
            //Font ipFont = Resource.GetFont(Resource.FontResources.IpFont);

            var tempHumDisplay = new TempHumDisplay(fullScreenBitmap, tempHumFont);
            tempHumDisplay.Initialize();
            tempHumDisplay.PrintStuff(21.21,52.36,networkProvider.GetIpAddress());

            var tempHumProvider = new TempHumProvider();
            var tempHumConsumer = new TempHumProducer(BrokerIp);
            

            if (isConnectedToNetwork) {
                DeviceFactory.Initialize();
                tempHumProvider.Initialize();
                
                tempHumConsumer.MqttClientGuid = Guid.NewGuid().ToString();
                tempHumConsumer.TempHumProvider = tempHumProvider;
                tempHumConsumer.NetworkProvider = networkProvider;
                tempHumConsumer.TempHumDisplay = tempHumDisplay;
                tempHumConsumer.Initialize();

                Thread.Sleep(-1);
            } else {
                Debug.WriteLine("Exiting...");
            }
        }
    }
}
