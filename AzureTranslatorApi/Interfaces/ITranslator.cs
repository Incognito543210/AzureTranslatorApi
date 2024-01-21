namespace AzureTranslatorApi.Interfaces
{
    public interface ITranslator
    {

         Task<bool> TranslateAsync(string inLanguage, string outLanguage);







    }
}
