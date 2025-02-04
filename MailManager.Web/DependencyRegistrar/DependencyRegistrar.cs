using Autofac;
using Autofac.Integration.Mvc;
using MailManager.Service.IService;
using MailManager.Service.Repository;
using MailManager.Service.Service;
using MailManager.Service.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace MailManager.Web.DependencyRegistrar
{
    public class DependencyRegistrar
    {
        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();
            //controller
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            builder.RegisterType<UserService>().As<IUserService>();
            builder.RegisterType<EmailAccountService>().As<IEmailAccountService>();
            builder.RegisterType<MasterMailService>().As<IMasterMailService>();
            
            var container = builder.Build();
            return container;
        }
    }
}