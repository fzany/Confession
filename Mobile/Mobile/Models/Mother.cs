using Microsoft.AppCenter.Crashes;
using Microsoft.AspNetCore.SignalR.Client;
using Mobile.Helpers;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Mobile.Models
{
    public class Mother
    {
        public HubConnection hubConnection = new HubConnectionBuilder()
                .WithUrl("https://confessbackend.azurewebsites.net/chatHub")
                .Build();
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
        public Mother()
        {
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChangedAsync;
            ConnectToHub().Wait();
        }
        private void Connectivity_ConnectivityChangedAsync(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess == NetworkAccess.Internet)
            {
                ConnectToHub().Wait();
            }
        }
    }
}
