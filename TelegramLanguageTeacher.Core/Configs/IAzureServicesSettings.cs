namespace TelegramLanguageTeacher.Core.Configs
{
    public interface IAzureServicesSettings
    {
        string AzureTranslatorEndpoint { get; set; }

        string FreeDictionaryApiEndpoint { get; set; }

        string IdiomsDictionaryApiEndpoint { get; set; }

        string AzureLocation { get; set; }

        string AzureAuthorizationToken { get; set; }
    }
}
