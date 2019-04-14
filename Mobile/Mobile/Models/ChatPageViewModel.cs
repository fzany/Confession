using Microsoft.AppCenter.Crashes;
using Microsoft.AspNetCore.SignalR.Client;
using Mobile.Helpers;
using Mobile.Helpers.Local;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Mobile.Models
{
    public class ChatPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ChatLoader> Messages { get; set; } = new ObservableCollection<ChatLoader>();
        public string TextToSend { get; set; }
        public ICommand OnSendCommand { get; set; }
        public ICommand CloseConnectionCommand { get; set; }
        public ICommand MessageAppearingCommand { get; set; }
        public ICommand MessageDisappearingCommand { get; set; }
        public ICommand OnQuoteCommand { get; set; }
        public ICommand RemoveQuoteCommand { get; set; }

        private bool isNoInternet;
        public bool IsNoInternet
        {
            get => isNoInternet;
            set
            {
                isNoInternet = value;
                OnPropertyChanged(nameof(IsNoInternet));
            }
        }


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

        private bool isHeaderDateVisible;
        public bool IsHeaderDateVisible
        {
            get => isHeaderDateVisible;
            set
            {
                isHeaderDateVisible = value;
                OnPropertyChanged(nameof(IsHeaderDateVisible));
            }
        }

        private string _headerDateText;
        public string HeaderDateText
        {
            get => _headerDateText;
            set
            {
                _headerDateText = value;
                OnPropertyChanged(nameof(HeaderDateText));
            }
        }

        public string Room_ID { get; set; }

        private bool isShowScrollTap;
        public bool ShowScrollTap
        {
            get => isShowScrollTap;
            set
            {
                isShowScrollTap = value;
                OnPropertyChanged(nameof(ShowScrollTap));
            }
        }

        private bool isLastMessageVisible;
        public bool LastMessageVisible
        {
            get => isLastMessageVisible;
            set
            {
                isLastMessageVisible = value;
                OnPropertyChanged(nameof(LastMessageVisible));
            }
        }


        private int localPendingMessageCount;
        public int PendingMessageCount
        {
            get => localPendingMessageCount;
            set
            {
                localPendingMessageCount = value;
                OnPropertyChanged(nameof(PendingMessageCount));
            }
        }


        private bool isPendingMessageCountVisible;
        public bool PendingMessageCountVisible
        {
            get => isPendingMessageCountVisible;
            set
            {
                isPendingMessageCountVisible = PendingMessageCount > 0;
                OnPropertyChanged(nameof(PendingMessageCountVisible));
            }
        }
        public Queue<ChatLoader> DelayedMessages { get; set; } = new Queue<ChatLoader>();

        private ChatLoader quotedChat;
        public ChatLoader QuotedChat
        {
            get => quotedChat;
            set
            {
                quotedChat = value;
                OnPropertyChanged(nameof(QuotedChat));
            }
        }

        private bool isQuotedChatAvailable;
        public bool IsQuotedChatAvailable
        {
            get => isQuotedChatAvailable;
            set
            {
                isQuotedChatAvailable = value;
                OnPropertyChanged(nameof(IsQuotedChatAvailable));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

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
                {
                    ResetConnection();
                }
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
        public ChatPageViewModel()
        {
            Task forget = ConnectToHub();

            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChangedAsync;
            ResetConnection();


            hubConnection.On<string>("ReceiveMessage", async (message) =>
            {
                try
                {
                    Chat incomingChat = JsonConvert.DeserializeObject<Chat>(message);
                    ChatLoader insert_loader = new ChatLoader
                    {
                        Room_ID = incomingChat.Room_ID,
                        Body = incomingChat.Body,
                        Date = incomingChat.Date,
                        IsAd = false,
                        SenderName = incomingChat.SenderName,
                        QuotedChatAvailable = incomingChat.QuotedChatAvailable,
                        ChatId = incomingChat.Id,
                        IsSent = true,
                        ImageUrl = incomingChat.ImageUrl,
                        IsImageAvailable = incomingChat.IsImageAvailable,
                        SenderKey = incomingChat.SenderKey,
                    };
                    //cater for image
                    if (incomingChat.IsImageAvailable)
                    {
                        insert_loader.ImageSource = new UriImageSource
                        {
                            CachingEnabled = true,
                            CacheValidity = TimeSpan.FromDays(7),
                            Uri = new Uri(incomingChat.ImageUrl)
                        };
                    }
                    //cater for Quote
                    if (incomingChat.Quote != null)
                    {
                        insert_loader.Quote = new QuoteLoader
                        {
                            Body = incomingChat.Quote.Body,
                            ImageUrl = incomingChat.Quote.ImageUrl,
                            IsImageAvailable = incomingChat.Quote.IsImageAvailable,
                            OwnerName = incomingChat.Quote.OwnerName,
                            OwnerKey = incomingChat.Quote.OwnerKey,
                            IsMine = await Logic.CheckIfMine(incomingChat.Quote.OwnerKey)
                        };
                        if (incomingChat.Quote.IsImageAvailable)
                        {
                            insert_loader.Quote.ImageSource = new UriImageSource { CachingEnabled = true, CacheValidity = TimeSpan.FromDays(7), Uri = new Uri(incomingChat.Quote.ImageUrl) };
                        }
                    }

                    if (incomingChat.Room_ID == Room_ID)
                    {
                        if (incomingChat.SenderKey != await Logic.GetKey())
                        {
                            insert_loader.IsMine = false;

                            #region MyRegion
                            if (LastMessageVisible)
                            {
                                Messages.Add(insert_loader);
                            }
                            else
                            {
                                DelayedMessages.Enqueue(insert_loader);
                                PendingMessageCount++;
                            }
                            #endregion

                            //Messages.Add(insert_loader);         //test uncomment
                            Logic.VibrateNow();

                        }
                        else
                        {
                            //the message is mine. so update delivered.
                            insert_loader.IsMine = true;
                            //Messages.FirstOrDefault(d => d.ChatId == incomingChat.Id).IsSent = true;

                            //rather than updating just IsSent property, just update the entire object
                            //check for existence
                            if (Messages.Any(d => d.ChatId == incomingChat.Id))
                            {
                                ChatLoader temp_msg = Messages.FirstOrDefault(d => d.ChatId == incomingChat.Id);
                                int index = Messages.IndexOf(temp_msg);
                                Messages.RemoveAt(index);
                                Messages.Insert(index, insert_loader);
                            }
                            OnPropertyChanged("IsSent");
                            await Task.Delay(10);
                        }
                    }

                    LocalStore.Chat.SaveLoader(insert_loader);
                    OnPropertyChanged(nameof(Messages));
                    await Task.Delay(10);
                    MessagingCenter.Send<object>(this, Constants.scroll_chat);
                }

                catch (Exception ex)
                {
                    Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
                }


            });

            MessageAppearingCommand = new Command<ChatLoader>(OnMessageAppearing);
            MessageDisappearingCommand = new Command<ChatLoader>(OnMessageDisappearing);

            OnSendCommand = new Command(async () =>
            {
                if (!string.IsNullOrWhiteSpace(TextToSend.Trim()))
                {
                    try
                    {
                        Chat new_send = new Chat()
                        {
                            Body = TextToSend.Trim(),
                            Room_ID = this.Room_ID,
                            SenderKey = await Logic.GetKey(),
                            SenderName = await Logic.GetChatName(),
                            QuotedChatAvailable = IsQuotedChatAvailable

                        };
                        TextToSend = string.Empty;
                        OnPropertyChanged(nameof(TextToSend));

                        ChatLoader newMsg = new ChatLoader()
                        {
                            Body = new_send.Body,
                            SenderName = new_send.SenderName,
                            Room_ID = new_send.Room_ID,
                            IsMine = true,
                            QuotedChatAvailable = new_send.QuotedChatAvailable,
                            ChatId = new_send.Id,
                            IsSent = false,
                            IsAd = false,
                            SenderKey = await Logic.GetKey()
                        };
                        if (IsQuotedChatAvailable & QuotedChat != null)
                        {
                            Quote newQuote = new Quote()
                            {
                                Body = QuotedChat.Body,
                                OwnerName = QuotedChat.SenderName,
                                OwnerKey = QuotedChat.SenderKey,
                                IsImageAvailable = QuotedChat.IsImageAvailable,
                                ImageUrl = QuotedChat.ImageUrl
                            };

                            QuoteLoader newQuoteLoader = new QuoteLoader()
                            {
                                Body = QuotedChat.Body,
                                OwnerName = QuotedChat.SenderName,
                                OwnerKey = QuotedChat.SenderKey,
                                IsImageAvailable = QuotedChat.IsImageAvailable,
                                ImageUrl = QuotedChat.ImageUrl,
                                IsMine = await Logic.CheckIfMine(QuotedChat.SenderKey)
                            };
                            if (quotedChat.IsImageAvailable)
                            {
                                newQuoteLoader.ImageSource = new UriImageSource { CachingEnabled = true, CacheValidity = TimeSpan.FromDays(7), Uri = new Uri(QuotedChat.ImageUrl) };
                            }
                            newMsg.Quote = newQuoteLoader;
                            new_send.Quote = newQuote;
                        }
                        if (IsQuotedChatAvailable & QuotedChat != null)
                        {
                            newMsg.Quote.ImageSource = QuotedChat.ImageSource;
                        }

                        Messages.Add(newMsg);
                        OnPropertyChanged(nameof(Messages));
                        MessagingCenter.Send<object>(this, Constants.scroll_chat);
                        RemoveQuoteCommand.Execute(null);
                        string serialisedMessage = JsonConvert.SerializeObject(new_send);

                        await ConnectToHub();
                        await hubConnection.InvokeAsync("SendMessage", serialisedMessage);

                        await Task.Delay(10);

                        MessagingCenter.Send<object, ChatRoomLoader>(this, Constants.update_chatroom_chat_list, new ChatRoomLoader() { Id = Room_ID, ChatsCount = (Messages.Count + 1).ToString() });

                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
                    }

                }
            });

            OnQuoteCommand = new Command((arg) =>
            {
                this.IsQuotedChatAvailable = true;
                this.QuotedChat = Messages.First(d => d.ChatId == (string)arg);
            });

            RemoveQuoteCommand = new Command(() =>
            {
                this.IsQuotedChatAvailable = false;
                this.QuotedChat = null;
            });

            CloseConnectionCommand = new Command(async () =>
            {
                //  await client.CloseAsync( WebSocketCloseStatus.NormalClosure, "User Navigation", new CancellationToken());
            });

            IsNoInternet = false;
            Task.Run(async () =>
            {
                await LoadData();
            });
            #region MyRegion
            //LoadData().Wait();

            //System.Net.ServicePointManager.ServerCertificateValidationCallback =
            //   (sender, certificate, chain, errors) => true;
            //// this seems to just delay the inevitable by setting a very large
            //// max idle. Not a scalable workaround as this would affect all
            //// ServicePoint's created after this call
            //System.Net.ServicePointManager.MaxServicePointIdleTime = int.MaxValue;



            //client = new ClientWebSocket();
            //cts = new CancellationTokenSource();
            //ConnectToServerAsync();
            ////Device.StartTimer(TimeSpan.FromSeconds(3), () =>
            //{
            //    if (IsPageActive)
            //    {
            //        Task.Run(async () =>
            //    {
            //        IsBusy = true;
            //        await LoadData();
            //        IsBusy = false;
            //    });
            //    }

            //    return true;
            //});

            //Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            //{
            //    if (LastMessageVisible)
            //    {
            //        Messages.Insert(0, new ChatLoader() { Body = "New message test", SenderName = "Mario" });
            //    }
            //    else
            //    {
            //        DelayedMessages.Enqueue(new ChatLoader() { Body = "New message test", SenderName = "Mario" });
            //        PendingMessageCount++;
            //    }
            //    return true;
            //}); 
            #endregion
            LoadSubscription();
        }

        private async void Connectivity_ConnectivityChangedAsync(object sender, ConnectivityChangedEventArgs e)
        {
            await ConnectToHub();
        }

        private void LoadSubscription()
        {
            MessagingCenter.Subscribe<object>(this, Constants.re_open_connection, async (sender) =>
            {
                if (!IsHubConnected())
                {
                    if (string.IsNullOrEmpty(Room_ID))
                    {
                        Room_ID = await Logic.GetRoomID();
                    }
                    //await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "User Navigation", new CancellationToken());
                    await ConnectToHub();
                }
            });

            MessagingCenter.Subscribe<object, ImageSender>(this, Constants.image_sender, async (sender, arg) =>
            {
                try
                {
                    if (arg != null)
                    {
                        //quickly send the data to the Viewmodel
                        //start uploading,
                        //upload once sent. 
                        Chat new_send = new Chat()
                        {
                            Body = arg.body,
                            Room_ID = this.Room_ID,
                            SenderKey = await Logic.GetKey(),
                            SenderName = await Logic.GetChatName(),
                            QuotedChatAvailable = IsQuotedChatAvailable,
                            IsImageAvailable = true

                        };

                        ChatLoader newMsg = new ChatLoader()
                        {
                            Body = new_send.Body,
                            SenderName = new_send.SenderName,
                            Room_ID = new_send.Room_ID,
                            IsMine = true,
                            QuotedChatAvailable = new_send.QuotedChatAvailable,
                            ChatId = new_send.Id,
                            IsSent = false,
                            IsAd = false,
                            IsImageAvailable = true,
                            ImageSource = ImageSource.FromStream(() => Logic.GetStreamFromByteArray(arg.stream)),
                            SenderKey = await Logic.GetKey()
                        };
                        if (IsQuotedChatAvailable & QuotedChat != null)
                        {
                            Quote newQuote = new Quote()
                            {
                                Body = QuotedChat.Body,
                                OwnerName = QuotedChat.SenderName,
                                OwnerKey = QuotedChat.SenderKey,
                                IsImageAvailable = quotedChat.IsImageAvailable,
                                ImageUrl = quotedChat.ImageUrl
                            };
                            QuoteLoader newQuoteLoader = new QuoteLoader()
                            {
                                Body = QuotedChat.Body,
                                OwnerName = QuotedChat.SenderName,
                                OwnerKey = QuotedChat.SenderKey,
                                IsImageAvailable = QuotedChat.IsImageAvailable,
                                ImageUrl = QuotedChat.ImageUrl,
                                IsMine = await Logic.CheckIfMine(QuotedChat.SenderKey)
                            };
                            if (quotedChat.IsImageAvailable)
                            {
                                newQuoteLoader.ImageSource = new UriImageSource { CachingEnabled = true, CacheValidity = TimeSpan.FromDays(7), Uri = new Uri(QuotedChat.ImageUrl) };
                            }
                            newMsg.Quote = newQuoteLoader;
                            new_send.Quote = newQuote;
                        }
                        //if (IsQuotedChatAvailable & QuotedChat != null)
                        //{
                        //    newMsg.Quote.SenderNameShow = Logic.GetTrueSenderName(quotedChat.IsMine, newMsg.Quote.OwnerName);
                        //}

                        Messages.Add(newMsg);
                        OnPropertyChanged(nameof(Messages));
                        MessagingCenter.Send<object>(this, Constants.scroll_chat);
                        RemoveQuoteCommand.Execute(null);
                        //get the string for the image from Cloudinary

                        //try to separate and run in backgroud.
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        Task.Run(async () =>
                        {
                            string ImageUrl = Cloud.SaveByteArray(arg.stream);// await BaseClient.PostImageStream(arg.stream);
                                                                              //DependencyService.Get<IMessage>().ShortAlert($"Url: {ImageUrl}");

                            if (!string.IsNullOrEmpty(ImageUrl))
                            {
                                new_send.ImageUrl = ImageUrl;
                                string serialisedMessage = JsonConvert.SerializeObject(new_send);

                                await ConnectToHub();
                                await hubConnection.InvokeAsync("SendMessage", serialisedMessage);

                                //update the local msg with the url
                                Messages.FirstOrDefault(d => d.ChatId == new_send.Id).ImageSource = new UriImageSource { CachingEnabled = true, CacheValidity = TimeSpan.FromDays(7), Uri = new Uri(ImageUrl) };// ImageSource.FromUri(new Uri(ImageUrl));
                                Messages.FirstOrDefault(d => d.ChatId == new_send.Id).ImageUrl = ImageUrl;
                                OnPropertyChanged(nameof(Messages));

                                await Task.Delay(10);

                                MessagingCenter.Send<object, ChatRoomLoader>(this, Constants.update_chatroom_chat_list, new ChatRoomLoader() { Id = Room_ID, ChatsCount = (Messages.Count + 1).ToString() });
                            }
                        });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed


                    }

                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex, Logic.GetErrorProperties(ex));
                }
            });
        }

        private async Task LoadData()
        {

            //Connect to the hub
            IsBusy = true;
            await ConnectToHub();
            ObservableCollection<ChatLoader> result = new ObservableCollection<ChatLoader>();
            try
            {
                if (!Logic.IsInternet())
                {
                    result = LocalStore.Chat.FetchByRoomID(this.Room_ID);
                }
                else
                {
                    result = await Store.ChatClass.ChatsByRoom(this.Room_ID);
                    if (result == null || result.Count == 0)
                    {
                        result = LocalStore.Chat.FetchByRoomID(this.Room_ID);
                    }
                }
                //process result 
                string UserKey = await Logic.GetKey();
                foreach (ChatLoader data in result)
                {
                    //preload data
                    if (data.IsImageAvailable & !string.IsNullOrEmpty(data.ImageUrl))
                    {
                        data.ImageSource = new UriImageSource { CachingEnabled = true, CacheValidity = TimeSpan.FromDays(7), Uri = new Uri(data.ImageUrl) };// ImageSource.FromUri(new Uri(data.ImageUrl));
                    }
                    if (data.Quote != null)
                    {
                        if (data.Quote.IsImageAvailable & !string.IsNullOrEmpty(data.Quote.ImageUrl))
                        {
                            data.Quote.ImageSource = new UriImageSource { CachingEnabled = true, CacheValidity = TimeSpan.FromDays(7), Uri = new Uri(data.ImageUrl) };// ImageSource.FromUri(new Uri(data.Quote.ImageUrl));
                        }
                    }
                    data.IsSent = true;
                    Messages.Add(data);
                    OnPropertyChanged(nameof(Messages));
                }
                MessagingCenter.Send<object>(this, Constants.scroll_chat);
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

        private void OnMessageAppearing(ChatLoader message)
        {
            int idx = Messages.IndexOf(message);
            int lastIndex = Messages.IndexOf(Messages.LastOrDefault());

            //check if the current guy is in the last 10

            if (lastIndex - idx <= 10)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    while (DelayedMessages.Count > 0)
                    {
                        //Messages.Insert(0, DelayedMessages.Dequeue());
                        Messages.Add(DelayedMessages.Dequeue());
                    }

                    ShowScrollTap = false;
                    LastMessageVisible = true;
                    PendingMessageCount = 0;
                });
            }
            else
            {
                ShowScrollTap = true;
                LastMessageVisible = false;
            }
            ShowHeaderData(message.Date);
        }

        private CancellationTokenSource _cts;

        private async void ShowHeaderData(DateTime date)
        {
            try
            {
                _cts?.Cancel();     // cancel previous stop
            }
            catch (ObjectDisposedException)     // in case previous search completed
            {
            }
            HeaderDateText = Logic.Ago(date);
            IsHeaderDateVisible = true;

            using (_cts = new CancellationTokenSource())
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(6), _cts.Token);  // buffer
                    IsHeaderDateVisible = false;
                }
                catch (TaskCanceledException)       // if the operation is cancelled, do nothing
                {

                }
            }
        }

        private void OnMessageDisappearing(ChatLoader message)
        {
            int idx = Messages.IndexOf(message);
            int lastIndex = Messages.IndexOf(Messages.LastOrDefault());

            if (lastIndex - idx >= 10)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ShowScrollTap = true;
                    LastMessageVisible = false;
                });

            }
        }

    }
}
