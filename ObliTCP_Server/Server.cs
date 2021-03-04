using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Opgave_1;

namespace ObliTCP_Server
{
    public class Server
    {
        private static List<Beer> _beers = new List<Beer>();


        public void Start()
        {
            TcpListener server = null;
            try
            {
                Int32 port = 4646;
                IPAddress localAddress = IPAddress.Loopback;
                server = new TcpListener(localAddress, port);
                server.Start();
                Console.WriteLine("Server started");

                while (true)
                {
                    TcpClient connectionSocket = server.AcceptTcpClient();
                    Task.Run(() =>
                    {
                        TcpClient tempSocket = connectionSocket;
                        DoClient(tempSocket);
                    });
                }
                //server.Stop();
            }
            catch(SocketException e)
            {
                Console.WriteLine($"SocketException {e}");
            }
        }

        private void DoClient(TcpClient connectionSocket)
        {
            Console.WriteLine("Server Activated");
            Stream ns = connectionSocket.GetStream();

            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);
            sw.AutoFlush = true;

            bool keepGoing = true;
            while (keepGoing)
            {
                string message = sr.ReadLine();
                switch (message)
                {
                    case "stop":
                        keepGoing = false;
                        break;
                    case "HentAlle":
                        foreach (var VARIABLE in _beers)
                        {
                            sw.WriteLine(VARIABLE.ToString());
                        }
                        break;
                    case "Hent":
                        int id = Convert.ToInt32(sr.ReadLine());
                        sw.WriteLine(_beers.Find(beer => beer.Id == id));
                        break;
                    case "Gem":
                        string jsonBeer = sr.ReadLine();
                        if(jsonBeer != null)
                            try
                            {
                                Beer newBeer = JsonConvert.DeserializeObject<Beer>(jsonBeer);
                                _beers.Add(newBeer);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Json objektet er skrevet forkert ind");
                            }
                        break;
                }
            }
            ns.Close();
            connectionSocket.Close();
        }

        
    }
}
