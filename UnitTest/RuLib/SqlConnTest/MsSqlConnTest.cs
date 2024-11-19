using Microsoft.VisualStudio.TestTools.UnitTesting;
using RuLib.SqlConn;
using System.Collections.Generic;
using System.Data;
using UnitTest.RuLib.ValueEnumTest;

namespace UnitTest.RuLib.SqlConnTest;

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

    public void OpenConnTest()
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
        var expected = "Northwind";
        var expectedDbColumn = "CurrentDatabase";

        // Act
        var query = "SELECT DB_NAME() AS CurrentDatabase;";
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
        Assert.IsTrue(result.Columns.Contains(expectedDbColumn), $"Column CurrentDatabase is missing.");
        Assert.AreEqual(1, result.Rows.Count, "Query returned more than one row.");
        var actualDatabaseName = result.Rows[0][expectedDbColumn]?.ToString();
        Assert.AreEqual(expected, actualDatabaseName, "Database name does not match.");
    }


    [TestMethod()]
    public void ExecuteNonQueryTest()
    {
        //arrange
        var expected = 1;

        //act
        var sql = @"
-- 定義要 Upsert 的目標資料
DECLARE @CustomerID NVARCHAR(5) = 'ALFKI'; -- 要查找的主鍵
DECLARE @CompanyName NVARCHAR(40) = 'Updated Company Name';
DECLARE @ContactName NVARCHAR(30) = 'Updated Contact';
DECLARE @ContactTitle NVARCHAR(30) = 'Updated Title';
DECLARE @Address NVARCHAR(60) = 'Updated Address';
DECLARE @City NVARCHAR(15) = 'Updated City';
DECLARE @Region NVARCHAR(15) = NULL;
DECLARE @PostalCode NVARCHAR(10) = '12345';
DECLARE @Country NVARCHAR(15) = 'Updated Country';
DECLARE @Phone NVARCHAR(24) = '123-4567';
DECLARE @Fax NVARCHAR(24) = NULL;

-- 判斷是否存在該記錄
IF EXISTS (SELECT 1 FROM Customers WHERE CustomerID = @CustomerID)
BEGIN
    -- 如果存在，則執行更新操作
    UPDATE Customers
    SET 
        CompanyName = @CompanyName,
        ContactName = @ContactName,
        ContactTitle = @ContactTitle,
        Address = @Address,
        City = @City,
        Region = @Region,
        PostalCode = @PostalCode,
        Country = @Country,
        Phone = @Phone,
        Fax = @Fax
    WHERE CustomerID = @CustomerID;
    PRINT 'Record updated successfully.';
END
ELSE
BEGIN
    -- 如果不存在，則執行插入操作
    INSERT INTO Customers (CustomerID, CompanyName, ContactName, ContactTitle, Address, City, Region, PostalCode, Country, Phone, Fax)
    VALUES 
    (
        @CustomerID, @CompanyName, @ContactName, @ContactTitle, 
        @Address, @City, @Region, @PostalCode, @Country, @Phone, @Fax
    );
    PRINT 'Record inserted successfully.';
END;
";
        var act = Conn.ExecuteNonQuery(sql);
        //assert
        Assert.AreEqual(expected, act);
        Assert.AreEqual("Record updated successfully.\r\n", Conn.PrintMessage);
    }

}