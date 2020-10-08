using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CrowdControl.Common
{
    public class TimeSpanLongConverter : JsonConverter<TimeSpan>
    {
        public override void WriteJson(JsonWriter writer, TimeSpan value, JsonSerializer serializer)
            => writer.WriteValue((long)value.TotalSeconds);

        public override TimeSpan ReadJson(JsonReader reader, Type objectType, TimeSpan existingValue, bool hasExistingValue, JsonSerializer serializer)
            => TimeSpan.FromSeconds((double)reader.Value);
    }

    public class DateTimeOffsetLongConverter : JsonConverter<DateTimeOffset>
    {
        private static readonly DateTimeOffset EPOCH = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        public override void WriteJson(JsonWriter writer, DateTimeOffset value, JsonSerializer serializer)
            => writer.WriteValue((long)(value.ToUniversalTime()- EPOCH).TotalSeconds);

        public override DateTimeOffset ReadJson(JsonReader reader, Type objectType, DateTimeOffset existingValue, bool hasExistingValue, JsonSerializer serializer)
            => (EPOCH + TimeSpan.FromSeconds((long)reader.Value));
    }

    public class FormulaVariableConverter : JsonConverter<List<IFormulaVariable>>
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, List<IFormulaVariable> value, JsonSerializer serializer)
            => throw new NotImplementedException();

        public override List<IFormulaVariable> ReadJson(JsonReader reader, Type objectType, List<IFormulaVariable> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JArray arr = JArray.Load(reader);
            List<IFormulaVariable> result = new List<IFormulaVariable>();
            foreach (JObject tok in arr.OfType<JObject>())
            {
                result.Add((FormulaVariableType)tok.GetValue("formulaVariableType", StringComparison.InvariantCultureIgnoreCase).Value<byte>() switch
                {
                    FormulaVariableType.Effect => tok.ToObject<Effect>(),
                    FormulaVariableType.EffectRequest => tok.ToObject<EffectRequest>(),
                    FormulaVariableType.InventoryItem => tok.ToObject<InventoryItem>(),
                    FormulaVariableType.RemoteItem => tok.ToObject<RemoteItem>(),
                    FormulaVariableType.FormulaBox => tok.ToObject<FormulaBank.FormulaBox>(),
                    _ => throw new ArgumentOutOfRangeException()
                });
            }
            return result;
        }
    }

    public class NullStringConverter : JsonConverter<string>
    {
        public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue, JsonSerializer serializer)
        {

            if (reader.Value == null) { return null; }
            string text = reader.Value.ToString();
#if NET35
            return string.IsNullOrEmpty(text.Trim()) ? null : text;
#else
            return string.IsNullOrWhiteSpace(text) ? null : text;
#endif
        }

        public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer)
#if NET35
            => writer.WriteValue(string.IsNullOrEmpty(value?.Trim()) ? null : value);
#else
            => writer.WriteValue(string.IsNullOrWhiteSpace(value) ? null : value);
#endif
    }

    /*public class CrowdControlBlockConverter : JsonConverter<CrowdControlBlock>
    {
        public override void WriteJson(JsonWriter writer, CrowdControlBlock value, JsonSerializer serializer)
            => JObject.FromObject(value).WriteTo(writer); //I can't fix this because the maintainer of Newtonsoft.Json is a chump - kat

        public override CrowdControlBlock ReadJson(JsonReader reader, Type objectType, CrowdControlBlock existingValue, bool hasExistingValue, JsonSerializer serializer)
            => CrowdControlBlock.Deserialize(JToken.Load(reader));
    }*/
}
