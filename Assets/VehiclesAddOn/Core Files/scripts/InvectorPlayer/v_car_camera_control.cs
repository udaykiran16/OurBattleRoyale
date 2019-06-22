using UnityEngine;
using System.Collections;
using Invector.vCharacterController;
namespace Invector.CharacterController
{
    public class v_car_camera_control : vMeleeCombatInput
    {
        [HideInInspector]
        public bool isdriving;
        [HideInInspector]
        public bool CarCameraOn;
        [HideInInspector]
        public bool CarCameraOff;
        private bool doCamOnce;
        private string lastState;

         void Update()
        {
            if (this.GetComponentInParent<vThirdPersonMotor>().isGrounded == false)
            {
                
                return;
            }
        }
        void LateUpdate()
        {
     
            #region If Player is not driving 

                #region If Vehicle is in Garage 
                if (this.GetComponentInParent<V_car_user>().inGarage && doCamOnce == true)
            {
                lastState = tpCamera.currentStateName;
                tpCamera.ChangeState(this.GetComponentInParent<V_car_user>().CarCameraStateSystem.inGarage_camerastate, true);
                doCamOnce = false;
            }
            #endregion

            #region If Vehicle is not in Garage 
            if (this.GetComponentInParent<V_car_user>().inGarage == false &&  doCamOnce == false)
            {
                if (tpCamera)
                {
                    tpCamera.ChangeState(this.GetComponentInParent<V_car_user>().CarCameraStateSystem.inGarage_camerastate, false);
                    tpCamera.ChangeState(lastState, true);

                    doCamOnce = true;
                }

            }
            #endregion

            #endregion

            //========================================================================//
            #region If Player is driving 
            //========================================================================//
            if (isdriving && this.GetComponentInParent<V_car_user>().inGarage == false)
            {   
                // get player input    
              //  CameraInputVehicle();
                this.GetComponent<vThirdPersonInput>().CameraInput();
                // initialize just once, so have a bool check here
                if (CarCameraOn == false) 
                    {
                    // if the player has the v_car_user component (which they should)
                    if (this.GetComponentInParent<V_car_user>().currCar.GetComponent<V_CarControl>() != null )
                        {
                        // turn off any residual camera states from Garage mode
                        tpCamera.ChangeState(this.GetComponentInParent<V_car_user>().CarCameraStateSystem.inGarage_camerastate, false);
                        
                        #region Large Vehicle Camera State
                        // if this vehicle is of the LARGE variety
                        if (this.GetComponentInParent<V_car_user>().currCar.GetComponent<V_CarControl>().CarCamStateSystem.isLargeVehicle)
                        {// set target of camera to vehicle instead of player (looks better)
                            tpCamera.SetTarget(this.GetComponentInParent<V_car_user>().currCar.transform);
                            tpCamera.ChangeState(this.GetComponentInParent<V_car_user>().CarCameraStateSystem.largecar_cameraState, true);
                            }
                        #endregion

                        #region Huge Vehicle Camera State
                        // if this vehicle is of the HUGE variety
                        else if (this.GetComponentInParent<V_car_user>().currCar.GetComponent<V_CarControl>().CarCamStateSystem.isHugeVehicle)
                            {
                            // set target of camera to vehicle instead of player (looks better)
                            tpCamera.SetTarget(this.GetComponentInParent<V_car_user>().currCar.transform);
                            tpCamera.ChangeState(this.GetComponentInParent<V_car_user>().CarCameraStateSystem.hugecar_cameraState, true);
                            }
                        #endregion

                        #region Medium Vehicle Camera State
                        // if this vehicle is of the MEDIUM variety
                        else if (this.GetComponentInParent<V_car_user>().currCar.GetComponent<V_CarControl>().CarCamStateSystem.isMediumVehicle)
                            {
                            // set target of camera to vehicle instead of player (looks better)
                            tpCamera.SetTarget(this.GetComponentInParent<V_car_user>().currCar.transform);
                            tpCamera.ChangeState(this.GetComponentInParent<V_car_user>().CarCameraStateSystem.mediumcar_cameraState, true);
                            }
                        #endregion

                        #region Default Vehicle Camera State
                        // if this vehicle is of the default car variety
                        else
                        {
                            // set target of camera to vehicle instead of player (looks better)
                            tpCamera.SetTarget(this.GetComponentInParent<V_car_user>().currCar.transform);
                            tpCamera.ChangeState(this.GetComponentInParent<V_car_user>().CarCameraStateSystem.car_cameraState, true);
                            }
                        #endregion
                    }

                    #region Glider Camera State
                    // if using Glider Control
                    else if (this.GetComponentInParent<V_car_user>().currCar.GetComponent<V_GliderControl>() != null)
                        {
                        // set target of camera to vehicle instead of player (looks better)
                        tpCamera.SetTarget(this.GetComponentInParent<V_car_user>().currCar.transform);
                        tpCamera.ChangeState(this.GetComponentInParent<V_car_user>().CarCameraStateSystem.glider_cameraState, true);
                        }
                    #endregion

                    #region Helicopter Camera State
                    // if using Helicopter Control
                    else if (this.GetComponentInParent<V_car_user>().currCar.GetComponent<v_helicopter_control>() != null)
                        {
                        // set target of camera to vehicle instead of player (looks better)
                        tpCamera.SetTarget(this.GetComponentInParent<V_car_user>().currCar.transform);
                        tpCamera.ChangeState(this.GetComponentInParent<V_car_user>().CarCameraStateSystem.helicopter_camerastate, true);
                        }
                    #endregion

                    #region Boat Camera State
                    // if using Boat Control
                    else if (this.GetComponentInParent<V_car_user>().currCar.GetComponent<v_boat_control>() != null)
                        {
                        
                        tpCamera.ChangeState(this.GetComponentInParent<V_car_user>().CarCameraStateSystem.boat_camerastate , true);
                        tpCamera.target = this.GetComponentInParent<V_car_user>().currCar.transform.Find("CameraState");
                    }
                    #endregion

                    // set bool so we dont trigger this again until we exit car
                    CarCameraOn = true;
                    }

                #region If LockOn triggered ON
                if (Input.GetButtonDown("RightStickClick") && tpCamera.lockCamera == false || (Input.GetKeyDown(KeyCode.Tab) && tpCamera.lockCamera == false))
                    {
                        tpCamera.lockCamera = true; // lock camera onto vehicle
                    }
                #endregion

                #region If LockOn triggered OFF
                else if (Input.GetButtonDown("RightStickClick") && tpCamera.lockCamera == true || (Input.GetKeyDown(KeyCode.Tab) && tpCamera.lockCamera == true))
                    {
                        tpCamera.lockCamera = false; //unlock camera from vehicle
                    }
                #endregion

                #region If Zoom IN triggered
                if (Input.GetButton("LeftStickClick") && (Input.GetAxis("LeftAnalogVertical") > 0))
                    {
                        tpCamera.Zoom(+1 * Time.deltaTime); // allow zoom in with joystick (mouse is already handled by default)
                    }
                #endregion

                #region If Zoom OUT triggered
                else if (Input.GetButton("LeftStickClick") && (Input.GetAxis("LeftAnalogVertical") <= 0))
                    {
                        tpCamera.Zoom(-1 * Time.deltaTime); // allow zoom out with joystick (mouse is already handled by default)
                    }
                #endregion


            }
            #endregion

            #region If Player exits vehicle
            else //not driving, so go back to default camera
            {
                    if (CarCameraOff == true)
                    {
                        tpCamera.SetTarget(this.transform);
  //
                        // if using Car Control
                        if (this.GetComponentInParent<V_car_user>().currCar.GetComponent<V_CarControl>() != null && this.GetComponentInParent<V_car_user>().inGarage == false)
                        {
                            // if this vehicle is of the LARGE variety
                            if (this.GetComponentInParent<V_car_user>().currCar.GetComponent<V_CarControl>().CarCamStateSystem.isLargeVehicle)
                            {
                                tpCamera.ChangeState(this.GetComponentInParent<V_car_user>().CarCameraStateSystem.largecar_cameraState, false);
                            }
                            // if this vehicle is of the HUGE variety
                            else if (this.GetComponentInParent<V_car_user>().currCar.GetComponent<V_CarControl>().CarCamStateSystem.isHugeVehicle)
                            {
                                tpCamera.ChangeState(this.GetComponentInParent<V_car_user>().CarCameraStateSystem.hugecar_cameraState, false);
                            }
                            // if this vehicle is of the MEDIUM variety
                            else if (this.GetComponentInParent<V_car_user>().currCar.GetComponent<V_CarControl>().CarCamStateSystem.isMediumVehicle)
                            {
                            tpCamera.ChangeState(this.GetComponentInParent<V_car_user>().CarCameraStateSystem.mediumcar_cameraState, false);
                            }
                            // if this vehicle is of the default car variety
                            else
                            {
                                tpCamera.ChangeState(this.GetComponentInParent<V_car_user>().CarCameraStateSystem.car_cameraState, false);
                            }
                        }
                        // if using Glider Control
                        else if (this.GetComponentInParent<V_car_user>().currCar.GetComponent<V_GliderControl>() != null)
                        {
                            tpCamera.ChangeState(this.GetComponentInParent<V_car_user>().CarCameraStateSystem.glider_cameraState, false);
                        }
                        // if using Helicopter Control
                        else if (this.GetComponentInParent<V_car_user>().currCar.GetComponent<v_helicopter_control>() != null)
                        {
                            tpCamera.ChangeState(this.GetComponentInParent<V_car_user>().CarCameraStateSystem.helicopter_camerastate, false);
                        }
                        // if using Boat Control
                        else if (this.GetComponentInParent<V_car_user>().currCar.GetComponent<v_boat_control>() != null)
                        {
                            tpCamera.ChangeState(this.GetComponentInParent<V_car_user>().CarCameraStateSystem.boat_camerastate , false);
                        }

                    CarCameraOff = false;
                        tpCamera.lockCamera = false;
                    }
                }

            #endregion

        }

        void CameraInputVehicle()
        {
            //borrowed from Malbers invector code for his horse pro asset merge. Posted on invectors website

             

            var Y = rotateCameraYInput.GetAxis();
            var X = rotateCameraXInput.GetAxis();
            var zoom = cameraZoomInput.GetAxis();


            tpCamera.RotateCamera(X, Y);
            tpCamera.Zoom(zoom);

            // transform Character direction from camera if not KeepDirection
            if (!keepDirection)
                cc.UpdateTargetDirection(tpCamera != null ? tpCamera.transform : null);

            // rotate the character with the camera while strafing    
            // RotateWithCamera(tpCamera != null ? tpCamera.transform : null);

            // change keedDirection from input diference
            if (keepDirection && Vector2.Distance(cc.input, oldInput) > 0.2f) keepDirection = true;
        }
    }
}