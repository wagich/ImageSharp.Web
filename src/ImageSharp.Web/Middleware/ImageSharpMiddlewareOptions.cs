// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp.Web.Commands;
using SixLabors.ImageSharp.Web.Processors;
using SixLabors.ImageSharp.Web.Providers;

namespace SixLabors.ImageSharp.Web.Middleware
{
    /// <summary>
    /// Configuration options for the <see cref="ImageSharpMiddleware"/> middleware.
    /// </summary>
    public class ImageSharpMiddlewareOptions
    {
        /// <summary>
        /// Gets or sets the base library configuration.
        /// </summary>
        public Configuration Configuration { get; set; } = Configuration.Default;

        /// <summary>
        /// Gets or sets the number of days to store images in the browser cache.
        /// </summary>
        public int MaxBrowserCacheDays { get; set; } = 7;

        /// <summary>
        /// Gets or sets the number of days to store images in the image cache.
        /// </summary>
        public int MaxCacheDays { get; set; } = 365;

        /// <summary>
        /// Gets or sets the length of the filename to use (minus the extension) when storing images in the image cache.
        /// </summary>
        public uint CachedNameLength { get; set; } = 12;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1516 // Elements should be separated by blank line
        public Action<ImageCommandContext> OnParseCommands { get; set; } = c =>
        {
            if (c.Commands.Count == 0)
            {
                return;
            }

            // It's a good idea to have this to provide very basic security.
            // We can safely use the static resize processor properties.
            uint width = c.Parser.ParseValue<uint>(c.Commands.GetValueOrDefault(ResizeWebProcessor.Width));
            uint height = c.Parser.ParseValue<uint>(c.Commands.GetValueOrDefault(ResizeWebProcessor.Height));

            if (width > 4000 && height > 4000)
            {
                c.Commands.Remove(ResizeWebProcessor.Width);
                c.Commands.Remove(ResizeWebProcessor.Height);
            }
        };
        public Func<ImageCommandContext, Task> OnParseCommandsAsync { get; set; } = c =>
        {
            if (c.Commands.Count != 0)
            {
                // It's a good idea to have this to provide very basic security.
                // We can safely use the static resize processor properties.
                uint width = c.Parser.ParseValue<uint>(c.Commands.GetValueOrDefault(ResizeWebProcessor.Width));
                uint height = c.Parser.ParseValue<uint>(c.Commands.GetValueOrDefault(ResizeWebProcessor.Height));

                if (width > 4000 && height > 4000)
                {
                    c.Commands.Remove(ResizeWebProcessor.Width);
                    c.Commands.Remove(ResizeWebProcessor.Height);
                }
            }

            return Task.CompletedTask;
        };
        public Func<ImageCommandContext, ValueTask> OnParseCommandsAsyncValueTask { get; set; } = c =>
        {
            if (c.Commands.Count != 0)
            {
                // It's a good idea to have this to provide very basic security.
                // We can safely use the static resize processor properties.
                uint width = c.Parser.ParseValue<uint>(c.Commands.GetValueOrDefault(ResizeWebProcessor.Width));
                uint height = c.Parser.ParseValue<uint>(c.Commands.GetValueOrDefault(ResizeWebProcessor.Height));

                if (width > 4000 && height > 4000)
                {
                    c.Commands.Remove(ResizeWebProcessor.Width);
                    c.Commands.Remove(ResizeWebProcessor.Height);
                }
            }

            return default;
        };

        public Action<FormattedImage> OnBeforeSave { get; set; } = _ => { };
        public Func<FormattedImage, Task> OnBeforeSaveAsync { get; set; } = _ => Task.CompletedTask;
        public Func<FormattedImage, ValueTask> OnBeforeSaveAsyncValueTask { get; set; } = _ => default;

        public Action<ImageProcessingContext> OnProcessed { get; set; } = _ => { };
        public Func<ImageProcessingContext, Task> OnProcessedAsync { get; set; } = _ => Task.CompletedTask;
        public Func<ImageProcessingContext, ValueTask> OnProcessedAsyncValueTask { get; set; } = _ => default;

        public Action<HttpContext> OnPrepareResponse { get; set; } = _ => { };
        public Func<HttpContext, Task> OnPrepareResponseAsync { get; set; } = _ => Task.CompletedTask;
        public Func<HttpContext, ValueTask> OnPrepareResponseAsyncValueTask { get; set; } = _ => default;

#pragma warning restore SA1516 // Elements should be separated by blank line
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning restore SA1600 // Elements should be documented

    }
}
