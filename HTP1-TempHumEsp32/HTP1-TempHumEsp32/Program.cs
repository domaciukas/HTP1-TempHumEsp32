using DevBot9.Protocols.Homie;
using nanoFramework.UI;
using System.Diagnostics;
using System.Threading;

namespace HTP1_TempHumEsp32 {

    public class Program {
        private const string BrokerIp = "172.16.0.2";
        public static void Main() {
            void AddToLog(string severity, string message) {
                Debug.WriteLine($"{severity}:{message}");
            }

            var networkProvider = new NetworkProvider();
            new Thread(() => { networkProvider.MonitorWifiNetworksContinuously(); }).Start();

            Bitmap fullScreenBitmap = DisplayControl.FullScreen;
            fullScreenBitmap.Clear();

            Font tempHumFont = Resource.GetFont(Resource.FontResources.temperatureFont);
            //Font ipFont = Resource.GetFont(Resource.FontResources.IpFont);

            var tempHumDisplay = new TempHumDisplay(fullScreenBitmap, tempHumFont);
            tempHumDisplay.Initialize();
            tempHumDisplay.PrintStuff(21.21, 52.36, NetworkProvider.GetIpAddress());

            var tempHumProvider = new TempHumProvider();
            var tempHumProducer = new TempHumProducer();

            DeviceFactory.Initialize();
            tempHumProvider.Initialize();

            tempHumProducer.TempHumProvider = tempHumProvider;
            tempHumProducer.TempHumDisplay = tempHumDisplay;
            tempHumProducer.Initialize(BrokerIp, (severity, message) => AddToLog(severity, "TempHumProducer:" + message));

            Thread.Sleep(-1);

            Debug.WriteLine("Exiting...");
        }
    }
}
