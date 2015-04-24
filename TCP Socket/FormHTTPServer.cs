using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

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
    public partial class FormHTTPServer : Form
    {
        //******************************************************************//
        // Constructor.                                                     //
        //******************************************************************//

        public FormHTTPServer()
        {
            InitializeComponent();
        }


        //******************************************************************//
        // Class (instance) variables.                                      //
        //******************************************************************//

        TcpListener   listeningSocket  = null;   //Listening socket.
        Socket        connection       = null;   //TCP/IP socket to handle the actual connection

         
        IPAddress     ipAddress        = null;
        IPHostEntry   localHostInfo    = null;
        /* Usual HTTP port is 80 BUT
         * I had to change this to run it on my laptop
         * as port 80 caused conflicts
         */
        int           listenPort       = 800;    

        String        hostName         = null;
        String        userName         = null;


        //*******************************************************************//
        //                                                                   //
        // All the action kicks off here.   I anm not trying to sell this to //
        // you as an example of good programming practise; it is just a      //
        // convenient place to put things - Nigel                            //
        //                                                                   //
        //*******************************************************************//

        public void init()
        {
            userName = Environment.UserName;

            //*****************************************************************//
            // The lines down to creating the listening socket aren't really   //
            // necessary; thay are just extracting environment stuff to display//
            // in my list box - Nigel                                          //
            //*****************************************************************//

            localHostInfo = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());

            //You may have several IP addresses; this only assumes you have one //
            //and gets the first out of an array of (potentially) many.         //

            hostName  = localHostInfo.HostName;


            listBoxStatus.Items.Add("Serving pages via localhost:"+listenPort);
            listBoxStatus.Items.Add("I seem to have the following IP numbers");

            foreach(IPAddress ip in localHostInfo.AddressList)
                listBoxStatus.Items.Add(ip.ToString());
            



            //*********************************************************//
            // Create our new listening TCP/IP socket. Read the        //
            // TCP/IP - Nigel.                                         //
            //*********************************************************//

            ipAddress = new IPAddress(new byte[] { 127, 0, 0, 1 });     //REMOVE IN LABS
            
            //listeningSocket = new TcpListener(ipAddress, listenPort);
            listeningSocket = new TcpListener(IPAddress.Any, listenPort);
            listeningSocket.Start();
            listBoxStatus.Refresh();  //update the form.
        }



        //*******************************************************************//
        //                                                                   //
        // When we turn ourselves into a Thread, this method will treated by //
        // the low level schedluer as a separate process.  It runs in its own//
        // time, but our memory space.   We can't access the User Interface  //
        // as C# complains they cannot be accessed from a different thread.  //
        // Answers on a postcard please - Nigel.                             //
        //                                                                   //
        //*******************************************************************//

        public void run()
        {
            //**************************************************************//
            // Sit and wait until something (a web browser here)            //
            // attempts to connect to us.                                   //
            //                                                              //
            // We block (i.e. wait) on the listeningSocket.AcceptSocket();  //
            // When it does, we get passed a new connection.                //
            //                                                              //
            // This run() method (I have called it run() to make C# look    //
            // like Java) runs in a sepatate Thread.   It has to.           //
            // Otherwise the blocking (i.e. waiting) while we wait for a    //
            // network conection would lock up the whole application.       //
            //**************************************************************//

            Console.Out.WriteLine("Thread running");

            while (true)
            {
                connection = listeningSocket.AcceptSocket();

                //***************************************************************//
                //The usual C# hokum cokum to create a new Thread.   This is one //
                //of the few things I don't like about C# - Nigel.               //
                //***************************************************************//

                ConnectionThread c = new ConnectionThread(connection);
                Thread ct = new Thread(new ThreadStart(c.run));

                ct.Start();
            }
        }
    }
}