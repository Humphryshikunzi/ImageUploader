using Microsoft.WindowsAzure.Storage;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ImageUploader
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void UpLoadImageButton_Clicked(object sender, EventArgs e)
        {
            //! added using Plugin.Media;
            await CrossMedia.Current.Initialize();

            //// if you want to take a picture use this
            // if(!CrossMedia.Current.IsTakePhotoSupported || !CrossMedia.Current.IsCameraAvailable)
            /// if you want to select from the gallery use this
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Not supported", "Your device does not currently support this functionality", "Ok");
                return;
            }

            //! added using Plugin.Media.Abstractions;
            // if you want to take a picture use StoreCameraMediaOptions instead of PickMediaOptions
            var mediaOptions = new PickMediaOptions()
            {
                PhotoSize = PhotoSize.Medium
            };
            // if you want to take a picture use TakePhotoAsync instead of PickPhotoAsync
            var selectedImageFile = await CrossMedia.Current.PickPhotoAsync(mediaOptions);

            if (SelectedImage == null)
            {
                await DisplayAlert("Error", "Could not get the image, please try again.", "Ok");
                return;
            }

            SelectedImage.Source = ImageSource.FromStream(() => selectedImageFile.GetStream());
            UploadImage(selectedImageFile.GetStream());
        }
        public async void UploadImage(Stream imageToUpload)
        {
            //var connectionString = "DefaultEndpointsProtocol=https;AccountName=salonimages;AccountKey=jkYCUdbL4gQBdp/Q7nXj3Y+VWunKlqF9upxuHkYzJ798Us5bC5F8PhZ8elJV45ZnipaKAqgxSeydHWNR/WxcmQ==;EndpointSuffix=core.windows.net";
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=salonimages;AccountKey=pXMU/BmRz8HBDNdjN1jRXCYU10585AsoUSxDAnnd++ip30P1exqlPo0k2FZQXKjagUUEWi0u7ZxpLIrtM7eNzw==;EndpointSuffix=core.windows.net";
            var account = CloudStorageAccount.Parse(connectionString);
            var blobClient = account.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("salonimages");
            string uniqueName = Guid.NewGuid().ToString();
            var userKey = "1/Products/";
            var blockBlob = container.GetBlockBlobReference($"{userKey}{uniqueName}.jpg");
            await blockBlob.UploadFromStreamAsync(imageToUpload);
            string thePlaceInTheInternetWhereThisImageIsNowLocated = blockBlob.Uri.OriginalString;
        }

    }
}
