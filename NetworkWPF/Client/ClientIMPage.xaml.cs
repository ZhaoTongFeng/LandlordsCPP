using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NetworkWPF.Client
{
    /// <summary>
    /// ClientIMPage.xaml 的交互逻辑
    /// </summary>
    public partial class ClientIMPage : Page,INetwork
    {
        public User user;
        public ClientWindow clientWindow;

        public ClientIMPage()
        {
            InitializeComponent();
        }

        public void onSendMessage(string data,User sender)
        {
            textBlockMessage.Dispatcher.InvokeAsync(() => {
                textBlockMessage.Text += data + "\n";
                
            });
        }

        public void ProcessData(Package package, User sender)
        {
            if (!package.clsName.Equals("IMPage"))
            {
                return;
            }
            switch (package.funName)
            {
                case "onSendMessage":
                    onSendMessage(package.data, sender);
                    break;
                default:
                    break;
            }
        }

        private void buttonSend_Click(object sender, RoutedEventArgs e)
        {
            if (user.isLogin)
            {
                string msg = textBoxMessage.Text;
                if (msg.Length != 0)
                {
                    user.Send(new Package(Package.OPT, "IM", "SendMsgToAll", msg));
                    textBoxMessage.Text = "";
                }
            }
        }
    }
}
