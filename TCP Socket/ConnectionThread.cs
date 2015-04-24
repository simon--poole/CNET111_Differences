using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;

//************************************************************************//
// Thread to actually handle an HTTP connection.                          //
//************************************************************************//

//************************************************************************//
// This project makes an extremely simple HTTP server.   By Nigel.        //
//                                                                        //
// Please use this code for any eduactional or non profit making          //
// research porposes on the conditions that.                              //
//                                                                        //
// 1.    You may only use it for educational and related research         //
//      pusposes.                                                         //
//                                                                        //
// 2.   You leave my name on it.                                          //
//                                                                        //
// 3.   You correct at least 10% of the typig and spekking mistskes.      //
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
        }

        
        //******************************************************************//
        // Class (instance) variables.                                      //
        //******************************************************************//

        Socket        connection       = null;   //TCP/IP socket to handle the actual connection
        NetworkStream connectionStream = null;
        BinaryReader  inStream         = null;
        BinaryWriter  outStream        = null;
        String        userName         = null;


        

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

                String[] items = s.Split();//This will contain all the stuff the browser transmitted,
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

            String stringOut = "HTTP/ 1.1 200 OK\r\n";
            outStream.Write(stringOut.ToCharArray());

            stringOut = "Content-Type: text/html\r\n";
            outStream.Write(stringOut.ToCharArray());

            stringOut = "\r\n";                         //Blank lines instead of//
                                                        //writing date and      //
            outStream.Write(stringOut.ToCharArray());   //content length.   That//
            stringOut = "\r\n";                         //is for you...         //

            stringOut = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">\r\n";
            outStream.Write(stringOut.ToCharArray());

            stringOut = "<html>\r\n";
            outStream.Write(stringOut.ToCharArray());

            stringOut = "<body>\r\n";
            outStream.Write(stringOut.ToCharArray());

            stringOut = "Welcome to <strong>" + userName + "'s </strong>primative HTTP server";
            outStream.Write(stringOut.ToCharArray());

            stringOut = "</body></html>\r\n";
            outStream.Write(stringOut.ToCharArray());



            inStream.Close();
            outStream.Flush();
            outStream.Close();
            connectionStream.Close();
            connection.Close();

            Console.Out.WriteLine("Done; Connection closed.");
        }

    }
}
