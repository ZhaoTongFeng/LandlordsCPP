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
    /// ClientGamePreparePage.xaml 的交互逻辑
    /// </summary>
    public partial class ClientGamePreparePage : Page,INetwork
    {
        public User user;
        public ClientWindow clientWindow;
        public ClientGamePreparePage()
        {
            InitializeComponent();
        }

        public void ProcessData(Package package, User sender)
        {

        }
    }
}
