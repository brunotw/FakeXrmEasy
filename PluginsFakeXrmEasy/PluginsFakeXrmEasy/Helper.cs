using System.IO;
using System.Runtime.Serialization.Json;

namespace PluginsFakeXrmEasy
{
    public class Helper
    {
        public static T Deserialize<T>(string body)
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(body);
                writer.Flush();
                stream.Position = 0;
                return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(stream);
            }
        }
    }
}
