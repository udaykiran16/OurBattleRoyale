using UnityEngine;
using System.Collections;

public class v_car_speedometer : MonoBehaviour {


    static float minAngle = 199;
    static float maxAngle= -20;
    static v_car_speedometer thisSpeedometer;

	// Use this for initialization
	void Start () {
        thisSpeedometer = this;
	}
	
    public static void ShowSpeed(float speed, float min, float max)
    {
        float ang = Mathf.Lerp(minAngle, maxAngle, Mathf.InverseLerp(min, max, speed));
        thisSpeedometer.transform.eulerAngles = new Vector3(0, 0, ang);
    }

}
