using System.Text.Json.Serialization;

namespace EqlibApi.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ECondition
    {
        Broken = 0,
        Poor = 1,
        Fair = 2,
        Good = 3,
        Excellent = 4,
    }
}
