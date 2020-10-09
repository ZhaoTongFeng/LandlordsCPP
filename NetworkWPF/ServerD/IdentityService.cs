using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NetworkWPF
{
    public class IdentityService : INetwork
    {
        static string PATH_USER = "user.json";
        JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,//带空格和缩进的格式化输出
        };

        public void ProcessData(Package package, User sender)
        {
            if (!package.clsName.Equals("Identity"))
            {
                return;
            }
            switch (package.funName)
            {
                case "login":
                    Login(package.data, sender);
                    break;
                default:
                    break;
            }
        }
        
        
        public void Login(string data,User sender)
        {
            User user = JsonSerializer.Deserialize<User>(data);
            user.isLogin = false;

            //查看本地用户数据是否存在
            if (!File.Exists(PATH_USER))
            {
                //如果不存在则创建用户数据文件，并注册该用户
                Dictionary<String, User> users = new Dictionary<String, User>();
                user.id = users.Count;
                users.Add(user.name, user);                
                File.WriteAllText(PATH_USER, JsonSerializer.Serialize(users, JsonSerializerOptions));

                user.isLogin = true;
                sender.name = user.name;
                sender.password= user.password;
            }
            else
            {
                //存在则读取本地数据
                string jsonString = File.ReadAllText(PATH_USER);
                Dictionary<String, User> users = JsonSerializer.Deserialize<Dictionary<String, User>>(jsonString);


                //和输入信息进行比较
                if (users.ContainsKey(user.name))
                {
                    User dbUser = users[user.name];
                    if (user.password == dbUser.password)
                    {
                        user.isLogin = true;
                        sender.name = user.name;
                        sender.password = user.password;
                    }
                    else
                    {
                        //如果用户存在但是密码错误则显示错误信息
                    }
                }
                else
                {
                    //如果用户不存在则注册，显示注册成功
                    user.id = users.Count;
                    users.Add(user.name, user);
                    jsonString = JsonSerializer.Serialize(users, JsonSerializerOptions);
                    File.WriteAllText(PATH_USER, jsonString);

                    user.isLogin = true;
                    sender.name = user.name;
                    sender.password = user.password;
                }
            }
            Server.Log(user.name + (user.isLogin == true ? "登录成功" : "登录失败"));
            Server.UpdateStatus();
            
            
            sender.Send(new Package(Package.OPT, "LoginPage", "onLogin", JsonSerializer.Serialize(user)));

        }
    }
}
