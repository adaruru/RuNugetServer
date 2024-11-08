using RuLib.ValueEnum;
using BatchApp = UnitTest.RuLib.TextEnumTest.HncbFakeBatchAppEnum;

namespace UnitTest.RuLib.TextEnumTest
{
    [TestClass()]
    public class ValueEnumWrapperTest
    {
        [TestInitialize]
        public void Init()
        {

        }

        [TestMethod()]
        public void GetCustom()
        {
            //arrange
            string Expected = "Custom1234";

            //act
            var wrap = new ValueEnumWrapper<BatchApp>();
            var act = wrap.GetCustom(BatchApp.UserDataImport);

            //assert
            Assert.AreEqual(Expected, act);
        }
    }
}