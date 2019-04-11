using FFImageLoading.Forms;
using Microsoft.AppCenter.Crashes;
using Mobile.Helpers;
using Mobile.Models;
using Plugin.Media;
using System;
using System.IO;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile.Cells
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CameraPage : ContentPage
    {
        private ChatRoomLoader chatRoomLoader;
        private byte[] bufferMemory;
        public CameraPage()
        {
            InitializeComponent();
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
                await GetPhotoCam();
            }
            else if (action == "Photos")
            {
                //Photos
                await GetPhotoLibrary();
            }
            else
            {
                await Navigation.PopModalAsync();
            }
        }

        private void ProcessPhotoInStream(Stream stream)
        {
            SendImage.CacheType = FFImageLoading.Cache.CacheType.None;
            SendImage.LoadingPlaceholder = "load.gif";
            SendImage.ErrorPlaceholder = "error.png";
            SendImage.Source = null;
            SendImage.Source = ImageSource.FromStream(() => stream);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            CachedImage.InvalidateCache(SendImage.Source, FFImageLoading.Cache.CacheType.All, true);
        }

     
        private async Task GetPhotoCam()
        {
            try
            {
                await CrossMedia.Current.Initialize();
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    DependencyService.Get<IMessage>().ShortAlert("No camera available.");
                    await Navigation.PopModalAsync();
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
                    await Navigation.PopModalAsync();
                }
                else
                {
                    Stream stream = file.GetStream();
                    bufferMemory = Logic.GetByteArrayFromString(stream);
                    ProcessPhotoInStream(stream);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
                await Navigation.PopModalAsync();
            }
        }
        private async Task GetPhotoLibrary()
        {
            try
            {
                await CrossMedia.Current.Initialize();
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    DependencyService.Get<IMessage>().ShortAlert("Gallery Permissions not available.");
                    await Navigation.PopModalAsync();
                }

                Plugin.Media.Abstractions.MediaFile file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    SaveMetaData = true,
                });

                if (file == null)
                {
                    await Navigation.PopModalAsync();
                }
                else
                {
                    Stream stream = file.GetStream();
                    bufferMemory = Logic.GetByteArrayFromString(stream);
                    ProcessPhotoInStream(stream);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
                await Navigation.PopModalAsync();
            }
        }

        private async void Send_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
            await Task.Delay(60);
            MessagingCenter.Send<object, ImageSender>(this, Constants.image_sender, new ImageSender
            {
                body = captionLabel.Text,
                stream = bufferMemory
            });

            //await Task.Delay(10);
        }
    }
}