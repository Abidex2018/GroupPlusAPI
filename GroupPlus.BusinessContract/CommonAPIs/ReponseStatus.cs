namespace GroupPlus.BusinessContract.CommonAPIs
{
    public class APIResponseStatus
    {
        public bool IsSuccessful;
        public string CustomToken;
        public string CustomSetting;
        public APIResponseMessage Message;
    }

    public class APIResponseMessage
    {
        public string FriendlyMessage;
        public string TechnicalMessage;
        public string MessageId;
        public string ShortErrorMessage;
    }
}
