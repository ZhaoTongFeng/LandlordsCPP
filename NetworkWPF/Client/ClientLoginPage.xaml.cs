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
using System.Text.Json;
using System.IO;
using System.Configuration;

namespace NetworkWPF
{

    

    /// <summary>
    /// ClientLoginPage.xaml 的交互逻辑
    /// </summary>
    public partial class ClientLoginPage : Page
    {
        
        public static string PATH_USER = "user.json";

        JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,//带空格和缩进的格式化输出
        };
        public void SetUser(User user)
        {
            textUserName.Text = user.name;
            textPassword.Text = user.password;
        }

        public ClientLoginPage()
        {
            InitializeComponent();
            //连接服务器

        }

        public void NextPage()
        {
            //要将User传入下一个Page
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            string password = textPassword.Text;
            string username = textUserName.Text;

            //查看本地用户数据是否存在
            if (!File.Exists(PATH_USER))
            {
                //如果不存在则创建用户数据文件
                User user = new User();
                user.id = 0;
                user.name = username;
                user.password = password;

                Dictionary<String ,User> users = new Dictionary<String, User>();
                users.Add(user.name,user);
                string jsonString = JsonSerializer.Serialize(users, JsonSerializerOptions);
                File.WriteAllText(PATH_USER, jsonString);
                //跳转游戏大厅
                NextPage();
            }
            else
            {
                //存在则读取本地数据
                string jsonString = File.ReadAllText(PATH_USER);
                Dictionary<String, User> users = JsonSerializer.Deserialize<Dictionary<String, User>>(jsonString);
                int id = users.Count;

                //和输入信息进行比较
                if (users.ContainsKey(username))
                {
                    User user = users[username];
                    if (user.password == password)
                    {
                        //跳转游戏大厅
                        NextPage();
                        labelErr.Content = "登录成功";
                    }
                    else
                    {
                        //如果用户存在但是密码错误则显示错误信息
                        textPassword.Text = "";
                        labelErr.Content = "密码错误";
                    }
                }
                else
                {
                    //如果用户不存在则注册，显示注册成功
                    User user = new User();
                    user.id = id;
                    user.name = username;
                    user.password = password;
                    users.Add(user.name, user);
                    jsonString = JsonSerializer.Serialize(users, JsonSerializerOptions);
                    File.WriteAllText(PATH_USER, jsonString);
                    labelErr.Content = "注册成功";
                    //跳转游戏大厅
                    NextPage();
                }
            }
        }
    }
}
