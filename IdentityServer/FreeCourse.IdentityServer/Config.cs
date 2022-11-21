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
                new ApiResource("resource_photo_stock"){Scopes={"photo_stock_fullpermission"}},
                //Basket Api için önce aşağıda scope oluşturduk şimdide burada ApiResource ekliyoruz.
                new ApiResource("resource_basket"){Scopes={"basket_fullpermission"}},
                //Discount Api için 
                new ApiResource("resource_discount"){Scopes={"discount_fullpermission"}},
                //Order Api için 
                new ApiResource("resource_order"){Scopes={"order_fullpermission"}},
                //FakePayment Api için 
                new ApiResource("resource_payment"){Scopes={"payment_fullpermission"}},
                //Gateway Api için 
                new ApiResource("resource_gateway"){Scopes={"gateway_fullpermission"}},

                //IdentityServer için hazır yapı kullandık
                new ApiResource(IdentityServerConstants.LocalApi.ScopeName)
            };

        //IdentityResource geriye token döndüğümüzde User ile ilgili hangi bilgileri ekleyeceğimizi belirtiyoruz.
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
                //Basket microservisi için scope oluşturduk.
                new ApiScope("basket_fullpermission","Access for Basket API"),
                //Discount microservisi için scope oluşturduk.
                new ApiScope("discount_fullpermission","Access for Discount API"),
                //Order microservisi için scope oluşturduk.
                new ApiScope("order_fullpermission","Access for Order API"),
                //FakePayment microservisi için scope oluşturduk.
                new ApiScope("payment_fullpermission","Access for Payment API"),
                //Gateway.API için scope oluşturduk.
                new ApiScope("gateway_fullpermission","Access for Gateway API"),
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
                    AllowedScopes={ "catalog_fullpermission", "photo_stock_fullpermission", "gateway_fullpermission", IdentityServerConstants.LocalApi.ScopeName }
                },
                //User için olan tokenımızın clientını oluşturduk. OfflineAccess => Offlineken RefreshToken alabilmek için
                new Client
                {
                    ClientName="Asp.Net Core MVC",
                    ClientId="WebMvcClientForUser",
                    AllowOfflineAccess=true,
                    ClientSecrets={new Secret("secret".Sha256())},
                    AllowedGrantTypes=GrantTypes.ResourceOwnerPassword,
                    AllowedScopes={ "basket_fullpermission", "order_fullpermission", "gateway_fullpermission", IdentityServerConstants.StandardScopes.Email,IdentityServerConstants.StandardScopes.OpenId,IdentityServerConstants.StandardScopes.Profile,IdentityServerConstants.StandardScopes.OfflineAccess, IdentityServerConstants.LocalApi.ScopeName, "roles"},
                    AccessTokenLifetime=1*60*60,
                    RefreshTokenExpiration=TokenExpiration.Absolute,
                    AbsoluteRefreshTokenLifetime=(int)(DateTime.Now.AddDays(60)-DateTime.Now).TotalSeconds,
                    RefreshTokenUsage=TokenUsage.ReUse

                },

                //Token exchange için oluşturduk
                new Client
                {
                    ClientName="Token Exchange Client",
                    ClientId="TokenExhangeClient",
                    ClientSecrets= {new Secret("secret".Sha256())},
                    AllowedGrantTypes= new []{ "urn:ietf:params:oauth:grant-type:token-exchange" },
                    //Hangi Apilere istek yapabileceğini belirtiyoruz. Payment ve Discount için token exchang sistemi olacağı için onları yukarıdaki clientdan silip bu clientın scopelarına ekledik. Bir de kullanıcı id aldık=> openid
                    AllowedScopes={ "discount_fullpermission", "payment_fullpermission", IdentityServerConstants.StandardScopes.OpenId }
                }

            };
    }
}