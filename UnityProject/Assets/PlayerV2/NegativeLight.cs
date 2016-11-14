using UnityEngine;
using System.Collections;

public class NegativeLight : MonoBehaviour {

	private Light l;
	[SerializeField]
	private Vector4 color;
	
	void Start () {
		l = GetComponent<Light>();
	}
	
	void Update () {
		l.color = new Color(color.x, color.y, color.z, color.w);
	}
}
