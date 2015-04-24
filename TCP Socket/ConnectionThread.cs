using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Diagnostics;

//************************************************************************//
// Thread to actually handle an HTTP connection.                          //
//************************************************************************//

//************************************************************************//
// This project makes an extremely simple HTTP server.   By Nigel.        //
//                                                                        //
// Please use this code for any educational or non profit making          //
// research porposes on the conditions that.                              //
//                                                                        //
// 1.    You may only use it for educational and related research         //
//      purposes.                                                         //
//                                                                        //
// 2.   You leave my name on it.                                          //
//                                                                        //
// 3.   You correct at least 10% of the typing and spelling mistskes.      //
//                                                                        //
// © Nigel Barlow nigel@soc.plymouth.ac.uk 2014                           //
//************************************************************************//

namespace TCP_Socket
{
    class ConnectionThread
    {


        //******************************************************************//
        // Constructor.                                                     //
        //******************************************************************//

        public ConnectionThread(Socket socketToHandleConnection)
        {
            connection = socketToHandleConnection;
            connection.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
            connection.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        }

        
        //******************************************************************//
        // Class (instance) variables.                                      //
        //******************************************************************//

        Socket        connection       = null;   //TCP/IP socket to handle the actual connection
        NetworkStream connectionStream = null;
        BinaryReader  inStream         = null;
        BinaryWriter  outStream        = null;
        String        userName         = null;
        

        public void outputLine(String input)
        {
            String stringOut = input+"\r\n";
            Debug.WriteLine(input);
            outStream.Write(stringOut.ToCharArray());
        }
        

        //******************************************************************//
        //There are Threads all over the place here, probably un necessarily//
        //Here is yet another Thread to handle the connection.              //
        //******************************************************************//

        public void run()
        {

            //***********************************************************//
            //We have now accepted a connection from a web browser.      //
            //                                                           //
            //There are several ways to do this next bit.   Here I make a//
            //network stream and use it to create two other streams, an  //
            //input and an output stream.   Life gets easier at that     //
            //point.                                                     //
            //***********************************************************//
            connectionStream = new NetworkStream(connection);

            inStream  = new BinaryReader(connectionStream);
            outStream = new BinaryWriter(connectionStream);

            userName = Environment.UserName;
            String[] items = null;
            Thread.Sleep(100);

            byte b = 0;
            String s = "";      //This will contain all the stuff the browser transmitted,
                                //but "all in one go".
            try
            {
                while (connectionStream.DataAvailable)
                {
                    b = (byte)inStream.ReadSByte();
                    Console.Out.Write((char)b);
                    s += (char)b;
                }

                items = s.Split();//This will contain all the stuff the browser transmitted,
                                           //but nicely split up.

            }
            catch (EndOfStreamException eos)
            {
                Console.Out.WriteLine("Unexpected end of stream.");
                Console.Out.WriteLine("Error caused by " + eos.Message);
            }
            Console.Out.WriteLine("End of stream.");


            //***********************************************************//
            // Finally, write the output to the web browser.   The HTTP  //
            // dialog seems wo want each line terminated with both       //
            // Carriage Return (CR or the "\r") and LF (Line Feed, or "\n//
            // for new line), so that is the significance if all the    //
            // "\r\n" that you see.   The output is written as chars, as//
            // C# strings are slightly different.                       //
            //***********************************************************//

            //Serve files from a particular directory, ie /public_html/ as is the de-facto standard
            //In this case it serves from the project folder/public_html
            ServerClass.WorkingDirectory = System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\..\\public_html";
            //If we got any data from the stream
            if (items.Length > 1) {
                //Then process the request
                string action = items[0];
                string file = items[1];
                switch(action){
                    case "GET":
                        //Get full file path from working directory + relative file path
                        file = ServerClass.getFileLoc(file);
                        Debug.WriteLine(file);
                        //Check if file exists
                        //if it does....serve the file to the browser.
                        if (ServerClass.fileExists(file))
                        {
                            Byte[] contents = ServerClass.loadFile(file);
                            outputLine("HTTP/1.1 200 OK");
                            outputLine("Content-Type: " + ServerClass.getContentType(file));
                            //This works fine in IE but causes a Content-Length Mismatch in Chrome - not sure why.
                            //outputLine("Content-Length: " + contents.Length);
                            outputLine("Date: " + DateTime.Now.ToString("ddd, dd MMM yyyy HH: mm: ss "));
                            outputLine("Connection: close");
                            outStream.Write("\r\n".ToCharArray());
                            Debug.Write(contents.Length);
                            outStream.Write(contents);
                            
                        }
                        //Otherwise send a 404 error.
                        else
                        {
                            outputLine("HTTP/1.1 404 Not Found");
                            outputLine("Content-Type: text/html");
                            outputLine("Date: " + DateTime.Now.ToString("ddd, dd MMM yyyy HH: mm: ss "));
                            outputLine("Connection: close");
                        }
                        break;
                    //This could be extended for other HTTP codes such as POST, PUT, DELETE etc...
                }
        }



            inStream.Close();
            outStream.Flush();
            outStream.Close();
            connectionStream.Close();
            connection.Close();

            Console.Out.WriteLine("Done; Connection closed.");
        }

    }
}
