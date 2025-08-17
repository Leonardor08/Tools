using System.Text.Json.Serialization;

namespace Domain;

public static class Enums
{
    public enum Devs {

        [JsonStringEnumMemberName("Fernando Bohórquez Ascanio")]
        Fernando,

        [JsonStringEnumMemberName("Andres Felipe Naranjo Grisales")]
        Felipe,

        [JsonStringEnumMemberName("Jhoan Sebastian Villalba Morales")]
        Sebastian
    };
}
