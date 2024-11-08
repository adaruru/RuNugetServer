using RuLib.ValueEnum;
using System.Reflection;

namespace RuLib.ValueEnum
{
    /// <summary>
    /// Wrapper 給自訂義enum class 可以使用GetXXX 方法
    /// </summary>
    /// <typeparam name="TDest">The type of the dest.</typeparam>
    public class ValueEnumWrapper<TDest>
    {
        /// <summary>
        /// GetDescription
        /// </summary>
        /// <param name="val">enum val</param>
        /// <returns>display string</returns>
        public string GetCustom(object val)
        {
            var method = typeof(TDest).GetMethod("GetCustom", BindingFlags.Public | BindingFlags.Static);
            var result = method?.Invoke(null, null) as string;//static 不需要物件也不需要參數
            result = string.IsNullOrEmpty(result) ? "找不到 GetCustom 方法異常" : result;
            return result;
        }
    }
}


