
using System.ComponentModel.DataAnnotations;


namespace CommonTest.ITSEnumTest;

public class ParentBatchAppEnum<T> : TextEnumBase<T>
{
    /// <summary>
    /// 使用者資料匯入批次作業
    /// </summary>
    [Display(Name = "使用者資料匯入批次作業")]
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
}