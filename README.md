# RuNugetServer

[TOC]



## Local Server project

## RuLib

### TextEnumBase

模擬 Enum 寫法包裝成 Class 物件，可以儲存 string ，做到處理類似 Create ct 、 Update upd 、 Delete del 資料得效果，且包裝物件，令其可以透由繼承，追加擴充屬性。

以一般 Enum type 來說明比較實際使用上差異。

#### Entity Type

```csharp
//SystemEnum 
public Enum.ApprovalFunction ApprovalFunction { get; set; } //sql 對應的是 int

//TextEnum
//重新命名、重設資料類別 int (Model property 、各function區域變數)
//Enum 需改名與 TextEnum，方便未來維護
public string ApprovalTypeTextEnum { get; set; } 
```

#### Use example

```csharp
Enum.LaunchProgram app //=0;

//TextEnum 目前僅支援 string 或 int
int BatchAppTextEnum
//or
string BatchAppTextEnum 
```

#### 個別擴充繼承(維護在各自專案)

`重要：使用 Production 沒有擴充也必須繼承於專案 ITSEnum 不可直接使用 production 的 ITSEnum`

```csharp
//SystemEnum 
//Enum 無法擴充

//TextEnum
//設計 Base TextEnum
public class BatchAppTextEnum<T> : BaseEnum<T>
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

//有擴充需求，繼承 Base TextEnum 並傳入自身泛型
public class XXXBatchAppTextEnum : BatchAppTextEnum<XXXBatchAppTextEnum>
{
    [Display(Name = "費用匯入作業")]
    public static string PaymentDetailImport => "DX403";
}

var app = XXXBatchAppTextEnum.UserDataImport;//app = "901"
var app = XXXBatchAppTextEnum.PaymentDetailImport;//app = "DX403"

//無擴充需求 也建議寫一組，不可直接使用基底 Base，另物件在使用上更好維護
public class ApprovalFunctionTextEnum<T> : BaseEnum<T>
{
    public static string RejectByClient => "RejectR011";
    public static string RejectByAuthority => "RejectR012";
}
public class XXXApprovalFunctionTextEnum : ApprovalFunctionTextEnum<XXXApprovalFunctionTextEnum>
{}

var r = XXXApprovalFunctionTextEnum.RejectByClient;//r = "RejectR011"
```

#### 判斷、設值修改

```csharp
//Enum
if (x.ApprovalFunction == SystemEnum.ApprovalStatus.BranchApproveRole){}
public override SystemEnum.ApprovalFunction ApprovalFunction => SystemEnum.ApprovalFunction.BranchApproveRole;

//TextEnum
if (x.ApprovalFunction == ApprovalFunctionEnum.BranchApproveRole){}
public override int ApprovalFunction => ApprovalFunctionEnum.BranchApproveRole;
```

#### Enum function 

已此 BatchAppTextEnum 結構為例

#### GetEnumName() 取得設定描述 

```csharp
//Enum
SystemEnum.BatchAppEnum.GetEnumName();
//TextEnum
BatchAppTextEnum.GetDescription("901"); // =使用者資料匯入批次作業
```

#### GetValueString() 取得設定描述名稱

```csharp
//Enum
SystemEnum.BatchAppEnum.GetValueString();
//TextEnum
BatchAppEnum.GetName("901"); // UserDataImport
```



#### Enum function GetValues GetInfos 取所有Enum 值與描述

```csharp
//Enum
foreach (var p in Enum.GetValues(typeof(SystemEnum.LaunchProgram)))
  {
     Enum.TryParse(p.ToString(), true, out SystemEnum.LaunchProgram program);
     System.Console.WriteLine($"{(int)program}.{program.GetEnumName()}");
  }

//TextEnum
foreach (var p in BatchAppEnum.GetInfos(isEnumString: true))
{
    System.Console.WriteLine($"{p.Key}.{p.Value}"); //{{"901","使用者"},{"902","部門"}}
}
//or
BatchAppEnum.GetValues(); //{"901", "902", "903", "904"}
```

#### 取得SelectList

```csharp
//Enum
SearchData.Add(new SelectListSearchField("ApprovalStatus", DisplayNameFor<ApprovalDataIndexModel>(r => r.ApprovalStatusEnum), selectListService.GetSelectListItems(typeof(SystemEnum.ApprovalStatus)), ""));

//取單一 TextEnum
SearchData.Add(new SelectListSearchField("ApprovalStatus", DisplayNameFor<ApprovalDataIndexModel>(r => r.ApprovalStatusEnum), selectListService.GetTextEnumSelectListItems(typeof(ApprovalStatusEnum)), ""));

//取擴充 TextEnum
SearchData.Add(new SelectListSearchField("ApprovalStatus", DisplayNameFor<ApprovalDataIndexModel>(r => r.ApprovalStatusEnum), selectListService.GetTextEnumSelectListItems(typeof(XXXBatchAppTextEnum), typeof(BatchAppTextEnum)), ""));
```