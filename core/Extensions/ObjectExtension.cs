namespace dotnet.redux.Extensions
{
    internal static class ObjectExtension
    {
        public static bool IsDefault<T>(this T value) where T : struct
        {
            var isDefault = value.Equals(default(T));

            return isDefault;
        }
    }
}