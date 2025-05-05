using System.Net.Sockets;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text;
using System.Net;

internal class Program
{
    private static void Main(string[] args)
    {
        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4040);
        TcpListener listener = new TcpListener(ipPoint);
        try
        {
            listener.Start();

            Console.WriteLine("Server started! Waiting for connection .... ");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Task.Run(() => ServerClient(client));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            listener.Stop();
        }
    }
    private static void ServerClient(TcpClient client)
    {
        try
        {
            NetworkStream ns = client.GetStream();
            StreamReader sr = new StreamReader(ns);
            string message = sr.ReadLine()!;

            Console.WriteLine($"{client.Client.RemoteEndPoint} - {message} at {DateTime.Now.ToLongTimeString()}");


            StreamWriter sw = new StreamWriter(ns);
          
            sw.WriteLine(GetMessage(message));

            sw.Close();
            sr.Close();
            ns.Close();

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally { client.Close(); }
    }
    private static string GetMessage(string message)
    {
        string Name = "";
        string messages1 = "";
        string[] parts = message.Split("|*|");
        if (parts.Length != 0)
        {
            Name = parts[0];
            messages1 = parts[1];
            return $"{Name}{":",-20}{messages1,-100}{DateTime.Now.ToShortTimeString}";   
        }
        return "";

    }

}