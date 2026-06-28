using AutoBogus;
using System.Reflection;

namespace TimeHacker.Tests.Helpers.AutoFaker;

public class AggregateBinder : AutoBinder
{
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
    public override void PopulateInstance<TType>(object instance, AutoGenerateContext context, IEnumerable<MemberInfo> members = null)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    {
        var filteredMembers = IgnoreNavigationPropertiesBinder.FilterNavigationProperties<TType>(instance, context, members);
        base.PopulateInstance<TType>(instance, context, filteredMembers);
    }

    public override TType CreateInstance<TType>(AutoGenerateContext context)
    {
        if (SupportDateOnlyBinder.CreateDateOrTimeOnly(context, out TType? value) && value is not null)
            return (TType)(object)value;

        return base.CreateInstance<TType>(context);
    }
}