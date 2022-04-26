using FakeItEasy;
using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using PluginsFakeXrmEasy.Interfaces;

namespace PluginsFakeXrmEasyTests
{
    [TestClass]
    public class SetContactGenderTests
    {
        public XrmFakedContext Context = new XrmFakedContext();
        public XrmFakedPluginExecutionContext ExecutionContext { get; set; }
        public Entity Target = new Entity("contact");
        public IGuessGenderAPI FakeGenderAPI { get; set; }


        [TestInitialize]
        public void SetUp()
        {
            ExecutionContext = Context.GetDefaultPluginContext();
            FakeGenderAPI = A.Fake<IGuessGenderAPI>();
        }

        [TestMethod]
        public void ShouldReturnMale()
        {
            //Arrange
            A.CallTo(() => FakeGenderAPI.GuessGenderBasedOnName("Bruno")).Returns("{\"name\":\"Bruno\",\"gender\":\"Male\",\"probability\":0.99,\"count\":103169}");

            SetContactGender pluginInstance = new SetContactGender(FakeGenderAPI);

            Target.Attributes["firstname"] = "Bruno";
            ExecutionContext.InputParameters.Add("Target", Target);

            //Act
            Context.ExecutePluginWith(ExecutionContext, pluginInstance);

            //Assert
            Assert.AreEqual(Target["gendercode"], (int)SetContactGender.EGender.Male);
        }

        [TestMethod]
        public void ShouldReturnFemale()
        {
            //Arrange
            Target.Attributes["firstname"] = "Lilian";

            //Act
            Context.ExecutePluginWithTarget<SetContactGender>(Target);

            //Assert
            Assert.AreEqual(Target["gendercode"], (int)SetContactGender.EGender.Female);
        }


        [TestMethod]
        public void ShouldNotCallGuessGenderBasedOnName()
        {
            //Arrange
            Target.Attributes["firstname"] = string.Empty;
            ExecutionContext.InputParameters.Add("Target", Target);

            SetContactGender pluginInstance = new SetContactGender(FakeGenderAPI);

            //Act
            var fakedPlugin = Context.ExecutePluginWith(ExecutionContext, pluginInstance);

            //Assert
            A.CallTo(() => FakeGenderAPI.GuessGenderBasedOnName(A<string>.Ignored)).MustNotHaveHappened();
        }
    }
}
