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
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void SendBtn(object sender, RoutedEventArgs e)
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(serverAddress), int.Parse(port));
            string message = nickname + "|*|" + msgTextBox.Text;

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void msgTextBox_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}