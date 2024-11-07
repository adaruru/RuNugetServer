using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Web.WebPages.Html;

namespace Common.TextEnum.BaseTextEnum;

/// <summary>
/// TextEnumBase<T>基底 不可直接繼承使用
/// </summary>
public class TextEnumBase
{
    public static List<SelectListItem> GetTextEnumServiceList(string value = null)
    {
        var item = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && t.Namespace == "Common.TextEnum").ToList()
                .Select(t => new SelectListItem()
                {
                    Text = t.Name,
                    Value = t.Name,
                    Selected = value != null && t.Name == value
                })
                .ToList();
        return item;
    }

    internal static List<SelectListItem> GetTextEnumSelectListItems(Type type, Func<string, bool> funcValue = null)
    {
        var t1Pros = type.GetProperties(BindingFlags.Public | BindingFlags.Static);
        if (type.BaseType != null)
        {
            var t2Pros = type.BaseType.GetProperties(BindingFlags.Public | BindingFlags.Static);
            var pros = new PropertyInfo[t1Pros.Length + t2Pros.Length];
            t1Pros.CopyTo(pros, 0);
            t2Pros.CopyTo(pros, t1Pros.Length);
            return MapPropertyInfoToSelectList(pros, funcValue);
        }
        return MapPropertyInfoToSelectList(t1Pros, funcValue);
    }

    internal static List<SelectListItem> MapPropertyInfoToSelectList(PropertyInfo[] pros, Func<string, bool> funcValue = null)
    {
        if (funcValue == null)
        {
            funcValue = x => false;
        }
        var list = new List<SelectListItem>();
        for (int i = 0; i < pros.Length; i++)
        {
            var key = pros[i].GetValue(null).ToString();
            var value = string.Empty;
            try
            {
                var att = pros[i].GetCustomAttributes(typeof(DisplayAttribute), true)[0];
                value = ((DisplayAttribute)att).Name;
            }
            catch (Exception ex)
            {
                value = $"{key} Enum 取 Display Name 錯誤";
            }
            list.Add(new SelectListItem()
            {
                Text = value,
                Value = key,
                Selected = funcValue(value)
            });
        }
        return list;
    }

    /// <summary>
    /// 取得TypeEnum所有內容 
    /// key Enum 值 int
    /// value Enum 描述 string 中文 (DisplayAttribute Name 值)
    /// </summary>
    /// <returns></returns>
    internal static Dictionary<int, string> GetInfos(PropertyInfo[] pros)
    {
        var result = new Dictionary<int, string>();
        for (int i = 0; i < pros.Length; i++)
        {
            int num;
            var value = string.Empty;
            if (int.TryParse(pros[i].GetValue(null).ToString(), out num))
            {
                var key = pros[i].GetValue(null).ToString();
                try
                {
                    var att = pros[i].GetCustomAttributes(typeof(DisplayAttribute), true)[0];
                    value = ((DisplayAttribute)att).Name;
                }
                catch (Exception ex)
                {
                    value = $"{key} Enum 取 Display Name 錯誤";
                }
                result.Add(num, value);
            }
        }
        if (result.Count() == 0)
        {
            Console.WriteLine("檢查 Enum 是否非 int");
        }
        return result;
    }

    /// <summary>
    /// 取得TextEnum所有內容 
    /// key Enum 值 string
    /// value Enum 描述 string 中文 (DisplayAttribute Name 值)
    /// </summary>
    /// <param name="isEnumString">是否 Enum 宣告值型別 string </param>
    /// <returns></returns>
    internal static Dictionary<string, string> GetInfos(bool isEnumString, PropertyInfo[] pros)
    {
        if (isEnumString)
        {
            var result = new Dictionary<string, string>();
            for (int i = 0; i < pros.Length; i++)
            {
                var key = pros[i].GetValue(null).ToString();
                var value = string.Empty;
                try
                {
                    var att = pros[i].GetCustomAttributes(typeof(DisplayAttribute), true)[0];
                    value = ((DisplayAttribute)att).Name;
                }
                catch (Exception ex)
                {
                    value = $"{key} Enum 取 Display Name 錯誤";
                }
                result.Add(key, value);
            }
            return result;
        }
        else
        {
            return GetInfos(pros).ToDictionary(entry => entry.Key.ToString(),
                                               entry => entry.Value);
        }
    }

    /// <summary>
    /// 取得TextEnum所有值
    /// </summary>
    /// <returns></returns>
    internal static List<int> GetValues(PropertyInfo[] pros)
    {
        var result = pros.Select(p =>
        {
            int num;
            int.TryParse(p.GetValue(null).ToString(), out num);
            return num;
        }).ToList();
        return result;
    }

    /// <summary>
    /// 取得 TextEnum 所有值
    /// </summary>
    /// <param name="isEnumString">是否 Enum 宣告值型別 string </param>
    /// <returns></returns>
    internal static List<string> GetValues(bool isEnumString, PropertyInfo[] pros)
    {
        if (isEnumString)
        {
            var result = pros.Select(p => p.GetValue(null).ToString()).ToList();
            return result;
        }
        else
        {
            return GetValues(pros).ConvertAll(x => x.ToString());
        }
    }

    /// <summary>
    /// 檢查值存在
    /// </summary>
    /// <returns></returns>
    internal static bool IsValueExist(object value, PropertyInfo[] pros)
    {
        var pro = pros.FirstOrDefault(p => p.GetValue(null).ToString() == value.ToString());
        return pro != null;
    }

    /// <summary>
    /// 取得 TextEnum 屬性名稱 英文
    /// </summary>
    /// <returns></returns>
    internal static string GetName(object value, PropertyInfo[] pros)
    {
        var pro = pros?.Where(p => p.GetValue(null).ToString() == value.ToString());
        if (pro.Count() != 1 || pro == null)
        {
            return $"{value} Enum 值重複或不存在";
        }
        var result = pro.FirstOrDefault().Name;
        return result;
    }

    /// <summary>
    /// 取得 TextEnum 屬性顯示 中文
    /// DisplayAttribute Name 值
    /// </summary>
    /// <returns></returns>
    internal static string GetDescription(object value, PropertyInfo[] pros)
    {
        var att = new object();
        var pro = pros?.Where(p => p.GetValue(null).ToString() == value.ToString());
        if (pro.Count() != 1 || pro == null)
        {
            return $"{value} Enum 值重複或不存在";
        }
        try
        {
            att = pro.FirstOrDefault().GetCustomAttributes(typeof(DisplayAttribute), true)[0];
        }
        catch (Exception ex)
        {

            return $"{value} Enum 取 Display Name 錯誤";
        }

        var description = (DisplayAttribute)att;
        var result = description.Name;
        return result;
    }
}

