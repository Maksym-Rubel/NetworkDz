using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection.PortableExecutable;

namespace Text
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        const string serverAddress = "127.0.0.1";
        const string port = "4040";
        string nickname = "Anonym";
        bool isJoin = false;
     
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void SendBtn(object sender, RoutedEventArgs e)
        {
            if(isJoin)
            {
                if(msgTextBox.Text != "" || !string.IsNullOrWhiteSpace(msgTextBox.Text))
                {
                    IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4040);
                    string message = nickname + "|*|" + msgTextBox.Text;
                    msgTextBox.Text = "";
                    TcpClient client = null;

                    try
                    {
                        client = new TcpClient();
                        await client.ConnectAsync(ipPoint);
                        NetworkStream stream = client.GetStream();

                        StreamWriter sw = new StreamWriter(stream);
                        await sw.WriteLineAsync(message);
                        await sw.FlushAsync();

                        StreamReader sr = new StreamReader(stream);
                        string result = await sr.ReadLineAsync();
                        sr.Close();
                        sw.Close();
                        stream.Close();

                        list.Items.Add(result);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        client?.Close();
                    }
                }

               
            }
            
        }


        // В мене лістенера не виходить зробить
        private async void JoinBtn(object sender, RoutedEventArgs e)
        {

            SecondWindow secondWindow = new SecondWindow();
            
            bool? result = secondWindow.ShowDialog();
                if (result == true)
                {
                    isJoin = true;
                    nickname = secondWindow.UserName;
                }

            //    if (result == true)
            //    {
            //        client = new TcpClient();
            //        await client.ConnectAsync(serverAddress, int.Parse(port));
            //        var stream = client.GetStream();

            //        sr = new StreamReader(stream);
            //        sw = new StreamWriter(stream) { AutoFlush = true };


            //        Task.Run(() => ListenForMessages());


            //    }


        }

        //private async Task ListenForMessages()
        //{
        //    try
        //    {
        //        while (isListening)
        //        {
        //            string? response = await sr.ReadLineAsync();
        //            if (response != null)
        //            {

        //                list.Items.Add($"Відповідь від сервера: {response}");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        list.Items.Add($"Помилка прослуховування: {ex.Message}");

        //    }
        //}
        private void msgTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                SendBtn(sender, e);
            }
            
        }
    }
}