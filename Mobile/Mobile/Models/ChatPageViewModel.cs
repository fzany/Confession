using Microsoft.AppCenter.Crashes;
using Mobile.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
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

        public ChatPageViewModel()
        {
            //Messages.Add(new ChatLoader() { Body = "Hi", Room_ID = Room_ID, IsMine = false, SenderName = "Seun"  });
            //OnPropertyChanged(nameof(Messages));
            //Messages.Add(new ChatLoader() { Body = "How are you?", Room_ID = Room_ID, IsMine =  false, SenderName = "Ranti" });
            //OnPropertyChanged(nameof(Messages));
            MessageAppearingCommand = new Command<ChatLoader>(OnMessageAppearing);
            MessageDisappearingCommand = new Command<ChatLoader>(OnMessageDisappearing);

            OnSendCommand = new Command(async () =>
            {
                if (!string.IsNullOrEmpty(TextToSend))
                {
                    // Messages.Add(new Message() { Text = TextToSend, User = App.User });
                    ChatLoader newMsg = new ChatLoader()
                    {
                        Body = TextToSend,
                        SenderName = await Logic.GetChatName(),
                        Room_ID = Room_ID,
                        IsMine = true,
                        QuotedChatAvailable = IsQuotedChatAvailable,
                    };
                    if (IsQuotedChatAvailable & QuotedChat != null)
                    {
                        newMsg.Quote = new Quote()
                        {
                            Body = QuotedChat.Body,
                            SenderName = quotedChat.SenderName,
                            SenderKey = await Logic.GetKey()
                        };
                    }
                    Messages.Add(newMsg);

                    Chat new_send = new Chat()
                    {
                        Body = TextToSend,
                        Room_ID = this.Room_ID,
                        SenderKey = await Logic.GetKey(),
                        SenderName = await Logic.GetChatName(),
                        QuotedChatAvailable = IsQuotedChatAvailable

                    };
                    if (IsQuotedChatAvailable & QuotedChat != null)
                    {
                        new_send.Quote = new Quote()
                        {
                            Body = QuotedChat.Body,
                            SenderName = quotedChat.SenderName,
                            SenderKey = await Logic.GetKey()
                        };
                    }
                    string serialisedMessage = JsonConvert.SerializeObject(new_send);

                    byte[] byteMessage = Encoding.UTF8.GetBytes(serialisedMessage);
                    ArraySegment<byte> segmnet = new ArraySegment<byte>(byteMessage);

                    await client.SendAsync(segmnet, WebSocketMessageType.Text, true, cts.Token);
                    await Task.Delay(10);

                    //await Store.ChatClass.Add(new Chat() { Body = TextToSend, Room_ID = Room_ID, SenderKey = await Logic.GetKey(), SenderName = await Logic.GetChatName() });
                    // await Task.Delay(60);
                    MessagingCenter.Send<object, ChatRoomLoader>(this, Constants.update_chatroom_chat_list, new ChatRoomLoader() { Id = Room_ID, ChatsCount = (Messages.Count + 1).ToString() });
                    TextToSend = string.Empty;
                    OnPropertyChanged(nameof(Messages));
                    OnPropertyChanged(nameof(TextToSend));
                    RemoveQuoteCommand.Execute(null);
                }

            });

            OnQuoteCommand = new Command((arg) =>
            {
                this.IsQuotedChatAvailable = true;
                this.QuotedChat = Messages.FirstOrDefault(d => d.ChatId == (string)arg);

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
            //LoadData().Wait();

            System.Net.ServicePointManager.ServerCertificateValidationCallback =
               (sender, certificate, chain, errors) => true;
            // this seems to just delay the inevitable by setting a very large
            // max idle. Not a scalable workaround as this would affect all
            // ServicePoint's created after this call
            System.Net.ServicePointManager.MaxServicePointIdleTime = int.MaxValue;
            



            client = new ClientWebSocket();
            cts = new CancellationTokenSource();
            ConnectToServerAsync();
            //Device.StartTimer(TimeSpan.FromSeconds(3), () =>
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
            LoadSubscription();
        }

        private void LoadSubscription()
        {
            MessagingCenter.Subscribe<object>(this, Constants.re_open_connection, async (sender) =>
            {
                if (!IsConnected)
                {
                    if (string.IsNullOrEmpty(Room_ID))
                    {
                        Room_ID = await Logic.GetRoomID();
                    }
                    await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "User Navigation", new CancellationToken());

                    ConnectToServerAsync();
                }
            });
        }

        private bool CanSendMessage(string message)
        {
            return IsConnected && !string.IsNullOrEmpty(message);
        }

        private readonly ClientWebSocket client;
        private readonly CancellationTokenSource cts;
        public bool IsConnected => client.State == WebSocketState.Open;

        private async void ReconnectConnectToServerAsync()
        {
            try
            {
                await client.ConnectAsync(new Uri("wss://confessbackend.azurewebsites.net/"), cts.Token);
                OnPropertyChanged(nameof(IsConnected));
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            //finally
            //{
            //    if (client != null)
            //    {
            //        client.Dispose();
            //    }
            //}
        }
        private async void ConnectToServerAsync()
        {
            await client.ConnectAsync(new Uri("wss://confessbackend.azurewebsites.net/"), cts.Token);

            UpdateClientState();

            await Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    WebSocketReceiveResult result;
                    ArraySegment<byte> message = new ArraySegment<byte>(new byte[4096]);
                    do
                    {
                        result = await client.ReceiveAsync(message, cts.Token);
                        if (result.MessageType == WebSocketMessageType.Close || result.CloseStatus.HasValue)
                        {
                            await Task.Delay(100);
                            // ReconnectConnectToServerAsync();
                        }
                        else
                        {
                            byte[] messageBytes = message.Skip(message.Offset).Take(result.Count).ToArray();
                            string serialisedMessage = Encoding.UTF8.GetString(messageBytes);

                            try
                            {
                                Chat incomingChat = JsonConvert.DeserializeObject<Chat>(serialisedMessage);
                                if (incomingChat.SenderKey != await Logic.GetKey() & incomingChat.Room_ID == Room_ID)
                                {
                                    ChatLoader insert_loader = new ChatLoader()
                                    {
                                        Room_ID = incomingChat.Room_ID,
                                        Body = incomingChat.Body,
                                        Date = incomingChat.Date,
                                        IsAd = false,
                                        SenderName = incomingChat.SenderName,
                                        IsMine = false,
                                        Quote = incomingChat.Quote,
                                        QuotedChatAvailable = incomingChat.QuotedChatAvailable,
                                        ChatId = incomingChat.Id
                                    };
                                    //if (LastMessageVisible)
                                    //{
                                    //    Messages.Add(insert_loader);
                                    //}
                                    //else
                                    //{
                                    //    DelayedMessages.Enqueue(insert_loader);
                                    //    PendingMessageCount++;
                                    //}

                                    Messages.Add(insert_loader);
                                    Logic.VibrateNow();
                                }
                            }
                            catch (Exception ex)
                            {
                                Crashes.TrackError(ex);
                            }
                        }

                    } while (!result.EndOfMessage);
                }
            }, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            void UpdateClientState()
            {
                OnPropertyChanged(nameof(IsConnected));
                Console.WriteLine($"Websocket state {client.State}");
            }
        }

        private async Task LoadData()
        {
            ObservableCollection<ChatLoader> result = await Store.ChatClass.ChatsByRoom(this.Room_ID);
            Device.BeginInvokeOnMainThread(() =>
            {
                foreach (ChatLoader data in result)
                {
                    Messages.Add(data);
                }
            });

            OnPropertyChanged(nameof(Messages));
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
