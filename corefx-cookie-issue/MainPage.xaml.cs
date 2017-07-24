using System;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using Windows.UI.Xaml.Controls;

namespace corefx_cookie_issue
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Run();
        }

        private async void Run()
        {
            // Save cookies for use in ClientWebSocket
            CookieContainer sharedCookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = sharedCookies;

            var client = new HttpClient(handler);
            // response contains AWSELB cookie (secure=false) and connect.sid cookie (secure=true)
            var response = await client.GetAsync("https://test.circuitsandbox.net");

            // Only cookies without 'secure' flag are sent on WebSocket upgrade request
            // I.e. AWSELB cookie is sent, but connect.sid is not sent
            // Expected result: connect.sid is also send in HTTPS WebSocket upgrade request
            var socket = new ClientWebSocket();
            socket.Options.Cookies = sharedCookies;
            await socket.ConnectAsync(new Uri("wss://test.circuitsandbox.net/api"), CancellationToken.None);
        }
    }
}
