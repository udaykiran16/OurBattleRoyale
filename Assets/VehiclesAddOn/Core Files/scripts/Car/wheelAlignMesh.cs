using UnityEngine;
using System.Collections;

public class wheelAlignMesh : MonoBehaviour {


    public Transform WheelMesh;
    public WheelCollider CorrespondingCollider;
    public bool CanSteer;
    // Use this for initialization

	
	// Update is called once per frame
	void Update () {
        if (CanSteer == true)
        {
            WheelMesh.localEulerAngles = new Vector3(WheelMesh.localEulerAngles.x, CorrespondingCollider.steerAngle - WheelMesh.localEulerAngles.z, WheelMesh.localEulerAngles.z);
        }
        WheelMesh.Rotate(CorrespondingCollider.rpm / 60 * 360 * Time.deltaTime, 0, 0);
	
	}
}
