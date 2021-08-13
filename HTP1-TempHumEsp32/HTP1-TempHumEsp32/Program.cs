using System.Diagnostics;
using System.Threading;
using DevBot9.Protocols.Homie;
using nanoFramework.UI;

namespace HTP1_TempHumEsp32 {

    public class Program {
        private const string _brokerIp = "172.16.0.136";
        public static void Main() {
            void AddToLog(string severity, string message) {
                Debug.WriteLine($"{severity}:{message}");
            }

            var networkProvider = new NetworkProvider();
            new Thread(() => { networkProvider.MonitorWifiNetworksContinuously(); }).Start();

            var fullScreenBitmap = DisplayControl.FullScreen;
            fullScreenBitmap.Clear();

            var tempHumFont = Resource.GetFont(Resource.FontResources.temperatureFont);

            var tempHumDisplay = new TempHumDisplay(fullScreenBitmap, tempHumFont);
            tempHumDisplay.Initialize();
            tempHumDisplay.PrintStuff(21.21, 52.36, NetworkProvider.GetIpAddress());

            var tempHumProvider = new TempHumProvider();
            var tempHumProducer = new TempHumProducer();

            DeviceFactory.Initialize();
            tempHumProvider.Initialize();

            tempHumProducer.TempHumProvider = tempHumProvider;
            tempHumProducer.TempHumDisplay = tempHumDisplay;
            tempHumProducer.Initialize(_brokerIp, (severity, message) => AddToLog(severity, "TempHumProducer:" + message));

            Thread.Sleep(-1);

            Debug.WriteLine("Exiting...");
        }
    }
}
