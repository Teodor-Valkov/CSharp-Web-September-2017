namespace GameStore.App.Infrastructure.Mapping
{
    using AutoMapper;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class AutoMapperConfiguration
    {
        public static void Initialize()
        {
            Mapper.Initialize(config =>
            {
                Type[] allTypes = Assembly
                    .GetEntryAssembly()
                    .GetTypes();

                var mappedTypes = allTypes
                    .Where(t => t
                        .GetInterfaces()
                        .Where(i => i.IsGenericType)
                        .Any(i => i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
                    .Select(t => new
                    {
                        Destination = t,
                        Source = t.GetInterfaces()
                            .Where(i => i.IsGenericType
                                && i.GetGenericTypeDefinition() == typeof(IMapFrom<>))
                            .SelectMany(i => i.GetGenericArguments())
                            .First()
                    })
                    .ToList();

                foreach (var type in mappedTypes)
                {
                    config.CreateMap(type.Source, type.Destination);
                }

                IList<IHaveCustomMapping> customMappedTypes = allTypes
                    .Where(t => t.IsClass
                        && typeof(IHaveCustomMapping).IsAssignableFrom(t))
                    .Select(t => (IHaveCustomMapping)Activator.CreateInstance(t))
                    .ToList();

                foreach (IHaveCustomMapping type in customMappedTypes)
                {
                    type.Configure(config);
                }
            });
        }
    }
}