using Newtonsoft.Json.Converters;
using proxygen.Models;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace proxygen.Helpers
{
    public class ScryfallConverter : CustomCreationConverter<CardModel>
    {
        public override CardModel Create(Type objectType)
        {

            throw new NotImplementedException();
        }
        //public override CardModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        //{
        //    CardModel instance = new CardModel();

        //    while (reader.Read())
        //    {
        //        var tokenType = reader.TokenType;
        //        var valueSpan = reader.ValueSpan;

        //        switch (tokenType)
        //        {
        //            case JsonTokenType.StartObject:
        //            case JsonTokenType.EndObject:
        //                break;
        //            case JsonTokenType.StartArray:
        //            case JsonTokenType.EndArray:
        //                break;
        //            case JsonTokenType.PropertyName:
        //                break;
        //            case JsonTokenType.String:
        //                string valueString = reader.GetString();
        //                break;
        //            case JsonTokenType.Number:
        //                if (!reader.TryGetInt32(out int valueInteger))
        //                {
        //                    throw new FormatException();
        //                }
        //                break;
        //            case JsonTokenType.True:
        //            case JsonTokenType.False:
        //                bool valueBool = reader.GetBoolean();
        //                break;
        //            case JsonTokenType.Null:
        //                break;
        //            default:
        //                throw new ArgumentException();
        //        }
        //    }

        //    return instance;
        //}
        //public override void Write(Utf8JsonWriter writer, CardModel value, JsonSerializerOptions options)
        //{
        //    throw new NotImplementedException();
        //}
        //public override bool CanConvert(Type typeToConvert)
        //{
        //    return typeToConvert == typeof(CardModel);
        //}
    }
}
