// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace SixLabors.ImageSharp.Web.Commands
{
    /// <summary>
    /// Parses commands from the request querystring.
    /// </summary>
    public sealed class QueryCollectionRequestParser : IRequestParser
    {
        /// <inheritdoc/>
        public ValueTask<IDictionary<string, string>> ParseRequestCommandsAsync(HttpContext context)
        {
            if (context.Request.Query.Count == 0)
            {
                return new ValueTask<IDictionary<string, string>>(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
            }

            Dictionary<string, StringValues> parsed = QueryHelpers.ParseQuery(context.Request.QueryString.ToUriComponent());
            var transformed = new Dictionary<string, string>(parsed.Count);
            foreach (KeyValuePair<string, StringValues> pair in parsed)
            {
                transformed[pair.Key] = pair.Value.ToString();
            }

            return new ValueTask<IDictionary<string, string>>(transformed);
        }
    }
}
