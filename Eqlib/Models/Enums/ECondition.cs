using System.Text.Json.Serialization;

namespace EqlibApi.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ECondition
    {
        Broken,
        Poor,
        Fair,
        Good,
        Excellent
    }
}
