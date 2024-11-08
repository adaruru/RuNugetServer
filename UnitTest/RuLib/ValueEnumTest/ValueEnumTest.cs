using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTest.RuLib.TextEnumTest
{
    [TestClass()]
    public class ValueEnumTest
    {
        [TestInitialize]
        public void Init()
        {

        }
        [TestMethod()]

        public void GetInfos()
        {
            //arrange
            Dictionary<string, string> Expected = new Dictionary<string, string>()
            {
                ["901"] = "使用者資料匯入批次作業",
                ["902"] = "部門資料匯入批次作業",
            };
            Dictionary<string, string> Expected2 = new Dictionary<string, string>()
            {
                ["0201"] = "台電媒體檔資料匯入",
                ["901"] = "使用者資料匯入批次作業",
                ["902"] = "部門資料匯入批次作業",
            };

            //act
            var act = ProdFakeBatchAppEnum.GetInfos(isEnumString: true);
            var act2 = HncbFakeBatchAppEnum.GetInfos(isEnumString: true);


            //assert
            CollectionAssert.AreEqual(Expected, act);
            CollectionAssert.AreEqual(Expected2, act2);
        }

        [TestMethod()]
        public void GetValues()
        {
            //arrange
            List<string> Expected = new List<string>() { "901", "902", };
            List<string> Expected2 = new List<string>() { "0201", "901", "902", };

            //act
            var act = ProdFakeBatchAppEnum.GetValues(isEnumString: true);
            var act2 = HncbFakeBatchAppEnum.GetValues(isEnumString: true);

            //assert
            CollectionAssert.AreEqual(Expected, act);
            CollectionAssert.AreEqual(Expected2, act2);
        }

        [TestMethod()]
        public void GetName()
        {
            //arrange
            string Expected = "使用者資料匯入批次作業";

            //act
            var act = ProdFakeBatchAppEnum.GetName(HncbFakeBatchAppEnum.UserDataImport);
            var act2 = HncbFakeBatchAppEnum.GetName(HncbFakeBatchAppEnum.UserDataImport);

            //assert
            Assert.AreEqual(Expected, act);
            Assert.AreEqual(Expected, act2);
        }

        [TestMethod()]
        public void GetPorpertyName()
        {
            //arrange
            string Expected = "UserDataImport";

            //act
            var act = ProdFakeBatchAppEnum.GetPorpertyName(HncbFakeBatchAppEnum.UserDataImport);
            var act2 = HncbFakeBatchAppEnum.GetPorpertyName(HncbFakeBatchAppEnum.UserDataImport);

            //assert
            Assert.AreEqual(Expected, act);
            Assert.AreEqual(Expected, act2);
        }
    }
}