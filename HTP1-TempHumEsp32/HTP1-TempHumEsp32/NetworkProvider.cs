using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading;
using Windows.Devices.WiFi;

namespace HTP1_TempHumEsp32 {
    class NetworkProvider {
        public void MonitorWifiNetworksContinuously() {
            var isOk = false;
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            // Get the first WiFI Adapter
            var wifiAdapter = WiFiAdapter.FindAllAdapters()[0];

            // Set up the AvailableNetworksChanged event to pick up when scan has completed
            wifiAdapter.AvailableNetworksChanged += HandleWifiNetworksChanged;

            // Loop forever scanning every 30 seconds
            while (true) {
                Debug.WriteLine("Scanning for WiFi networks...");
                wifiAdapter.ScanAsync();

                Thread.Sleep(30000);
            }
        }

        public static string GetIpAddress() {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            NetworkInterface networkInterface = networkInterfaces[0];
            return networkInterface.IPv4Address;
        }

        private void HandleWifiNetworksChanged(WiFiAdapter sender, object e) {
            Debug.WriteLine("WiFi networks changed.");

            var currentIpAddres = GetIpAddress();
            var needToConnect = string.IsNullOrEmpty(currentIpAddres) || (currentIpAddres == "0.0.0.0");


            if (needToConnect) {
                Debug.WriteLine("We're not connected to any WiFi network. Connecting.");
                var wiFiConfiguration = Wireless80211Configuration.GetAllWireless80211Configurations()[0];
                var report = sender.NetworkReport;
                foreach (var network in report.AvailableNetworks) {
                    // Show all networks found
                    Debug.WriteLine($"Net SSID :{network.Ssid},  BSSID : {network.Bsid},  rssi : {network.NetworkRssiInDecibelMilliwatts},  signal : {network.SignalBars}");


                    // If its our Network then try to connect
                    if (network.Ssid == wiFiConfiguration.Ssid) {

                        var result = sender.Connect(network, WiFiReconnectionKind.Automatic, wiFiConfiguration.Password);

                        // Display status
                        if (result.ConnectionStatus == WiFiConnectionStatus.Success) {
                            Debug.WriteLine($"Connected to Wifi network {network.Ssid}.");
                        }
                        else {
                            Debug.WriteLine($"Error {result.ConnectionStatus} connecting to Wifi network {network.Ssid}.");
                        }
                    }
                }
            }
            else {
                Debug.WriteLine("We're still connected to WiFi. Will do nothing.");
            }
        }
    }
}
