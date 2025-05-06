using System.Net.Sockets;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text;
using System.Net;

internal class Program
{

    static List<StreamWriter> clients = new List<StreamWriter>();
    public static class Settings
    {
        public const int MaxClient = 1;
    }
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
                if (clients.Count < Settings.MaxClient)
                {
                    
                    TcpClient client = listener.AcceptTcpClient();
                    Task.Run(() => ServerClient(client)); 
                }
               
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
            if (clients.Count <= Settings.MaxClient)
            {
                clients.Add(sw);
            }
        
            

            while ((message = sr.ReadLine()!) != null)
            {
                string response = GetMessage(message);
                foreach (var sw1 in clients)
                {
                    try { sw1.WriteLine(response);
                        sw1.Flush();
                    } catch { }
                }
            }


            CloseConnection(client, sr, sw);

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
            return $"{Name}{":",-5}{messages1,-205}{DateTime.Now.ToShortTimeString()}";
        }
        return "";

    }


    private static void CloseConnection(TcpClient client1, StreamReader sr1, StreamWriter sw1)
    {
        try
        {
            if (sw1 != null)
            {
                sw1.Close();
            }
            if (sr1 != null)
            {
                sr1.Close();
            }
            if (client1 != null)
            {
                client1.Close();
            }


            if(sw1 != null && clients.Contains(sw1))
        {
                clients.Remove(sw1);
                Console.WriteLine("Client removed from the list.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while disconnecting: {ex.Message}");
        }
    }
}

