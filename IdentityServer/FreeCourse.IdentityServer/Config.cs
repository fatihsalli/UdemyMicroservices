// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;

namespace FreeCourse.IdentityServer
{
    public static class Config
    {
        //Auidencelar için ApiResources oluşturduk
        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource("resource_catalog"){Scopes={"catalog_fullpermission"}},
                new ApiResource("photo_stock_catalog"){Scopes={"photo_stock_fullpermission"}},
                //IdentityServer için hazır yapı kullandık
                new ApiResource(IdentityServerConstants.LocalApi.ScopeName)
            };

        //IdentityResource geriye token doldurduğumuzda User ile ilgili hangi bilgileri ekleyeceğimizi belirtiyoruz.
        public static IEnumerable<IdentityResource> IdentityResources =>
                   new IdentityResource[]
                   {
                       new IdentityResources.Email(),
                       //OpenId mutlaka olmaldır.Bunu kullanmaz isek OpenId protolüne aykırı hareket etmiş oluruz.
                       new IdentityResources.OpenId(),
                       new IdentityResources.Profile(),
                       //Hazır olmadığı için kendimiz ekledik.Kendimiz yazdığımız için Claim'i de eklememiz gerekiyor. Yukarıdakiler için hazır geliyor.
                       new IdentityResource(){Name="roles",DisplayName="Roles",Description="User Roles",UserClaims=new []{"role"} }
                   };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                //Cataglog ve Photostock için oluşturduk
                new ApiScope("catalog_fullpermission","Access for Catalog API"),
                new ApiScope("photo_stock_fullpermission","Access for Photostock API"),
                //IdentityServer ile gelen "IdentityServerConstants" kullandık. Identity serverın kendisine yapılan istekler için tanımladık.
                new ApiScope(IdentityServerConstants.LocalApi.ScopeName)
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientName="Asp.Net Core MVC",
                    ClientId="WebMvcClient",
                    ClientSecrets={new Secret("secret".Sha256())},
                    AllowedGrantTypes=GrantTypes.ClientCredentials,
                    //Hangi Apilere istek yapabileceğini belirtiyoruz.
                    AllowedScopes={ "catalog_fullpermission", "photo_stock_fullpermission",IdentityServerConstants.LocalApi.ScopeName }
                },
                //User için olan tokenımızın clientını oluşturduk. OfflineAccess => Offlineken RefreshToken alabilmek için
                new Client
                {
                    ClientName="Asp.Net Core MVC",
                    ClientId="WebMvcClientForUser",
                    AllowOfflineAccess=true,
                    ClientSecrets={new Secret("secret".Sha256())},
                    AllowedGrantTypes=GrantTypes.ResourceOwnerPassword,
                    AllowedScopes={IdentityServerConstants.StandardScopes.Email,IdentityServerConstants.StandardScopes.OpenId,IdentityServerConstants.StandardScopes.Profile,IdentityServerConstants.StandardScopes.OfflineAccess, IdentityServerConstants.LocalApi.ScopeName, "roles"},
                    AccessTokenLifetime=1*60*60,
                    RefreshTokenExpiration=TokenExpiration.Absolute,
                    AbsoluteRefreshTokenLifetime=(int)(DateTime.Now.AddDays(60)-DateTime.Now).TotalSeconds,
                    RefreshTokenUsage=TokenUsage.ReUse

                }
            };
    }
}