import math

from calculations import calculate_point, calculate_distance_between_points, create_coordinate_plot


def test_calculate_distance_between_points():
    point1 = (0, 0)
    point2 = (3, 4)

    distance = calculate_distance_between_points(point1, point2)

    assert distance == 5


def test_calculate_distance_from_origin():
    point = (6, 8)

    distance = calculate_distance_between_points(point)

    assert distance == 10


def test_calculate_point_symmetric_triangle():
    baseline = 100
    angle_r = 45
    angle_q = 45

    x, y = calculate_point(baseline, angle_r, angle_q)

    assert math.isclose(x, 0, abs_tol=1e-9)
    assert math.isclose(y, 50, abs_tol=1e-9)


def test_calculate_point_asymmetric_triangle():
    baseline = 100
    angle_r = 60
    angle_q = 30

    x, y = calculate_point(baseline, angle_r, angle_q)

    expected_distance_from_r = (
        baseline * math.sin(math.radians(angle_q))
        / math.sin(math.radians(90))
    )

    expected_x = -baseline / 2 + expected_distance_from_r * math.cos(math.radians(angle_r))
    expected_y = expected_distance_from_r * math.sin(math.radians(angle_r))

    assert math.isclose(x, expected_x, rel_tol=1e-9)
    assert math.isclose(y, expected_y, rel_tol=1e-9)


def test_invalid_angles():
    result = calculate_point(100, 100, 80)

    assert "error" in result


def test_create_coordinate_plot_returns_png():
    points = {
        "P": (0, 0),
        "R": (-150, 0),
        "Q": (150, 0),
        "A": (-40, 200),
        "B": (20, 250),
        "C": (120, 170)
    }

    image = create_coordinate_plot(points)

    assert isinstance(image, bytes)

    # PNG files always start with this byte signature
    assert image.startswith(b"\x89PNG\r\n\x1a\n")

    # Basic check that an actual image was produced
    assert len(image) > 1000
