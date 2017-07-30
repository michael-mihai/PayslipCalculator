namespace Domain.Helpers
{
    public class DomainConstants
    {
        public const int LOG_EVENT_VALIDATE_REQUEST = 1000;

        public const string EXCEPTION_MISSING_TAX_SETTINGS = "Missing tax settings for required date";

        public const string EXCEPTION_MISSING_TAX_BRACKET = "Invalid tax brackets settings! The configuration is not covering all possible income values.";
    }
}
