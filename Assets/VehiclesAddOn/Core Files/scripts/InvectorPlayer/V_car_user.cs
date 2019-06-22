using UnityEngine;
using System.Collections;
using Invector.vCharacterController;
using Invector.CharacterController;
using Invector.vShooter;

public class V_car_user : MonoBehaviour
{

    public vThirdPersonController _charCtrl;  
  

    public GameObject carUI_Prefab;
    public enum ControllerSetup
    {
        LB,
        RB,
        LT,
        RT,
        X,
        Y,
        B,
        A,
        RightStickClick,
        LeftStickClick,
        Start,
        Back,
        Submit

    }
    #region Controller System   
    [System.Serializable]
    public class vControlSystem
    {//controller setups        
        public ControllerSetup EnterVehicle = ControllerSetup.A;
        public ControllerSetup AddFuelToVehicle = ControllerSetup.B;
    }
    [Header("______ Car Enter Setup ___________________________")]
    [Tooltip("Setup the controller enter setup here")]
    public vControlSystem CarControllingSystem;
    #endregion

    #region Door System   
    [System.Serializable]
    public class vDoorSystem
    {//door setups        
        public AudioClip openCarDoor;
        public AudioClip closeCarDoor;
    }
    [Header("______ Door Open/Close SFX Setup ___________________________")]
    [Tooltip("Setup the door open/closesound effect here")]
    public vDoorSystem DoorSystem;
    #endregion

    #region Camera States System   
    [System.Serializable]
    public class vCameraSystem
    {//Camera States Setup       
        public string car_cameraState= "DrivingVehicle";
        public string mediumcar_cameraState = "DrivingMediumVehicle";
        public string largecar_cameraState = "DrivingLargeVehicle";
        public string hugecar_cameraState = "DrivingHugeVehicle";
        public string glider_cameraState = "DrivingGlider";
        public string helicopter_camerastate = "DrivingHelicopter";
        public string boat_camerastate = "DrivingBoat";
        public string jet_camerastate = "DrivingJet";
        public string inGarage_camerastate = "inGarage";
        public string usercustom_camerastate;
    }
    [Header("______ Camera Setup ___________________________")]
    [Tooltip("Setup the Camera States Names here")]
    public vCameraSystem CarCameraStateSystem;
    #endregion

    #region Vehicle Tag System   
    [System.Serializable]
    public class vTagSystem
    {//Camera States Setup       
        public string car_Tag = "carDoor";
        public string glider_Tag = "glider";
        public string Helicopter_Tag = "choppaDoor";
        public string Boat_Tag = "Boat";
        public string Jet_Tag = "Jet";
        public string Usercustom_Tag;
    }
    [Header("______ vehicle TAG Setup ___________________________")]
    [Tooltip("Setup the vehicle TAGS here")]
    public vTagSystem CarTagSystem;
    #endregion

    //
    private string s_entervehicle;
    private bool _getInCar;
    private bool _addFuelToCar;
    private bool _getInLowCar;
    private bool _Gliding;
    private bool _Boating;
    [HideInInspector]
    public bool driving;
    [HideInInspector]
    public bool flying;
    [HideInInspector]
    public bool boating;
    [HideInInspector]
    public bool gliding;
    [HideInInspector]
    public bool inGarage;
    [HideInInspector]
    public bool GlideON;
    [HideInInspector]
    public bool OutOfGas;
    private AudioSource m_AudioSource;
    private int EnterExitLayer;
    private Animator _animator;
    [HideInInspector]
    public GameObject currCar;
    [HideInInspector ]
    public GameObject myHub;
    [HideInInspector ]
    public bool hasFuelinInventory;
    //
    /*
    #region UI System   
    [System.Serializable]
    public class vUISystem
    {//UI Setups        
        public GameObject HUDcontroller;
    }
    [Header("______ Custom UI Setup ___________________________")]
    [Tooltip("Setup the UI components here")]
    public vUISystem UI_System;
    #endregion
    */
    //=================================================================================//

    void Awake()
    {
        if (myHub == null)
        {
            myHub = Instantiate(carUI_Prefab) as GameObject;
        }
        myHub.SetActive(false);
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        
        SetupDriveControls();
        m_AudioSource = gameObject.AddComponent<AudioSource>();
        EnterExitLayer = _animator.GetLayerIndex("EnterExitVehicle");
    }

