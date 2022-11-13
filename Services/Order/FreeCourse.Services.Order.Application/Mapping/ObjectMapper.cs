using AutoMapper;
using System;

namespace FreeCourse.Services.Order.Application.Mapping
{
    public static class ObjectMapper
    {
        //Lazy loading-sadece istenildiği anda nesne türetmek için
        private static readonly Lazy<IMapper> lazy = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CustomMapping>();
            });

            return config.CreateMapper();
        });

        //Mapper'ı çağırana kadar üstteki kodlar çalışmayacak
        public static IMapper Mapper => lazy.Value;

    }
}
