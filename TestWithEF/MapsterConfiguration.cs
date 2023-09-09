using Mapster;
using TestWithEF.Dtos;

namespace TestWithEF;

public static class MapsterConfiguration
{
    public static void AddMapster(this IServiceCollection services)
    {
        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        var applicationAssembly = typeof(BaseDto<,>).Assembly;
        typeAdapterConfig.Scan(applicationAssembly);
    }
}
