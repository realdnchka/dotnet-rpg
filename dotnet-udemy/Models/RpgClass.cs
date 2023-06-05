using System.Text.Json.Serialization;

namespace dotnet_udemy.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RpgClass
{
    Knight,
    Mage,
    Thief,
    Assassin,
    Cleric
}