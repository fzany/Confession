using Microsoft.AppCenter.Crashes;
using Mobile.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Mobile.Models
{
    public class CommentViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<CommentLoader> Loaders { get; set; } = new ObservableCollection<CommentLoader>();
        public ConfessLoader newloader { get; set; }

        public string TextToSend { get; set; }

        public string ConfessionTitle { get; set; }
        public ICommand OnSendCommand { get; set; }
        public ICommand OnQuoteCommand { get; set; }
        public ICommand RemoveQuoteCommand { get; set; }

        public ICommand OnLikeCommentCommand { get; set; }
        public ICommand OnDislikeCommentCommand { get; set; }

        public ICommand OnDeleteCommentCommand { get; set; }

        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        private bool isEmptyComments;
        public bool IsEmptyComments
        {
            get => isEmptyComments;
            set
            {
                isEmptyComments = value;
                OnPropertyChanged(nameof(IsEmptyComments));
            }
        }

        private bool isBannerAdVisible;
        public bool IsBannerAdVisible
        {
            get => isBannerAdVisible;
            set
            {
                isBannerAdVisible = value;
                OnPropertyChanged(nameof(IsBannerAdVisible));
            }
        }

        public string Confess_Guid { get; set; }

        private CommentLoader quotedComment;
        public CommentLoader QuotedComment
        {
            get => quotedComment;
            set
            {
                quotedComment = value;
                OnPropertyChanged(nameof(QuotedComment));
            }
        }

        private bool isQuotedCommentAvailable;
        public bool IsQuotedCommentAvailable
        {
            get => isQuotedCommentAvailable;
            set
            {
                isQuotedCommentAvailable = value;
                OnPropertyChanged(nameof(IsQuotedCommentAvailable));
            }
        }


        public CommentViewModel()
        {

            OnLikeCommentCommand = new Command(async (arg) =>
            {
                if (!Logic.IsInternet())
                {
                    DependencyService.Get<IMessage>().ShortAlert(Constants.No_Internet);
                }
                else
                {
                    string guid = (string)arg;
                    CommentLoader load = new CommentLoader();
                    if (Loaders.Any(d => d.Guid.Equals(guid)))
                    {
                        load = Loaders.FirstOrDefault(d => d.Guid.Equals(guid));
                    }
                    //check if this user owns this confession


                    if (load.Owner_Guid == await Logic.GetKey())
                    {
                        DependencyService.Get<IMessage>().ShortAlert("You can't like your Comment.");
                    }
                    else
                    {
                        //post a new like 
                        try
                        {
                            ConfessSender result = await Store.LikeClass.Post(guid, true, Confess_Guid);

                            ConfessLoader data = Logic.ProcessConfessLoader(result.Loader);
                            MessagingCenter.Send<object, ConfessLoader>(this, Constants.ReloadViewPage, data);

                            Task.Run(async () =>
                             {
                                 await LoadData();
                             });

                            if (!result.IsSuccessful)
                            {
                                //update the model
                                Logic.VibrateNow();
                            }

                        }
                        catch (Exception ex)
                        {
                            Crashes.TrackError(ex);
                        }
                    }
                }
            });
            OnDislikeCommentCommand = new Command(async (arg) =>
            {

                if (!Logic.IsInternet())
                {
                    DependencyService.Get<IMessage>().ShortAlert(Constants.No_Internet);
                    return;
                }
                string guid = (string)arg;
                CommentLoader load = new CommentLoader();
                if (Loaders.Any(d => d.Guid.Equals(guid)))
                {
                    load = Loaders.FirstOrDefault(d => d.Guid.Equals(guid));
                }
                //check if this user owns this confession


                if (load.Owner_Guid == await Logic.GetKey())
                {
                    DependencyService.Get<IMessage>().ShortAlert("You can't dislike your Comment.");
                }
                else
                {
                    //post a new like 
                    try
                    {
                        ConfessSender result = await Store.DislikeClass.Post(guid, true, Confess_Guid);
                        ConfessLoader data = Logic.ProcessConfessLoader(result.Loader);

                        //ViewPage viewPage = new ViewPage()
                        //{
                        //    BindingContext = data
                        //};

                        Task.Run(async () =>
                        {
                            await LoadData();
                        });
                        if (!result.IsSuccessful)
                        {
                            Logic.VibrateNow();
                        }
                        MessagingCenter.Send<object, ConfessLoader>(this, Constants.ReloadViewPage, data);

                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                }
            });
            OnDeleteCommentCommand = new Command(async (arg) =>
            {
                try
                {
                    string guid = (string)arg;
                    newloader = await Store.CommentClass.DeleteComment(guid, Confess_Guid);
                    if (newloader != null)
                    {
                        ConfessLoader data = Logic.ProcessConfessLoader(newloader);
                        MessagingCenter.Send<object, ConfessLoader>(this, Constants.ReloadViewPage, data);

                        //ViewPage viewPage = new ViewPage()
                        //{
                        //    BindingContext = data
                        //};
                    };
                    RemoveQuoteCommand.Execute(null);
                    Task.Run(async () =>
                    {
                        await LoadData();
                    });
                    Logic.VibrateNow();
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
                DependencyService.Get<IMessage>().ShortAlert("Deleted");
            });


            OnSendCommand = new Command(async () =>
            {

                if (!Logic.IsInternet())
                {
                    DependencyService.Get<IMessage>().ShortAlert(Constants.No_Internet);
                    return;
                }
                //comment_Input
                if (string.IsNullOrWhiteSpace(TextToSend))
                {
                    DependencyService.Get<IMessage>().ShortAlert("Enter some text");
                    return;
                }
                try
                {
                    Comment newComment = new Comment()
                    {
                        Body = TextToSend,
                        Confess_Guid = Confess_Guid,
                        Owner_Guid = await Logic.GetKey(),
                        QuotedCommentAvailable = IsQuotedCommentAvailable,
                    };
                    TextToSend = string.Empty;
                    OnPropertyChanged(nameof(TextToSend));
                    if (IsQuotedCommentAvailable & QuotedComment != null)
                    {
                        newComment.Quote = new CommentQuote()
                        {
                            Body = QuotedComment.Body,
                        };
                    }

                    newloader = await Store.CommentClass.CreateComment(newComment, Confess_Guid);
                    ConfessLoader data = Logic.ProcessConfessLoader(newloader);
                    MessagingCenter.Send<object, ConfessLoader>(this, Constants.ReloadViewPage, data);

                    DependencyService.Get<IMessage>().ShortAlert("Comment Posted.");
                    RemoveQuoteCommand.Execute(null);
                
                    Task.Run(async () =>
                    {
                        await LoadData();
                    });
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
                //reload data
                if (AppConstants.ShowAds)
                {
                    await DependencyService.Get<IAdmobInterstitialAds>().Display(AppConstants.InterstitialAdId);
                }
            });

            OnQuoteCommand = new Command((arg) =>
            {
                this.IsQuotedCommentAvailable = true;
                this.QuotedComment = Loaders.FirstOrDefault(d => d.Guid == (string)arg);

            });

            RemoveQuoteCommand = new Command(() =>
            {
                this.IsQuotedCommentAvailable = false;
                this.QuotedComment = null;
            });

            Task.Run(async () =>
            {
                await LoadData();
            });

        }
        private async Task LoadData()
        {
            try
            {
                IsBusy = true;
                ObservableCollection<CommentLoader> temploaders = await Store.CommentClass.FetchComment(Confess_Guid);
                if (temploaders == null || temploaders.Count == 0)
                {
                    IsEmptyComments = true;
                }
                else
                {
                    IsEmptyComments = false;
                    int adCounter = 0;
                    foreach (CommentLoader load in temploaders)
                    {
                        if (!string.IsNullOrEmpty(load.LikeColorString))
                        {
                            load.LikeColor = Color.FromHex(load.LikeColorString);
                        }

                        if (!string.IsNullOrEmpty(load.DislikeColorString))
                        {
                            load.DislikeColor = Color.FromHex(load.DislikeColorString);
                        }

                        //set ad visibility to every 6 items
                        adCounter++;
                        if (adCounter >= 9)
                        {
                            load.IsAdVisible = true;
                            adCounter = 0;
                        }
                        else if (Loaders.Count < 10)
                        {
                            if (adCounter == 1)
                            {
                                load.IsAdVisible = true;
                                IsBannerAdVisible = false;
                            }
                        }

                    }
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        foreach (CommentLoader data in temploaders)
                        {
                            Loaders.Add(data);
                        }
                    });
                    OnPropertyChanged(nameof(Loaders));
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            finally
            {
                IsBusy = false;
            }
            
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this,
       new PropertyChangedEventArgs(propertyName));
        }
    }
}
