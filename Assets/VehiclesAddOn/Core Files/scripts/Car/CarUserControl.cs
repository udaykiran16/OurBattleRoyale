using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;
using Invector.CharacterController;
using Invector;
using UnityEngine.EventSystems;


namespace UnityStandardAssets.Vehicles.Car
{


   
    [RequireComponent(typeof(CarController))]
    public class CarUserControl : MonoBehaviour
    {
        [HideInInspector]
        public V_CarControl carcontrol;
        private CarController m_Car; // the car controller we want to use
        [HideInInspector]
        public float CurrentCarSpeed;
        [HideInInspector]
        public float h;
        [HideInInspector]
        public float v;
        float hbrake;
        private GameObject objPlayer;
        [HideInInspector]
        public bool isFrozen;
        [HideInInspector]
        public Vector3 movement;
        [HideInInspector]
        public Vector3 prevpos;
        [HideInInspector]
        public Vector3 newpos;
        [HideInInspector]
        public Vector3 forwd;


        private void Awake()
        {
            //==get the car controller==//
            m_Car = GetComponent<CarController>();
            carcontrol = GetComponent<V_CarControl>();
        }

        public void Start()
        {
            //==get the game player==//
            if (GetComponentInParent<V_CarControl>().isInGarage == false)
            {
                objPlayer = GetComponent<V_CarControl>().Player;
            }
        }

        void Update()
        {
            newpos = transform.position;
            movement = (newpos - prevpos);

            if (Vector3.Dot(forwd, movement) < 0 && Input.GetAxis("LeftAnalogVertical") < 0)
            {
                GetComponent<V_CarControl>().reversing = true;
            }
            else
            {
                GetComponent<V_CarControl>().reversing = false;
            }
            if (isFrozen)
            {
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
            else
            {
         

                hbrake = CrossPlatformInputManager.GetAxis(carcontrol.s_brake);


 

            }
        }

        private void FixedUpdate()
        {
            prevpos = transform.position;
            forwd = transform.forward;
            if (isFrozen) { return; }
            if (GetComponentInParent<V_CarControl>().isInGarage == true)
            
                { return; }
            //== if our player is currently driving the car they entered then allow input of direction/speed to be allowed from local player==//
            if ((objPlayer.GetComponent<V_car_user>().driving) && (objPlayer.GetComponent<V_car_user>().currCar == this.gameObject))

            {    //==get current speed in MPH of vehicle currently driving==//
                CurrentCarSpeed = GetComponent<CarController>().CurrentSpeed;

                // pass the player input to the car==//
                if (Input.GetAxis("LeftAnalogHorizontal") > 0 || Input.GetAxis("LeftAnalogVertical") > 0 || Input.GetAxis("LeftAnalogHorizontal") < 0 || Input.GetAxis("LeftAnalogVertical") < 0)
                {   
                    //== Using Joystick == //
                    h = Input.GetAxis("LeftAnalogHorizontal");
                    v = Input.GetAxis("LeftAnalogVertical");
                }
                else if (Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Vertical") > 0 || Input.GetAxis("Horizontal") < 0 || Input.GetAxis("Vertical") < 0)
                {
                    //== Using Mouse/Keyboard ==//
                    h = CrossPlatformInputManager.GetAxis("Horizontal");
                    v = CrossPlatformInputManager.GetAxis("Vertical");
                }
                else
                {
                 //== let wheels drift at last value if neither joystick or keyboard is being used==//
                    h = 0.0f;
                    v = 0.0f;
                }

                //==get handbrake value==//
                    float handbrake = CrossPlatformInputManager.GetAxis(carcontrol.s_brake);


                //==if gas button is held down, apply gas to vehicle
                if (Input.GetAxis(carcontrol.s_gas) > 0)
                {

                    m_Car.Move(h, 1, v, hbrake);
                }
                //==or use the forward motion to apply gas==//
                    else
                    {
                        m_Car.Move(h, v, v, handbrake);
                    }
            }
            else
            {  //if gas is not held down and motion is at center, leave car still
                m_Car.Move(0, 0, 0, 1);
            }
        }
    }
    
}

