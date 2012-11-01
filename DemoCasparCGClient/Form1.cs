using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Security.Principal;
using System.IO;
using Svt.Caspar;

namespace DemoCasparCGClient
{
    public partial class Form1 : Form
    {
        //Main Caspar Device
        private CasparDevice casparDevice = new CasparDevice();

        //AMCP/OSC Connection Bits
        public string caspar_server_IP { get; set; }
        public int caspar_server_OSC_port { get; set; }
        public int caspar_server_AMCP_port { get; set; }
        private bool boolIsClientConnected = false;
        

        public Form1()
        {
            InitializeComponent();
            caspar_server_IP = ConfigurationManager.AppSettings["ServerIP"];
            caspar_server_OSC_port = Int32.Parse(ConfigurationManager.AppSettings["OSCPort"]);
            caspar_server_AMCP_port = Int32.Parse(ConfigurationManager.AppSettings["AMCPPort"]);
           
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            ChangeControlStatus(false);
            SetupCasparCGDevice();
        }

        private void SetupCasparCGDevice()
        {
            casparDevice.Settings.Hostname = caspar_server_IP;
            casparDevice.Settings.Port = caspar_server_AMCP_port;
            casparDevice.Settings.AutoConnect = false;
            casparDevice.Connected += new EventHandler<Svt.Network.NetworkEventArgs>(caspar_AMCP_Connected);
            casparDevice.Disconnected += new EventHandler<Svt.Network.NetworkEventArgs>(caspar_AMCP_Disconnected);
            casparDevice.FailedConnect += new EventHandler<Svt.Network.NetworkEventArgs>(casparDevice_AMCPFailed_Connect);
            casparDevice.Connect();
        }

        void caspar_AMCP_Connected(object sender, Svt.Network.NetworkEventArgs e)
        {
            Console.WriteLine("Caspar AMCP Client Connected");
            ChangeControlStatus(true);

            this.Invoke(new MethodInvoker(delegate()
            {
                toolstriplabelAMCPConnected.BackColor = Color.Green;
                toolstriplabelAMCPConnected.Text = "AMCP Connected";
            }));
        }

        void caspar_AMCP_Disconnected(object sender, Svt.Network.NetworkEventArgs e)
        {
            Console.WriteLine("Caspar AMCP Client Disconnected");
            casparDevice_AMCPFailed_Connect(sender, e);
        }

        void casparDevice_AMCPFailed_Connect(object sender, Svt.Network.NetworkEventArgs e)
        {
            
            this.Invoke(new MethodInvoker(delegate()
            {
                toolstriplabelAMCPConnected.BackColor = Color.Red;
                toolstriplabelAMCPConnected.Text = "AMCP Disconnected";
            }));
            //MessageBox.Show("Caspar Device has gone away");
            ChangeControlStatus(false);

            DialogResult result = MessageBox.Show("Cannot connect to CasparCG Server: " + caspar_server_IP + "\nDo you wish to wait to reconnect?", "Important Query", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
            {
                Application.Exit();
            }

            casparDevice.Connect();

        }

        private void ChangeControlStatus(bool p)
        {
            this.Invoke(new MethodInvoker(delegate()
            {
                foreach (Control c in this.Controls)
                {
                    c.Enabled = p;
                    //c.Visible = p;
                }
            }));
        }

               
    }
}
