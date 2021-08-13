using nanoFramework.Presentation.Media;
using nanoFramework.UI;

namespace HTP1_TempHumEsp32 {
    public class TempHumDisplay {
        private Bitmap _fullScreenBitmap;
        private Font _tempHumFont;

        public TempHumDisplay(Bitmap fullScreenBitmap, Font tempHumFont) {
            _fullScreenBitmap = fullScreenBitmap;
            _tempHumFont = tempHumFont;
        }
        public void Initialize() {
            _fullScreenBitmap.Clear();
            _fullScreenBitmap.Flush();
        }
        public void PrintStuff(double temperature, double humidity, string ipAddress) {
            _fullScreenBitmap.Clear();
            _fullScreenBitmap.DrawText(temperature.ToString("F1"), _tempHumFont, Color.Cyan, 15, 30);
            _fullScreenBitmap.DrawText(humidity.ToString("F1"), _tempHumFont, Color.Magenta, 15, 60);
            _fullScreenBitmap.DrawText(ipAddress, _tempHumFont, Color.White, 15, 100);
            _fullScreenBitmap.Flush();
        }
    }
}
