// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Domain.SystemModeller;
using Econolite.Ode.Models.Entities;
using Econolite.Ode.Models.Entities.Spatial;
using Econolite.Ode.Models.Entities.Types;
using Econolite.Ode.Models.VehiclePriority;
using Econolite.Ode.Models.VehiclePriority.Api;
using Econolite.Ode.Models.VehiclePriority.Status;
using Econolite.Ode.Repository.VehiclePriority;

namespace Econolite.Ode.Domain.VehiclePriority;

public class VehiclePriorityService : IVehiclePriorityService
{
    private readonly IRoutePriorityStatusRepository _routePriorityStatusRepository;
    private readonly IVehicleLocationStatusRepository _vehicleLocationStatusRepository;
    private readonly IRouteStatusRepository _routeStatusRepository;
    private readonly IRegisteredRouteRepository _registeredRouteRepository;
    private readonly IPriorityRequestVehicleRepository _priorityRequestVehicleRepository;
    private readonly IVehiclePriorityPublisher _vehiclePriorityPublisher;
    private readonly IEntityConfigurationService _entityConfigurationService;

    public async Task<bool> RegisterHeartbeatAsync(string sourceId)
    {
        return await Task.FromResult(false);
    }
    
    public VehiclePriorityService(IRoutePriorityStatusRepository routePriorityStatusRepository, IVehicleLocationStatusRepository vehicleLocationStatusRepository, IRouteStatusRepository routeStatusRepository, IRegisteredRouteRepository registeredRouteRepository, IPriorityRequestVehicleRepository priorityRequestVehicleRepository, IVehiclePriorityPublisher vehiclePriorityPublisher, IEntityConfigurationService entityConfigurationService)
    {
        _routePriorityStatusRepository = routePriorityStatusRepository;
        _vehicleLocationStatusRepository = vehicleLocationStatusRepository;
        _routeStatusRepository = routeStatusRepository;
        _registeredRouteRepository = registeredRouteRepository;
        _priorityRequestVehicleRepository = priorityRequestVehicleRepository;
        _vehiclePriorityPublisher = vehiclePriorityPublisher;
        _entityConfigurationService = entityConfigurationService;
    }
    
    public async Task<PriorityRequest?> RequestPriorityAsync(PriorityRequest request)
    {
        request.RequestId = Guid.NewGuid().ToString();
        var route = await _registeredRouteRepository.GetByIdAsync(Guid.Parse(request.RouteId));
        var status = request.ToRouteStatus();
        status.NextIntersection = route?.Intersections.FirstOrDefault();
        status.Geometry = route?.Geometry;
        _routeStatusRepository.Add(status);
        var (success, _) = await _routeStatusRepository.DbContext.SaveChangesAsync();
        return !success ? null : request;
    }

    public async Task<bool> CancelPriorityRequestAsync(string requestId)
    {
        _routeStatusRepository.Remove(Guid.Parse(requestId));
        var (success, _) = await _routeStatusRepository.DbContext.SaveChangesAsync();
        return success;
    }
    
    public async Task<string?> RegisterRouteAsync(RouteUpdate route)
    {
        var registered = route.ToRegisteredRoute();
        if (registered.Geometry != null)
            registered.Intersections = await CalculateIntersectionPlansAsync(registered.Geometry);
        _registeredRouteRepository.Add(registered);
        var (success, _) = await _registeredRouteRepository.DbContext.SaveChangesAsync();
        if (!success)
        {
            return null;
        }
        return registered.Id.ToString();
    }
    
    public async Task<string?> UpdateRouteAsync(RouteUpdate route)
    {
        var registered = route.ToRegisteredRoute();
        if (registered.Geometry != null)
            registered.Intersections = await CalculateIntersectionPlansAsync(registered.Geometry);
        _registeredRouteRepository.Update(registered);
        var (success, _) = await _registeredRouteRepository.DbContext.SaveChangesAsync();
        if (!success)
        {
            return null;
        }
        return registered.Id.ToString();
    }

