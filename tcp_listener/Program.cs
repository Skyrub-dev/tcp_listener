using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace tcp_listener
{
    class Program
    {
        static void Main(string[] args)
        {
            tcp();
        }
        static void tcp()
        {
            string temp = "192.XXX.X.X";
            //string conf = "";
            //https://docs.microsoft.com/en-us/dotnet/api/system.net.ipaddress.parse?redirectedfrom=MSDN&view=net-5.0#System_Net_IPAddress_Parse_System_String_
            //https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcplistener.-ctor?view=net-5.0#System_Net_Sockets_TcpListener__ctor_System_Net_IPAddress_System_Int32_
            Console.WriteLine("Enter the port you want to start the process on...");
            int portno = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter the IP address you want to listen for (local address)...\n");
            //string localAddr = Console.ReadLine();
            Console.WriteLine("Is this okay? y/n");
            string conf = Console.ReadLine();
            Console.WriteLine("IP Address:\n" + temp);
            Console.WriteLine("Port:\n" + portno);
            if (conf == "y")
            {
                TcpListener server = null;
                try
                {
                    //Int32 port = Console.ReadLine();
                    // Set the TcpListener on port 13000.
                    Int32 port = portno;
                    IPAddress localAddr = IPAddress.Parse("192.168.1.13");

                    // TcpListener server = new TcpListener(port);
                    server = new TcpListener(localAddr, port);

                    // Start listening for client requests.
                    server.Start();

                    // Buffer for reading data
                    Byte[] bytes = new Byte[256];
                    String data = null;

                    // Enter the listening loop.
                    while (true)
                    {
                        Console.Write("\n\nWaiting for a connection on " + portno);

                        // Perform a blocking call to accept requests.
                        // You could also use server.AcceptSocket() here.
                        TcpClient client = server.AcceptTcpClient();
                        Console.WriteLine("\nConnected!");

                        data = null;

                        // Get a stream object for reading and writing
                        NetworkStream stream = client.GetStream();

                        int i;

                        // Loop to receive all the data sent by the client.
                        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            // Translate data bytes to a ASCII string.
                            data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                            Console.WriteLine("Received: {0}", data);

                            // Process the data sent by the client.
                            data = data.ToUpper();

                            byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                            // Send back a response.
                            stream.Write(msg, 0, msg.Length);
                            Console.WriteLine("Sent: {0}", data);
                        }

                        // Shutdown and end connection
                        client.Close();
                    }
                }
                catch (SocketException e)
                {
                    Console.WriteLine("SocketException: {0}", e);
                }
                finally
                {
                    // Stop listening for new clients.
                    server.Stop();
                }

                Console.WriteLine("\nHit enter to continue...");
                Console.Read();
            }

            else
            {
                Console.WriteLine("Canceled! abandoning...");
                //return;
            }
        }

        
    }
    
    
}
