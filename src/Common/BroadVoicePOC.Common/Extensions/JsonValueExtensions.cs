using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace BroadVoicePOC.Common.Extensions
{
    public static class JsonValueExtensions
    {
        public static JsonValueKind GetValueKind(this JsonValue jvalue)
        {
            JsonElement element = jvalue.GetValue<JsonElement>();
            return element.ValueKind;
        }
    }
}
