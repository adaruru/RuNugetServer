
using System.Web.WebPages.Html;

namespace RuLib.ValueEnum;

public class ValueEnumSelectList
{
    /// <summary>
    /// production ValueEnum 下拉選單(無預設值)
    /// </summary>
    /// <param name="type">處理來自 Common.ValueEnum 的下拉選單</param>
    /// <returns></returns>
    public List<SelectListItem> GetValueEnumSelectListItems(Type type)
    {
        return GetValueEnumSelectListItems(type, x => false);
    }

    /// <summary>
    /// production ValueEnum 下拉選單
    /// </summary>
    /// <param name="type">處理來自 Common.ValueEnum 的下拉選單</param>
    /// <param name="value">預設值</param>
    /// <returns></returns>
    public List<SelectListItem> GetValueEnumSelectListItems(Type type, string value)
    {
        return GetValueEnumSelectListItems(type, x => x == value);
    }

    public List<SelectListItem> GetValueEnumSelectListItems(Type type, Func<string, bool> funcValue = null)
    {
        return ValueEnumBase<Type>.GetValueEnumSelectListItems(type, funcValue);
    }
}