// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdSNet
{
    public static class Config
    {
      public static IEnumerable<IdentityResource> Ids =>
        new IdentityResource[]
        {
          new IdentityResources.OpenId(),
          new IdentityResources.Profile(),
        };


      public static IEnumerable<ApiResource> Apis =>
        new ApiResource[]
        {
          new ApiResource("mainApp-api", "My API #1")
        };


      public static IEnumerable<Client> Clients =>
        new Client[]
        {
          // SPA client using code flow + pkce
          new Client
          {
            ClientId = "mainApp-api",
            ClientName = "SPA Client",
            ClientUri = "http://identityserver.io",

            AllowedGrantTypes = GrantTypes.Code,
            RequirePkce = true,
            RequireClientSecret = false,

            RedirectUris = {"http://localhost:3214/signin-callback", 
              "http://localhost:3214/assets/silent-callback.html"},

            PostLogoutRedirectUris = {"http://localhost:3214/signout-callback" },
            AllowedCorsOrigins = { "http://localhost:3214" },

            AllowedScopes = { "openid", "profile", "mainApp-api" },
            AccessTokenLifetime = 600

          }
                

        };
    }
}