using System;
using System.Collections.Generic;
using System.Text;

namespace LandlordsClient
{
    /// <summary>
    /// 网络传输协议
    /// 
    /// 操作类型:目标类名:函数名:数据
    /// 
    /// MSG:DATA
    /// OPT:DESTINATION CLASS NAME:FUNCATION NAME:DATA
    /// 
    /// 目标类名：和类名相同
    /// 函数名：和函数名相同，同时隐含了DATA的类型，当玩家创建房间之后，服务器将房间进行序列化传到客户端，客户端对房间进行反序列化，但是数据中并不显式指定DATA就是ROOM，如果在同一个函数中传入多段数据那么则需要指定每一段数据的名称，但也不需要指定类型，因为名称就隐含了类型。
    /// 数据：统一使用JSON格式
    /// </summary>
    public class Package
    {
        public string type;
        public string clsName;
        public string funName;
        public string data;

        /// 操作类型：MSG，OPT
        public static string MSG = "MSG";
        public static string OPT = "OPT";
        public static string ERR = "ERR";

        public static char SEP = ':';

        public bool isErr = false;


        public Package()
        {
        }

        public Package(string source)
        {
            string[] splits = source.Split(SEP);
            type = splits[0];
            if (type.Equals(MSG))
            {
                data = splits[1];
            }
            else if (type.Equals(OPT))
            {
                clsName = splits[1];
                funName = splits[2];
                data = splits[3];
            }
            else
            {
                isErr = true;
            }
        }

        public bool IsSucc()
        {
            return isErr;
        }

        public override string ToString()
        {
            if (type == null)
            {
                return ERR;
            }else if (type.Equals(MSG))
            {
                StringBuilder stringBuilder = new StringBuilder(Package.MSG);
                stringBuilder.Append(Package.SEP);
                stringBuilder.Append(data);
                return stringBuilder.ToString();
            }else if (type.Equals(OPT))
            {
                StringBuilder stringBuilder = new StringBuilder(Package.OPT);
                stringBuilder.Append(Package.SEP);
                stringBuilder.Append(clsName);
                stringBuilder.Append(Package.SEP);
                stringBuilder.Append(funName);
                stringBuilder.Append(Package.SEP);
                stringBuilder.Append(data);
                return stringBuilder.ToString();
            }
            else
            {
                return ERR;
            }
        }


    }
}
