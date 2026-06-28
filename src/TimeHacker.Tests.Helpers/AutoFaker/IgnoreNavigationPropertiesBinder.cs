using AutoBogus;
using System.Collections;
using System.Reflection;

namespace TimeHacker.Tests.Helpers.AutoFaker;

public class IgnoreNavigationPropertiesBinder: AutoBinder
{
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
    public override void PopulateInstance<TType>(object instance, AutoGenerateContext context, IEnumerable<MemberInfo> members = null)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    {
        ArgumentNullException.ThrowIfNull(context);

        // Fallback to type properties if members list is null
        var incomingMembers = members?.ToArray() ?? context.GenerateType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var virtualPropertyNames = incomingMembers.Where(member => member is PropertyInfo property 
            && property.GetGetMethod() is { } accessor && accessor.IsVirtual && !accessor.IsFinal).ToHashSet();

        var filteredMembers = incomingMembers.Except(virtualPropertyNames)
            .Where(member => !virtualPropertyNames.Any(name => name.Name + "Id" == member.Name)).ToList(); // Usually if we have an Id for virtual property, its name would be {virtualPropertyName} + "Id"

        // Pass only the non-navigation properties back to AutoBogus to populate
        base.PopulateInstance<TType>(instance, context, filteredMembers);
    }
}
