namespace TelegramLanguageTeacher.Core.Configs;

public class DefaultAzureServicesSettings : IAzureServicesSettings
{
    public string AzureTranslatorEndpoint { get; set; } = "https://api.cognitive.microsofttranslator.com/";

    public string FreeDictionaryApiEndpoint { get; set; } = "https://api.dictionaryapi.dev/api/v2/entries/en_US/";

    public string IdiomsDictionaryApiEndpoint { get; set; } = "https://idioms.thefreedictionary.com/";

    public string AzureLocation { get; set; } = "global";

    public string AzureAuthorizationToken { get; set; } = "ecf368aea57a4d40a49cd4a24bbab704";
}