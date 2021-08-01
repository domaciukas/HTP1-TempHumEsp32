using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using DevBot9.Protocols.Homie;
using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Exceptions;
using nanoFramework.M2Mqtt.Messages;

namespace HTP1_TempHumEsp32
{
    class TempHumProducer {
        public string MqttBrokerIp;
        public string MqttClientGuid;
        public TempHumProvider TempHumProvider;
        public TempHumDisplay TempHumDisplay;
        public NetworkProvider NetworkProvider;

        private MqttClient _mqttClient;
        private HostDevice _hostDevice;
        private HostFloatProperty _temperature;
        private HostFloatProperty _humidity;

        public TempHumProducer(string brokerIp) {
            MqttBrokerIp = brokerIp;
            _mqttClient = new MqttClient(MqttBrokerIp);
            _mqttClient.MqttMsgPublishReceived += HandlePublishReceived;
        }
        private void HandlePublishReceived(object sender, nanoFramework.M2Mqtt.Messages.MqttMsgPublishEventArgs e) {
            _hostDevice.HandlePublishReceived(e.Topic, Encoding.UTF8.GetString(e.Message, 0, e.Message.Length));
        }

        public void Initialize() {
            try {
                _mqttClient.Connect(MqttClientGuid);
            }
            catch (MqttConnectionException ex)
            {
                Debug.WriteLine("Failed to connect to MQTT server.");
            }

            _hostDevice = DeviceFactory.CreateHostDevice("si7020", "Si7020 on ESP32");

            _hostDevice.UpdateNodeInfo("general", "General information and properties", "no-type");

            _temperature = _hostDevice.CreateHostFloatProperty(PropertyType.State, "general", "temperature", "Measured temperature", 0.00f, "°C");
            _humidity = _hostDevice.CreateHostFloatProperty(PropertyType.State, "general", "humidity", "Measured humidity", 0.00f, "%");


            

            _hostDevice.Initialize((topic, value, qosLevel, isRetained) => {
                _mqttClient.Publish(topic, Encoding.UTF8.GetBytes(value), MqttQoSLevel.AtLeastOnce, true);

            }, topic => {
                _mqttClient.Subscribe(new string[] { topic }, new [] {MqttQoSLevel.AtLeastOnce});
            });

            var gettingValues = new Thread(GetTemperatureAndHumidityValues);
            gettingValues.Start();
        }

        private void GetTemperatureAndHumidityValues() {
            while (true) {
                _temperature.Value = (float) TempHumProvider.GetTemperature();
                _humidity.Value = (float) TempHumProvider.GetHumidity();
                TempHumDisplay.PrintStuff(TempHumProvider.GetTemperature(), TempHumProvider.GetHumidity(), NetworkProvider.GetIpAddress());
                Thread.Sleep(1500);
            }
        }

    }
}
