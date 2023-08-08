namespace Services.Models
{
    public class ExternalContactsApiOptions
    {
        public string BaseUrl { get; set;  }
        public int PeriodSecondsRateLimit { get; set;  }
        public int RateLimit { get; set;  }
    }
}