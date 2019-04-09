using FFImageLoading.Forms;
using Mobile.Helpers;
using Mobile.Models;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile.Cells
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CameraPage : ContentPage
	{
        private ChatRoomLoader chatRoomLoader;
        private Stream stream;
		public CameraPage ()
		{
			InitializeComponent ();
		}

        public CameraPage(ChatRoomLoader _chatRoomLoader)
        {
            InitializeComponent();
            this.chatRoomLoader = _chatRoomLoader;
           
        }

        protected override void OnAppearing()
        {
            LoadData();
        }

        private async void LoadData()
        {
            string action = await DisplayActionSheet("Pick photo from", "Cancel", null, "Camera", "Photos");

            if (action == "Camera")
            {
                //Camera
                stream = await GetPhotoCam();
                if (stream == null)
                {
                    await Navigation.PopModalAsync();
                }
                else
                {
                    //process image
                    ProcessPhotoInStream();
                }
            }
            else if (action == "Photos")
            {
                //Photos
                stream = await GetPhotoLibrary();
                if (stream == null)
                {
                    await Navigation.PopModalAsync();
                }
                else
                {
                    //process
                    ProcessPhotoInStream();
                }
            }
            else
            {
                await Navigation.PopModalAsync();
            }
        }

        private void ProcessPhotoInStream()
        {
            SendImage.CacheDuration = TimeSpan.FromMinutes(5);
            SendImage.LoadingPlaceholder = "load.gif";
            SendImage.ErrorPlaceholder = "error.png";
            SendImage.Source = ImageSource.FromStream(() => stream);
        }

        private async void Send_Clicked(object sender, EventArgs e)
        {
            //send the stream and the caption to the lister in chatmodel
            MessagingCenter.Send<object, ImageSender>(this, Constants.image_sender, new ImageSender
            {
                body = captionLabel.Text,
                stream = this.stream
            });
            await Task.Delay(10);
            await Navigation.PopModalAsync();
        }

        private async Task<Stream> GetPhotoCam()
        {
            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                DependencyService.Get<IMessage>().ShortAlert("No camera available.");
                return null;
            }

            Plugin.Media.Abstractions.MediaFile file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Profile_Pictures",
                Name = $"Confessor{Guid.NewGuid().ToString().Replace("-", "")}.png",
                SaveToAlbum = true,
                AllowCropping = true,
                DefaultCamera = Plugin.Media.Abstractions.CameraDevice.Front,
                SaveMetaData = true
            });

            if (file == null)
            {
                return null;
            }
            Stream stream = file.GetStream();
            return stream;
        }
        private async Task<Stream> GetPhotoLibrary()
        {
            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                DependencyService.Get<IMessage>().ShortAlert("Gallery Permissions not available.");
                return null;
            }

            Plugin.Media.Abstractions.MediaFile file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                SaveMetaData = true,
            });

            if (file == null)
            {
                return null;
            }
            Stream stream = file.GetStream();
            return stream;
        }
    }
}