## Required Software
- Redis Cache
- MongoDB
- Kafka

## Vehicle Priority Processing
::: mermaid
graph TD
A(Kafka:Priority) -->|Consumer Group| B(Priority Router)
B --> F[(Vehicle Location Cache)]
subgraph UI
F
end
subgraph Processing
B --> C{Algorithm}
C -->|Publish| D[Kafka:Topic]
C -->|Publish| E[Kafka:Topic]
D -->|Consumer Group| G[Worker Service]
E --> G
G --> I[(State Cache)]
I --> G
end
G -->|Publish| H(Kafka:Priority.Suggestion)
subgraph Signal Command
H
end
:::
### Signal Command
::: mermaid
graph TB;
subgraph Command Processing
H(Kafka:Priority.Suggestion) -->|Consumer Group| K[Signal Commands]
K -->|Publish| L[Kafka:Command.Request]
L -->|Consumer Group| M[Device Manager]
end
:::

Items
- Entities
  - Bus Stops
    - Configure the geofence
  - Fire Stations
    - Configure the geofence
- Signals(IDM)
  - Configure the geofence
  - Configure the approach
    - Stop bar
      - Movement
        - Phase
        - Preempt
      - Detectors
- Street Segments
  - Configure geofence from origin to destination signals
    - Request from Azure Maps Directions to get segment
      - Distance
      - Travel time in sec
      - GeoJSON
    - Offset from center to create the geofence
- Device Manager
  - Configure Signal Communication
- GTFS
  - Import into mongodb
- External Priority Request Api
- Priority Request Producer
  - Filter Vehicles
  - Get route/trip
    - Request from Azure Maps Directions
      - Calculate signal order for request
  - Vehicle Location
- ETA Algorithms
  - Algorithm Router
  - Publish Result
- Signal Command Request
- Map Editor
- Map Vehicle Priority Layer
- Priority Status
- Priority Logging
