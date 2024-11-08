using CommonTest.ITSEnum.Wrapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using BatchApp = CommonTest.ITSEnumTest.HncbFakeBatchAppEnum;

namespace LibTests.Common.ITSEnum
{
    [TestClass()]
    public class ITSEnumWrapperTest
    {
        [TestInitialize]
        public void Init()
        {

        }
        [TestMethod()]

        public void GetInfos()
        {
            //arrange
            var batchAppEnum = BatchApp.UserDataImport;
            Dictionary<string, string> Expected = new Dictionary<string, string>()
            {
                ["0201"] = "台電媒體檔資料匯入",
                ["901"] = "使用者資料匯入批次作業",
                ["902"] = "部門資料匯入批次作業",
            };

            //act
            var act = BatchApp.GetInfos(isEnumString: true);

            //assert
            CollectionAssert.AreEqual(Expected, act);
        }

        [TestMethod()]
        public void GetValues()
        {
            //arrange
            List<string> Expected = new List<string>() { "0201", "901", "902", };

            //act
            var act = BatchApp.GetValues(isEnumString: true);

            //assert
            CollectionAssert.AreEqual(Expected, act);
        }

        [TestMethod()]
        public void GetDescription()
        {
            //arrange
            string Expected = "使用者資料匯入批次作業";

            //act
            var wrap = new GetEnumValueWrapper<BatchApp>();
            var act = wrap.GetDescription(BatchApp.UserDataImport);


            //assert
            Assert.AreEqual(Expected, act);
        }

        [TestMethod()]
        public void GetName()
        {
            //arrange
            string Expected = "UserDataImport";

            //act
            var act = BatchApp.GetName(BatchApp.UserDataImport);

            //assert
            Assert.AreEqual(Expected, act);
        }
    }
}