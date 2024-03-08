// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Client.VehiclePriority.Model.J2735;

namespace Econolite.Ode.Repository.VehiclePriority;

public interface ISrmMessageRepository
{
    Task<SrmMessage?> GetByIdAsync(string id);
    SrmMessage GetById(string id);
    Task<IEnumerable<SrmMessage>> GetAllAsync();
    IEnumerable<SrmMessage> GetAll();
    void Update(SrmMessage document);
    void Remove(string id);
    Task ReplaceDataAsync(IEnumerable<SrmMessage> models);
}