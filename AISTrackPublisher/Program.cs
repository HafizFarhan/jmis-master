using Microsoft.Extensions.Configuration;
using MTC.JMIS.AISTrackPublisher;
using MTC.JMIS.AISTrackPublisher.Models;
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
using System.Threading;
using System.Threading.Tasks;

namespace AISTrackPublisher
{
    class Program
    {
        private static Track AISTrack { get; set; }
        private static Parser AISParser { get; set; }
        private static RestClient RestClient { get; set; }
        private static RestRequest TrackRequest { get; set; }
        private static RestRequest TrackUpdateRequest { get; set; }
        private static int AISPort = 0;
        private static string APIBaseURL = "";
        static void Main(string[] args)
        {
            try
            {
                SetupLogger();
                IntializeObjects();
                ReadAISData();
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
        private static void SetupLogger()
        {
            try
            {
                Log.Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.Console().WriteTo.File("AISLogs_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss-ms") + ".txt").CreateLogger();
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
                        if (!int.TryParse(configuration.GetSection("AppKeys").GetSection("Port").Value, out AISPort))
                            throw new Exception("AIS port is not valid");
                    }
                    else
                        throw new Exception("Port key not found in AppKeys section");

                    if (string.IsNullOrEmpty(configuration.GetSection("AppKeys").GetSection("BaseURL").Value))
                        throw new Exception("BaseURL key not found in AppKeys section");

                    APIBaseURL = configuration.GetSection("AppKeys").GetSection("BaseURL").Value;
                }
                else
                    throw new Exception("AppKeys section not found in appsettings");

                AISParser = new Parser();
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


            }
            catch (Exception ex)
            {
                Log.Error(ex, "{@Message}" + Environment.NewLine + "{StackTrace}.", ex.Message, ex.StackTrace);
            }
        }
        private static void ReadAISData()
        {
            try
            {
                using (UdpClient aisUDPClient = new UdpClient(AISPort))
                {
                    IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    Log.Information("Waiting for AIS data ...");
                    while (true)
                    {
                        byte[] receiveBytes = aisUDPClient.Receive(ref RemoteIpEndPoint);
                        string receivedData = Encoding.ASCII.GetString(receiveBytes);
                        if (!string.IsNullOrWhiteSpace(receivedData))
                        {
                            string packet = receivedData.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)[0];
                            if (packet.StartsWith("!AIVDM"))
                                Task.Run(() => PublishAISPacket(packet));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "{@Message}" + Environment.NewLine + "{StackTrace}.", ex.Message, ex.StackTrace);
            }
        }
        private static void PublishAISPacket(string packet)
        {
            try
            {
                Log.Information(packet);
                Hashtable parsedAIS = new Hashtable();
                parsedAIS = AISParser.Parse(packet);
                if (parsedAIS != null)
                {
                    AISTrack = new Track();
                    AISTrack.TRACK_NUMBER = Convert.ToInt64(parsedAIS["MMSI"]);
                    if ((AISTrack.TRACK_NUMBER.ToString().Length) == 9)
                    {
                        AISTrack.LAT = Convert.ToDecimal(parsedAIS["Latitude"]);
                        AISTrack.LON = Convert.ToDecimal(parsedAIS["Longitude"]);
                        AISTrack.SPEED = Convert.ToDecimal(parsedAIS["SpeedOverGround"]);
                        AISTrack.HEADING = Convert.ToDecimal(parsedAIS["TrueHeading"]);
                        AISTrack.COURSE = Convert.ToDecimal(parsedAIS["CourseOverGround"]);
                        AISTrack.STATUS = Convert.ToInt64(parsedAIS["NavigationalStatus"]);
                        AISTrack.ROT = Convert.ToInt64(parsedAIS["RateOfTurn"]);
                        AISTrack.FLAG = Convert.ToString(parsedAIS["RAIMFlag"]);
                        AISTrack.IMO = Convert.ToInt64(parsedAIS["IMO"]);
                        AISTrack.SHIPTYPENAME = ShipTypes.GetShipTypesCategory(Convert.ToInt32(parsedAIS["ShipType"]));         //unspecified if ShipType not present in packet(0)
                        AISTrack.TRACKTYPE = "Local AIS";
                        AISTrack.TRACKSOURCE = "AIS";

                        //Log.Information("Message Type :"+ Convert.ToDecimal(parsedAIS["MessageType"])+", Ship Type: "+ Convert.ToInt32(parsedAIS["ShipType"]));

                        if (Convert.ToDecimal(parsedAIS["MessageType"]) == 5)
                        {
                            Log.Information("TN:{0,-8}\tLAT:{2,-5}\tLON:{3,-8}\tSP:{4,-5} CO:{5,-5}\tIMO:{1,-5} (Source:{6,-5})", AISTrack.TRACK_NUMBER, AISTrack.IMO, Math.Round((decimal)(AISTrack.LAT), 5), Math.Round((decimal)(AISTrack.LON), 5), AISTrack.SPEED, AISTrack.COURSE, AISTrack.TRACKTYPE);
                            AISTrack.TRACK_LABEL = Convert.ToString(parsedAIS["VesselName"]);
                            AISTrack.DESTINATION = Convert.ToString(parsedAIS["Destination"]);
                            AISTrack.SHIPTYPENAME = ShipTypes.GetShipTypesCategory(Convert.ToInt32(parsedAIS["ShipType"]));

                            TrackUpdateRequest = new RestRequest("api/AISTracks/" + AISTrack.TRACK_NUMBER, Method.PUT);
                            TrackUpdateRequest.RequestFormat = DataFormat.Json;
                            TrackUpdateRequest.AddJsonBody(AISTrack);
                            IRestResponse response = RestClient.Execute(TrackUpdateRequest);
                            Log.Information(response.StatusCode.ToString());
                        }
                        else
                        {
                            Log.Information("TN:{0,-8}\tLAT:{2,-5}\tLON:{3,-8}\tSP:{4,-5} CO:{5,-5}\tIMO:{1,-5} (Source:{6,-5})", AISTrack.TRACK_NUMBER, AISTrack.IMO, Math.Round((decimal)(AISTrack.LAT), 5), Math.Round((decimal)(AISTrack.LON), 5), AISTrack.SPEED, AISTrack.COURSE, AISTrack.TRACKTYPE);
                            TrackRequest.Parameters.Clear();
                            TrackRequest.AddJsonBody(AISTrack);
                            IRestResponse response = RestClient.Execute(TrackRequest);
                            Log.Information(response.StatusCode.ToString());
                        }
                    }
                }
                else
                {
                    Log.Error(packet);
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex, "{@Message}" + Environment.NewLine + "{StackTrace}.", ex.Message, ex.StackTrace);
            }

        }
    }
}
