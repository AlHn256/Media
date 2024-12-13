using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace FramesPlayer
{
   public class VideoImageExporter
    {
        private string _resultDirPath;
        private int _oldTimeMetricValue;
        private int _step;
        private ImageSaveResizeSettings _currentExportSettings;

        public void InitData(string filePath,int framesCount, ImageSaveResizeSettings settings)
        {
            _currentExportSettings = settings;
            if (_currentExportSettings.EnableSaveImage)
            {
                if (!Directory.Exists(_currentExportSettings.ImageSaveFolder))
                    Directory.CreateDirectory(_currentExportSettings.ImageSaveFolder);

                string dirPath = Path.GetFileNameWithoutExtension(filePath);
                _currentExportSettings.TrainFolderId = dirPath;
                _resultDirPath = (_currentExportSettings.ImageSaveFolder.EndsWith("\\") ? _currentExportSettings.ImageSaveFolder : _currentExportSettings.ImageSaveFolder + '\\') + dirPath;
                if (!Directory.Exists(_resultDirPath))
                    Directory.CreateDirectory(_resultDirPath);
                _resultDirPath += '\\';

                if(framesCount>0 && _currentExportSettings.VideoFrameCount > 1)
                {
                    _step = framesCount / _currentExportSettings.VideoFrameCount;
                    if (_step == 0)
                        _step = 1;
                }
                else
                {
                    _step = _currentExportSettings.FrameExportStep;
                    if (_step == 0)
                        _step = 1;
                }
                _step *= 40;//Шаг по времени 40 мс
                _oldTimeMetricValue = 0;
            }
        }

        private Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public void ExportImage(byte[] image, int channelID, int marker)
        {
            try
            {
                if (_currentExportSettings == null)
                    return;
                if (!_currentExportSettings.EnableSaveImage)
                    return;
                if (_currentExportSettings.ChannelID != channelID)
                    return;
                if ((marker - _oldTimeMetricValue) >= _step)//Нужно сохранить кадр
                {
                    _oldTimeMetricValue = marker;
                    using (MemoryStream ms = new MemoryStream(image))
                    {
                        Image img = Image.FromStream(ms);
                        if (_currentExportSettings.EnableResize)
                            img = ResizeImage(img, _currentExportSettings.ResolutionX, _currentExportSettings.ResolutionY);
                        img.Save(_resultDirPath + marker.ToString() + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }
            }
            catch { }
        }
    }
}
