using RuLib.JsonI18n;


namespace UnitTest.RuLib.TextEnumTest
{
    [TestClass()]
    public class LanTest
    {
        [TestInitialize]
        public void Init()
        {

        }
        [TestMethod()]

        public void LoadLanguageTest()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RuLib", "JsonI18nTest");
            var arrange = "en_US";
            Lan.LoadLanguage(arrange, path);
            var act = Lan.currentLan.ConnectionString;

            var arrange2 = "zh_TW";
            Lan.LoadLanguage(arrange2, path);
            var act2 = Lan.currentLan.ConnectionString;

            //assert
            Assert.AreEqual("Connection String", act);
            Assert.AreEqual("連線字串", act2);
        }
    }
}