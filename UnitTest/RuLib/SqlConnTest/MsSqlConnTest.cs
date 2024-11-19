using Microsoft.VisualStudio.TestTools.UnitTesting;
using RuLib.SqlConn;
using System.Collections.Generic;
using System.Data;
using UnitTest.RuLib.ValueEnumTest;

namespace UnitTest.RuLib.SqlConnTest
{
    [TestClass()]
    public class MsSqlConnTest
    {
        public IConn Conn;
        [TestInitialize]
        public void Init()
        {
            var options = new ConnOptions()
            {
                ConnectionString = "Data Source=192.168.1.63;Initial Catalog=Northwind;user id=sa;password=sa",
                DbType = ConnType.MsSql
            };
            Conn = new MsSqlConn(options);
        }
        [TestMethod()]

        public void TestConnectionTest()
        {
            var err = string.Empty;
            //arrange

            //act
            var a = Conn.OpenConn(out err);

            //assert
            Assert.AreEqual(true, a);
            Assert.AreEqual(err, "連線成功。");
        }

        [TestMethod()]
        public void ExecuteQueryTest()
        {
            //arrange
            var query = "SELECT DB_NAME() AS CurrentDatabase;";

            // Act
            DataTable result;
            try
            {
                result = Conn.ExecuteQuery(query); // 假設 Conn 是已初始化的連線實例
            }
            catch (Exception ex)
            {
                Assert.Fail($"Query execution failed with exception: {ex.Message}");
                return;
            }

            // Assert
            Assert.IsNotNull(result, "Result DataTable is null.");
            Assert.IsTrue(result.Columns.Contains("CurrentDatabase"), $"Column CurrentDatabase is missing.");

            Assert.AreEqual(1, result.Rows.Count, "Query returned more than one row.");
            var actualDatabaseName = result.Rows[0]["CurrentDatabase"]?.ToString();
            Assert.AreEqual("Northwind", actualDatabaseName, "Database name does not match.");
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