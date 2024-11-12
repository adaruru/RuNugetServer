using System.ComponentModel.DataAnnotations;
using RuLib.ValueEnum;

namespace UnitTest.RuLib.ValueEnumTest;

public class ParentBatchAppEnum<T> : ValueEnumBase<T>
{
    /// <summary>
    /// 使用者資料匯入批次作業
    /// </summary>
    [Display(Name = "使用者資料匯入批次作業", Description = "使用者資料匯入批次作業Description")]
    public static string UserDataImport => "901";

    /// <summary>
    /// 部門資料匯入批次作業
    /// </summary>
    [Display(Name = "部門資料匯入批次作業")]
    public static string DeptDataImport => "902";
}

public class ProdFakeBatchAppEnum : ParentBatchAppEnum<ProdFakeBatchAppEnum>
{
    ///Production不應有值
}

public class HncbFakeBatchAppEnum : ParentBatchAppEnum<HncbFakeBatchAppEnum>
{
    /// <summary>
    /// 台電媒體檔資料匯入
    /// </summary>
    [Display(Name = "台電媒體檔資料匯入")]
    public static string TaiwanPowerImport => "0201";

    public static string GetCustom()
    {
        return "Custom1234";
    }

}