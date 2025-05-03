using Humanizer;

namespace JobTrackingAPI.Mappers;

public static class EnumMappers
{
    public static List<EnumItem> EnumToList<T>()
        where T : Enum
    {
        return Enum.GetValues(typeof(T))
            .Cast<T>()
            .Select(e => e.ToEnumItem())
            .ToList();
    }

    public static EnumItem ToEnumItem(this Enum enumValue)
    {
        return new EnumItem
        (
            Convert.ToInt32(enumValue),
            enumValue.Humanize()
        );
    }
}

public record EnumItem(int Id, string Name);