    void FixedUpdate()
    {
        _animator.SetBool("GetInCar", _getInCar);
        _animator.SetBool("GetInLowCar", _getInLowCar);
        _animator.SetBool("Gliding", _Gliding);
        _animator.SetBool("Boating", _Boating);
        _animator.SetBool("AddFuel", _addFuelToCar);
    }

    void Update()
    {

        //============detection method================================//
        Vector3 ahead = transform.forward;
        Vector3 rayStart = new Vector3(this.transform.position.x, this.transform.position.y + 1f, this.transform.position.z);
        Ray ray = new Ray(rayStart, ahead);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1f))
        {
            #region Detect and enter Car
            //==================================================================================//
            // ENTER CAR ///--------------------------------------------------------------------//
            //==================================================================================//
            if (hit.transform.gameObject.tag == CarTagSystem.car_Tag)
            {
                currCar = hit.transform.gameObject;
                // if player is not driving and current car is valid (within range of raycast)
                if ((currCar != null && !driving))
                {
                    // if we hit the enter key, start the animation for entering vehicle
                    if (Input.GetButton(CarControllingSystem.EnterVehicle.ToString()))
                    {
                        // if this animation should be for a low vehicle, play the get down into vehicle animation
                        if (currCar.GetComponent<V_CarControl>().DoorSystem.lowDoors)
                        {
                            _animator.SetLayerWeight(EnterExitLayer, 1);
                            _getInLowCar = true;
                            if (_charCtrl.enabled)
                            {
                                StartCoroutine(GetInCar());
                            }
                        }
                        // if not a low vehicle, then play the step up into vehicle animation
                        else
                        {
                            _animator.SetLayerWeight(EnterExitLayer, 1);
                            _getInCar = true;
                            if (_charCtrl.enabled)
                            {
                                StartCoroutine(GetInCar());
                            }
                        }
                    }
                    //==============================================================================//
                    // if we hit the add fuel button, check to see if we have fuel and if so, use it
                    if (Input.GetButton(CarControllingSystem.AddFuelToVehicle.ToString()) && hasFuelinInventory)
                    {
                        if (currCar.GetComponent<V_CarControl>().FuelSystem.CurrentFuelAmount < currCar.GetComponent<V_CarControl>().FuelSystem.MaxFuelAmount)
                        {
                            // add fuel to vehicle, but since this isnt a fuel station, we will only fill it up halfway
                            currCar.GetComponent<V_CarControl>().FuelSystem.CurrentFuelAmount = currCar.GetComponent<V_CarControl>().FuelSystem.MaxFuelAmount/2;
                            hasFuelinInventory = false;
                            // to do : add fuel animation
                            // to do : destroy Gas Can object from inventory
                        }
                    }
                    //=====================================================================================//
                }
            }
            #endregion

            #region Detect and enter Glider
            //==================================================================================//
            // ENTER GLIDER ///-----------------------------------------------------------------//
            //==================================================================================//
            if (hit.transform.gameObject.tag == CarTagSystem.glider_Tag)
            {
                currCar = hit.transform.gameObject;
                if ((currCar != null && !flying && hit.transform.GetComponent<V_GliderControl>().gliderBroken == false))
                {
                    if (Input.GetButton(CarControllingSystem.EnterVehicle.ToString()))
                    {
                        _animator.SetLayerWeight(EnterExitLayer, 1);
                        if (_charCtrl.enabled)
                        {
                            StartCoroutine(GetInGlider());
                        }
                    }
                }
            }
            #endregion

            #region Detect and enter Helicopter
            //==================================================================================//
            // ENTER HELICOPTER ///-------------------------------------------------------------//
            //==================================================================================//
            if (hit.transform.gameObject.tag == CarTagSystem.Helicopter_Tag)
            {
                currCar = hit.transform.gameObject;
                if ((currCar != null && !flying))
                {
                    if (Input.GetButton(CarControllingSystem.EnterVehicle.ToString()))
                    {
                        _animator.SetLayerWeight(EnterExitLayer, 1);
                        _getInCar = true;
                        if (_charCtrl.enabled)
                        {
                            StartCoroutine(GetInChoppa());
                        }
                    }
                }
            }
            #endregion

            #region Detect and enter Boat
            //==================================================================================//
            // ENTER BOAT ///-------------------------------------------------------------//
            //==================================================================================//
            if (hit.transform.gameObject.tag == "boatDoor")
            {
                currCar = hit.transform.gameObject;
                if ((currCar != null && !boating))
                {
                    if (Input.GetButton(CarControllingSystem.EnterVehicle.ToString()))
                    {
                        _animator.SetLayerWeight(EnterExitLayer, 1);
                        if (_charCtrl.enabled)
                        {
                            StartCoroutine(GetInBoat());

                        }
                    }
                }
            }
            #endregion

            #region Detect and enter User Custom
            //==================================================================================//
            // ENTER USER CUSTOM ///------------------------------------------------------------//
            //==================================================================================//
            if (hit.transform.gameObject.tag == CarTagSystem.Usercustom_Tag)
            {
                currCar = hit.transform.gameObject;
                if ((currCar != null && !flying))
                {
                    if (Input.GetButton(CarControllingSystem.EnterVehicle.ToString()))
                    {
                        _animator.SetLayerWeight(EnterExitLayer, 1);
                        _getInCar = true;
                        if (_charCtrl.enabled)
                        {
                            StartCoroutine(GetInCustom());
                        }
                    }
                }
            }
            #endregion
        }

        //=========action methods=========================//
        if (inGarage) { return; }
            #region If Driving Vehicle
        //==================================================================================//
        // EXIT CAR ///--------------------------------------------------------------------//
        //==================================================================================//
        if (driving)
            {
                if (Input.GetButtonDown(currCar.GetComponent<V_CarControl>().CarControllingSystem.ExitVehicle.ToString()))
                {
                //GameObject sitPoint = currCar.transform.Find("sitPoint").gameObject;
                GameObject sitPoint = currCar.GetComponent<V_CarControl>().PlayerPositionSystem.sitPoint;
                    this.transform.position = sitPoint.transform.position;
                    this.transform.rotation = sitPoint.transform.rotation;
                    this.transform.parent = null;
                    driving = false;
                    _getInCar = false;
                    _getInLowCar = false;
                    OutOfGas = false;
                    StartCoroutine(GetOutCar());
                }
            }
            if (driving == false && OutOfGas == true && flying == false)
            {
                if (Input.GetButtonDown(currCar.GetComponent<V_CarControl>().CarControllingSystem.ExitVehicle.ToString()))
                {
                    GameObject sitPoint = currCar.transform.Find("sitPoint").gameObject;
                    this.transform.position = sitPoint.transform.position;
                    this.transform.rotation = sitPoint.transform.rotation;
                    this.transform.parent = null;
                    _getInCar = false;
                    _getInLowCar = false;
                    StartCoroutine(GetOutCar());
                }

            }
            #endregion

            #region If Flying Glider
            //==================================================================================//
            // EXIT GLIDER ///------------------------------------------------------------------//
            //==================================================================================//
            if (gliding)
            {
                //===================//
            }
            #endregion

            #region If Flying Helicopter
            //==================================================================================//
            // EXIT HELICOPTER ///--------------------------------------------------------------//
            //==================================================================================//
            if (flying)
            {
                if (Input.GetButtonDown(currCar.GetComponent<v_helicopter_control>().HelicopterControllingSystem.ExitVehicle.ToString()) && GetComponentInParent<v_helicopter_control>().EngineForce <= 0)
                {
                    StartCoroutine(GetOutChoppa());
                    _getInCar = false;
                    _getInLowCar = false;
                }
            }
        #endregion

            #region If Boating
        //==================================================================================//
        // EXIT BOAT ///--------------------------------------------------------------//
        //==================================================================================//
        if (boating)
        {
            if (Input.GetButtonDown(currCar.GetComponent<v_boat_control>().BoatControllingSystem.ExitVehicle.ToString()))
            {
                StartCoroutine(GetOutBoat());
                _getInCar = false;
                _getInLowCar = false;
            }
        }
        #endregion

            #region If Driving User Custom
        //==================================================================================//
        // EXIT USER CUSTOM ///-------------------------------------------------------------//
        //==================================================================================//
        if (driving)
            {
                if (Input.GetButtonDown(currCar.GetComponent<V_CarControl>().CarControllingSystem.ExitVehicle.ToString()))
                {
                    //GameObject sitPoint = currCar.transform.Find("sitPoint").gameObject;
                GameObject sitPoint = currCar.GetComponent<V_CarControl>().PlayerPositionSystem.sitPoint;
                this.transform.position = sitPoint.transform.position;
                    this.transform.rotation = sitPoint.transform.rotation;
                    this.transform.parent = null;
                    driving = false;
                    _getInCar = false;
                    _getInLowCar = false;
                    OutOfGas = false;
                    StartCoroutine(GetOutCustom());
                }
            }
            #endregion
       
    }

    //=================================================================================//
    // ------------------------------FUNCTIONS START-----------------------------------//
    //=================================================================================//

    void openCarDoorSound()
    {
        if (currCar.GetComponent<V_CarControl>() != null)
        {
            if (currCar.GetComponent<V_CarControl>().DoorSystem.OpenDoorSound != null)
            {
                m_AudioSource.clip = currCar.GetComponent<V_CarControl>().DoorSystem.OpenDoorSound;
            }
            else
            {
                m_AudioSource.clip = DoorSystem.openCarDoor;
            }
        }

        else if (currCar.GetComponent<v_helicopter_control>() != null)
        {
            if (currCar.GetComponent<v_helicopter_control>().DoorSystem.OpenDoorSound != null)
            {
                m_AudioSource.clip = currCar.GetComponent<v_helicopter_control>().DoorSystem.OpenDoorSound;
            }
            else
            {
                m_AudioSource.clip = DoorSystem.openCarDoor;
            }
        }

        else
        {
            m_AudioSource.clip = DoorSystem.openCarDoor;
        }
        
        m_AudioSource.PlayOneShot(m_AudioSource.clip);
    }

    void closeCarDoorSound()
    {
        if (currCar.GetComponent<V_CarControl>() != null)
        {
            if (currCar.GetComponent<V_CarControl>().DoorSystem.CloseDoorSound != null)
            {
                m_AudioSource.clip = currCar.GetComponent<V_CarControl>().DoorSystem.CloseDoorSound;
            }
            else
            {
                m_AudioSource.clip = DoorSystem.closeCarDoor;
            }
        }

        else if (currCar.GetComponent<v_helicopter_control>() != null)
        {
            if (currCar.GetComponent<v_helicopter_control>().DoorSystem.CloseDoorSound != null)
            {
                m_AudioSource.clip = currCar.GetComponent<v_helicopter_control>().DoorSystem.CloseDoorSound;
            }
            else
            {
                m_AudioSource.clip = DoorSystem.closeCarDoor;
            }
        }

        else
        {
            m_AudioSource.clip = DoorSystem.closeCarDoor;
        }
  
        m_AudioSource.PlayOneShot(m_AudioSource.clip);
    }

    //=================================================================================//
    // ------------------------------CAR ENTER/EXIT------------------------------------//
    //=================================================================================//

    public IEnumerator GetInCar()
    {
        _charCtrl.enabled = false;
        
        this.GetComponent<Rigidbody>().isKinematic = true;
        this.GetComponent<Rigidbody>().useGravity = false;
        this.GetComponent<Rigidbody>().detectCollisions = false;
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        this.GetComponent<v_car_camera_control>().CarCameraOff = false;
        
        this.GetComponent<v_car_camera_control>().isdriving = true;
        if (this.GetComponent<vShooterMeleeInput>() != null)
        {
            this.GetComponent<vShooterMeleeInput>().enabled = false;
        }
        if (this.GetComponent<vHeadTrack>() != null)
        {
            this.GetComponent<vHeadTrack>().enabled = false;
        }
        GameObject sitPoint = currCar.GetComponent<V_CarControl>().PlayerPositionSystem.sitPoint;
        //GameObject sitPoint = currCar.transform.Find("sitPoint").gameObject;
        
        currCar.GetComponent<V_CarControl>().DoorSystem.openDoor = true;
        this.transform.parent = sitPoint.transform;
        this.transform.position = sitPoint.transform.position;
        this.transform.rotation = sitPoint.transform.rotation;
        yield return new WaitForSeconds(4.5f);
        driving = true;
        currCar.GetComponent<V_CarControl>().DoorSystem.openDoor = false;
        //Transform Seat = currCar.transform.Find("seat");
        Transform Seat = currCar.GetComponent<V_CarControl>().PlayerPositionSystem.Seat;
        this.transform.parent = Seat.transform;
        this.transform.position = Seat.transform.position;
        this.transform.rotation = Seat.transform.rotation;
    }

    public IEnumerator GetOutCar()
    {

        //Transform Seat = currCar.transform.Find("seat");
        Transform Seat = currCar.GetComponent<V_CarControl>().PlayerPositionSystem.Seat;
        this.transform.parent = Seat.transform;
        this.transform.position = Seat.transform.position;
        this.transform.rotation = Seat.transform.rotation;
       // GameObject sitPoint = currCar.transform.Find("sitPoint").gameObject;
        GameObject sitPoint = currCar.GetComponent<V_CarControl>().PlayerPositionSystem.sitPoint;

        currCar.GetComponent<V_CarControl>().DoorSystem.openDoor = true;
        yield return new WaitForSeconds(4.5f);
        this.GetComponent<v_car_camera_control>().isdriving = false;
       
        this.GetComponent<v_car_camera_control>().CarCameraOn = false;
        this.GetComponent<v_car_camera_control>().CarCameraOff = true;
        if (this.GetComponent<vShooterMeleeInput>() != null)
        {
            this.GetComponent<vShooterMeleeInput>().enabled = true;
        }
        if (this.GetComponent<vHeadTrack>() != null)
        {
            this.GetComponent<vHeadTrack>().enabled = true;
        }
        driving = false;
        currCar.GetComponent<V_CarControl>().DoorSystem.openDoor = false;
        this.transform.position = sitPoint.transform.position;
        this.transform.rotation = sitPoint.transform.rotation;
        this.GetComponent<Rigidbody>().isKinematic = false;
        this.GetComponent<Rigidbody>().useGravity = true;
        this.GetComponent<Rigidbody>().detectCollisions = true;
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        sitPoint = null;
        Seat = null;
        this.transform.parent = null;
        OutOfGas = false;
        _charCtrl.enabled = true;
    }


    //=================================================================================//
    // ------------------------------HELICOPTER ENTER/EXIT-----------------------------//
    //=================================================================================//

    public IEnumerator GetInChoppa()
    {
        this.GetComponent<Rigidbody>().isKinematic = true;
        this.GetComponent<Rigidbody>().useGravity = false;
        this.GetComponent<Rigidbody>().detectCollisions = false;
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        _charCtrl.enabled = false;
        if (this.GetComponent<vMeleeCombatInput>() != null)
        {
            this.GetComponent<vMeleeCombatInput>().enabled = false;
        }
        if (this.GetComponent<vHeadTrack>() != null)
        {
            this.GetComponent<vHeadTrack>().enabled = false;
        }
        this.GetComponent<v_car_camera_control>().isdriving = true;
        GameObject sitPoint = currCar.GetComponent<v_helicopter_control>().PlayerPositionSystem.sitPoint;
       // GameObject sitPoint = currCar.transform.Find("sitPoint").gameObject;
        currCar.GetComponent<v_helicopter_control>().DoorSystem.openDoor = true;
        this.transform.parent = sitPoint.transform;
        this.transform.position = sitPoint.transform.position;
        this.transform.rotation = sitPoint.transform.rotation;
        yield return new WaitForSeconds(2.8f);
        flying = true;
        currCar.GetComponent<v_helicopter_control>().DoorSystem.openDoor = false;
        Transform Seat = currCar.GetComponent<v_helicopter_control>().PlayerPositionSystem.Seat;
       // Transform Seat = currCar.transform.Find("seat");
        this.transform.parent = Seat.transform;
        this.transform.position = Seat.transform.position;
        this.transform.rotation = Seat.transform.rotation;
    }

    public IEnumerator GetOutChoppa()
    {
        Transform Seat = currCar.GetComponent<v_helicopter_control>().PlayerPositionSystem.Seat;
        //Transform Seat = currCar.transform.Find("seat");
        this.transform.parent = Seat.transform;
        this.transform.position = Seat.transform.position;
        this.transform.rotation = Seat.transform.rotation;
        GameObject sitPoint = currCar.GetComponent<v_helicopter_control>().PlayerPositionSystem.sitPoint;
        //GameObject sitPoint = currCar.transform.Find("sitPoint").gameObject;
        currCar.GetComponent<v_helicopter_control>().DoorSystem.openDoor = true;
        yield return new WaitForSeconds(2.1f);
        this.GetComponent<v_car_camera_control>().isdriving = false;
        _charCtrl.enabled = true;
        this.GetComponent<v_car_camera_control>().CarCameraOn = false;
        this.GetComponent<v_car_camera_control>().CarCameraOff = true;
        if (this.GetComponent<vMeleeCombatInput>() != null)
        {
            this.GetComponent<vMeleeCombatInput>().enabled = true;
        }
        if (this.GetComponent<vHeadTrack>() != null)
        {
            this.GetComponent<vHeadTrack>().enabled = true;
        }
        flying = false;
        currCar.GetComponent<v_helicopter_control>().DoorSystem.openDoor = false;
        this.transform.position = sitPoint.transform.position;
        this.transform.rotation = sitPoint.transform.rotation;
        this.GetComponent<Rigidbody>().isKinematic = false;
        this.GetComponent<Rigidbody>().useGravity = true;
        this.GetComponent<Rigidbody>().detectCollisions = true;
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        sitPoint = null;
        Seat = null;
        this.transform.parent = null;
    }

    //=================================================================================//
    // ------------------------------GLIDER ENTER/EXIT---------------------------------//
    //=================================================================================//

    public IEnumerator GetInGlider()
    {
        this.GetComponent<Rigidbody>().isKinematic = true;
        this.GetComponent<Rigidbody>().useGravity = false;
        this.GetComponent<Rigidbody>().detectCollisions = false;
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        _charCtrl.enabled = false;
        this.GetComponent<v_car_camera_control>().isdriving = true;
        if (this.GetComponent<vMeleeCombatInput>() != null)
        {
            this.GetComponent<vMeleeCombatInput>().enabled = false;
        }
        if (this.GetComponent<vHeadTrack>() != null)
        {
            this.GetComponent<vHeadTrack>().enabled = false;
        }
        GameObject sitPoint = currCar.GetComponent<V_GliderControl>().PlayerPositionSystem.sitPoint;
        //GameObject sitPoint = currCar.transform.Find("sitPoint").gameObject;
        this.transform.parent = sitPoint.transform;
        this.transform.position = sitPoint.transform.position;
        this.transform.rotation = sitPoint.transform.rotation;
        yield return new WaitForSeconds(0.1f);
        gliding = true;
        _Gliding = true;
        yield return new WaitForSeconds(1.0f);
        GlideON = true;

    }

    public IEnumerator GetOutGlider()
    {
        GameObject sitPoint = currCar.GetComponent<V_GliderControl>().PlayerPositionSystem.sitPoint;
        //GameObject sitPoint = currCar.transform.Find("sitPoint").gameObject;
        yield return new WaitForSeconds(0.5f);
        this.GetComponent<v_car_camera_control>().isdriving = false;
        _charCtrl.enabled = true;
        this.GetComponent<v_car_camera_control>().CarCameraOn = false;
        this.GetComponent<v_car_camera_control>().CarCameraOff = true;

        gliding = false;
        _Gliding = false;
        GlideON = false;
        this.transform.position = sitPoint.transform.position;
        this.transform.rotation = sitPoint.transform.rotation;
        this.GetComponent<Rigidbody>().isKinematic = false;
        this.GetComponent<Rigidbody>().useGravity = true;
        this.GetComponent<Rigidbody>().detectCollisions = true;
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        sitPoint = null;
        this.transform.parent = null;
        if (this.GetComponent<vMeleeCombatInput>() != null)
        {
            this.GetComponent<vMeleeCombatInput>().enabled = true;
        }
        if (this.GetComponent<vHeadTrack>() != null)
        {
            this.GetComponent<vHeadTrack>().enabled = true;
        }

    }

    public IEnumerator SafeOutGlider()
    {
        GameObject sitPoint = currCar.GetComponent<V_GliderControl>().PlayerPositionSystem.sitPoint;
        //GameObject sitPoint = currCar.transform.Find("sitPoint").gameObject;
        yield return new WaitForSeconds(0.5f);
        this.GetComponent<v_car_camera_control>().isdriving = false;
        _charCtrl.enabled = true;
        this.GetComponent<v_car_camera_control>().CarCameraOn = false;
        this.GetComponent<v_car_camera_control>().CarCameraOff = true;
        if (this.GetComponent<vMeleeCombatInput>() != null)
        {
            this.GetComponent<vMeleeCombatInput>().enabled = true;
        }
        if (this.GetComponent<vHeadTrack>() != null)
        {
            this.GetComponent<vHeadTrack>().enabled = true;
        }
        gliding = false;
        _Gliding = false;
        this.transform.position = sitPoint.transform.position;
        this.transform.rotation = sitPoint.transform.rotation;
        this.GetComponent<Rigidbody>().isKinematic = false;
        this.GetComponent<Rigidbody>().useGravity = true;
        this.GetComponent<Rigidbody>().detectCollisions = true;
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        sitPoint = null;
        this.transform.parent = null;


    }

    //=================================================================================//
    // ------------------------------BOAT ENTER/EXIT----------------------------//
    //=================================================================================//

    IEnumerator GetInBoat()
    {
        this.GetComponent<Rigidbody>().isKinematic = true;
        this.GetComponent<Rigidbody>().useGravity = false;
        this.GetComponent<Rigidbody>().detectCollisions = false;
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        this.GetComponent<v_car_camera_control>().CarCameraOff = false;
        _charCtrl.enabled = false;
        
        this.GetComponent<v_car_camera_control>().isdriving = true;
        if (this.GetComponent<vMeleeCombatInput>() != null)
        {
            this.GetComponent<vMeleeCombatInput>().enabled = false;
        }
        if (this.GetComponent<vHeadTrack>() != null)
        {
            this.GetComponent<vHeadTrack>().enabled = false;
        }
        //GameObject sitPoint = currCar.transform.Find("sitPoint").gameObject;
        GameObject sitPoint = currCar.GetComponent<v_boat_control>().PlayerPositionSystem.sitPoint;
        //currCar.GetComponent<V_CarControl>().DoorSystem.openDoor = true;
        this.transform.parent = sitPoint.transform;
        this.transform.position = sitPoint.transform.position;
        this.transform.rotation = sitPoint.transform.rotation;
        yield return new WaitForSeconds(4.5f);
        boating = true;
        _Boating = true;
        //currCar.GetComponent<V_CarControl>().DoorSystem.openDoor = false;
        Transform Seat = currCar.GetComponent<v_boat_control>().PlayerPositionSystem.Seat;
        //Transform Seat = currCar.transform.Find("seat");
        this.transform.parent = Seat.transform;
        this.transform.position = Seat.transform.position;
        this.transform.rotation = Seat.transform.rotation;
        this.transform.localScale = new Vector3(.1f, .1f, .1f);
        //
    }

    IEnumerator GetOutBoat()
    {
       
        Transform Seat = currCar.GetComponent<v_boat_control>().PlayerPositionSystem.Seat;
       // Transform Seat = currCar.transform.Find("seat");
        this.transform.parent = Seat.transform;
        this.transform.position = Seat.transform.position;
        this.transform.rotation = Seat.transform.rotation;
        GameObject sitPoint = currCar.GetComponent<v_boat_control>().BoatExitPoint;
        //currCar.GetComponent<V_CarControl>().DoorSystem.openDoor = true;
        yield return new WaitForSeconds(4.5f);
        this.GetComponent<v_car_camera_control>().isdriving = false;
        _charCtrl.enabled = true;
        this.GetComponent<v_car_camera_control>().CarCameraOn = false;
        this.GetComponent<v_car_camera_control>().CarCameraOff = true;
        if (this.GetComponent<vMeleeCombatInput>() != null)
        {
            this.GetComponent<vMeleeCombatInput>().enabled = true;
        }
        if (this.GetComponent<vHeadTrack>() != null)
        {
            this.GetComponent<vHeadTrack>().enabled = true;
        }
        boating = false;
        _Boating = false;
        //currCar.GetComponent<V_CarControl>().DoorSystem.openDoor = false;
        this.transform.position = sitPoint.transform.position;
        this.transform.rotation = sitPoint.transform.rotation;
        this.GetComponent<Rigidbody>().isKinematic = false;
        this.GetComponent<Rigidbody>().useGravity = true;
        this.GetComponent<Rigidbody>().detectCollisions = true;
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        sitPoint = null;
        Seat = null;
        this.transform.parent = null;
        this.transform.localScale = new Vector3(1, 1, 1);
    }

    //=================================================================================//
    // ------------------------------CUSTOM USER ENTER/EXIT----------------------------//
    //=================================================================================//

    public IEnumerator GetInCustom()
    {
        this.GetComponent<Rigidbody>().isKinematic = true;
        this.GetComponent<Rigidbody>().useGravity = false;
        this.GetComponent<Rigidbody>().detectCollisions = false;
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        this.GetComponent<v_car_camera_control>().CarCameraOff = false;
        _charCtrl.enabled = false;
        this.GetComponent<v_car_camera_control>().isdriving = true;
        if (this.GetComponent<vMeleeCombatInput>() != null)
        {
            this.GetComponent<vMeleeCombatInput>().enabled = false;
        }
        if (this.GetComponent<vHeadTrack>() != null)
        {
            this.GetComponent<vHeadTrack>().enabled = false;
        }
        GameObject sitPoint = currCar.transform.Find("sitPoint").gameObject;
        currCar.GetComponent<V_CarControl>().DoorSystem.openDoor = true;
        this.transform.parent = sitPoint.transform;
        this.transform.position = sitPoint.transform.position;
        this.transform.rotation = sitPoint.transform.rotation;
        yield return new WaitForSeconds(4.5f);
        driving = true;
        currCar.GetComponent<V_CarControl>().DoorSystem.openDoor = false;
        Transform Seat = currCar.transform.Find("seat");
        this.transform.parent = Seat.transform;
        this.transform.position = Seat.transform.position;
        this.transform.rotation = Seat.transform.rotation;
    }

    public IEnumerator GetOutCustom()
    {
        Transform Seat = currCar.transform.Find("seat");
        this.transform.parent = Seat.transform;
        this.transform.position = Seat.transform.position;
        this.transform.rotation = Seat.transform.rotation;
        GameObject sitPoint = currCar.transform.Find("sitPoint").gameObject;
        currCar.GetComponent<V_CarControl>().DoorSystem.openDoor = true;
        yield return new WaitForSeconds(4.5f);
        this.GetComponent<v_car_camera_control>().isdriving = false;
        _charCtrl.enabled = true;
        this.GetComponent<v_car_camera_control>().CarCameraOn = false;
        this.GetComponent<v_car_camera_control>().CarCameraOff = true;
        if (this.GetComponent<vMeleeCombatInput>() != null)
        {
            this.GetComponent<vMeleeCombatInput>().enabled = true;
        }
        if (this.GetComponent<vHeadTrack>() != null)
        {
            this.GetComponent<vHeadTrack>().enabled = true;
        }
        driving = false;
        currCar.GetComponent<V_CarControl>().DoorSystem.openDoor = false;
        this.transform.position = sitPoint.transform.position;
        this.transform.rotation = sitPoint.transform.rotation;
        this.GetComponent<Rigidbody>().isKinematic = false;
        this.GetComponent<Rigidbody>().useGravity = true;
        this.GetComponent<Rigidbody>().detectCollisions = true;
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        sitPoint = null;
        Seat = null;
        this.transform.parent = null;
        OutOfGas = false;
    }


    //=================================================================================//
    // --------------------------------------------------------------------------------//
    //=================================================================================//



    void SetupDriveControls()
    {
        s_entervehicle = CarControllingSystem.EnterVehicle.ToString();
    }


    //=================================================================================//
    // ------------------------------FUNCTIONS END-------------------------------------//
    //=================================================================================//

}
