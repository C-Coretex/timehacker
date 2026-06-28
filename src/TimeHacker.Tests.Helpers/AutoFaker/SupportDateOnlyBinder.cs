using AutoBogus;

namespace TimeHacker.Tests.Helpers.AutoFaker;

public class SupportDateOnlyBinder : AutoBinder
{
    public override TType CreateInstance<TType>(AutoGenerateContext context)
    {
        if(CreateDateOrTimeOnly(context, out TType? value) && value is not null)
            return (TType)(object)value;

        return base.CreateInstance<TType>(context);
    }

    /// <returns>true if the type is supported, false otherwise</returns>
    public static bool CreateDateOrTimeOnly<TType>(AutoGenerateContext context, out TType? value)
    {
        Type type = typeof(TType);
        if (type == typeof(DateOnly))
        {
            value = (TType)(object)DateOnly.FromDateTime(DateTime.Now);
            return true;
        }
        if (type == typeof(TimeOnly))
        {
            value = (TType)(object)TimeOnly.FromDateTime(DateTime.Now);
            return true;
        }

        value = default;
        return false;
    }
}
