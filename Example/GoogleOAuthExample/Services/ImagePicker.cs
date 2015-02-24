using System;
using System.Threading.Tasks;
using GoogleOAuthExample.Extensions;
using Microsoft.Phone.Tasks;

namespace GoogleOAuthExample.Services
{
    public class ImagePicker : IImagePicker
    {
        public async Task<byte[]> FromLibrary()
        {
            PhotoResult photoResult;
            try
            {
                photoResult = await new PhotoChooserTask().ShowAsync();
            }
            catch (Exception)
            {
                return null;
            }

            if (photoResult.ChosenPhoto == null)
                return null;

            return GetByteArray(photoResult);
        }

        public async Task<byte[]> FromCamera()
        {
            PhotoResult photoResult;
            try
            {
                photoResult = await new CameraCaptureTask().ShowAsync();
            }
            catch (Exception)
            {
                return null;
            }

            if (photoResult.ChosenPhoto == null)
                return null;

            return GetByteArray(photoResult);
        }

        private byte[] GetByteArray(PhotoResult photoResult)
        {
            var imageData = new byte[photoResult.ChosenPhoto.Length];
            photoResult.ChosenPhoto.Read(imageData, 0, Convert.ToInt32(photoResult.ChosenPhoto.Length));
            return imageData;
        }
    }
}
