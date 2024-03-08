// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
// ReSharper disable HeapView.BoxingAllocation
// using System;
// using System.Text.Json;
// using Econolite.Ode.Model.SystemModeller;
// using Econolite.Ode.Model.SystemModeller.Db;
// using Econolite.Ode.Persistence.Mongo.Test.Repository;
// using Econolite.OdeRepository.SystemModeller;
// using Microsoft.Extensions.Logging;
// using Moq;
// using Xunit;
//
//
// namespace Econolite.Ode.Repository.SystemModeller.Test;
//
// public class EntityModelRepositoryTests : IdRepositoryBaseTest<Guid, EntityModelRepository, EntityModel>, IClassFixture<MongoFixture>
// {
//     private readonly ILogger<EntityModelRepository> _logger = Mock.Of<ILogger<EntityModelRepository>>();
//     
//     public EntityModelRepositoryTests(MongoFixture fixture) : base(fixture)
//     {
//     }
//
//     protected override Guid Id { get; } = Guid.NewGuid();
//
//     protected override string ExpectedJsonIdFilter => "{ \"_id\" : UUID(\"" + Id + "\") }";
//     protected override EntityModelRepository CreateRepository()
//     {
//         return new EntityModelRepository(Context, _logger);
//     }
//
//     protected override EntityModel CreateDocument()
//     {
//         var jsonOptions = new JsonSerializerOptions
//         {
//             PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
//             PropertyNameCaseInsensitive = true,
//             WriteIndented = true
//         };
//             
//         var approach = new EntityModel()
//         {
//             EntityType = "Approach",
//             Geometry = JsonSerializer.SerializeToDocument(new GeoJsonLineString(){
//                 Coordinates = new[]
//                 {
//                     new[] {-111.89123779535294, 40.760442523425205},
//                     new[] {-111.89096823334694, 40.76043642853944}
//                 },
//                 Type = "LineString"
//             }, jsonOptions),
//             Properties = JsonSerializer.SerializeToDocument(new ApproachPropertiesModel{Intersection = Guid.NewGuid().ToString(), Bearing = Bearing.NB.ToString(), Phases = new []{
//                 new PhaseModel(){ Number = 2, Movement = Movement.Thru.ToString(), Lanes = 2, Detectors = new []{new DetectorModel(){Advanced = true}}},
//                 new PhaseModel(){ Number = 1, Movement = Movement.Left.ToString(), Lanes = 1}}},jsonOptions)
//         };
//
//         return approach;
//     }
// }
