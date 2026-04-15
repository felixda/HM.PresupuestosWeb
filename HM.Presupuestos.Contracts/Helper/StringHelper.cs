namespace HM.Presupuestos.Contratos.Helper
{
    public static class StringHelper
    {
        /// <summary>
        /// Capitalize each word (length>3) in text
        /// </summary>
        /// <param name="text">Text to capitalize</param>
        /// <param name="minLength">Min word length to capitalize</param>
        /// <returns>Text capitalized</returns>
        public static string CapitalizeText(string text, int minLength=3)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            var obWordList = text.Trim().ToLower().Split(' ');
            for (int i = 0; i < obWordList.Length; i++)
            {
                if (obWordList[i].Length > minLength || i==0)
                {
                    var word = obWordList[i].ToLower();
                    obWordList[i] = char.ToUpper(word[0]) + word.Substring(1);
                }
                else
                {
                    obWordList[i] = obWordList[i];
                }
            }
            return string.Join(' ', obWordList);
        }
    }
}
