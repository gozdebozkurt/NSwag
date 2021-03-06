﻿//-----------------------------------------------------------------------
// <copyright file="SwaggerResponse.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;
using NJsonSchema;

namespace NSwag
{
    /// <summary>The Swagger response.</summary>
    public class SwaggerResponse : JsonExtensionObject
    {
        /// <summary>Gets the parent <see cref="SwaggerOperation"/>.</summary>
        [JsonIgnore]
        public SwaggerOperation Parent { get; internal set; }

        /// <summary>Gets or sets the response's description.</summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; } = "";

        /// <summary>Gets or sets the response schema.</summary>
        [JsonProperty(PropertyName = "schema", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public JsonSchema4 Schema { get; set; }

        /// <summary>Gets or sets the headers.</summary>
        [JsonProperty(PropertyName = "headers", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public SwaggerHeaders Headers { get; set; }

        /// <summary>Sets a value indicating whether the response can be null (use IsNullable() to get a parameter's nullability).</summary>
        /// <remarks>The Swagger spec does not support null in schemas, see https://github.com/OAI/OpenAPI-Specification/issues/229 </remarks>
        [JsonProperty(PropertyName = "x-nullable", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool? IsNullableRaw { internal get; set; }

        /// <summary>Gets the actual non-nullable response schema (either oneOf schema or the actual schema).</summary>
        [JsonIgnore]
        public JsonSchema4 ActualResponseSchema => GetActualResponseSchema();

        /// <summary>Gets or sets the expected child schemas of the base schema (can be used for generating enhanced typings/documentation).</summary>
        [JsonProperty(PropertyName = "x-expectedSchemas", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public ICollection<JsonExpectedSchema> ExpectedSchemas { get; set; }

        /// <summary>Determines whether the specified null handling is nullable (fallback value: false).</summary>
        /// <param name="nullHandling">The null handling.</param>
        /// <returns>The result.</returns>
        public bool IsNullable(NullHandling nullHandling)
        {
            return IsNullable(nullHandling, false);
        }

        /// <summary>Determines whether the specified null handling is nullable.</summary>
        /// <param name="nullHandling">The null handling.</param>
        /// <param name="fallbackValue">The fallback value when 'x-nullable' is not defined.</param>
        /// <returns>The result.</returns>
        public bool IsNullable(NullHandling nullHandling, bool fallbackValue)
        {
            if (nullHandling == NullHandling.Swagger)
            {
                if (IsNullableRaw == null)
                    return fallbackValue;

                return IsNullableRaw.Value;
            }

            return Schema?.ActualSchema.IsNullable(nullHandling) ?? false;
        }

        private JsonSchema4 GetActualResponseSchema()
        {
            if (Parent?.Produces?.Contains("application/octet-stream") == true)
                return new JsonSchema4 { Type = JsonObjectType.File };

            return Schema?.ActualSchema;
        }
    }

    /// <summary></summary>
    public class JsonExpectedSchema
    {
        /// <summary>Gets or sets the description.</summary>
        [JsonProperty(PropertyName = "description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Description { get; set; }

        /// <summary>Gets or sets the schema.</summary>
        [JsonProperty(PropertyName = "schema", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public JsonSchema4 Schema { get; set; }
    }
}
