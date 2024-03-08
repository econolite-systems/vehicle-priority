// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.VehiclePriority.Network
{
    /// <summary>
    ///     Used for objects that need to be unique.
    /// </summary>
    /// <typeparam name="T">The type of the Id.</typeparam>
    public class Unique<T>
    {
        /// <summary>
        ///     Gets or sets the Id (T) of the entity.
        /// </summary>
        /// <value>
        ///     The Id (T) of the entity.
        /// </value>
#pragma warning disable CS8618
        public T Id { get; set; }
#pragma warning restore CS8618
    }
}
