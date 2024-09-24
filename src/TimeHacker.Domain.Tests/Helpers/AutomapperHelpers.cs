using AutoMapper;

namespace TimeHacker.Tests.Helpers
{
    public static class AutomapperHelpers
    {
        public static MapperConfiguration GetMapperConfiguration()
        {
            var types = AppDomain.CurrentDomain
                                .GetAssemblies()
                                .Where(x => x.FullName!.StartsWith("TimeHacker."))
            .SelectMany(s => s.GetTypes())
                                .Where(p => typeof(Profile).IsAssignableFrom(p));

            return new MapperConfiguration(cfg =>
            {
                foreach (var type in types)
                    cfg.AddProfile(type);
            });
        }
    }
}
