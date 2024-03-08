# Entities

- Route
  - LineString
  - Intersections
    - Distance to next intersection
- Intersection
  - Signal
    - Geofence
  - Bus Stop
    - Geofence
  - Approach
    - Direction
    - Phase/Overlap
      - Lanes
      - Geofence
  - Detector
    - Type
    - Geofence

- Vehicle
  - Location
  - Distance to Next Signal
  - Speed
  - ETA
  - History

- Egress Signal
  - Location
- Ingress Signal
  - Location
  - Approach

## Configuration

### Entity Type

Entity types define

- Icon
- Name
- Description
- Properties List
  - Name
  - Type (string | number | object | array | enum)
  - Validation
- Geo
  - Type ( Point, Polygon, Linestring)
- Allowed Children Types

#### Enums

- Ordered List label/values

#### Geo Types