/// <summary>
/// Production TextEnum 物件基底
/// 不可直接繼承使用
/// </summary>
/// <typeparam name="T">Production 的 TextEnum</typeparam>
public class BaseEnum<T>
{
    private static PropertyInfo[] pros
    {
        get
        {
            var t1Pros = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Static);
            if (typeof(T).BaseType is not BaseEnum<T>)
            {
                var t2Pros = typeof(T).BaseType.GetProperties(BindingFlags.Public | BindingFlags.Static);
                var Pros = new PropertyInfo[t1Pros.Length + t2Pros.Length];
                t1Pros.CopyTo(Pros, 0);
                t2Pros.CopyTo(Pros, t1Pros.Length);
                return Pros;
            }
            return t1Pros;
        }
    }

    public static List<SelectListItem> GetTextEnumSelectListItems(Type type, Func<string, bool> funcValue = null)
    {
        return TextEnumBase.GetTextEnumSelectListItems(type, funcValue);
    }

    /// <summary>
    /// 取得TextEnum所有內容 
    /// key Enum 值 int
    /// value Enum 描述 string 中文 (DisplayAttribute Name 值)
    /// </summary>
    /// <returns></returns>
    public static Dictionary<int, string> GetInfos()
    {
        return TextEnumBase.GetInfos(pros);
    }

    /// <summary>
    /// 取得TextEnum所有內容 
    /// key Enum 值 string
    /// value Enum 描述 string 中文 (DisplayAttribute Name 值)
    /// </summary>
    /// <param name="isEnumString">是否 Enum 宣告值型別 string </param>
    /// <returns></returns>
    public static Dictionary<string, string> GetInfos(bool isEnumString)
    {
        return TextEnumBase.GetInfos(isEnumString, pros);
    }

    /// <summary>
    /// 取得TextEnum所有值
    /// </summary>
    /// <returns></returns>
    public static List<int> GetValues()
    {
        return TextEnumBase.GetValues(pros);
    }

    /// <summary>
    /// 取得TextEnum所有值
    /// </summary>
    /// <param name="isEnumString">是否 Enum 宣告值型別 string </param>
    /// <returns></returns>
    public static List<string> GetValues(bool isEnumString)
    {
        return TextEnumBase.GetValues(isEnumString, pros);
    }

    /// <summary>
    /// 檢查值存在
    /// </summary>
    /// <returns></returns>
    public static bool IsValueExist(object value)
    {
        return TextEnumBase.IsValueExist(value, pros);
    }

    /// <summary>
    /// 取得 TextEnum 屬性名稱 英文
    /// </summary>
    /// <returns></returns>
    public static string GetName(object value)
    {
        return TextEnumBase.GetName(value, pros);
    }

    /// <summary>
    /// 取得 TextEnum 屬性顯示 中文
    /// DisplayAttribute Name 值
    /// </summary>
    /// <returns></returns>
    public static string GetDescription(object value)
    {
        if (value == null)
            return "";
        return TextEnumBase.GetDescription(value, pros);
    }
}