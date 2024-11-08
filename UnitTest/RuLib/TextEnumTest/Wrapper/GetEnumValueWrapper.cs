using System.Reflection;

namespace CommonTest.ITSEnum.Wrapper
{
    /// <summary>
    /// Wrapper 給自訂義enum class 可以使用GetXXX 方法
    /// </summary>
    /// <typeparam name="TDest">The type of the dest.</typeparam>
    public class GetEnumValueWrapper<TDest>
    {
        /// <summary>
        /// GetDescription
        /// </summary>
        /// <param name="val">enum val</param>
        /// <returns>display string</returns>
        public string GetDescription(object val)
        {
            var method = typeof(TextEnumBase<TDest>).GetMethod("GetDesc", BindingFlags.Public | BindingFlags.Static);
            var generic = method.MakeGenericMethod(typeof(TDest));
            return (string)generic.Invoke(new TextEnumBase<TDest>(), new object[] { val.ToString() });
        }
    }
}


