using UnityEngine;

public class Segment {

	public Vector2 start = new Vector2(0.0f, 0.0f);
	public Vector2 end = new Vector2(0.0f, 0.0f);

	public Segment(Vector2 s, Vector2 e) 	{
		start = s;
		end = e;
	}

	public Segment() {
	}

	public float SquareLength() {
		return Mathf.Pow(end.x - start.x, 2.0f) + Mathf.Pow(end.y - start.y, 2.0f);
	}

	public float Length() {
		return Mathf.Sqrt(SquareLength());
	}

	public float Angle() {
		return Mathf.Atan2(end.x - start.x, end.y - start.y) * Mathf.Rad2Deg;
	}

}
