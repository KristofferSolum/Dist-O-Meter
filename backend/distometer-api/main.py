import base64
from itertools import combinations

from fastapi import FastAPI, HTTPException
from pydantic import BaseModel, Field

from calculations import (
    calculate_point,
    calculate_distance_between_points,
    create_coordinate_plot
)


app = FastAPI(
    title="Dist-O-Meter API",
    description="API for triangulation, distance calculations and coordinate plotting.",
    version="1.0.0"
)

# --------------------------------------------------
# Request models
# --------------------------------------------------
class ObjectMeasurement(BaseModel):
    name: str
    angle_r: float = Field(gt=0, lt=180)
    angle_q: float = Field(gt=0, lt=180)


class MeasurementRequest(BaseModel):
    baseline: float = Field(gt=0)
    objects: list[ObjectMeasurement]


# --------------------------------------------------
# Routes
# --------------------------------------------------

@app.get("/")
def root():
    return {
        "message": "Dist-O-Meter API is running"
    }


@app.post("/calculate")
def calculate_measurement(request: MeasurementRequest):

    if len(request.objects) == 0:
        raise HTTPException(
            status_code=400,
            detail="At least one object is required."
        )

    # Check for duplicate object names
    names = [obj.name for obj in request.objects]

    if len(names) != len(set(names)):
        raise HTTPException(
            status_code=400,
            detail="Object names must be unique."
        )

    baseline = request.baseline

    # P is always the origin and the midpoint
    p = (0.0, 0.0)
    r = (-baseline / 2, 0.0)
    q = (baseline / 2, 0.0)

    object_positions = {}

    # --------------------------------------------------
    # Calculate object coordinates
    # --------------------------------------------------

    for obj in request.objects:

        result = calculate_point(
            baseline,
            obj.angle_r,
            obj.angle_q
        )

        # calculate_point currently returns an error
        # dictionary when the angles are invalid
        if isinstance(result, dict) and "error" in result:
            raise HTTPException(
                status_code=400,
                detail=f"{obj.name}: {result['error']}"
            )

        object_positions[obj.name] = result

    # --------------------------------------------------
    # Distances from P
    # --------------------------------------------------

    distances_from_p = {}

    for name, position in object_positions.items():
        distances_from_p[name] = calculate_distance_between_points(
            position,
            p
        )

    # --------------------------------------------------
    # Distances between objects
    # --------------------------------------------------

    distances_between_objects = []

    for (name1, point1), (name2, point2) in combinations(
        object_positions.items(),
        2
    ):
        distance = calculate_distance_between_points(
            point1,
            point2
        )

        distances_between_objects.append({
            "from": name1,
            "to": name2,
            "distance": distance
        })

    # --------------------------------------------------
    # Plot
    # --------------------------------------------------

    plot_points = {
        "P": p,
        "R": r,
        "Q": q,
        **object_positions
    }

    plot_bytes = create_coordinate_plot(plot_points)

    plot_base64 = base64.b64encode(
        plot_bytes
    ).decode("utf-8")

    # --------------------------------------------------
    # Response
    # --------------------------------------------------

    return {
        "baseline": baseline,

        "reference_points": {
            "P": {
                "x": p[0],
                "y": p[1]
            },
            "R": {
                "x": r[0],
                "y": r[1]
            },
            "Q": {
                "x": q[0],
                "y": q[1]
            }
        },

        "objects": {
            name: {
                "x": position[0],
                "y": position[1]
            }
            for name, position in object_positions.items()
        },

        "distances_from_p": distances_from_p,

        "distances_between_objects": distances_between_objects,

        "plot_base64": plot_base64
    }
