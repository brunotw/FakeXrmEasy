using Microsoft.Xrm.Sdk;
using System;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace PluginsFakeXrmEasy
{
    public class SetContactGender : IPlugin
    {
        private static IGuessGenderAPI GenderAPI { get; set; }

        public enum EGender
        {
            Male = 1,
            Female = 2
        }

        public SetContactGender()
        {
            GenderAPI = new GenderAPI();
        }

        public SetContactGender(IGuessGenderAPI genderAPI)
        {
            GenderAPI = genderAPI;
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity entity)
            {
                Entity contact = entity;
                var firstName = contact.GetAttributeValue<string>("firstname");

                if (string.IsNullOrEmpty(firstName))
                    return;

                var json = GenderAPI.GuessGenderBasedOnName(firstName);

                GenderModel response = Deserialize<GenderModel>(json);
                contact.Attributes["gendercode"] = response.Gender.Equals("male", StringComparison.OrdinalIgnoreCase) ? (int)EGender.Male : (int)EGender.Male;
            }
        }

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


    public interface IGuessGenderAPI
    {
        string GuessGenderBasedOnName(string name);
    }

    public class GenderAPI : IGuessGenderAPI
    {
        public string GuessGenderBasedOnName(string name)
        {
            using (var client = new HttpClient())
            {
                var url = string.Format("https://api.genderize.io?name={0}", name);
                var result = client.GetStringAsync(url).Result;
                return result;
            }
        }
    }

    [DataContract]
    public class GenderModel
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "gender")]
        public string Gender { get; set; }

        [DataMember(Name = "probability")]
        public double Probability { get; set; }

        [DataMember(Name = "count")]
        public int Count { get; set; }
    }

}
