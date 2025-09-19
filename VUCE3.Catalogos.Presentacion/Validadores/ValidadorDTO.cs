using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace VUCE3.Catalogos.Presentacion.Validadores
{
    [ExcludeFromCodeCoverage]
    public static class ValidadorDto
    {
        public static bool IsAnyNullOrEmpty(IEnumerable<object> myObject)
        {
            var _nullabilityContext = new NullabilityInfoContext();
            foreach (object obj in myObject)
            {
                foreach (PropertyInfo pi in obj.GetType().GetProperties())
                {
                    var nullabilityInfo = _nullabilityContext.Create(pi);
                    if (nullabilityInfo.WriteState != NullabilityState.Nullable && pi.PropertyType == typeof(string))
                    {
                        string? value = (string?)pi.GetValue(obj);

                        if (string.IsNullOrEmpty(value))
                        {
                            return true;
                        }
                    }
                }                
            }

            return false;
        }

    }
}
