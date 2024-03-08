// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Text.Json;

namespace Econolite.Ode.Model.SystemModeller;

public static class EntityModelExtensions
{
    public static Guid? GetIntersectionId(this EntityModel model)
    {
        JsonElement intersection = new JsonElement();
        if (model.Properties != null && !model.Properties.RootElement.TryGetProperty("intersection", out intersection))
        {
            return null;
        }
        return Guid.Parse((ReadOnlySpan<char>) intersection.GetString());
    }
}
