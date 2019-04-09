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
        public bool ShowScrollTap { get; set; } = false; //Show the jump icon 
        public bool LastMessageVisible { get; set; } = true;
        public int PendingMessageCount { get; set; } = 0;
        public bool PendingMessageCountVisible => PendingMessageCount > 0;
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
                if (!IsHubConnected())
                {
                    await hubConnection.StartAsync();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
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
                Crashes.TrackError(ex);
            }
        }
        public bool IsHubConnected()
        {
            try
            {
                HubConnectionState state = hubConnection.State;
                return state == HubConnectionState.Connected;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return false;
            }
        }
        public ChatPageViewModel()
        {
            hubConnection = new HubConnectionBuilder()
        .WithUrl("https://confessbackend.azurewebsites.net/chatHub")
        .Build();

            hubConnection.On<string>("ReceiveMessage", async (message) =>
            {
                Chat incomingChat = JsonConvert.DeserializeObject<Chat>(message);
                ChatLoader insert_loader = new ChatLoader
                {
                    Room_ID = incomingChat.Room_ID,
                    Body = incomingChat.Body,
                    Date = incomingChat.Date,
                    IsAd = false,
                    SenderName = incomingChat.SenderName,
                    Quote = incomingChat.Quote,
                    QuotedChatAvailable = incomingChat.QuotedChatAvailable,
                    ChatId = incomingChat.Id,
                    IsSent = true,
                };

                try
                {
                    if (incomingChat.Room_ID == Room_ID)
                    {
                        if (incomingChat.SenderKey != await Logic.GetKey())
                        {
                            insert_loader.IsMine = false;

                            #region MyRegion
                            //if (LastMessageVisible)
                            //{
                            //    Messages.Add(insert_loader);
                            //}
                            //else
                            //{
                            //    DelayedMessages.Enqueue(insert_loader);
                            //    PendingMessageCount++;
                            //} 
                            #endregion

                            Messages.Add(insert_loader);
                            Logic.VibrateNow();

                        }
                        else
                        {
                            //the message is mine. so update delivered.
                            insert_loader.IsMine = true;
                            Messages.FirstOrDefault(d => d.ChatId == incomingChat.Id).IsSent = true;
                            OnPropertyChanged("IsSent");
                        }
                    }

                    LocalStore.Chat.SaveLoader(insert_loader);

                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
                finally
                {
                    OnPropertyChanged(nameof(Messages));
                }

            });

            MessageAppearingCommand = new Command<ChatLoader>(OnMessageAppearing);
            MessageDisappearingCommand = new Command<ChatLoader>(OnMessageDisappearing);

            OnSendCommand = new Command(async () =>
            {
                if (!string.IsNullOrEmpty(TextToSend))
                {
                    Chat new_send = new Chat()
                    {
                        Body = TextToSend,
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
                            OwnerKey = QuotedChat.SenderKey
                        };
                        newMsg.Quote = newQuote;
                        new_send.Quote = newQuote;
                    }
                    if (IsQuotedChatAvailable & QuotedChat != null)
                    {
                        newMsg.Quote.SenderNameShow = Logic.GetTrueSenderName(quotedChat.IsMine, newMsg.Quote.OwnerName);
                    }

                    Messages.Add(newMsg);
                    OnPropertyChanged(nameof(Messages));
                    RemoveQuoteCommand.Execute(null);
                    string serialisedMessage = JsonConvert.SerializeObject(new_send);

                    try
                    {
                        await hubConnection.InvokeAsync("SendMessage", serialisedMessage);
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }

                    //websocket codes
                    //byte[] byteMessage = Encoding.UTF8.GetBytes(serialisedMessage);
                    //ArraySegment<byte> segmnet = new ArraySegment<byte>(byteMessage);
                    //await client.SendAsync(segmnet, WebSocketMessageType.Text, true, cts.Token);

                    await Task.Delay(10);

                    //await Store.ChatClass.Add(new Chat() { Body = TextToSend, Room_ID = Room_ID, SenderKey = await Logic.GetKey(), SenderName = await Logic.GetChatName() });
                    // await Task.Delay(60);
                    MessagingCenter.Send<object, ChatRoomLoader>(this, Constants.update_chatroom_chat_list, new ChatRoomLoader() { Id = Room_ID, ChatsCount = (Messages.Count + 1).ToString() });


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
                        ImageSource = ImageSource.FromStream(() => arg.stream),
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
                            ImageUrl = quotedChat.ImageUrl,
                        };
                        newMsg.Quote = newQuote;
                        new_send.Quote = newQuote;
                    }
                    newMsg.Quote.SenderNameShow = Logic.GetTrueSenderName(quotedChat.IsMine, newMsg.Quote.OwnerName);
                    Messages.Add(newMsg);
                    OnPropertyChanged(nameof(Messages));
                    RemoveQuoteCommand.Execute(null);
                    //get the string for the image from Cloudinary
                    string ImageUrl = await BaseClient.PostImageStream(arg.stream);
                    new_send.ImageUrl = ImageUrl;

                    //update the local msg with the url
                    string serialisedMessage = JsonConvert.SerializeObject(new_send);

                    try
                    {
                        await hubConnection.InvokeAsync("SendMessage", serialisedMessage);
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }


                    await Task.Delay(10);

                    MessagingCenter.Send<object, ChatRoomLoader>(this, Constants.update_chatroom_chat_list, new ChatRoomLoader() { Id = Room_ID, ChatsCount = (Messages.Count + 1).ToString() });



                }
            });
        }

        #region WebSocket
        // private readonly ClientWebSocket client;
        // private readonly CancellationTokenSource cts;

        //private async void ConnectToServerAsync()
        //{
        //    await client.ConnectAsync(new Uri("wss://confessbackend.azurewebsites.net/"), cts.Token);

        //    UpdateClientState();

        //    await Task.Factory.StartNew(async () =>
        //    {
        //        while (true)
        //        {
        //            WebSocketReceiveResult result;
        //            ArraySegment<byte> message = new ArraySegment<byte>(new byte[4096]);
        //            do
        //            {
        //                result = await client.ReceiveAsync(message, cts.Token);
        //                if (result.MessageType == WebSocketMessageType.Close || result.CloseStatus.HasValue)
        //                {
        //                    await Task.Delay(100);
        //                    await client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);

        //                    // ReconnectConnectToServerAsync();
        //                }
        //                else
        //                {
        //                    byte[] messageBytes = message.Skip(message.Offset).Take(result.Count).ToArray();
        //                    string serialisedMessage = Encoding.UTF8.GetString(messageBytes);

        //                    try
        //                    {
        //                        Chat incomingChat = JsonConvert.DeserializeObject<Chat>(serialisedMessage);
        //                        if (incomingChat.SenderKey != await Logic.GetKey() & incomingChat.Room_ID == Room_ID)
        //                        {
        //                            ChatLoader insert_loader = new ChatLoader()
        //                            {
        //                                Room_ID = incomingChat.Room_ID,
        //                                Body = incomingChat.Body,
        //                                Date = incomingChat.Date,
        //                                IsAd = false,
        //                                SenderName = incomingChat.SenderName,
        //                                IsMine = false,
        //                                Quote = incomingChat.Quote,
        //                                QuotedChatAvailable = incomingChat.QuotedChatAvailable,
        //                                ChatId = incomingChat.Id
        //                            };
        //                            //if (LastMessageVisible)
        //                            //{
        //                            //    Messages.Add(insert_loader);
        //                            //}
        //                            //else
        //                            //{
        //                            //    DelayedMessages.Enqueue(insert_loader);
        //                            //    PendingMessageCount++;
        //                            //}

        //                            Messages.Add(insert_loader);
        //                            Logic.VibrateNow();
        //                        }
        //                        else
        //                        {
        //                            //the message is mine. so update delivered.
        //                            Messages.FirstOrDefault(d => d.ChatId == incomingChat.Id).IsSent = true;

        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Crashes.TrackError(ex);
        //                    }
        //                    finally
        //                    {
        //                        OnPropertyChanged(nameof(Messages));
        //                    }
        //                }

        //            } while (!result.EndOfMessage);
        //        }
        //    }, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

        //    void UpdateClientState()
        //    {
        //        OnPropertyChanged(nameof(IsConnected));
        //        Console.WriteLine($"Websocket state {client.State}");
        //    }
        //}

        #endregion
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
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                result = LocalStore.Chat.FetchByRoomID(this.Room_ID);
            }
            finally
            {
                Device.BeginInvokeOnMainThread(async () =>
                         {
                             string UserKey = await Logic.GetKey();
                             foreach (ChatLoader data in result)
                             {
                                 if (data.QuotedChatAvailable & data.Quote != null)
                                 {
                                     data.Quote.SenderNameShow = await Logic.GetTrueSenderName(data.Quote.OwnerKey, data.Quote.OwnerName);
                                 }

                                 Messages.Add(data);
                                 OnPropertyChanged(nameof(Messages));
                             }
                         });
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
            if (idx <= 6)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    while (DelayedMessages.Count > 0)
                    {
                        Messages.Insert(0, DelayedMessages.Dequeue());
                    }
                    ShowScrollTap = false;
                    LastMessageVisible = true;
                    PendingMessageCount = 0;
                });
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
            if (idx >= 6)
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
