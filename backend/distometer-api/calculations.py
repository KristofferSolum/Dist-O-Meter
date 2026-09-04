import math
import io
import matplotlib
matplotlib.use("Agg")
import matplotlib.pyplot as plt


def calculate_point(baseline: int, angel_r: float, angel_q: float):
    """
        Finds the coordinates of a point with triangulation

        p = (0, 0)
        r = (-1/2 * baseline, 0)
        q = (1/2 * baseline, 0)

        angle_r: angle at R, measured from the direction R -> P
        angle_q: angle at Q, measured from the direction Q -> P
        """
    angel_r = 180 - angel_r

    # Convert angles from degrees to radians
    alpha = math.radians(angel_r)
    beta = math.radians(angel_q)

    # Calculate the third angle of the triangle
    gamma = math.pi - alpha - beta

    if gamma <= 0:
        return {"error": "Invalid angles. The sum of the angles must be less than 180 degrees."}

    # sinus law to find the distances from p to the point
    distance_from_r = (baseline * math.sin(beta)) / math.sin(gamma)

    x = -baseline / 2 + distance_from_r * math.cos(alpha)
    y = distance_from_r * math.sin(alpha)

    return x, y


def calculate_distance_between_points(point1: tuple, point2: tuple = (0.0, 0.0)):
    """
    Calculate the distance between two points in 2D space.

    point1: A dictionary with 'x' and 'y' coordinates of the first point.
    point2: A dictionary with 'x' and 'y' coordinates of the second point.
    """
    x1, y1 = point1
    x2, y2 = point2

    return math.sqrt((x2 - x1) ** 2 + (y2 - y1) ** 2)


def create_coordinate_plot(points: dict[str, tuple[float, float]]) -> bytes:
    """
    Creates a PNG image showing P, R, Q and the measured objects
    in a 2D coordinate system.

    points:
        Dictionary containing point names and coordinates.

        Example:
        {
            "P": (0, 0),
            "R": (-150, 0),
            "Q": (150, 0),
            "A": (-40, 200),
            "B": (20, 250),
            "C": (120, 170)
        }

    Returns:
        PNG image as bytes.
    """

    fig, ax = plt.subplots(figsize=(9, 7))

    for name, (x, y) in points.items():
        ax.scatter(x, y)

        ax.annotate(
            name,
            (x, y),
            xytext=(8, 8),
            textcoords="offset points"
        )

    # Coordinate axes
    ax.axhline(0, linewidth=0.8)
    ax.axvline(0, linewidth=0.8)

    # Make one unit on x-axis equal one unit on y-axis
    ax.set_aspect("equal", adjustable="datalim")

    ax.set_xlabel("X (cm)")
    ax.set_ylabel("Y (cm)")
    ax.set_title("Dist-O-Meter Measurement")

    ax.grid(True)

    # Add some space around the points
    ax.margins(0.15)

    image_buffer = io.BytesIO()

    fig.savefig(
        image_buffer,
        format="png",
        bbox_inches="tight",
        dpi=150
    )

    plt.close(fig)

    image_buffer.seek(0)

    return image_buffer.getvalue()