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
    /// RoomWidget.xaml 的交互逻辑
    /// </summary>
    public partial class RoomWidget : UserControl
    {
        List<Border> borders;
        public RoomWidget()
        {
            InitializeComponent();
            borders = new List<Border>();
            borders.Add(border_0);
            borders.Add(border_1);
            borders.Add(border_2);
        }
        
        public void onUpdate(Room room)
        {
            onCountUpdated(room.count,room.prepareCount);
            onStateUpdate(room.state);
        }

        //人数变化
        private void onCountUpdated(int count,int prepareCount)
        {
            for(int i = 0; i < count; i++)
            {
                borders[i].Background= Brushes.Red;
            }
            for (int i = 0; i < prepareCount; i++)
            {
                borders[i].Background = Brushes.Yellow;
            }
        }

        private void onStateUpdate(RoomState state)
        {
            switch (state)
            {
                case RoomState.Prepare:
                    labelRoomState.Content = "准备中";
                    break;
                case RoomState.Playing:
                    labelRoomState.Content = "游戏中";
                    buttonJoin.IsEnabled = false;
                    break;
                default:
                    break;
            }
        }


    }
}
