﻿using System;
using AuthorizationService.Handlers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace AuthorizationService.Extensions
{
    public static class AuthenticationExtensions
    {
        public static AuthenticationBuilder AddBasicAuthentication<T>(this AuthenticationBuilder builder, Action<Models.AuthenticationOptions> options)
            where T : class, Services.IAuthenticationService
        {
            //builder.Services.AddHttpClient<Services.IAuthenticationService, T>();
            return builder.AddScheme<Models.AuthenticationOptions, BasicAuthenticationHandler>(Enums.AuthenticationSchemes.Basic, options);
        }

        public static AuthenticationBuilder AddHMACAuthentication<T>(this AuthenticationBuilder builder, Action<Models.AuthenticationOptions> options)
            where T : class, Services.IAuthenticationService
        {
            //builder.Services.AddHttpClient<Services.IAuthenticationService, T>();
            return builder.AddScheme<Models.AuthenticationOptions, HMACAuthenticationHandler>(Enums.AuthenticationSchemes.HMAC, options);
        }

        public static AuthenticationBuilder AddBeareruthentication<T>(this AuthenticationBuilder builder, Action<Models.AuthenticationOptions> options)
            where T : class, Services.IAuthenticationService
        {
            //builder.Services.AddHttpClient<Services.IAuthenticationService, T>();
            return builder.AddScheme<Models.AuthenticationOptions, BearerAuthenticationHandler>(Enums.AuthenticationSchemes.Bearer, options);
        }
    }
}

