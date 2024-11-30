using AutoMapper;

namespace api.Helpers
{
    public static class DataHelper
    {
        public static DateTime? DateToUtc(DateTime? date, int timezone, DateTime? time = null)
        {
            if (date == null) return null;
            DateTime? Result = date; ;
            if (time != null)
            {
                Result = date.Value.AddHours(time?.Hour ?? 0).AddMinutes(time?.Minute ?? 0);
            }

            return Result?.AddMinutes(timezone).ToUniversalTime();
        }

        public static TDestination Map<TSource, TDestination>(TSource inputModel)
        {
            var config = new MapperConfiguration(cfg =>
                    cfg.CreateMap<TSource, TDestination>());

            var mapper = new Mapper(config);
            var result = mapper.Map<TSource, TDestination>(inputModel);


            return result;
        }

        public static List<TDestination> MapList<TSource, TDestination>(List<TSource> inputModel)
        {
            var config = new MapperConfiguration(cfg =>
                    cfg.CreateMap<TSource, TDestination>());

            var mapper = new Mapper(config);
            var result = mapper.Map<List<TSource>, List<TDestination>>(inputModel);


            return result;
        }
       public class AdressPoint
        {
            public string Latitude { get; set; } = null!;
            public string Longitude { get; set; } = null!;
            public string Adress { get; set; } = null!;
            public string Name { get; internal set; }
        }
    }
}

