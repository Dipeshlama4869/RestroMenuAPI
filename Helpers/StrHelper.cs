namespace RestroMenu.Helpers
{
    public static class StrHelper
    {
        public static string? NullIfEmpty(string? value)
        {
            return string.IsNullOrEmpty(value) ? null : value;
        }
    }
}
