// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Models.Entities;
using Econolite.Ode.Models.Entities.Spatial;
using Econolite.Ode.Models.Entities.Types;
using Econolite.Ode.Models.VehiclePriority;
using Econolite.Ode.Models.VehiclePriority.Api;
using Econolite.Ode.Models.VehiclePriority.Status;
using Econolite.Ode.Repository.VehiclePriority;
using Econolite.OdeRepository.SystemModeller;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Domain.VehiclePriority;

public class VehiclePriorityEdgeService : IVehiclePriorityService
{
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;
    private readonly ICvFiltering _cvFiltering;
    private readonly IRouteStatusRepository _routeStatusRepository;
    private readonly IEntityNodeJsonFileRepository _entityNodeJsonFileRepository;
    private readonly IVehiclePriorityEdgePublisher _vehiclePriorityEdgePublisher;
    private readonly IPriorityRequestVehicleEdgeRepository _priorityRequestVehicleRepository;

    public async Task<bool> RegisterHeartbeatAsync(string sourceid)
    {
        return await Task.FromResult(false);
    }
    
    public VehiclePriorityEdgeService(IConfiguration configuration, ICvFiltering cvFiltering, IRouteStatusRepository routeStatusRepository, IEntityNodeJsonFileRepository entityNodeJsonFileRepository, IVehiclePriorityEdgePublisher vehiclePriorityEdgePublisher, IPriorityRequestVehicleEdgeRepository priorityRequestVehicleRepository, ILoggerFactory loggerFactory)
    {
        if (loggerFactory is null)
        {
            throw new ArgumentNullException(nameof(loggerFactory));
        }

        _logger = loggerFactory.CreateLogger(GetType().Name);
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _cvFiltering = cvFiltering ?? throw new ArgumentNullException(nameof(cvFiltering));
        _routeStatusRepository = routeStatusRepository ?? throw new ArgumentNullException(nameof(routeStatusRepository));
        _entityNodeJsonFileRepository = entityNodeJsonFileRepository ?? throw new ArgumentNullException(nameof(entityNodeJsonFileRepository));
        _vehiclePriorityEdgePublisher = vehiclePriorityEdgePublisher ?? throw new ArgumentNullException(nameof(vehiclePriorityEdgePublisher));
        _priorityRequestVehicleRepository = priorityRequestVehicleRepository ?? throw new ArgumentNullException(nameof(priorityRequestVehicleRepository));;
    }
    
    public async Task<PriorityRequest?> RequestPriorityAsync(PriorityRequest request)
    {
        return await Task.FromResult(request);
    }

    public async Task<bool> CancelPriorityRequestAsync(string requestId)
    {
        return await Task.FromResult(true);
    }
    
    public async Task<string?> RegisterRouteAsync(RouteUpdate route)
    {
        return await Task.FromResult<string?>(null);
    }
    
    public async Task<string?> UpdateRouteAsync(RouteUpdate route)
    {
        return await Task.FromResult<string?>(null);
    }

    public async Task UpdateVehicleAsync(VehicleUpdate update)
    {
        var currentDateTime = DateTime.Now;
        var vehicle = update.ToVehicle();
        var status = await _routeStatusRepository.GetByVehicleIdAsync(vehicle.Id);
        
        // Check to see if the vehicle should be processed.
        if (!await _cvFiltering.ProcessVehicleAsync(update.VehicleId))
        {
            _logger.LogInformation("Vehicle filtered out {@}", update.VehicleId);
            if (!status?.Completed ?? true)
            {
                if (await CancelledAsync(currentDateTime, new EntityNode(), status))
                {
                    _logger.LogInformation("Canceling request for {@}", update.VehicleId);
                    await _vehiclePriorityEdgePublisher.PublishVehicleUpdateAsync(update);
                }
            }
            return;
        }
        
        var entityModels = await _entityNodeJsonFileRepository.QueryIntersectingGeoFences(vehicle.Location);
        var enumerable = entityModels as EntityNode[] ?? entityModels.ToArray();
        var intersection =
            enumerable.FirstOrDefault(m => m.Type.Id == IntersectionTypeId.Id || m.Type.Id == SignalTypeId.Id);

        if (await CancelledAsync(currentDateTime, intersection, status))
        {
            _logger.LogInformation("Canceling request for {@}", update.VehicleId);
            await _vehiclePriorityEdgePublisher.PublishVehicleUpdateAsync(update);
            return;
        }
        if (status != null && status.LastUpdate > currentDateTime.AddSeconds(-5))
        {
            _logger.LogTrace("Skipping update {@}", update.VehicleId);
            return;
        }
        if (intersection != null)
        {
            _logger.LogTrace("Intersection not found for {@}", vehicle.Location);
            return;
        }
        await _vehiclePriorityEdgePublisher.PublishVehicleUpdateAsync(update);
        
        var streetSegment = CurrentStreetSegment(enumerable);

        var properties = streetSegment?.Geometry.LineString?.Properties;
        if (properties?.TripPointLocations == null || properties?.Destination == null)
        {
            _logger.LogTrace("Unable to find street segments for {@}", vehicle.Location);
            return;
        }
        var plan = StrategyNumber(properties.Destination.ToString());
        if (!plan.HasValue)
        {
            _logger.LogWarning("Unable to determine the strategy number for {@}", properties.Destination.ToString());
            return;
        }

        var tripPointLocation = vehicle.Location.ToTripPointLocation(properties.TripPointLocations);

        var levelPriority = await _priorityRequestVehicleRepository.GetLevelPriorityAsync(vehicle.Id);
        
        if (status == null)
        {
            status = new RouteStatus()
            {
                Id = Guid.NewGuid(),
                RouteId = Guid.Empty,
                RequestId = await _routeStatusRepository.GetNewTaskId(Guid.Empty),
                VehicleId = vehicle.Id,
                VehicleTypePriority = levelPriority.priority,
                NextIntersection = new Controller()
                {
                    IntersectionId = Guid.Empty,
                    Plan = plan.Value
                },
                IsInitial = true,
                DesiredClassLevel = levelPriority.level,
            };
        }
        else
        {
            status.IsInitial = false;
        }

        status.DesiredClassLevel = levelPriority.level;
        status.VehicleTypePriority = levelPriority.priority;
        status.Vehicle = vehicle;
        status.Location = tripPointLocation;

        if (vehicle.Speed != null)
            status.EtaInSeconds = CalculateTimeFromSpeedAndDistance((float) vehicle.Speed, tripPointLocation.Distance);
        
        status.Eta = currentDateTime.AddSeconds(
            status.EtaInSeconds);
        status.LastUpdate = currentDateTime;
        
        _routeStatusRepository.Update(status);
        
        await _vehiclePriorityEdgePublisher.PublishEtaAsync(status);
    }

