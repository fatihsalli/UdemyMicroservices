namespace FreeCourse.Services.Basket.Settings
{
    //Options pattern uyguladık startup tarafında appsettings'den aldığımız bilgiler ile bu classı dolduracağız.
    public class RedisSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }



    }
}
