using BroadVoicePOC.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BroadVoicePOC.Common.Helpers
{
    public class JSONHelper
    {
        public delegate bool TryTransformDelegate(string propName, JsonValue value, JsonNode parentNode, out JsonValue transformed);

        private static void IterateJArray(string propName, JsonArray jarr, TryTransformDelegate TryTransform)
        {
            List<Tuple<int, JsonValue>> changes = new List<Tuple<int, JsonValue>>();
            int count = jarr.Count;
            for (int i = 0; i < count; i++)
            {
                JsonNode token = jarr[i];
                if (token is JsonArray)
                {
                    IterateJArray(null, token as JsonArray, TryTransform);
                }
                else if (token is JsonObject)
                {
                    IterateJObject(token as JsonObject, TryTransform);
                }
                else if (token is JsonValue)
                {
                    JsonValue jvalue = token as JsonValue;
                    JsonValue newJValue;
                    if (TryTransform(null, jvalue, jarr, out newJValue))
                        changes.Add(new Tuple<int, JsonValue>(i, newJValue));
                }
            }

            // apply changes
            foreach (Tuple<int, JsonValue> change in changes)
                jarr[change.Item1] = change.Item2;
        }

        public static void IterateJObject(JsonObject jobj, TryTransformDelegate TryTransform)
        {
            List<Tuple<string, JsonValue>> changes = new List<Tuple<string, JsonValue>>();
            foreach (KeyValuePair<string, JsonNode> prop in jobj)
            {
                if (prop.Value is JsonArray)
                {
                    IterateJArray(prop.Key, prop.Value as JsonArray, TryTransform);
                }
                else if (prop.Value is JsonObject)
                {
                    IterateJObject(prop.Value as JsonObject, TryTransform);
                }
                else if (prop.Value is JsonValue)
                {
                    JsonValue jvalue = prop.Value as JsonValue;
                    JsonValue newJValue;
                    if (TryTransform(prop.Key, jvalue, jobj, out newJValue))
                        changes.Add(new Tuple<string, JsonValue>(prop.Key, newJValue));
                }
            }

            // apply changes
            foreach (Tuple<string, JsonValue> change in changes)
                jobj[change.Item1] = change.Item2;
        }

        public static string TransformJSON(string json, TryTransformDelegate TryTransform)
        {
            JsonNode jtoken = JsonNode.Parse(json);
            if (jtoken is JsonArray)
                IterateJArray(null, jtoken as JsonArray, TryTransform);
            else if (jtoken is JsonObject)
                IterateJObject(jtoken as JsonObject, TryTransform);
            else
                throw new Exception("Invalid JSON");
            return jtoken.ToString();
        }

        public static bool IsJSON(string strInput)
        {
            if (String.IsNullOrWhiteSpace(strInput))
                return false;
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || // object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) // array
            {
                try
                {
                    JsonNode obj = JsonNode.Parse(strInput);
                    return true;
                }
                catch (Exception) { return false; }
            }
            else return false;
        }

        public static string TruncateJSON(string json, int maxLength)
        {
            if (IsJSON(json))
            {
                return TransformJSON(json, (string propName, JsonValue jvalue, JsonNode parent, out JsonValue transformed) =>
                {
                    if (jvalue.GetValueKind() == JsonValueKind.String)
                    {
                        string value = jvalue.GetValue<string>();
                        if (!String.IsNullOrEmpty(value) && value.Length > maxLength)
                        {
                            transformed = JsonValue.Create(value.Substring(0, maxLength) + "...");
                            return true;
                        }
                    }
                    transformed = null;
                    return false;
                });
            }
            else return json;
        }

        public static string TruncateAndRedactJSON(string json, int? maxLength, IEnumerable<string> redactFields)
        {
            if (IsJSON(json))
            {
                return JSONHelper.TransformJSON(json, (string propName, JsonValue jvalue, JsonNode parent, out JsonValue transformed) =>
                {
                    if (jvalue.GetValueKind() == JsonValueKind.String)
                    {
                        // redact
                        bool redact = !String.IsNullOrEmpty(propName) && redactFields != null && redactFields.Any(x => propName.ToLower().Like(x.ToLower()));
                        if (redact)
                        {
                            transformed = JsonValue.Create("[Redacted]");
                            return true;
                        }

                        // truncate
                        string value = jvalue.GetValue<string>();
                        if (!String.IsNullOrEmpty(value) && value.Length > maxLength.Value)
                        {
                            transformed = JsonValue.Create(value.Substring(0, maxLength.Value) + "...");
                            return true;
                        }
                    }
                    transformed = null;
                    return false;
                });
            }
            else return json;
        }
    }
}