    public Task UpdateVehicleLocationStatusAsync(VehicleLocationStatus status)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<VehicleLocationStatus>> RemoveOldVehicleStatusAsync()
    {
        throw new NotImplementedException();
    }

    public Task UpdatePriorityStatusAsync(RoutePriorityStatus status)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<RoutePriorityStatus>> RemoveOldPriorityStatusAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<PriorityRequestVehicleConfiguration> GetAllPriorityRequestVehicleClassesAsync()
    {
        var configs = await _priorityRequestVehicleRepository.GetAllAsync();
        if (!configs.Any())
        {
            var list = configs.ToList();
            list.Add(new PriorityRequestVehicleConfiguration(Id: Guid.NewGuid().ToString(), Vehicles: Array.Empty<PriorityRequestVehicle>(),
                new PriorityRequestVehicleClassType[]
                {
                    new PriorityRequestVehicleClassType(1, "ev"),
                    new PriorityRequestVehicleClassType(5, "bus")
                },
                new PriorityRequestVehicleClassLevel[]
                {
                    new PriorityRequestVehicleClassLevel(1, "ev"),
                    new PriorityRequestVehicleClassLevel(5, "bus"),
                },
                Array.Empty<PriorityRequestVehicleIntersection>()));
            configs = list;
            await AddPriorityRequestVehicleClassesAsync(configs.First());
        }

        return configs.First();
    }

    public Task<string?> AddPriorityRequestVehicleClassesAsync(PriorityRequestVehicleConfiguration config)
    {
        throw new NotImplementedException();
    }

    public Task<string?> UpdatePriorityRequestVehicleClassesAsync(PriorityRequestVehicleConfiguration config)
    {
        throw new NotImplementedException();
    }

    private async Task<IEnumerable<Controller>> CalculateIntersectionPlansAsync(GeoJsonLineStringFeature route)
    {
        var approaches = await _entityNodeJsonFileRepository.QueryIntersectingApproaches(route);
        var orderedApproaches = approaches.ToOrderedIntersectionPhase(route);
        return orderedApproaches.Select(oa =>
        {
            var plan = 0;
            if (oa.Plan.Number != null)
                plan = oa.Plan.Number.Value;
            return new Controller()
                {IntersectionId = Guid.Parse(oa.Intersection), Plan = plan};
        }).Where(c => c.Plan > 0);
    }
    
    private int CalculateTimeFromSpeedAndDistance(float speed, int distance)
    {
        var ftPerSec = speed * 5280 / 3600;
        return (int) (distance / ftPerSec);
    }

    private async Task<bool> CancelledAsync(DateTime currentDateTime, EntityNode? intersection, RouteStatus? status)
    {
        var result = false;
        if (intersection != null && status != null)
        {
            var cancelled = status.ToCancelled(currentDateTime);
            _routeStatusRepository.Remove(status.Id);
            await _vehiclePriorityEdgePublisher.PublishEtaAsync(cancelled);
            result = true;
        }

        return result;
    }
    
    private async Task PublishEtaAsync(RouteStatus status)
    {
        _routeStatusRepository.Remove(status.Id);
        await _vehiclePriorityEdgePublisher.PublishEtaAsync(status);
    }

    private int? StrategyNumber(string destination)
    {
        var plan = _configuration["Plans:" + destination];
        if (!int.TryParse(plan, out var result)) return null;
        return result;
    }
    
    private TripPointLocation? Location(EntityNode streetSegment, Vehicle vehicle)
    {
        var properties = streetSegment.Geometry.LineString?.Properties;
        return properties?.TripPointLocations != null ? vehicle.Location.ToTripPointLocation(properties.TripPointLocations): null;
    }

    private EntityNode? CurrentStreetSegment(IEnumerable<EntityNode> enumerable)
    {
        var streetSegments = enumerable.Where(m => m.Type.Id == StreetSegmentTypeId.Id);
        return streetSegments.FirstOrDefault();
    }
}
