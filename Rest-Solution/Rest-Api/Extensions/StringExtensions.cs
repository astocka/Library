namespace Rest_Api.Extensions
{
    /// <summary>
    ///     Klasa z metodami rozszrzonymi dla klasy string.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        ///     Zwraca tekst z pierwszą dużą literą.
        /// </summary>
        /// <param name="str">Tekst.</param>
        /// <returns>Tekst z pierwszą dużą literą.</returns>
        public static string ToUpperFirstLetter(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }
            return $"{char.ToUpper(str[0])}{str.Substring(1)}";
        }
    }
}