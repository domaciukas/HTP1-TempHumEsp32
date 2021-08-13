using System.Threading;
using DevBot9.Protocols.Homie;
using DevBot9.Protocols.Homie.Utilities;

namespace HTP1_TempHumEsp32 {
    class TempHumProducer {
        public string MqttBrokerIp;
        public TempHumProvider TempHumProvider;
        public TempHumDisplay TempHumDisplay;

        private HostDevice _hostDevice;
        private HostNumberProperty _temperature;
        private HostNumberProperty _humidity;
        private PahoHostDeviceConnection _broker;

        public TempHumProducer() {
            _broker = new PahoHostDeviceConnection();
        }

        public void Initialize(string brokerIp, AddToLogDelegate addToLog) {
            MqttBrokerIp = brokerIp;

            _hostDevice = DeviceFactory.CreateHostDevice("si7020", "Si7020 on ESP32");

            _hostDevice.UpdateNodeInfo("general", "General information and properties", "no-type");

            _temperature = _hostDevice.CreateHostNumberProperty(PropertyType.State, "general", "temperature", "Measured temperature", 0.00f, "°C");
            _humidity = _hostDevice.CreateHostNumberProperty(PropertyType.State, "general", "humidity", "Measured humidity", 0.00f, "%");

            _broker.Initialize(MqttBrokerIp, (severity, message) => addToLog(severity, "Broker:" + message));
            _hostDevice.Initialize(_broker, (severity, message) => addToLog(severity, message));

            var gettingValues = new Thread(GetTemperatureAndHumidityValues);
            gettingValues.Start();
        }

        private void GetTemperatureAndHumidityValues() {
            while (true) {
                _temperature.Value = (float)TempHumProvider.GetTemperature();
                _humidity.Value = (float)TempHumProvider.GetHumidity();
                TempHumDisplay.PrintStuff(TempHumProvider.GetTemperature(), TempHumProvider.GetHumidity(), NetworkProvider.GetIpAddress());
                Thread.Sleep(1500);
            }
        }

    }
}
