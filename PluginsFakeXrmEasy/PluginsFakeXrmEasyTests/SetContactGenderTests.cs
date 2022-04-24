using FakeItEasy;
using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using PluginsFakeXrmEasy;

namespace PluginsFakeXrmEasyTests
{
    [TestClass]
    public class SetContactGenderTests
    {
        public XrmFakedContext Context { get; set; }
        public XrmFakedPluginExecutionContext ExecutionContext { get; set; }

        [TestInitialize]
        public void SetUp()
        {
            Context = new XrmFakedContext();
            ExecutionContext = Context.GetDefaultPluginContext();
        }

        [TestMethod]
        public void TestFakingThirdPartyCall()
        {
            //Arrange
            var fakeGenderAPI = A.Fake<IGuessGenderAPI>();
            A.CallTo(() => fakeGenderAPI.GuessGenderBasedOnName("Bruno")).Returns("{\"name\":\"Bruno\",\"gender\":\"Male\",\"probability\":0.99,\"count\":103169}");
            SetContactGender pluginInstance = new SetContactGender(fakeGenderAPI);

            var target = new Entity("contact");
            target.Attributes["firstname"] = "Bruno";
            ExecutionContext.InputParameters.Add("Target", target);

            //Act
            var fakedPlugin = Context.ExecutePluginWith(ExecutionContext, pluginInstance);

            //Assert
            Assert.AreEqual(target["gendercode"], (int)SetContactGender.EGender.Male);
        }

        [TestMethod]
        public void TestCallingThirdParty()
        {
            //Arrange
            var target = new Entity("contact");
            target.Attributes["firstname"] = "Lilian";

            //Act
            var fakedPlugin = Context.ExecutePluginWithTarget<SetContactGender>(target);

            //Assert
            Assert.AreEqual(target["gendercode"], (int)SetContactGender.EGender.Male);

        }
    }
}
