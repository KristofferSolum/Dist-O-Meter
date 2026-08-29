import base64

from fastapi.testclient import TestClient

from main import app


client = TestClient(app)


def test_root():
    response = client.get("/")

    assert response.status_code == 200
    assert response.json() == {
        "message": "Dist-O-Meter API is running"
    }


def test_calculate_valid_measurement():
    request_data = {
        "baseline": 300,
        "objects": [
            {
                "name": "A",
                "angle_r": 70,
                "angle_q": 40
            },
            {
                "name": "B",
                "angle_r": 55,
                "angle_q": 50
            },
            {
                "name": "C",
                "angle_r": 30,
                "angle_q": 60
            }
        ]
    }

    response = client.post(
        "/calculate",
        json=request_data
    )

    assert response.status_code == 200

    data = response.json()

    assert data["baseline"] == 300

    assert data["reference_points"]["P"] == {
        "x": 0,
        "y": 0
    }

    assert data["reference_points"]["R"] == {
        "x": -150,
        "y": 0
    }

    assert data["reference_points"]["Q"] == {
        "x": 150,
        "y": 0
    }

    assert len(data["objects"]) == 3

    assert "A" in data["objects"]
    assert "B" in data["objects"]
    assert "C" in data["objects"]

    assert len(data["distances_from_p"]) == 3

    assert len(data["distances_between_objects"]) == 3

    assert "plot_base64" in data

    plot_bytes = base64.b64decode(
        data["plot_base64"]
    )

    assert plot_bytes.startswith(
        b"\x89PNG\r\n\x1a\n"
    )


def test_calculate_invalid_angles():
    request_data = {
        "baseline": 300,
        "objects": [
            {
                "name": "A",
                "angle_r": 100,
                "angle_q": 80
            }
        ]
    }

    response = client.post(
        "/calculate",
        json=request_data
    )

    assert response.status_code == 400


def test_calculate_duplicate_names():
    request_data = {
        "baseline": 300,
        "objects": [
            {
                "name": "A",
                "angle_r": 70,
                "angle_q": 40
            },
            {
                "name": "A",
                "angle_r": 60,
                "angle_q": 50
            }
        ]
    }

    response = client.post(
        "/calculate",
        json=request_data
    )

    assert response.status_code == 400


def test_calculate_negative_baseline():
    request_data = {
        "baseline": -100,
        "objects": [
            {
                "name": "A",
                "angle_r": 70,
                "angle_q": 40
            }
        ]
    }

    response = client.post(
        "/calculate",
        json=request_data
    )

    assert response.status_code == 422


def test_calculate_without_objects():
    request_data = {
        "baseline": 300,
        "objects": []
    }

    response = client.post(
        "/calculate",
        json=request_data
    )

    assert response.status_code == 400