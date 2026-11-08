using MassTransit;

namespace Inventory.Service.Infrastructure.Messaging.ExchangeName;

public sealed class KebabCaseExchangeNameFormatter : IEntityNameFormatter
{
    public string FormatEntityName<T>()
    {
        var name = typeof(T).Name;

        if (name.EndsWith("Event"))
        {
            name = name[..^"Event".Length];
        }

        return ToKebabCase(name);
    }

    private static string ToKebabCase(string value)
    {
        return string.Concat(
            value.Select((c, i) =>
                i > 0 && char.IsUpper(c)
                    ? "-" + char.ToLowerInvariant(c)
                    : char.ToLowerInvariant(c).ToString()));
    }
}