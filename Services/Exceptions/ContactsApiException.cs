namespace Services.Exceptions
{
    public class ContactsApiException : Exception
    {
        public ContactsApiException(string message) : base(message) { }

        public ContactsApiException(string message, Exception innerException) : base(message, innerException) { }
    }

}