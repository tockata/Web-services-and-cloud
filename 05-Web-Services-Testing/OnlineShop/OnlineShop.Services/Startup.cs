﻿using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(OnlineShop.Services.Startup))]

namespace OnlineShop.Services
{
    using System.Data.Entity;
    using System.Reflection;
    using System.Web.Http;

    using Ninject;
    using Ninject.Web.Common.OwinHost;
    using Ninject.Web.WebApi.OwinHost;

    using OnlineShop.Data;
    using OnlineShop.Data.Contracts;
    using OnlineShop.Services.Infrastructure;

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            this.ConfigureAuth(app);

            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            kernel.Bind<IUserIdProvider>().To<AspNetUserIdProvider>();
            kernel.Bind<IOnlineShopData>().To<OnlineShopData>();
            kernel.Bind<DbContext>().To<OnlineShopContext>();

            var httpConfig = new HttpConfiguration();
            WebApiConfig.Register(httpConfig);
            app.UseNinjectMiddleware(() => kernel).UseNinjectWebApi(httpConfig);
        }
    }
}
