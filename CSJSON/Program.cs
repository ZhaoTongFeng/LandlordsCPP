using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace CSJSON
{
    //1.简单类
    public class WeatherForecast
    {

        
        //如果将显示的 JSON 反序列化为显示的类型，则 DatesAvailable 和 SummaryWords 属性无处可去，会丢失。 若要捕获额外数据（如这些属性），请将 JsonExtensionData 特性应用于类型 Dictionary<string,object> 或 Dictionary<string,JsonElement> 的属性：
        [JsonExtensionData]
        public Dictionary<String, String> dic { get; set; }

        //如果没有Set和Get就不行了
        public DateTimeOffset Date;

        //自定义JSON属性名称，优先级高于option
        [JsonPropertyName("Temperature")]
        public int TemperatureCelsius { get; set; }
        //排除属性
        [JsonIgnore]
        public string Summary { get; set; }
    }

    /// <summary>
    /// 派生类，用之前的办法就不管用，得指定Serialize<object>
    /// </summary>
    public class WeatherForecastDerived : WeatherForecast
    {
        public int WindSpeed { get; set; }
    }

    /// <summary>
    /// 默认情况枚举会转化为数字，如果要将枚举名称转化为字符串要使用JsonStringEnumConverter
    /// </summary>
    public enum Summary
    {
        Cold, Cool, Warm, Hot
    }


    //2.嵌套类
    public class WeatherForecastWithPOCOs
    {
        public DateTimeOffset Date { get; set; }
        public int TemperatureCelsius { get; set; }
        public string Summary { get; set; }
        public string SummaryField;
        public IList<DateTimeOffset> DatesAvailable { get; set; }
        public Dictionary<string, HighLowTemps> TemperatureRanges { get; set; }
        public string[] SummaryWords { get; set; }
    }

    public class HighLowTemps
    {
        public int High { get; set; }
        public int Low { get; set; }
    }


    class Program
    {
        static string fileName = "weather.json";
        static  void Main(string[] args)
        {
            //序列化
            WeatherForecast weather = new WeatherForecast();
            weather.TemperatureCelsius = 20;
            weather.Summary = "A";


            string jsonString = JsonSerializer.Serialize(weather, new JsonSerializerOptions {
                AllowTrailingCommas = true,//允许注释
                IgnoreNullValues = true,//排除所有NULL值
                WriteIndented = true,//带空格和缩进的格式化输出
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase//驼峰命名，首字母小写
            });

            //以UTF8编码进行转换，和上面的区别是上面需要转换成UTF16，这个快5%-10%
            byte[] utf8String = JsonSerializer.SerializeToUtf8Bytes(weather);

            Console.WriteLine(jsonString);





            //Thread thread = new Thread(Test);
            //thread.Start();


            WeatherForecastDerived weatherForecastDerived = new WeatherForecastDerived();
            jsonString = JsonSerializer.Serialize<object>(weatherForecastDerived);
            //写入文件
            File.WriteAllText(fileName, jsonString);


            //反序列化

            jsonString = File.ReadAllText(fileName);
            WeatherForecastDerived weather2 = JsonSerializer.Deserialize<WeatherForecastDerived>(jsonString,new JsonSerializerOptions {
                PropertyNameCaseInsensitive = true,//不区分大小写属性匹配
                //默认情况下，如果 JSON 中的属性为 null，则目标对象中的对应属性会设置为 null。 在某些情况下，目标属性可能具有默认值，并且你不希望 null 值替代默认值。
                IgnoreNullValues = true
            });
            Console.WriteLine(jsonString);

            //使用 JsonDocument 访问数据
            double sum = 0;
            int count = 0;
            
            using (JsonDocument document = JsonDocument.Parse(jsonString))
            {
                JsonElement root = document.RootElement;
                JsonElement studentsElement = root.GetProperty("Students");
                foreach (JsonElement student in studentsElement.EnumerateArray())
                {
                    if (student.TryGetProperty("Grade", out JsonElement gradeElement))
                    {
                        sum += gradeElement.GetDouble();
                    }
                    else
                    {
                        sum += 70;
                    }
                    count++;
                }
            }

            double average = sum / count;
            Console.WriteLine($"Average grade : {average}");

            //使用 JsonDocument 写入 JSON
            string inputFileName = "";
            jsonString = File.ReadAllText(inputFileName);

            var writerOptions = new JsonWriterOptions
            {
                Indented = true
            };

            var documentOptions = new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip
            };

            using FileStream fs = File.Create(outputFileName);
            using var writer = new Utf8JsonWriter(fs, options: writerOptions);
            using JsonDocument document = JsonDocument.Parse(jsonString, documentOptions);

            JsonElement root = document.RootElement;

            if (root.ValueKind == JsonValueKind.Object)
            {
                writer.WriteStartObject();
            }
            else
            {
                return;
            }

            foreach (JsonProperty property in root.EnumerateObject())
            {
                property.WriteTo(writer);
            }

            writer.WriteEndObject();

            writer.Flush();
        }

        /// <summary>
        /// 异步序列化
        /// </summary>
        public static async void SerializeAsyncTest()
        {
            WeatherForecast weather = new WeatherForecast();
            using (FileStream fs = File.Create(fileName))
            {
                await JsonSerializer.SerializeAsync(fs, weather);
            }
        }
        /// <summary>
        /// 异步反序列化
        /// </summary>
        public static async void DeSerializeAsyncTest()
        {
            WeatherForecast weather = new WeatherForecast();
            using (FileStream fs = File.OpenRead(fileName))
            {
                weather = await JsonSerializer.DeserializeAsync<WeatherForecast>(fs);
            }
        }


        
    }
}
