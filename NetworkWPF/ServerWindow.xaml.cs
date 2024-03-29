﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NetworkWPF
{
    /// <summary>
    /// ServerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ServerWindow : Window
    {
        public ServerWindow()
        {
            InitializeComponent();
            Closing += ServerWindow_Closing;

            labelIP.Content = Server.localIP;
            labelPort.Content = Server.port;

            Server.Start(textBlockMessage, textBlockLog, labelRoomCount, labelUserCount);
        }

        private void ServerWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Server.Stop();
        }
        
    }
}
