using System;
using System.Device.I2c;

namespace HTP1_TempHumEsp32
{
    class TempHumProvider
    {
        private const byte Address = 0x40;
        private const byte MeasureHumidityCommand = 0xF5;
        private const byte MeasureTemperatureCommand = 0xF3;
        private const byte SetupCommandWrite = 0xE6;
        private const byte SetupCommandRead = 0xE7;
        private const byte SetupData = 0x00;

        private I2cDevice _sensor;

        public TempHumProvider(int i2cBus) {
            _sensor = I2cDevice.Create(new I2cConnectionSettings(i2cBus, Address, I2cBusSpeed.StandardMode));
        }

        public void Initialize() {
            var initSettings = ReadByte(SetupCommandRead);
            if (initSettings != SetupData) WriteByte(SetupCommandWrite, SetupData);
        }

        public double GetTemperature() {
            var temperatureMeasurement = ReadWord(MeasureTemperatureCommand);
            var result = ((175.72 * temperatureMeasurement) / 65536) - 46.85;
            return result;
        }

        public double GetHumidity() {
            var humidityMeasurement = ReadWord(MeasureHumidityCommand);
            var result = ((125 * humidityMeasurement) / 65536) - 6;
            return result;
        }

        private void WriteByte(byte command, byte value) {
            _sensor.Write(new byte[] { command });
            _sensor.Write(new byte[] { value });
        }
        private byte ReadByte(byte command) {
            byte[] buffer = new byte[1];
            _sensor.Write(new byte[] {command});
            _sensor.Read(buffer);
            return buffer[0];
        }

        private int ReadWord(byte command) {
            byte[] buffer = new byte[2];
            _sensor.Write(new byte[] {command});
            _sensor.Read(buffer);
            return buffer[0] << 8 + buffer[1];
        }

    }
    
}
