namespace FreeCourse.Shared.Services
{
    //Token içinden userId yi alarak Basket.Apı ya da diğer microservislerde kullanmamız için shared tarafında oluşturduk. Her bir microservice için tek tek oluşturmamak için.
    public interface ISharedIdentityService
    {
        public string GetUserId { get; }


    }
}
