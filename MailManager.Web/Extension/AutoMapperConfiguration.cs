using AutoMapper;
using MailManager.Data;
using MailManager.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MailManager.Web.Extension
{
    public static class AutoMapperConfiguration
    {
        private static IMapper _mapper;

        public static void InitMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {

                cfg.CreateMap<EmailAccount, EmailAccountVM>().ReverseMap();
                cfg.CreateMap<MasterMail, MasterMailVM>().ReverseMap();
                
            });
            _mapper = config.CreateMapper();
        }
        /// <summary>
        /// Mapper
        /// </summary>
        public static IMapper Mapper => _mapper;

    }
}