﻿using Michael.Net.Exceptions;
using System.Text.Json;

namespace Michael.Net.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<T> FromJson<T>(this HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStreamAsync();
            var @object = await JsonSerializer.DeserializeAsync<T>(content, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            return @object ?? throw new MichaelNetException("Deserialization from JSON failed. Result is null.");
        }
    }
}
