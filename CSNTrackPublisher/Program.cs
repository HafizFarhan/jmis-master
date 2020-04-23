using Microsoft.Extensions.Configuration;
using MTC.JMIS.CSNTrackPublisher.Models;
using RestSharp;
using Serilog;
using SharpAIS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace CSNTrackPublisher
{
    class Program
    {
        private static RestClient RestClient { get; set; }
        private static RestRequest TrackRequest { get; set; }
        private static RestRequest SystemTrackRequest { get; set; }

        private static string APIBaseURL = "";
        private static IPAddress MulticastIpAddress;
        private static int MulticastPort = 0;
        private static string CSNEncKey = "S0YenXi6j_8wIU3ec9zwEerSyc8Y9CE_-LXXiFG6D1Q=";
        static void Main(string[] args)
        {
            SetupLogger();
            IntializeObjects();
            ReadCSNData();
        }

        private static void SetupLogger()
        {
            try
            {
                Log.Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.Console().WriteTo.File("CSNLogs.txt").CreateLogger();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Logger was not setup. Additional Details= " + ex.Message + Environment.NewLine);
            }
        }

        private static void IntializeObjects()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                IConfigurationRoot configuration = builder.Build();
                if (configuration.GetSection("AppKeys") != null)
                {
                    if (configuration.GetSection("AppKeys").GetSection("Port") != null)
                    {
                        if (!int.TryParse(configuration.GetSection("AppKeys").GetSection("Port").Value, out MulticastPort))
                            throw new Exception("CSN port is not valid");
                    }
                    else
                        throw new Exception("Port key not found in AppKeys section");

                    if (configuration.GetSection("AppKeys").GetSection("MulticastIp") != null)
                    {
                        if (!IPAddress.TryParse(configuration.GetSection("AppKeys").GetSection("MulticastIp").Value, out MulticastIpAddress))
                            throw new Exception("multicast IP address is not valid");
                    }
                    else
                        throw new Exception("MulticastIp key not found in AppKeys section");

                    if (string.IsNullOrEmpty(configuration.GetSection("AppKeys").GetSection("BaseURL").Value))
                        throw new Exception("BaseURL key not found in AppKeys section");

                    APIBaseURL = configuration.GetSection("AppKeys").GetSection("BaseURL").Value;
                }
                else
                    throw new Exception("AppKeys section not found in appsettings");

                SetupRestClient();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "{@Message}" + Environment.NewLine + "{StackTrace}.", ex.Message, ex.StackTrace);
            }
        }
        private static void SetupRestClient()
        {
            try
            {
                RestClient = new RestClient(APIBaseURL);
                TrackRequest = new RestRequest("api/AISTracks/", Method.POST);
                TrackRequest.RequestFormat = DataFormat.Json;
                SystemTrackRequest = new RestRequest("api/SystemTracks/", Method.POST);
                SystemTrackRequest.RequestFormat = DataFormat.Json;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "{@Message}" + Environment.NewLine + "{StackTrace}.", ex.Message, ex.StackTrace);
            }
        }
        private static void ReadCSNData()
        {
            try
            {
                using (UdpClient csnUDPClient = new UdpClient(MulticastPort))
                {

                    csnUDPClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    csnUDPClient.JoinMulticastGroup(MulticastIpAddress);
                    IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    Log.Information("Waiting to receive data..");
                    while (true)
                    {
                        Byte[] receiveBytes = csnUDPClient.Receive(ref RemoteIpEndPoint);
                        string encMessage = Encoding.ASCII.GetString(receiveBytes, 0, receiveBytes.Length);
                        byte[] key = Base64UrlDecode(CSNEncKey);
                        byte[] result = DecryptFernet(key, encMessage, out _);
                        MemoryStream memoryStream = new MemoryStream(result);
                        PublishCSNPacket(memoryStream);
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex, "{@Message}" + Environment.NewLine + "{StackTrace}.", ex.Message, ex.StackTrace);
            }
        }

        private static void PublishCSNPacket(MemoryStream memoryStream)
        {
            try
            {
                Hashtable CSNraddarTrack = new Hashtable();
                Track track = new Track();
                BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.BigEndianUnicode);
                byte[] messageIdBytes = binaryReader.ReadBytes(2);
                ////Array.Reverse(messageIdBytes);
                if (BitConverter.ToInt16(messageIdBytes) == 16)
                {
                    //CSNraddarTrack.Add("MSG_ID", BitConverter.ToInt16(messageIdBytes));
                    byte[] messagelenghtBytes = binaryReader.ReadBytes(2);
                    // //Array.Reverse(messagelenghtBytes);
                    //CSNraddarTrack.Add("MSG_LENGTH", BitConverter.ToInt16(messagelenghtBytes));
                    byte[] trackNum = binaryReader.ReadBytes(4);
                    ////Array.Reverse(trackNum);
                    track.TRACK_NUMBER = BitConverter.ToInt32(trackNum);
                    //CSNraddarTrack.Add("MMSI", BitConverter.ToInt32(trackNum));
                    byte[] tracklabel = binaryReader.ReadBytes(32);
                    ////Array.Reverse(tracklabel);
                    track.TRACK_LABEL = Encoding.ASCII.GetString(tracklabel).TrimStart('@');
                    //CSNraddarTrack.Add("TRACK_LABEL", Encoding.ASCII.GetString(tracklabel).TrimStart('@'));
                    byte[] latitude = binaryReader.ReadBytes(4);
                    ////Array.Reverse(latitude);
                    track.LAT = Convert.ToDecimal(BitConverter.ToSingle(latitude));
                    //CSNraddarTrack.Add("LAT", BitConverter.ToSingle(latitude));
                    byte[] longitude = binaryReader.ReadBytes(4);
                    //Array.Reverse(longitude);
                    track.LON = Convert.ToDecimal(BitConverter.ToSingle(longitude));
                    //CSNraddarTrack.Add("LON", BitConverter.ToSingle(longitude));
                    byte[] course = binaryReader.ReadBytes(4);
                    //Array.Reverse(course);
                    track.COURSE = Convert.ToDecimal(BitConverter.ToSingle(course));
                    //CSNraddarTrack.Add("COURSE", BitConverter.ToSingle(course));
                    byte[] speed = binaryReader.ReadBytes(4);
                    //Array.Reverse(speed);
                    track.SPEED = Convert.ToDecimal(BitConverter.ToSingle(speed));
                    //CSNraddarTrack.Add("SPEED", BitConverter.ToSingle(speed));
                    byte[] bearing = binaryReader.ReadBytes(4);
                    //Array.Reverse(bearing);
                    track.BEARING = Convert.ToDecimal(BitConverter.ToSingle(bearing));
                    //CSNraddarTrack.Add("BEARING", BitConverter.ToSingle(bearing));
                    byte[] range = binaryReader.ReadBytes(4);
                    //Array.Reverse(range);
                    track.RANGE = BitConverter.ToSingle(range);
                    //CSNraddarTrack.Add("RANGE", BitConverter.ToSingle(range));
                    byte[] depth = binaryReader.ReadBytes(4);
                    //Array.Reverse(depth);
                    track.DEPTH = Convert.ToDecimal(BitConverter.ToSingle(depth));
                    //CSNraddarTrack.Add("DEPTH", BitConverter.ToSingle(depth));
                    byte[] affiliation = binaryReader.ReadBytes(2);
                    //Array.Reverse(affiliation);
                    track.AFFILIATION = BitConverter.ToInt16(affiliation);
                    //CSNraddarTrack.Add("AFFILIATION", BitConverter.ToInt16(affiliation));
                    byte[] category = binaryReader.ReadBytes(2);
                    //Array.Reverse(category);
                    track.CATEGORY = BitConverter.ToInt16(category);
                    //CSNraddarTrack.Add("CATEGORY", BitConverter.ToInt16(category));
                    byte[] subCat = binaryReader.ReadBytes(2);
                    //Array.Reverse(subCat);
                    track.SUB_CAT = BitConverter.ToInt16(subCat);
                    //CSNraddarTrack.Add("SUB_CAT", BitConverter.ToInt16(subCat));
                    byte[] updateTime = binaryReader.ReadBytes(8);
                    // //Array.Reverse(updateTime);
                    CSNraddarTrack.Add("UPDATE_TIME", Encoding.ASCII.GetString(updateTime));// should modify
                    byte[] trackSource = binaryReader.ReadBytes(10);
                    ////Array.Reverse(trackSource);
                    track.TRACKSOURCE = Encoding.ASCII.GetString(trackSource).TrimStart('@');
                    track.TRACKTYPE = "CSN Raddar";
                    //CSNraddarTrack.Add("TRACK_SOURCE", Encoding.ASCII.GetString(trackSource).TrimStart('@'));

                    byte[] sourceStation = binaryReader.ReadBytes(10);
                    track.SOURCE_STATION = Encoding.ASCII.GetString(sourceStation).TrimStart('@');

                    if (track.TRACKSOURCE == "AIS")
                    {
                        byte[] IMO = binaryReader.ReadBytes(4);
                        //Array.Reverse(IMO);
                        track.IMO = BitConverter.ToInt32(IMO);
                        //CSNraddarTrack.Add("IMO", BitConverter.ToInt32(IMO));
                        byte[] shipType = binaryReader.ReadBytes(2);
                        //Array.Reverse(shipType);
                        track.SHIPTYPE = BitConverter.ToInt16(shipType);
                        //CSNraddarTrack.Add("SHIP_TYPE", BitConverter.ToInt16(shipType));
                        byte[] callSign = binaryReader.ReadBytes(8);
                        ////Array.Reverse(callSign);
                        track.CALLSIGN = Encoding.ASCII.GetString(callSign).TrimStart('@');
                        //CSNraddarTrack.Add("CALLSIGN", Encoding.ASCII.GetString(callSign).TrimStart('@'));
                        byte[] draught = binaryReader.ReadBytes(2);
                        //Array.Reverse(draught);
                        track.DRAUGHT = BitConverter.ToInt16(draught);
                        //CSNraddarTrack.Add("DRAUGHT", BitConverter.ToInt16(draught));
                        byte[] destination = binaryReader.ReadBytes(20);
                        ////Array.Reverse(destination);
                        track.DESTINATION = Encoding.ASCII.GetString(destination).TrimStart('@');
                        //CSNraddarTrack.Add("DESTINATION", Encoding.ASCII.GetString(destination).TrimStart('@'));
                        byte[] ETA_Month = binaryReader.ReadBytes(2);
                        //Array.Reverse(ETA_Month);
                        //CSNraddarTrack.Add("ETA_MONTH", BitConverter.ToInt16(ETA_Month));
                        byte[] eta_Day = binaryReader.ReadBytes(2);
                        //Array.Reverse(eta_Day);
                        // CSNraddarTrack.Add("ETA_DAY", BitConverter.ToInt16(eta_Day));
                        byte[] eta_Hour = binaryReader.ReadBytes(2);
                        //Array.Reverse(eta_Hour);
                        //CSNraddarTrack.Add("ETA_HOUR", BitConverter.ToInt16(eta_Hour));
                        byte[] eta_Minute = binaryReader.ReadBytes(2);
                        //Array.Reverse(eta_Minute);
                        //CSNraddarTrack.Add("ETA_MINUTE", BitConverter.ToInt16(eta_Minute));
                        byte[] nav_Status = binaryReader.ReadBytes(2);
                        //Array.Reverse(nav_Status);
                        track.STATUS = BitConverter.ToInt16(nav_Status);
                        track.TRACKTYPE = "CSN AIS";
                        //CSNraddarTrack.Add("NAV_STATUS", BitConverter.ToInt16(nav_Status));
                        if (track != null)
                        {
                            Log.Information("TN:{0,-8}\tLAT:{2,-5}\tLON:{3,-8}\tSP:{4,-5} CO:{5,-5}\tIMO:{1,-5} (Source:{6,-5})", track.TRACK_NUMBER, track.IMO, Math.Round((decimal)(track.LAT), 5), Math.Round((decimal)(track.LON), 5), track.SPEED, track.COURSE, track.TRACKTYPE);
                            PublishAIS(track);
                        }
                    }
                    else
                    {
                        if (track != null)
                        {
                            Log.Information("TN:{0,-8}\tLAT:{2,-5}\tLON:{3,-8}\tSP:{4,-5} CO:{5,-5}\t    {1,-5} (Source:{6,-5})", track.TRACK_NUMBER, " ", Math.Round((decimal)(track.LAT), 5), Math.Round((decimal)(track.LON), 5), track.SPEED, track.COURSE, track.TRACKTYPE);
                            PublishSystemTrack(track);          // CSN Radar 
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "{@Message}" + Environment.NewLine + "{StackTrace}.", ex.Message, ex.StackTrace);
            }
        }

        private static void PublishAIS(Track track)
        {
            try
            {
                TrackRequest.Parameters.Clear();
                TrackRequest.AddJsonBody(track);
                IRestResponse response = RestClient.Execute(TrackRequest);
                Log.Information(response.StatusCode.ToString());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "{@Message}" + Environment.NewLine + "{StackTrace}.", ex.Message, ex.StackTrace);
            }
        }

        private static void PublishSystemTrack(Track track)
        {
            try
            {
                SystemTrackRequest.Parameters.Clear();
                SystemTrackRequest.AddJsonBody(track);
                IRestResponse response = RestClient.Execute(SystemTrackRequest);
                Log.Information(response.StatusCode.ToString());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "{@Message}" + Environment.NewLine + "{StackTrace}.", ex.Message, ex.StackTrace);
            }
        }


        #region "Helper Functions"
        private static byte[] DecryptFernet(byte[] key, string token, out DateTime timestamp)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (key.Length != 32)
            {
                throw new ArgumentException(nameof(key));
            }

            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            byte[] token2 = Base64UrlDecode(token);

            if (token2.Length < 57)
            {
                throw new ArgumentException(nameof(token));
            }

            byte version = token2[0];

            if (version != 0x80)
            {
                throw new Exception("version");
            }

            // Check the hmac
            {
                byte[] signingKey = new byte[16];
                Buffer.BlockCopy(key, 0, signingKey, 0, 16);

                using (var hmac = new HMACSHA256(signingKey))
                {
                    hmac.TransformFinalBlock(token2, 0, token2.Length - 32);
                    //byte[] hash2 = hmac.Hash;

                    //IEnumerable<byte> hash = token2.Skip(token2.Length - 32).Take(32);

                    //if (!hash.SequenceEqual(hash2))
                    //{
                    //    throw new Exception("Wrong HMAC!");
                    //}
                }
            }

            {
                // BigEndian to LittleEndian
                long timestamp2 = BitConverter.ToInt64(token2, 1);
                timestamp2 = IPAddress.NetworkToHostOrder(timestamp2);

                timestamp = DateTimeOffset.FromUnixTimeSeconds(timestamp2).UtcDateTime;
            }

            byte[] decrypted;

            using (var aes = new AesManaged())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                byte[] encryptionKey = new byte[16];
                Buffer.BlockCopy(key, 16, encryptionKey, 0, 16);
                aes.Key = encryptionKey;

                byte[] iv = new byte[16];
                Buffer.BlockCopy(token2, 9, iv, 0, 16);
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor())
                {
                    const int startCipherText = 25;
                    int cipherTextLength = token2.Length - 32 - 25;
                    decrypted = decryptor.TransformFinalBlock(token2, startCipherText, cipherTextLength);
                }
            }

            return decrypted;
        }
        private static byte[] Base64UrlDecode(string s)
        {
            // https://stackoverflow.com/a/26354677/613130
            // But totally rewritten :-)

            char[] chars;

            switch (s.Length % 4)
            {
                case 2:
                    chars = new char[s.Length + 2];
                    chars[chars.Length - 2] = '=';
                    chars[chars.Length - 1] = '=';
                    break;
                case 3:
                    chars = new char[s.Length + 1];
                    chars[chars.Length - 1] = '=';
                    break;
                default:
                    chars = new char[s.Length];
                    break;
            }

            for (int i = 0; i < s.Length; i++)
            {
                switch (s[i])
                {
                    case '_':
                        chars[i] = '/';
                        break;
                    case '-':
                        chars[i] = '+';
                        break;
                    default:
                        chars[i] = s[i];
                        break;
                }
            }

            byte[] result = Convert.FromBase64CharArray(chars, 0, chars.Length);
            return result;
        }
        #endregion


    }
}