    public async Task UpdateVehicleAsync(VehicleUpdate update)
    {
        var currentDateTime = DateTime.Now;
        var vehicle = update.ToVehicle();

        var entityModels = await _entityConfigurationService.QueryIntersectingGeoFencesAsync(vehicle.Location);
        var streetSegments = entityModels.Where(m => m.Type.Id == StreetSegmentTypeId.Id);
        var streetSegment = streetSegments.FirstOrDefault();

        var properties = streetSegment.Geometry?.LineString?.Properties;
        if (properties?.TripPointLocations == null) return;

        var tripPointLocation = vehicle.Location.ToTripPointLocation(properties.TripPointLocations);
        var status = await _routeStatusRepository.GetByRouteIdAsync(vehicle.RouteId);
        var route = await _registeredRouteRepository.GetByIdAsync(vehicle.RouteId);

        if (status == null || route == null) return;
        
        status.Vehicle = vehicle;
        status.NextIntersection = route.Intersections.FirstOrDefault(i => i.IntersectionId == properties.Intersection);
        status.Location = tripPointLocation;
        // TODO: Create a smoothing service.
        if (status.LastUpdate.AddSeconds(5) >= currentDateTime)
        {
            _routeStatusRepository.Update(status);
        }
        else
        {
            return;
        }
        // TODO: This should be using a service to calculate the ETA.
        if (vehicle.Speed != null)
            status.EtaInSeconds = CalculateTimeFromSpeedAndDistance((float) vehicle.Speed, tripPointLocation.Distance);
        status.Eta = currentDateTime.AddSeconds(status.EtaInSeconds);
        _routeStatusRepository.Update(status);

        var (success, _) = await _routeStatusRepository.DbContext.SaveChangesAsync();
        if (!success) return;
        await _vehiclePriorityPublisher.PublishEtaAsync(status);
    }

    public async Task UpdateVehicleLocationStatusAsync(VehicleLocationStatus status)
    {
        await _vehicleLocationStatusRepository.UpdateStatus(status);
    }

    public async Task<IEnumerable<VehicleLocationStatus>> RemoveOldVehicleStatusAsync()
    {
        return await _vehicleLocationStatusRepository.RemoveOldStatusAsync();
    }

    public async Task UpdatePriorityStatusAsync(RoutePriorityStatus status)
    {
        await _routePriorityStatusRepository.UpdateStatus(status);
    }

    public async Task<IEnumerable<RoutePriorityStatus>> RemoveOldPriorityStatusAsync()
    {
        return await _routePriorityStatusRepository.RemoveOldStatusAsync();
    }

    public async Task<PriorityRequestVehicleConfiguration> GetAllPriorityRequestVehicleClassesAsync()
    {
        var configs = await _priorityRequestVehicleRepository.GetAllAsync();
        var intersections = await _entityConfigurationService.GetNodesByTypeAsync(IntersectionTypeId.Name);
        var config = configs?.FirstOrDefault();
        
        if (config == null)
        {
            var prvIntersections = intersections.ToPriorityRequestVehicleIntersections();
            config = new PriorityRequestVehicleConfiguration(Id: Guid.NewGuid().ToString(), Vehicles: Array.Empty<PriorityRequestVehicle>(),
                new PriorityRequestVehicleClassType[]
                {
                    new(1, "ev"),
                    new(5, "bus")
                },
                new PriorityRequestVehicleClassLevel[]
                {
                    new(1, "ev"),
                    new(5, "bus"),
                },
                prvIntersections);

            await AddPriorityRequestVehicleClassesAsync(config);
        }
        else
        {
            config = config.UpdateIntersections(intersections);
        }

        return config;
    }
    
    public async Task<string?> AddPriorityRequestVehicleClassesAsync(PriorityRequestVehicleConfiguration config)
    {
        _priorityRequestVehicleRepository.Add(config);
        var (success, _) = await _priorityRequestVehicleRepository.DbContext.SaveChangesAsync();
        if (!success) return null;
        await _vehiclePriorityPublisher.PublishConfigAsync(config);
        return config.Id;
    }
    
    public async Task<string?> UpdatePriorityRequestVehicleClassesAsync(PriorityRequestVehicleConfiguration config)
    {
        _priorityRequestVehicleRepository.Update(config);
        var (success, _) = await _priorityRequestVehicleRepository.DbContext.SaveChangesAsync();
        if (!success) return null;
        await _vehiclePriorityPublisher.PublishConfigAsync(config);
        return config.Id;
    }

    private async Task<IEnumerable<Controller>> CalculateIntersectionPlansAsync(GeoJsonLineStringFeature route)
    {
        var approaches = await _entityConfigurationService.QueryIntersectingApproachesAsync(route);
        var orderedApproaches = approaches.ToOrderedIntersectionPhase(route);
        return orderedApproaches.Select(oa =>
        {
            var plan = 0;
            if (oa.Plan.Number != null)
                plan = oa.Plan.Number.Value;
            return new Controller
                {IntersectionId = Guid.Parse((string) oa.Intersection), Plan = plan};
        }).Where(c => c.Plan > 0);
    }
    
    private int CalculateTimeFromSpeedAndDistance(float speed, int distance)
    {
        var ftPerSec = speed * 5280 / 3600;
        return (int) (distance / ftPerSec);
    }
}
