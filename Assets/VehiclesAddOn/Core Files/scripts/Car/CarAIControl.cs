using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]
    public class CarAIControl : MonoBehaviour
    {
        public enum BrakeCondition
        {
            NeverBrake,                 // the car simply accelerates at full throttle all the time.
            TargetDirectionDifference,  // the car will brake according to the upcoming change in direction of the target. Useful for route-based AI, slowing for corners.
            TargetDistance,             // the car will brake as it approaches its target, regardless of the target's direction. Useful if you want the car to
                                        // head for a stationary target and come to rest when it arrives there.
        }

        // This script provides input to the car controller in the same way that the user control script does.
        // As such, it is really 'driving' the car, with no special physics or animation tricks to make the car behave properly.

        // "wandering" is used to give the cars a more human, less robotic feel. They can waver slightly
        // in speed and direction while driving towards their target.

        [SerializeField]
        [Range(0, 1)]
        private float m_CautiousSpeedFactor = 0.05f;               // percentage of max speed to use when being maximally cautious
        [SerializeField]
        [Range(0, 180)]
        private float m_CautiousMaxAngle = 50f;                  // angle of approaching corner to treat as warranting maximum caution
        [SerializeField]
        private float m_CautiousMaxDistance = 100f;                              // distance at which distance-based cautiousness begins
        [SerializeField]
        private float m_CautiousAngularVelocityFactor = 30f;                     // how cautious the AI should be when considering its own current angular velocity (i.e. easing off acceleration if spinning!)
        [SerializeField]
        private float m_SteerSensitivity = 0.05f;                                // how sensitively the AI uses steering input to turn to the desired direction
        [SerializeField]
        private float m_AccelSensitivity = 0.04f;                                // How sensitively the AI uses the accelerator to reach the current desired speed
        [SerializeField]
        private float m_BrakeSensitivity = 1f;                                   // How sensitively the AI uses the brake to reach the current desired speed
        [SerializeField]
        private float m_LateralWanderDistance = 3f;                              // how far the car will wander laterally towards its target
        [SerializeField]
        private float m_LateralWanderSpeed = 0.1f;                               // how fast the lateral wandering will fluctuate
        [SerializeField]
        [Range(0, 1)]
        private float m_AccelWanderAmount = 0.1f;                  // how much the cars acceleration will wander
        [SerializeField]
        private float m_AccelWanderSpeed = 0.1f;                                 // how fast the cars acceleration wandering will fluctuate
        [SerializeField]
        private BrakeCondition m_BrakeCondition = BrakeCondition.TargetDistance; // what should the AI consider when accelerating/braking?
        [SerializeField]
        private bool m_Driving;                                                  // whether the AI is currently actively driving or stopped.
        [SerializeField]
        private Transform m_Target;                                              // 'target' the target object to aim for.
        [SerializeField]
        private bool m_StopWhenTargetReached;                                    // should we stop driving when we reach the target?
        [SerializeField]
        private float m_ReachTargetThreshold = 2;                                // proximity to target to consider we 'reached' it, and stop driving.

        private float m_RandomPerlin;             // A random value for the car to base its wander on (so that AI cars don't all wander in the same pattern)
        private CarController m_CarController;    // Reference to actual car controller we are controlling
        private float m_AvoidOtherCarTime;        // time until which to avoid the car we recently collided with
        private float m_AvoidOtherCarSlowdown;    // how much to slow down due to colliding with another car, whilst avoiding
        private float m_AvoidPathOffset;          // direction (-1 or 1) in which to offset path to avoid other car, whilst avoiding
        private Rigidbody m_Rigidbody;
        [Space(10)]
        [Header("=====Reverse Settings=====")]

        public bool reversing = false;
        public float waitToReverse = 2;
        public float reverseCounter = 0;
        public float reverseFor = 1.6f;
        [Space(10)]
        // // // // //
        [Header("======Respawn Settings=====")]
        public float RespawnWaitTime = 5f;
        public float RespawnCounter = 0.0f;
        public Transform SpawnLocation;
        [Space(10)]
        [Header("=====Target Settings======")]
        public float RetargetCounter = 0.0f;
        public float RetargetWaitTime = 5f;
        private GameObject[] TargetList;
        private int index;
        private GameObject targetGO;
        private Transform targetTransform;
        private Transform Original;
        public string TargetTag = "Player";
        private bool isShooting;
        [Space(10)]
        // // // // //
        //=================================================================================
        // new
        //---------------------------------------------------
        [Header("=====Object Avoidance Settings=====")]
        public float frontSensorStartPoint = 2;
        public float sensorLength = 9;
        public float frontSensorAngle = 20;
        public float frontSensorAngle2 = 30;
        public float frontSensorSideDist = 1.44f;
        public float sidewaySensorLength = 1.5f;
        public Transform sensorLocation;
        private Vector3 pos;
        private int flag = 0;
        private bool turnRight;
        private bool turnLeft;
        private bool turnRightAngle;
        private bool turnLeftAngle;
        public float direction = 0;
        private bool LeftOn;
        private bool RightOn;
        public GameObject bulletPrefab;
        public Transform shootspot;
        //
        //-----------------------------------------------------
        //================================================================================



        void Start()
        {
            TargetList = GameObject.FindGameObjectsWithTag(TargetTag);
            Original = m_Target;
        }
        private void Awake()
        {
            // get the car controller reference
            m_CarController = GetComponent<CarController>();

            // give the random perlin a random value
            m_RandomPerlin = Random.value * 100;

            m_Rigidbody = GetComponent<Rigidbody>();
        }

        void TargetRandomPlayer()
        {
            int iRandom = Random.Range(0, 100);
            if (iRandom > 50)
            {
                index = Random.Range(0, TargetList.Length);
                targetGO = TargetList[index];
                if (targetGO != null && targetGO != this.gameObject)
                {
                    targetTransform = targetGO.transform;

                    SetTarget(targetTransform);
                    Debug.Log(this.gameObject.name + " is chasing " + targetGO.name);
                }
            }
            else
            {
                SetTarget(Original);
            }

        }



        //rivate void FixedUpdate()
        void Update()
        {
            flag = 0;


            ReverseAI();
            RespawnOnStuck();
            Sensors();
        }

        void LateUpdate()
        {
            turnRight = false;
            turnLeft = false;
            if (flag == 0)
            {
                // Debug.Log("driving normal");
                Retarget();
                if (m_Target == null || !m_Driving)
                {
                    // Car should not be moving,
                    // use handbrake to stop
                    m_CarController.Move(0, 0, -1f, 1f);
                }
                else
                {
                    Vector3 fwd = transform.forward;
                    if (m_Rigidbody.velocity.magnitude > m_CarController.MaxSpeed * 0.1f)
                    {
                        fwd = m_Rigidbody.velocity;
                    }

                    float desiredSpeed = m_CarController.MaxSpeed;

                    // now it's time to decide if we should be slowing down...
                    switch (m_BrakeCondition)
                    {
                        case BrakeCondition.TargetDirectionDifference:
                            {
                                // the car will brake according to the upcoming change in direction of the target. Useful for route-based AI, slowing for corners.

                                // check out the angle of our target compared to the current direction of the car
                                float approachingCornerAngle = Vector3.Angle(m_Target.forward, fwd);

                                // also consider the current amount we're turning, multiplied up and then compared in the same way as an upcoming corner angle
                                float spinningAngle = m_Rigidbody.angularVelocity.magnitude * m_CautiousAngularVelocityFactor;

                                // if it's different to our current angle, we need to be cautious (i.e. slow down) a certain amount
                                float cautiousnessRequired = Mathf.InverseLerp(0, m_CautiousMaxAngle,
                                                                               Mathf.Max(spinningAngle,
                                                                                         approachingCornerAngle));
                                desiredSpeed = Mathf.Lerp(m_CarController.MaxSpeed, m_CarController.MaxSpeed * m_CautiousSpeedFactor,
                                                          cautiousnessRequired);
                                break;
                            }

                        case BrakeCondition.TargetDistance:
                            {
                                // the car will brake as it approaches its target, regardless of the target's direction. Useful if you want the car to
                                // head for a stationary target and come to rest when it arrives there.

                                // check out the distance to target
                                Vector3 delta = m_Target.position - transform.position;
                                float distanceCautiousFactor = Mathf.InverseLerp(m_CautiousMaxDistance, 0, delta.magnitude);

                                // also consider the current amount we're turning, multiplied up and then compared in the same way as an upcoming corner angle
                                float spinningAngle = m_Rigidbody.angularVelocity.magnitude * m_CautiousAngularVelocityFactor;

                                // if it's different to our current angle, we need to be cautious (i.e. slow down) a certain amount
                                float cautiousnessRequired = Mathf.Max(
                                    Mathf.InverseLerp(0, m_CautiousMaxAngle, spinningAngle), distanceCautiousFactor);
                                desiredSpeed = Mathf.Lerp(m_CarController.MaxSpeed, m_CarController.MaxSpeed * m_CautiousSpeedFactor,
                                                          cautiousnessRequired);
                                break;
                            }

                        case BrakeCondition.NeverBrake:
                            break;
                    }

                    // Evasive action due to collision with other cars:

                    // our target position starts off as the 'real' target position
                    Vector3 offsetTargetPos = m_Target.position;

                    // if are we currently taking evasive action to prevent being stuck against another car:
                    //if (Time.time < m_AvoidOtherCarTime)
                    if (Time.deltaTime < m_AvoidOtherCarTime)
                    {

                        // slow down if necessary (if we were behind the other car when collision occured)
                        desiredSpeed *= m_AvoidOtherCarSlowdown;

                        // and veer towards the side of our path-to-target that is away from the other car
                        offsetTargetPos += m_Target.right * m_AvoidPathOffset;
                    }
                    else
                    {
                        // no need for evasive action, we can just wander across the path-to-target in a random way,
                        // which can help prevent AI from seeming too uniform and robotic in their driving
                        offsetTargetPos += m_Target.right *
                                           //(Mathf.PerlinNoise(Time.time*m_LateralWanderSpeed, m_RandomPerlin)*2 - 1)*
                                           (Mathf.PerlinNoise(Time.deltaTime * m_LateralWanderSpeed, m_RandomPerlin) * 2 - 1) *

                                           m_LateralWanderDistance;
                    }

                    // use different sensitivity depending on whether accelerating or braking:
                    float accelBrakeSensitivity = (desiredSpeed < m_CarController.CurrentSpeed)
                                                      ? m_BrakeSensitivity
                                                      : m_AccelSensitivity;

                    // decide the actual amount of accel/brake input to achieve desired speed.
                    float accel = Mathf.Clamp((desiredSpeed - m_CarController.CurrentSpeed) * accelBrakeSensitivity, -1, 1);

                    // add acceleration 'wander', which also prevents AI from seeming too uniform and robotic in their driving
                    // i.e. increasing the accel wander amount can introduce jostling and bumps between AI cars in a race
                    accel *= (1 - m_AccelWanderAmount) +
                    //(Mathf.PerlinNoise(Time.time*m_AccelWanderSpeed, m_RandomPerlin)*m_AccelWanderAmount);
                    (Mathf.PerlinNoise(Time.deltaTime * m_AccelWanderSpeed, m_RandomPerlin) * m_AccelWanderAmount);

                    // calculate the local-relative position of the target, to steer towards
                    Vector3 localTarget = transform.InverseTransformPoint(offsetTargetPos);

                    // work out the local angle towards the target
                    float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

                    // get the amount of steering needed to aim the car towards the target
                    float steer = Mathf.Clamp(targetAngle * m_SteerSensitivity, -1, 1) * Mathf.Sign(m_CarController.CurrentSpeed);

                    ///////////////////////


                    // feed input to the car controller.
                    if (!reversing)
                    {

                        m_CarController.Move(steer, accel, accel, 0f);

                    }
                    else
                    {

                        m_CarController.Move(0, accel, -accel, 0f);
                        // Debug.Log("StartReverse");

                    }
                    ///////////////////////////////////////////////////////////////////////////////////////

                    // if appropriate, stop driving when we're close enough to the target.
                    if (m_StopWhenTargetReached && localTarget.magnitude < m_ReachTargetThreshold)
                    {
                        m_Driving = false;
                    }
                }
            }
        }



        /////////////////////
        public void Retarget()
        {  // if we are not moving but still driving , then we are stuck on something
           // so we should then move the vehicle to a safe zone location 
            if (m_Rigidbody.velocity.magnitude < 1 && m_Driving)
            { // countdown to our Respawn Time
                RetargetCounter += Time.deltaTime;
                if (RetargetCounter >= RetargetWaitTime)
                { // if we exceed our WaitTime then it is time to reposition the vehicle
                    TargetRandomPlayer();
                    //transform.position = SpawnLocation.position;
                    //Debug.Log("StartRespawn");
                    // reset the respawn counter 
                    RetargetCounter = 0;

                }
            }
        }


        public void RespawnOnStuck()
        {  // if we are not moving but still driving , then we are stuck on something
           // so we should then move the vehicle to a safe zone location 
            if (m_Rigidbody.velocity.magnitude < 1 && m_Driving)
            { // countdown to our Respawn Time
                RespawnCounter += Time.deltaTime;
                if (RespawnCounter >= RespawnWaitTime)
                { // if we exceed our WaitTime then it is time to reposition the vehicle
                    transform.position = SpawnLocation.position;
                    //Debug.Log("StartRespawn");
                    // reset the respawn counter 
                    RespawnCounter = 0;
                    RetargetCounter = 0;
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////
        public void ReverseAI()
        {

            // if we are not moving but still driving and not in reverse, then we are stuck 
            // on something in front of us, so lets reverse for a little bit and see if we
            // can become unstuck
            if (m_Rigidbody.velocity.magnitude < .5f && !reversing && m_Driving)
            {// adjust the countdown for when we should trigger a reverse
                reverseCounter += Time.deltaTime;
            }
            // if we are moving and we are driving, no need to keep counting the Respawn 
            // since we are obviously not stuck
            if (m_Rigidbody.velocity.magnitude > 3 && m_Driving && !reversing)
            {
                RespawnCounter = 0;

                RetargetCounter = 0;

            }
            // if our reverse counter is greater than or equal to our WaitTime to reverse
            // then go ahead and start the reverse function and set reverse counter back 
            // to a zero value
            if (reverseCounter >= waitToReverse)
            {
                reverseCounter = 0;
                RespawnCounter = 0;
                RespawnCounter = 0;
                RetargetCounter = 0;
                reversing = true;

            }

            else if (!reversing) // not sure is we need this, but here in case we do
            {
                //
            }
            // if we are reversing, countdown to our ReverseFor time until 
            // we exceed the value, at which point we can stop reversing and
            // continue to drive as normal
            if (reversing)
            {
                reverseCounter += Time.deltaTime;
                if (reverseCounter >= reverseFor)
                {
                    reverseCounter = 0;
                    reversing = false;
                }
            }
        }

        //////////////////////////////////////////////////////////////

        private void OnCollisionStay(Collision col)
        {
            // detect collision against other cars, so that we can take evasive action
            if (col.rigidbody != null)
            {
                var otherAI = col.rigidbody.GetComponent<CarAIControl>();
                if (otherAI != null)
                {
                    // we'll take evasive action for 1 second
                    // m_AvoidOtherCarTime = Time.time + 1;
                    m_AvoidOtherCarTime = Time.deltaTime + 1;

                    // but who's in front?...
                    if (Vector3.Angle(transform.forward, otherAI.transform.position - transform.position) < 90)
                    {
                        // the other ai is in front, so it is only good manners that we ought to brake...
                        m_AvoidOtherCarSlowdown = 0.5f;
                    }
                    else
                    {
                        // we're in front! ain't slowing down for anybody...
                        m_AvoidOtherCarSlowdown = 1;
                    }

                    // both cars should take evasive action by driving along an offset from the path centre,
                    // away from the other car
                    var otherCarLocalDelta = transform.InverseTransformPoint(otherAI.transform.position);
                    float otherCarAngle = Mathf.Atan2(otherCarLocalDelta.x, otherCarLocalDelta.z);
                    m_AvoidPathOffset = m_LateralWanderDistance * -Mathf.Sign(otherCarAngle);
                }
            }
        }

        public void Sensors()
        {
            if (!reversing)
            {

                RaycastHit hit;
                pos = sensorLocation.position;
                pos += sensorLocation.forward * frontSensorStartPoint;
                Vector3 rightAngle = Quaternion.AngleAxis(frontSensorAngle, sensorLocation.up) * transform.forward;
                Vector3 leftAngle = Quaternion.AngleAxis(-frontSensorAngle, sensorLocation.up) * transform.forward;
                Vector3 rightAngle2 = Quaternion.AngleAxis(frontSensorAngle2, sensorLocation.up) * transform.forward;
                Vector3 leftAngle2 = Quaternion.AngleAxis(-frontSensorAngle2, sensorLocation.up) * transform.forward;



                #region FRONT RIGHT SENSOR
                // FRONT RIGHT SENSOR
                pos += sensorLocation.right * frontSensorSideDist;
                if (Physics.Raycast(pos, sensorLocation.forward, out hit, sensorLength))
                {

                    if (hit.transform.tag != "Terrain" && hit.transform != m_Target.transform)
                    {

                        flag++;
                        Debug.DrawLine(pos, hit.point, Color.red);
                        if (!reversing && !turnRight)
                        {
                            //Debug.Log("turnnleft");
                            turnLeft = true;
                            m_CarController.Move(-45, 90, 0, 0f);
                        }
                    }
                }
                #endregion

                #region FRONT RIGHT ANGLE SENSOR
                //FRONT RIGHT ANGLE SENSOR
                else if (Physics.Raycast(pos, rightAngle, out hit, sensorLength + 5))
                {

                    if (hit.transform.tag != "Terrain" && hit.transform != m_Target.transform)
                    {
                        RightOn = true;
                        LeftOn = false;
                        flag++;
                        if (!reversing && !turnRight)
                        {
                            //Debug.Log("turnnleftANGLE");
                            turnLeft = true;
                            m_CarController.Move(-45, 90, 0, 0f);
                        }
                    }


                    Debug.DrawLine(pos, hit.point, Color.red);
                }
                #endregion

                #region FRONT RIGHT ANGLE SENSOR#2
                //FRONT RIGHT ANGLE SENSOR
                else if (Physics.Raycast(pos, rightAngle2, out hit, sensorLength))
                {

                    if (hit.transform.tag != "Terrain" && hit.transform != m_Target.transform)
                    {
                        RightOn = true;
                        LeftOn = false;
                        flag++;
                        if (!reversing && !turnRight)
                        {
                            //Debug.Log("turnnleftANGLE");
                            turnLeft = true;
                            m_CarController.Move(-45, 90, 0, 0f);
                        }
                    }


                    Debug.DrawLine(pos, hit.point, Color.red);
                }
                #endregion

                #region FRONT LEFT SENSOR
                //FRONT LEFT SENSOR
                pos = sensorLocation.position;
                pos += sensorLocation.forward * frontSensorStartPoint;
                pos -= sensorLocation.right * frontSensorSideDist;
                if (Physics.Raycast(pos, sensorLocation.forward, out hit, sensorLength))
                {
                    if (hit.transform.tag != "Terrain" && hit.transform != m_Target.transform)
                    {
                        flag++;
                        Debug.DrawLine(pos, hit.point, Color.red);
                        if (!reversing && !turnLeft)
                        {
                            //Debug.Log("turnnright");
                            turnRight = true;
                            m_CarController.Move(45, 90, 0, 0f);
                        }
                    }

                }
                #endregion

                #region FRONT LEFT ANGLE SENSOR
                //FRONT LEFT ANGLE SENSOR
                else if (Physics.Raycast(pos, leftAngle, out hit, sensorLength + 5))
                {

                    if (hit.transform.tag != "Terrain" && hit.transform != m_Target.transform)
                    {
                        LeftOn = true;
                        RightOn = false;
                        flag++;
                        Debug.DrawLine(pos, hit.point, Color.red);
                        if (!reversing && !turnLeft)
                        {
                            // Debug.Log("turnnrightANGLE");
                            turnRight = true;
                            m_CarController.Move(45, 90, 0, 0f);
                        }
                    }

                    Debug.DrawLine(pos, hit.point, Color.red);
                }
                #endregion

                #region FRONT LEFT ANGLE SENSOR2
                //FRONT LEFT ANGLE SENSOR
                else if (Physics.Raycast(pos, leftAngle2, out hit, sensorLength))
                {

                    if (hit.transform.tag != "Terrain" && hit.transform != m_Target.transform)
                    {
                        LeftOn = true;
                        RightOn = false;
                        flag++;
                        Debug.DrawLine(pos, hit.point, Color.red);
                        if (!reversing && !turnLeft)
                        {
                            // Debug.Log("turnnrightANGLE");
                            turnRight = true;
                            m_CarController.Move(45, 90, 0, 0f);
                        }
                    }

                    Debug.DrawLine(pos, hit.point, Color.red);
                }
                #endregion

                #region RIGHT SIDE 90 DEGREE SENSOR
                //RIGHT SIDE 90 DEGREE SENSOR
                else if (Physics.Raycast(sensorLocation.position, sensorLocation.right, out hit, sidewaySensorLength))
                {
                    if (hit.transform.tag != "Terrain" && hit.transform != m_Target.transform)
                    {
                        flag++;
                        if (!reversing && !turnRight)
                        {
                            // Debug.Log("turnnleft");
                            turnLeft = true;
                            m_CarController.Move(-45, 90, 50, 0f);
                        }
                    }
                    Debug.DrawLine(transform.position, hit.point, Color.green);
                }
                #endregion

                #region LEFT SIDE 90 DEGREE SENSOR
                //LEFT SIDE 90 DEGREE SENSOR
                else if (Physics.Raycast(sensorLocation.position, -sensorLocation.right, out hit, sidewaySensorLength))
                {
                    if (hit.transform.tag != "Terrain" && hit.transform != m_Target.transform)
                    {
                        flag++;
                        if (!reversing && !turnLeft)
                        {
                            //Debug.Log("turnnright");
                            turnRight = true;
                            m_CarController.Move(45, 90, 50, 0f);
                        }
                    }
                    Debug.DrawLine(transform.position, hit.point, Color.green);
                }
                #endregion

                #region MIDDLE FORWARD SENSOR
                // MIDDLE FORWARD SENSOR
                if (Physics.Raycast(pos, sensorLocation.forward, out hit, sensorLength + 10))
                {
                    if (hit.transform.tag == TargetTag)
                    {
                        if (!isShooting)
                        {
                            isShooting = true;
                            StartCoroutine(Wait(.5f));

                        }
                    }
                    //if (hit.transform.tag != "Terrain" && hit.transform != m_Target.transform)
                    // {
                    //   flag++;
                    //   if (!reversing && !turnRight)
                    //   {
                    // Debug.Log("turnnleft");
                    //       turnLeft = true;
                    //       m_CarController.Move(-35, 90, 0, 0f);
                    //   }
                    // }


                    Debug.DrawLine(pos, hit.point, Color.red);
                }
                #endregion
            }
        }

        public IEnumerator Wait(float waitTime)
        {
            Debug.Log("SHGOOTING");
            GameObject bullet = Instantiate(bulletPrefab, shootspot.position, shootspot.rotation) as GameObject;
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 10000);
            yield return new WaitForSeconds(waitTime);
            isShooting = false;
        }

        public void SetTarget(Transform target)
        {
            m_Target = target;
            m_Driving = true;
        }
    }
}
