﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Events;
using Microsoft.DotNet.Interactive.Formatting;

namespace Microsoft.DotNet.Interactive
{
    public static class KernelInvocationContextExtensions
    {
        public static DisplayedValue Display(
            this KernelInvocationContext context,
            object value,
            params string[] mimeTypes)
        {
            var displayId = Guid.NewGuid().ToString();

            var formattedValues = FormattedValue.FromObject(value, mimeTypes);

            context.Publish(
                new DisplayedValueProduced(
                    value,
                    context?.Command,
                    formattedValues,
                    displayId));

            var displayedValue = new DisplayedValue(displayId, formattedValues.Select(fv => fv.MimeType).ToArray(), context);

            return displayedValue;
        }

        public static DisplayedValue DisplayAs(
            this KernelInvocationContext context,
            string value,
            string mimeType)
        {
            if (string.IsNullOrWhiteSpace(mimeType))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(mimeType));
            }

            var displayId = Guid.NewGuid().ToString();

            var formattedValue = new FormattedValue(
                mimeType,
                value);

            context.Publish(
                new DisplayedValueProduced(
                    value,
                    context?.Command,
                    new[] { formattedValue },
                    displayId));

            var displayedValue = new DisplayedValue(displayId, mimeType, context);

            return displayedValue;
        }

        public static void DisplayStandardOut(
            this KernelInvocationContext context,
            string output,
            KernelCommand command = null)
        {
            var formattedValues = new List<FormattedValue>
            {
                new(PlainTextFormatter.MimeType, output)
            };

            context.Publish(
                new StandardOutputValueProduced(
                    command ?? context.Command,
                    formattedValues));
        }

        public static void DisplayStandardError(
            this KernelInvocationContext context,
            string error,
            KernelCommand command = null)
        {
            var formattedValues = new List<FormattedValue>
            {
                new(PlainTextFormatter.MimeType, error)
            };

            context.Publish(
                new StandardErrorValueProduced(
                    command ?? context.Command,
                    formattedValues));
        }
    }
}