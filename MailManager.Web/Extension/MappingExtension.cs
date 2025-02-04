using MailManager.Data;
using MailManager.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace MailManager.Web.Extension
{
    public static class MappingExtension
    {
        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            return AutoMapperConfiguration.Mapper.Map<TSource, TDestination>(source);
        }

        public static EmailAccountVM ToModel(this EmailAccount entity)
        {
            var model = entity.MapTo<EmailAccount, EmailAccountVM>();
            return model;
        }
        public static EmailAccount ToEntity(this EmailAccountVM model)
        {
            var entity = model.MapTo<EmailAccountVM, EmailAccount>();
            return entity;
        }

        public static MasterMailVM ToModel(this MasterMail entity)
        {
            var model = entity.MapTo<MasterMail, MasterMailVM>();
            return model;
        }
        public static MasterMail ToEntity(this MasterMailVM model)
        {
            var entity = model.MapTo<MasterMailVM, MasterMail>();
            return entity;
        }
    }
}