using System;
using System.Configuration;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Text
{
    public partial class MainWindow : Window
    {
        const string serverAddress = "127.0.0.1";
        const int port = 4040;

        TcpClient client;
        StreamReader sr;
        StreamWriter sw;

        string nickname = "Anon";
        bool isJoined = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void JoinBtn(object sender, RoutedEventArgs e)
        {
            SecondWindow swWindow = new SecondWindow();
            bool? result = swWindow.ShowDialog();

            if (result == true)
            {
                nickname = swWindow.UserName;

                try
                {
                    
                    client = new TcpClient();
                    await client.ConnectAsync(serverAddress, port);

                    NetworkStream stream = client.GetStream();
                    sr = new StreamReader(stream);
                    sw = new StreamWriter(stream) { AutoFlush = true };

                    await sw.WriteLineAsync(nickname);
                    isJoined = true;

                    Task.Run(ListenForMessages);
                    

                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private async Task ListenForMessages()
        {
            try
            {
                string? line;
                while ((line = await sr.ReadLineAsync()) != null)
                {
                    Dispatcher.Invoke(() => list.Items.Add(line));
                }
            }
            catch (Exception ex)
            {
                
            }
        }
        private async void SendBtn(object sender, RoutedEventArgs e)
        {
            if (!isJoined || sw == null) return;

            string msg = msgTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(msg))
            {
                await sw.WriteLineAsync($"{nickname}|*|{msg}");
                msgTextBox.Text = "";
            }
        }

        private void msgTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) SendBtn(sender, e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            sr?.Close();
            sw?.Close();
            client?.Close();
        }
         
        private void DisconnectBtn(object sender, RoutedEventArgs e)
        {
            try
            {
                if (client != null && client.Connected)
                {
                    
                    sw.Close();  
                    sr.Close();  
                    client.Close(); 

                    isJoined = false;  

                    MessageBox.Show("You have been disconnected from the server.");
                }
                else
                {
                    MessageBox.Show("You are not connected to the server.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while disconnecting: {ex.Message}");
            }

        }
    }
}