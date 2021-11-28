using DotWriterServer.Dto;
using MovementManager;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;

namespace DotWriterServer.Services
{
    public class DotWriterService : IDotWriterService
    {
        private readonly IActuatorService _actuator;
        public DotWriterService(IActuatorService actuator)
        {
            _actuator = actuator;
        }
        public WebResponse<bool> Execute(string base64Image)
        {
            Bitmap bitmap = ToBitmap(base64Image);
            _ = Task.Run(() =>
            {
                try 
                {
                    _actuator.ExecuteImageWriter(bitmap);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Failed to execute: " + e.Message);
                }
            });
            return WebResponse<bool>.SuccessResponse(true);
        }

        private Bitmap ToBitmap(string base64Image)
        {
            byte[] bitmapData = System.Convert.FromBase64String(FixBase64ForImage(base64Image));
            System.IO.MemoryStream streamBitmap = new System.IO.MemoryStream(bitmapData);
            Bitmap bitImage = new Bitmap((Bitmap)Image.FromStream(streamBitmap));
            return bitImage;
        }

        public string FixBase64ForImage(string Image)
        {
            System.Text.StringBuilder sbText = new System.Text.StringBuilder(Image, Image.Length);
            sbText.Replace("\r\n", string.Empty); 
            sbText.Replace(" ", string.Empty);
            return sbText.ToString();
        }
    }
}