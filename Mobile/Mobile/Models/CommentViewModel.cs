using Microsoft.AppCenter.Crashes;
using Microsoft.AspNetCore.SignalR.Client;
using Mobile.Helpers;
using Mobile.Helpers.Local;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Mobile.Models
{
    public class CommentViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<CommentLoader> Loaders { get; set; } = new ObservableCollection<CommentLoader>();

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

        private HubConnection hubConnection;
        public async Task ConnectToHub()
        {
            try
            {
                if (Logic.IsInternet())
                {
                    if (!IsHubConnected())
                    {
                        await hubConnection.StartAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
            }
        }
        public async Task DisConnectHub()
        {
            try
            {
                if (hubConnection == null)
                { ResetConnection(); }
                await hubConnection.StopAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
            }
        }
        public bool IsHubConnected()
        {
            try
            {
                if (hubConnection == null)
                { ResetConnection(); }
                HubConnectionState state = hubConnection.State;
                return state == HubConnectionState.Connected;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
                return false;
            }
        }

        private void ResetConnection()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl("https://confessbackend.azurewebsites.net/chatHub")
                .Build();
        }
        public CommentViewModel()
        {
            #region Signal
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChangedAsync;
            ResetConnection();

            hubConnection.On<string>("ReceiveAddComment", (message) =>
            {
                try
                {
                    CommentLoader load = JsonConvert.DeserializeObject<CommentLoader>(message);
                    //filter
                    if (!string.IsNullOrEmpty(load.LikeColorString))
                    {
                        load.LikeColor = Color.FromHex(load.LikeColorString);
                    }

                    if (!string.IsNullOrEmpty(load.DislikeColorString))
                    {
                        load.DislikeColor = Color.FromHex(load.DislikeColorString);
                    }
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Loaders.Add(load);
                    });
                    OnPropertyChanged(nameof(Loaders));

                    //save to local db
                    LocalStore.Comment.SaveLoader(load);

                    //maybe scroll down

                }

                catch (Exception ex)
                {
                    Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
                }
            });

            hubConnection.On<string>("ReceiveLikeComment", async (message) =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(message))
                    {
                        CommentSender incoming = JsonConvert.DeserializeObject<CommentSender>(message);
                        //update the model
                        if (Loaders.Any(d => d.Guid == incoming.CommentGuid))
                        {
                            int index = Loaders.IndexOf(Loaders.FirstOrDefault(d => d.Guid == incoming.CommentGuid));
                            CommentLoader replacer = Loaders.FirstOrDefault(d => d.Guid == incoming.CommentGuid);
                            replacer.Likes = incoming.Count;
                            //mark it blue if I am the one that liked
                            if (incoming.Key == await Logic.GetKey())
                            {
                                replacer.LikeColorString = "#1976D2";
                                replacer.LikeColor = Color.FromHex("#1976D2");
                            }
                            else { Logic.VibrateNow(); }
                            Loaders.RemoveAt(index);
                            Loaders.Insert(index, replacer);

                            //notify UI
                            OnPropertyChanged(nameof(Loaders));
                            //update the db
                            LocalStore.Comment.UpdateLoader(replacer);
                        }

                    }
                }

                catch (Exception ex)
                {
                    Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
                }
            });

            hubConnection.On<string>("ReceiveDislikeComment", async (message) =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(message))
                    {
                        CommentSender incoming = JsonConvert.DeserializeObject<CommentSender>(message);
                        //update the model
                        if (Loaders.Any(d => d.Guid == incoming.CommentGuid))
                        {
                            int index = Loaders.IndexOf(Loaders.FirstOrDefault(d => d.Guid == incoming.CommentGuid));
                            CommentLoader replacer = Loaders.FirstOrDefault(d => d.Guid == incoming.CommentGuid);
                            replacer.DisLikes = incoming.Count;
                            //mark it blue if I am the one that liked
                            if (incoming.Key == await Logic.GetKey())
                            {
                                replacer.DislikeColorString = "#1976D2";
                                replacer.DislikeColor = Color.FromHex("#1976D2");
                            }
                            else { Logic.VibrateNow(); }
                            Loaders.RemoveAt(index);
                            Loaders.Insert(index, replacer);

                            //notify UI
                            OnPropertyChanged(nameof(Loaders));
                            //update the db
                            LocalStore.Comment.UpdateLoader(replacer);
                        }

                    }
                }

                catch (Exception ex)
                {
                    Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
                }
            });

            hubConnection.On<string>("ReceiveDeleteComment", (message) =>
            {
                try
                {
                    //delete from model
                    if (!string.IsNullOrEmpty(message))
                    {
                        Loaders.Remove(Loaders.FirstOrDefault(d=>d.Guid == message));
                        OnPropertyChanged(nameof(Loaders));

                        //remove from db
                        LocalStore.Comment.DeleteLoader(message);
                    }
                }

                catch (Exception ex)
                {
                    Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
                }
            });
            #endregion


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
                    //check if this user owns this comment


                    if (load.Owner_Guid == await Logic.GetKey())
                    {
                        DependencyService.Get<IMessage>().ShortAlert("You can't like your Comment.");
                    }
                    else
                    {
                        //post a new like 
                        try
                        {
                            IsBusy = true;
                            CommentSender sender = new CommentSender
                            {
                                CommentGuid = guid,
                                IsComment = true,
                                ConfessGuid = Confess_Guid,
                                Key = await Logic.GetKey()
                            };
                            //send to signal r
                            string serialisedMessage = JsonConvert.SerializeObject(sender);
                            await ConnectToHub();
                            await hubConnection.InvokeAsync("SendLikeComment", serialisedMessage);
                            await Task.Delay(10);
                        }
                        catch (Exception ex)
                        {
                            Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
                        }
                        finally
                        {
                            IsBusy = false;
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
                //check if this user owns this comment

                if (load.Owner_Guid == await Logic.GetKey())
                {
                    DependencyService.Get<IMessage>().ShortAlert("You can't dislike your Comment.");
                }
                else
                {
                    //post a new dislike 
                    try
                    {
                        IsBusy = true;
                        CommentSender sender = new CommentSender
                        {
                            CommentGuid = guid,
                            IsComment = true,
                            ConfessGuid = Confess_Guid,
                            Key = await Logic.GetKey()
                        };
                        //send to signal r
                        string serialisedMessage = JsonConvert.SerializeObject(sender);
                        await ConnectToHub();
                        await hubConnection.InvokeAsync("SendDislikeComment", serialisedMessage);
                        await Task.Delay(10);
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
                    }
                    finally
                    {
                        IsBusy = false;
                    }
                }
            });
            OnDeleteCommentCommand = new Command(async (arg) =>
            {
                try
                {
                    IsBusy = true;
                    string guid = (string)arg;

                    //send to signal r
                    await ConnectToHub();
                    await hubConnection.InvokeAsync("SendDeleteComment", guid);
                    await Task.Delay(10);
                    RemoveQuoteCommand.Execute(null);
                    DependencyService.Get<IMessage>().ShortAlert("Deleted");

                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
                }
                finally
                {
                    IsBusy = false;
                }
            });
            OnSendCommand = new Command(async () =>
            {

                if (!Logic.IsInternet())
                {
                    DependencyService.Get<IMessage>().ShortAlert(Constants.No_Internet);
                    return;
                }
                //comment_Input
                if (string.IsNullOrWhiteSpace(TextToSend.Trim()))
                {
                    DependencyService.Get<IMessage>().ShortAlert("Enter some text");
                    return;
                }
                try
                {
                    IsBusy = true;
                    Comment newComment = new Comment()
                    {
                        Body = TextToSend.Trim(),
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

                    //send to signal r
                    string serialisedMessage = JsonConvert.SerializeObject(newComment);
                    await ConnectToHub();
                    await hubConnection.InvokeAsync("SendAddComment", serialisedMessage);
                    await Task.Delay(10);


                    RemoveQuoteCommand.Execute(null);
                    //reload data
                    if (AppConstants.ShowAds)
                    {
                        await DependencyService.Get<IAdmobInterstitialAds>().Display(AppConstants.InterstitialAdId);
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
                }
                finally
                {
                    IsBusy = false;
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

        private void Connectivity_ConnectivityChangedAsync(object sender, ConnectivityChangedEventArgs e)
        {
            Task.Run(async () =>
            {
                await LoadData();
            });
        }
        private async Task LoadData()
        {
            ObservableCollection<CommentLoader> temploaders = new ObservableCollection<CommentLoader>();
            try
            {
                IsBusy = true;
                if (!Logic.IsInternet())
                {
                    temploaders = LocalStore.Comment.FetchByConfessGuid(Confess_Guid);
                }
                else
                {
                    temploaders = await Store.CommentClass.FetchComment(Confess_Guid);
                    if (temploaders == null || temploaders.Count == 0)
                    {
                        temploaders = LocalStore.Comment.FetchByConfessGuid(Confess_Guid);
                        if (temploaders == null || temploaders.Count == 0)
                        {
                            IsEmptyComments = true;
                        }
                    }
                    else
                    {
                        LocalStore.Comment.SaveLoaders(temploaders);
                        IsEmptyComments = false;
                    }
                }
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
                }
                Device.BeginInvokeOnMainThread(() =>
                {
                    Loaders = temploaders;
                });
                OnPropertyChanged(nameof(Loaders));
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
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
