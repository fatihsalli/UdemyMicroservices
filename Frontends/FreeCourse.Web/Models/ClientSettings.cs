namespace FreeCourse.Web.Models
{
    public class ClientSettings
    {
        //2 adet olduğu için options patternde bu şekilde bir class daha oluşturduk.
        public Client WebClient { get; set; }
        public Client WebClientForUser { get; set; }

    }

    public class Client
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

    }


}
