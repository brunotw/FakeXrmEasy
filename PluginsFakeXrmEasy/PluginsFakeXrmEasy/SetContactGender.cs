using Microsoft.Xrm.Sdk;
using PluginsFakeXrmEasy.Model;
using System;

namespace PluginsFakeXrmEasy.Interfaces
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
            GenderAPI = new GuessGenderAPIService();
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
                GenderModel response = Helper.Deserialize<GenderModel>(json);

                contact.Attributes["gendercode"] = response.Gender.Equals("male", StringComparison.OrdinalIgnoreCase) ? (int)EGender.Male : (int)EGender.Female;
            }
        }
    }
}
