using static Domain.Enums;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Application.Shared;

public static class Extensions
{
    public static string GetAttributeMemberValue(this Devs value)
    {
        Type type = value.GetType();
        MemberInfo? memberValue = type.GetMember(value.ToString()).FirstOrDefault();

        if (memberValue == null)
            return value.ToString();

        JsonStringEnumMemberNameAttribute? attribute = memberValue
            .GetCustomAttributes(typeof(JsonStringEnumMemberNameAttribute), false)
            .Cast<JsonStringEnumMemberNameAttribute>()
            .FirstOrDefault();

        if (attribute != null)
            return attribute.Name;

        return value.ToString();
    }
}
