using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
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
    public partial class FormStartServer : Form
    {



        //******************************************************************//
        // Constructor.                                                     //
        //******************************************************************//

        public FormStartServer()
        {
            InitializeComponent();
        }


        //******************************************************************//
        // Class (instance) variables.                                      //
        //******************************************************************//


        Thread         threadRunner    = null;  //Define a Thread.
        FormHTTPServer commsThreadForm = null;

        
        //*********************************************************************//
        // This method starts it all up.                                       //
        //*********************************************************************//

        private void startServer()
        {
            commsThreadForm = new FormHTTPServer();
            commsThreadForm.Show();
            commsThreadForm.init();

            threadRunner = new Thread(new ThreadStart(commsThreadForm.run));

            threadRunner.Start();
        }

        
        //*********************************************************************//
        // ...And this method shuts it all down.                               //
        //*********************************************************************//

        private void stopServer()
        {
            threadRunner.Suspend();
            commsThreadForm.Dispose();
        }



        
        //*******************************************************************//
        // Methods for start and stio buttons being pressed.                 //
        //*******************************************************************//

        private void buttonStart_Click(object sender, EventArgs e)
        {
            buttonStart.Enabled = false;
            buttonStop.Enabled  = true;
            startServer();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            buttonStart.Enabled = true;
            buttonStop.Enabled  = false;
            stopServer();
        }


        private void FormStartServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }





    }
}