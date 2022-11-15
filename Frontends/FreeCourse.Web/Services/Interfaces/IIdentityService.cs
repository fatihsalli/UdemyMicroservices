using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models;
using IdentityModel.Client;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services.Interfaces
{
    public interface IIdentityService
    {
        //IdentityModel kütüphanesini yükledik.
        Task<Response<bool>> SignIn(SignInInput signInInput);

        //IdentityModel içerisinde TokenResponse adında hazır bir sınıfımız var.Herhangi bir parametre almasına gerek yok refresh tokenı cookie üzerinden okuyacağız.
        Task<TokenResponse> GetAccessTokenByRefreshToken();

        //Tokenı silmek için
        Task RevokeRefreshToken();


    }
}
