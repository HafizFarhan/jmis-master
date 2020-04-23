using Serilog;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MTC.JMICS.TrackTransmitter
{
    class Program
    {
        public static string FileURL { get; set; }
        public static IPAddress IPAddress { get; set; }
        public static int Port { get; set; }
        public static int Seconds { get; set; }

        static void Main()
        {
            SetupLogger();
            GetFileURL();
        }

        private static void GetFileURL()
        {
            try
            {
                Console.Write("File Absolute Path: ");
                FileURL = Console.ReadLine();
                if (!File.Exists(FileURL))
                {
                    Log.Warning("File path does not exist or invalid");
                    GetFileURL();
                }
                else
                {
                    GetIPAddress();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "{@Message}", ex.Message);
                GetFileURL();
            }
        }

        private static void GetIPAddress()
        {
            try
            {
                Console.Write("Receiver IP Address: ");
                string ipAddress = Console.ReadLine();
                if (!ValidateIPv4(ipAddress))
                {
                    Log.Warning("IP Address is not valid");
                    GetIPAddress();
                }
                else
                {
                    IPAddress = IPAddress.Parse(ipAddress);
                    GetPort();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "{@Message}", ex.Message);
                GetIPAddress();
            }
        }

        private static void GetPort()
        {
            try
            {
                Console.Write("Receiver Port: ");
                string port = Console.ReadLine();
                if (!int.TryParse(port, out int _port))
                {
                    Log.Warning("Port should be numeric");
                    GetPort();
                }
                else
                {
                    Port = _port;
                    SendData();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "{@Message}", ex.Message);
                GetPort();
            }
        }

        //private static void GetDelaySeconds()
        //{
        //    try
        //    {
        //        Console.Write("Delay (seconds): ");
        //        string delayInSeconds = Console.ReadLine();
        //        if (!int.TryParse(delayInSeconds, out int _seconds))
        //        {
        //            Log.Warning("seconds should be numeric");
        //            GetDelaySeconds();
        //        }
        //        else
        //        {
        //            Seconds = _seconds;
        //            SendData();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex, "{@Message}", ex.Message);
        //        GetPort();
        //    }
        //}

        private static void SendData()
        {
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint endPoint = new IPEndPoint(IPAddress, Port);
                string[] aisData = File.ReadAllLines(FileURL);//.Where(line => line.StartsWith("!AIVDM")).ToArray()
                foreach (string aisPacket in aisData)
                {
                    socket.SendTo(Encoding.ASCII.GetBytes(aisPacket), endPoint);
                    Log.Information(aisPacket);
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "{@Message}", ex.Message);
            }
            finally
            {
                Console.Write("Send Again? Press 'Y'");
                if (Console.ReadLine().Trim().ToUpper() == "Y")
                {
                    SendData();
                }
            }
        }

        private static void SetupLogger()
        {
            try
            {
                Log.Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.Console().CreateLogger();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Logger was not setup. Additional Details: " + ex.Message + Environment.NewLine);
            }
        }

        private static bool ValidateIPv4(string ipString)
        {
            if (String.IsNullOrWhiteSpace(ipString))
            {
                return false;
            }

            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }

        private static bool ValidatePort(int Port)
        {
            return System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners().Any(p => p.Port == Port);
        }
    }
}
