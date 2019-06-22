using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Vehicles.Car;
using UnityEngine.UI;

public class V_CarControl : MonoBehaviour
{
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

    // The Player must be defined here, for ease of use I have it as a simple variable to be filled
    // in with the Unity Inspector, however you can define it any way that you wish, you only have to
    // define it here and all vehicles using the CarControl script will update to the new gameObject
    [Tooltip("This should contain your Invector 3rd person Controller. This is vital for the system to work")]
    [Space(10)]
    public GameObject Player;// the Player (this is vital for the system to work)
    
    #region Camera States System   
    //=======Camera States Setup=================================//
    [System.Serializable]
    public class vCamStatesSystem
    {//cmaera state setups  
     //---------------------------------------------------------------------------------------------//
     // Custom Camera States for different types of vehicles, since some vehicles are small and     //
     // others large, it was difficult to have one camera state work for all of them, this helps    //
     // with that by having several options for varying degrees of car heights/lengths.             //
     //=============================================================================================//
    [Tooltip("if checked this will trigger the Invector Camera State defined on the [V_car_user] component on the Player Character, you can custom define this in the Unity Inspector under ['Mediumcar_cameraState']  in the [Camera Setup])] category of ['V-car_user']")]
    public bool isMediumVehicle; // is car of the medium variety ex: Hearse, ScionXL, etc..
    [Tooltip("if checked this will trigger the Invector Camera State defined on the [V_car_user] component on the Player Character, you can custom define this in the Unity Inspector under ['Largecar_cameraState']  in the [Camera Setup])] category of ['V-car_user']")]
    public bool isLargeVehicle; // is car of the large variety ex: Monster Truck, Dump Truck, etc..
    [Tooltip("if checked this will trigger the Invector Camera State defined on the [V_car_user] component on the Player Character, you can custom define this in the Unity Inspector under ['Hugecar_cameraState']  in the [Camera Setup])] category of ['V-car_user']")]
    public bool isHugeVehicle; // is car of the huge variety ex: Trailer Trucks, Trains, etc..
    }
    [Header("______ Camera State Setup ___________________________")]
    [Tooltip("Define which camera state to use for your vehicle here, default is small car (nothing checked)")]
    public vCamStatesSystem CarCamStateSystem;
    //===================================================================//
    #endregion

    #region Controller System   
    //=======Game Controller Setup=================================//
    [System.Serializable]
    public class vControlSystem
    {//controller setups        
        public ControllerSetup Gas = ControllerSetup.X;
        public ControllerSetup Brake = ControllerSetup.A;
        public ControllerSetup HandBrake = ControllerSetup.B;
        public ControllerSetup Turbo = ControllerSetup.Y;
        public ControllerSetup StandardGun = ControllerSetup.LB;
        public ControllerSetup SpecialWeapon = ControllerSetup.RB;
        public ControllerSetup SelectedWeapon1 = ControllerSetup.RT;
        public ControllerSetup SelectedWeapon2 = ControllerSetup.LT;
        public ControllerSetup LightsOnOff = ControllerSetup.Back;
        public ControllerSetup ChaseCameraOnOff = ControllerSetup.RightStickClick;
        public ControllerSetup ExitVehicle = ControllerSetup.Start;
        
    }
    [Header("______ Car Controller Setup ___________________________")]
    [Tooltip("Setup the controller input/setup here")]
    public vControlSystem CarControllingSystem;
    //===================================================================//
    #endregion

    #region PlayerPositioning System   
    [System.Serializable]
    public class vPlayerPositionSystem
    {//Player Position systems
        public Transform Seat;
        public GameObject sitPoint;
    }
    [Tooltip("This handles all the Player Positioning components")]
    [Header("______Player Positioning System ___________________________")]
    public vPlayerPositionSystem PlayerPositionSystem;
    #endregion

    #region Armor System 
    //===========Armor System==============================================//  
    [System.Serializable]
    public class vArmorSystem
    {//armor system
        public int MaxArmor;
        public int currentArmor;
        public GameObject DestructPrefab;
        [HideInInspector]
        public bool doOnce;
        [HideInInspector]
        public int DamageValue;
        public Transform raycastout;
        public bool useColor;
        [HideInInspector]
        public Image Health_HUD;
        [HideInInspector]
        public Image ArmorIcon;
        [HideInInspector]
        public Image Health_HUDBack;
        [HideInInspector]
        public Image PlayerHealth_HUD;
        [HideInInspector]
        public Image PlayerHealth_HUDBack;
        public Color FullHealth = Color.green;
        public Color MedHealth = Color.yellow;
        public Color LowHealth = Color.red;
        public bool AlwaysShowHUD;
        [HideInInspector]
        public bool isPlayerDriving;
    }
    [Header("______ Armor System ___________________________")]
    [Tooltip("This handles all the Armor system components")]
    public vArmorSystem ArmorSystem;
    //========================================================================//
    #endregion

    #region Paint/Color System   
    [System.Serializable]
    public class vColorSystem
    {//Paint system
        //use a white/Grey texture material base for the parts to color
        public GameObject[] BodyPartsToColor;
    }
    [Header("______ Paint System ___________________________")]
    [Tooltip("This handles all the Paint system components")]
    public vColorSystem PaintSystem;
    #endregion

    #region Door System   
    [System.Serializable]
    public class vDoorSystem
    {//door system
        [Tooltip("drag your door object into the DoorHinge variable, you must have a RigidBody and Joint attached to this object. Leave empty if you do not wish to use door animations")]
        public GameObject DoorHinge;
        [Tooltip("Check lowDoors of your car needs an animation where you get down into a vehicle (sports cars, station wagons, etc), leave unchecked if your vehicle needs you to step up into it (trucks,rigs,etc)")]
        public bool lowDoors;
        [Tooltip("This is the soundclip used for Custom Open Door sound, leave blank if you wish to use default sounds (setup on player )")]
        public AudioClip OpenDoorSound;
        [Tooltip("This is the soundclip used for Custom Close Door sound, leave blank if you wish to use default sounds (setup on player )")]
        public AudioClip CloseDoorSound;
        [HideInInspector]
        public bool openDoor = false;
    }
    [Header("______ Door System ___________________________")]
    [Tooltip("This handles all the Door system components")]
    public vDoorSystem DoorSystem;
    #endregion

    #region Chassis Leaning System   
    [System.Serializable]
    public class vchassisSystem
    {//Chassis Simulation.
        [Tooltip("Set your car body model here (dont include wheels), if you have more than one body part you can set a list of all the parts that should lean upon taking corners. Leave BLANK if you wish not to use this feature")]
        public GameObject[] chassis;
        [Tooltip("Set your RIGHT REAR WHEEL COLLIDER here, this is important. without it we cannot calculate the lean values which may have strange results if any")]
        public WheelCollider RearRightWheel;
        [Tooltip("How much should the vehicle lean vertically on turns")]
        public float chassisVerticalLean = 3.0f;
        [Tooltip("How much should the vehicle lean horizontally on turns")]
        public float chassisHorizontalLean = 3.0f;
        [HideInInspector]
        public float horizontalLean = 0.0f;
        [HideInInspector]
        public float verticalLean = 0.0f;
    }
    [Tooltip("This handles the leaning value of vehicle on turns, leave chassis object [empty] if you dont wish to use this feature")]
    [Header("______ Chassis Lean System ___________________________")]
    public vchassisSystem ChassisSystem;
    #endregion

    #region Damage System   
    [System.Serializable]
    public class vDamageSystem
    {//damage system
        [Tooltip("Set a list or single object prefab for your crash/impact effects, if a list is used than a random one will be selected upon impact")]
        public List<GameObject> crashdebris;
        [Tooltip("Set a list or single audio sound clip for your crash/impact sound effects, if a list is used than a random one will be selected upon impact. This will also slightly randomize the pitch and volume to offer greater variety in sounds.")]
        public AudioClip[] crashClips;
    }
    [Tooltip("This handles all the crash fx and soundfx")]
    [Header("______ Damage System ___________________________")]
    public vDamageSystem DamageSystem;
    #endregion

    #region Motor System   
    [System.Serializable]
    public class vMotorSystem
    { //motor system
        [HideInInspector]
        public bool engineStopped;
        [HideInInspector]
        public bool engineStarted;
        [Tooltip("Set your audio clip of your vehicle engine starting here")]
        public AudioClip engineStart;
        [Tooltip("Set the audio clip of your vehicle engine stopping here   ")]
        public AudioClip engineStop;
        //[Tooltip("Set the audio source for your car engine start/stop sound effect, setting as a separate field rather than creating one on start allows the designer to adjust each cars engine sound volume.")]
        //public AudioSource CarAudioSouce;
    }
    [Tooltip("This handles all the engine sound fx")]
    [Header("______ Motor System ___________________________")]
    public vMotorSystem MotorSystem;
    #endregion

    #region Exhaust System   
    [System.Serializable]
    public class vExhaustSystem
    {//exhaust systems
        [Tooltip("Use this to start/stop a exhause prefab gameobject during start/stop of vehicle engine. Simple place a single or list of gameobjects that you would like to enable/disable on start/stop of engine")]
        public GameObject[] ExhaustEffect;
    }
    [Tooltip("This handles all the exhaust FX")]
    [Header("______ Exhaust System ___________________________")]
    public vExhaustSystem ExhaustSystem;
    #endregion

    #region Nitrous System   
    [System.Serializable]
    public class vNitroSystem
    {//Nitro systems
        public bool InfiniteNitro = true;
        public GameObject[] NitroFX;
        [Space(15)]
        public float boostPower = 5; //how powerful it is
        public float MaxNitroAmount = 1;
        [Space(15)]
        public float RefillRate = 0.05f;
        public float ConsumeRate = 0.6f; //how fast the boost empties it's supply. and how fast it regenerates.
        [Space(10)]
        public AudioClip Nitro_AudioClip;
        [HideInInspector]
        public float CurrentNitroAmount;
        [HideInInspector]
        public Color OriginalMPHColor;
        [HideInInspector]
        public Image NitroGauge;
        [HideInInspector]
        public bool boosting; //just a static bool  to see if we're boosting or not.
        [HideInInspector]
        public WheelCollider[] wcs; //since were using a simple Rigidbody and adding a force to it; it doesn't know if your car is on the ground or not. This variable is to prevent your car from flying like a rocket when you boost while in the air.
    }
    [Header("______ Nitrous System ___________________________")]
    public vNitroSystem NitrousSystem;
    #endregion

    #region Fuel System   
    [System.Serializable]
    public class vFuelSystem
    {//fuel systems
        public bool InfiniteFuel = false;
        public float MaxFuelAmount;
        public float CurrentFuelAmount;
        public float ConsumeRate=0.05f;
        [HideInInspector]
        public Image GasTankFill;
    }
    [Header("______ Fuel System ___________________________")]
    public vFuelSystem FuelSystem;
    #endregion

    #region Lighting System   
    [System.Serializable]
    public class vLightingSystem
    {//lighting systems
        public Renderer[] headlightObjects;
        public Material headlightsOnMaterial;
        public Material headlightsOffMaterial;
        //
        public Renderer[] brakelightObjects;
        public Material brakelightsOnMaterial;
        public Material brakelightsOffMaterial;
        //
        public Renderer[] reverselightObjects;
        public Material reverselightsOnMaterial;
        public Material reverselightsOffMaterial;
        [Header("=================================")]
        public Light[] VehicleLights;
        public Light[] BrakeLights;
        public Light[] ReverseLights;
        [Header("=================================")]
        public GameObject[] VehicleFlares;
        public GameObject[] BrakeFlares;
        public GameObject[] ReverseFlares;
    }
    [Header("______ Lighting System ___________________________")]
    public vLightingSystem LightingSystem;
    #endregion

    #region GUI System   
    [System.Serializable]
    public class vGUISystem
    {//GUI systems
        [HideInInspector]
        public float CurrentCarSpeed;
        [HideInInspector]
        public Image MPH_GAUGE;
        [HideInInspector]
        public Text MPH_SPEED;
        public bool useSpeedometerImage;
        public Sprite SpeedometerImage;
    }
    [Tooltip("This handles all the custom car UI components")]
    [Header("______UI System ___________________________")]
    public vGUISystem GUISystem;
    #endregion

    #region Audio Components System   
    [System.Serializable]
    public class vAudioSystem
    {//Audio system
        
        public AudioSource OtherAudio;
        public AudioSource NitroAudio;
        public AudioSource CarAudio;
        public AudioSource MotorAudio;
        public AudioSource DamageAudio;
    }
    [Header("______ Audio Components System ___________________________")]
    [Tooltip("This handles all the Audio system components")]
    public vAudioSystem AudioComponents;
    #endregion

    #region Private Variables
    //Private Variables
    // joints
    private bool OnOff;
    private Vector3 Anchor;
    private Vector3 Axis;
    private bool autojoint;
    private Rigidbody JoinedObject;
    private bool JointAdded;
    private bool HhOnOff;
    private bool HhPlayedOnce;
    private HingeJoint Hhhinge;
    private JointMotor Hhmotor;
    private JointLimits Hhhlimits;
    //player state info
    [HideInInspector]
    public bool PlayerDriving;
    [HideInInspector]
    public bool reversing;
    // Controller setup
    [HideInInspector]
    public string s_gas;
    [HideInInspector]
    public string s_brake;
    [HideInInspector]
    public string s_turbo;
    [HideInInspector]
    public string s_gunstandard;
    [HideInInspector]
    public string s_specialweapon;
    [HideInInspector]
    public string s_weapon1;
    [HideInInspector]
    public string s_weapon2;
    [HideInInspector]
    public string s_lightsonoff;
    [HideInInspector]
    public string s_chasecam;
    [HideInInspector]
    public string s_exitvehicle;
    [HideInInspector]
    public string s_handbrake;

    //door original position/rotation
    private Vector3 doorPos;
    private Quaternion doorRot;
    // garage stuff
    [HideInInspector]
    public Transform PaintShopExitPoint;
    [HideInInspector]
    public AudioClip PaintShopExitSound;
    [HideInInspector]
    public Transform GarageExitPoint;
    [HideInInspector]
    public AudioClip GarageExitSound;
    //UI references
    private v_vehicle_UI myUI;
    [HideInInspector]
    public GameObject PlayerHub;
    private Text MyMessageMinor;
    private Text MyMessageMajor;
    [HideInInspector]
    public Text exitPaintShopText;
    private GameObject exitPaintShopIcon;
    [HideInInspector]
    public Text applyPaintShopText;
    private GameObject applyPaintShopIcon;
    private Image FadeInOut;
    private GameObject UIColorPicker;
    private Color originalColor;
    [HideInInspector]
    public AudioClip spraypaint_sfx;
    [HideInInspector]
    public bool isInGarage; // is car currently is Garage
    [HideInInspector]
    public GameObject trailerhitch;
    private bool inPaintShop;
    private bool inGarageShop;
    [HideInInspector]
    public bool isPumpingGas;
    #endregion

    //=================================================================================//
    // ------------------------------STANDARD FUNCTIONS START--------------------------//
    //=================================================================================//

    // Use this for initialization
    void Start()
    {
        if (!isInGarage)
        {
            //myUI.FadeInOut.CrossFadeAlpha(0, .01f, true);
            //setup UI components for this vehicle
            StartCoroutine(SetupUI());

            //setup drive controls for this vehicle
            SetupDriveControls();

            //intialize door hinges
            if (DoorSystem.DoorHinge != null) { StartCoroutine(DestroyHingesOnStart()); }

            // Turn off all exhaust fx game objects
            foreach (GameObject GO in ExhaustSystem.ExhaustEffect) { GO.SetActive(false); }

            // Turn off any vehicle lights
            foreach (Light VL in LightingSystem.VehicleLights) { VL.enabled = false; }
            foreach (Light BL in LightingSystem.BrakeLights) { BL.enabled = false; }
            foreach (Light RL in LightingSystem.ReverseLights) { RL.enabled = false; }

            // Turn off any vehicle lights
            foreach (GameObject VF in LightingSystem.VehicleFlares) { VF.SetActive(false); }
            foreach (GameObject BF in LightingSystem.BrakeFlares) { BF.SetActive(false); }
            foreach (GameObject RF in LightingSystem.ReverseFlares) { RF.SetActive(false); }

            // Armor Systems initialize values
            ArmorSystem.currentArmor = ArmorSystem.MaxArmor;

            // Nitro system intialize values
            NitrousSystem.CurrentNitroAmount = NitrousSystem.MaxNitroAmount;
            NitrousSystem.wcs = gameObject.GetComponentsInChildren<WheelCollider>();//get wheel colliders
            
            // turn off any nitro effects 
            foreach (GameObject GO in NitrousSystem.NitroFX) { GO.SetActive(false); }

            // Fuel system intialize values
            FuelSystem.CurrentFuelAmount = FuelSystem.MaxFuelAmount;
            // 
            if (NitrousSystem.Nitro_AudioClip != null)
            {
                AudioComponents.NitroAudio.clip = NitrousSystem.Nitro_AudioClip;
            }
              
        }
    }

    // Update is called once per frame
    void Update()
    {
        //==================================================================
        #region If Player is not in a Garage/Paint Shop

        if (!isInGarage) // if player is not in a Garage
        {
            Cursor.visible = false;

            #region Door System
            // start open vehicle door animation if door open is triggered using joint
            if (DoorSystem.DoorHinge != null)
            {
                if (DoorSystem.openDoor == true)
                {
                    if (JointAdded == false) { AddJoint(); } // add joint system 
                    if (JointAdded == true) { StartCoroutine(CCdoorOpen()); } // then open door using joint
                }

                else if (DoorSystem.openDoor == false && JointAdded == true) // if door isnt open yet, and joint has been added then get info about joint and store
                {
                    HingeJoint hinge2 = DoorSystem.DoorHinge.GetComponent<HingeJoint>();
                    JointMotor motor;
                    JointLimits hlimits;
                    hlimits = hinge2.limits;
                    motor = hinge2.motor;
                    hlimits.min = 0;
                    hlimits.max = 0;
                    motor.force = 100;
                    //motor.targetVelocity = DoorSystem.DoorOpenSpeed;
                    motor.targetVelocity = 120;
                    hinge2.motor = motor;
                    hinge2.limits = hlimits;
                    StartCoroutine(DestroyHinges()); //  after we store the info, lets destroy the hinge so chassis and door can move as single object
                }
            }
            #endregion

            #region Enable Armor
            Armor(); // run the armor function if enable armor is true
                     //if (ArmorSystem.AlwaysShowHUD == true) { ArmorSystem.Health_HUD.enabled = true; ArmorSystem.Health_HUDBack.enabled = true; }
            #endregion

            #region Enable UI
            EnableUI();
            #endregion

            //---------------------------------------------------------------------------------//

            #region IF PLAYER IS DRIVING THIS CAR
            // if player is driving *this* vehicle
            if ((Player.GetComponent<V_car_user>().driving) && (Player.GetComponent<V_car_user>().currCar == this.gameObject))
            {

                #region Allow Car Rotation when Idle
                /*
               //===============================================================================================//
               //== This will allow the player to rotate on the spot 360 degrees while still, its not lifelike, 
               //== but is good for fast paced car action games and the like where movement is more essential 
               //== than realism so its here if anyone wants to use it, just uncomment all the code below.
               //===============================================================================================// 
                if (GetComponent<Rigidbody>().velocity.magnitude <= 0.1f)
                {
                    if (Input.GetAxis("LeftAnalogHorizontal") > 0 || Input.GetAxis("Horizontal") > 0)
                    {
                        this.transform.Rotate(Vector3.up * 1);// * Time.deltaTime);
                    }
                    if (Input.GetAxis("LeftAnalogHorizontal") < 0 || Input.GetAxis("Horizontal") < 0)
                    {
                        this.transform.Rotate(Vector3.up * -1);// * Time.deltaTime);
                    }

                }
                */
                #endregion

                #region Armor System

                PlayerDriving = true;
                //AISystem.AI_Enabled = false; //turn off AI for this car if we are driving it

                ArmorSystem.isPlayerDriving = true;
                // if armor system is disabled
                //if (ArmorSystem.PlayerHealth_HUD != null) { ArmorSystem.PlayerHealth_HUD.enabled = true; }
                //if (ArmorSystem.PlayerHealth_HUDBack != null) { ArmorSystem.PlayerHealth_HUDBack.enabled = true; }

                #endregion

                #region Chassis System
                if (ChassisSystem.chassis.Length > 0) { Chassis(); } // if we are using the chassis lean system ,start chassis function
                #endregion

                #region Motor System
                //if we haven't started our engine yet, lets start engine
                if (!MotorSystem.engineStarted) { if (!AudioComponents.MotorAudio.isPlaying) { StartCoroutine(EngineStart()); } }

                #endregion

                #region Exhaust System
                // turn on any exhaust game objects FX
                foreach (GameObject GO in ExhaustSystem.ExhaustEffect) { GO.SetActive(true); }
                #endregion

                #region Fuel System
                // if we are not using infinite fuel and have run out of fuel, stop the car from running
                if (FuelSystem.InfiniteFuel == false && FuelSystem.CurrentFuelAmount <= 0 )
                {
                    Player.GetComponent<V_car_user>().driving = false;
                    Player.GetComponent<V_car_user>().OutOfGas = true;

                }

                // if we are not using infinite fuel than subtract the gas amount by ConsumeRate variable
                if (FuelSystem.InfiniteFuel == false)
                {
                    if (FuelSystem.CurrentFuelAmount > 0)
                    {
                        FuelSystem.CurrentFuelAmount -= FuelSystem.ConsumeRate * Time.deltaTime;
                    }

                }
                // if we are using the built in GUI sysem than display the value with a fill UI graphic

                FuelSystem.GasTankFill.enabled = true;
                float GasPercent = FuelSystem.CurrentFuelAmount / FuelSystem.MaxFuelAmount;
                if (FuelSystem.GasTankFill != null)
                {
                    FuelSystem.GasTankFill.fillAmount = GasPercent;
                    
                }
                #endregion

                #region Nitrous System
                // if nitrous system is enabled than run Boost routine
                Nitro();
                #endregion

                #region Brakelight System
                // turn on brake light when brakes applied
                if (Input.GetButton(CarControllingSystem.Brake.ToString()) || Input.GetButton(CarControllingSystem.HandBrake.ToString()) )
                { StartCoroutine(BrakelightsOn()); }

                else
                // otherwise turn off brake lights
                {
                    if (reversing)
                    {
                        StartCoroutine(ReverselightsOn());
                    }
                    else
                    {
                        StartCoroutine(ReverselightsOff());
                        StartCoroutine(BrakelightsOff());
                    }
                }
                #endregion

                #region Headlights System
                // Headlight on/off switch
                if (Input.GetButton(CarControllingSystem.LightsOnOff.ToString()) && OnOff == false) { StartCoroutine(HeadlightsOn()); }
                else if (Input.GetButton(CarControllingSystem.LightsOnOff.ToString()) && OnOff == true) { StartCoroutine(HeadlightsOff()); }

                #endregion

                #region GUI System
                if (myUI.Speedometer_Gauge != null && GUISystem.useSpeedometerImage == true)
                {
                    myUI.Speedometer_Gauge.SetActive(true);
                    if (GUISystem.SpeedometerImage != null)
                    {
                        myUI.Speedometer_Gauge.GetComponent<Image>().sprite = GUISystem.SpeedometerImage;
                    }
                    if (GUISystem.CurrentCarSpeed != 0  )
                    {
                        v_car_speedometer.ShowSpeed(GUISystem.CurrentCarSpeed, 0, this.GetComponentInParent<CarController>().MaxSpeed);
                    }
                }
            
                if (GetComponent<CarUserControl>() != null) { GUISystem.CurrentCarSpeed = GetComponent<CarUserControl>().CurrentCarSpeed; }
                float RPMPercent = GUISystem.CurrentCarSpeed / this.GetComponentInParent<CarController>().MaxSpeed;
                // if GUI is enabled show information
                if (GUISystem.MPH_GAUGE != null)
                { // show MPH gauge with a Fill UI texture sprite
                    GUISystem.MPH_GAUGE.enabled = true;
                    GUISystem.MPH_GAUGE.fillAmount = RPMPercent;
                }
                //
                if (GUISystem.MPH_SPEED != null)
                { // show MPH speed with a UI text sprite
                    GUISystem.MPH_SPEED.enabled = true;
                    GUISystem.MPH_SPEED.text = GUISystem.CurrentCarSpeed.ToString("###") ;
                   
                }

                #endregion

            }
            #endregion

            #region IF PLAYER IS NOT DRIVING THIS CAR
            // if player is not driving this vehicle
            else
            {
                #region Turn off Driving System
                PlayerDriving = false;
                #endregion

                #region Turn off Particles System
                foreach (GameObject GO in ExhaustSystem.ExhaustEffect) { GO.SetActive(false); }
                #endregion

                #region Turn off Nitro System
                NitrousSystem.boosting = false; // disable nitro boosting
                #endregion

                #region Turn off lights
                // Turn off any vehicle lights
                foreach (Light VL in LightingSystem.VehicleLights) { VL.enabled = false; }
                foreach (Light BL in LightingSystem.BrakeLights) { BL.enabled = false; }
                foreach (Light RL in LightingSystem.ReverseLights) { RL.enabled = false; }

                foreach (GameObject VF in LightingSystem.VehicleFlares) { VF.SetActive(false); }
                foreach (GameObject BF in LightingSystem.BrakeFlares) { BF.SetActive(false); }
                foreach (GameObject RF in LightingSystem.ReverseFlares) { RF.SetActive(false); }

                foreach (Renderer HRL in LightingSystem.headlightObjects) { HRL.material = LightingSystem.headlightsOffMaterial; }
                foreach (Renderer BRL in LightingSystem.brakelightObjects) { BRL.material = LightingSystem.brakelightsOffMaterial; }
                foreach (Renderer RRL in LightingSystem.reverselightObjects) { RRL.material = LightingSystem.reverselightsOffMaterial; }
                OnOff = false;
                #endregion

            }
            #endregion

            #region IF PLAYER EXITS THIS CAR

            #region Turn off Motor/Engine
            // if engine started and player exits vehicle, stop engine
            if (MotorSystem.engineStarted)
            {
                if ((!Player.GetComponent<V_car_user>().driving) && (!MotorSystem.engineStopped))
                {
                    // if GUI images are being used hide them on exiting vehicle
                    // if (NitrousSystem.NitroGauge != null) { NitrousSystem.NitroGauge.enabled = false; }
                    // if (FuelSystem.GasTankFill != null) { FuelSystem.GasTankFill.enabled = false; }
                    StartCoroutine(EngineStop());
                }
            }
            #endregion

            #endregion
        }
        #endregion

        #region Player is currently in a Garage/Paint Shop
        else
        {
            
            exitPaintShopIcon.SetActive(true);
            exitPaintShopText.enabled = true;
            applyPaintShopIcon.SetActive(true);
            applyPaintShopText.enabled = true;
            FadeInOut.enabled = true;
            if (!Cursor.visible)
                Cursor.visible = true;
            if (myUI.Speedometer_Gauge != null && GUISystem.useSpeedometerImage == true)
            {
                myUI.Speedometer_Gauge.SetActive(false);
            }
            //start the Garage function which includes
            //customizing your car by paint color
            InGarage();
        }
        #endregion
    }


    //=================================================================================//
    // ------------------------------CUSTOM FUNCTIONS START----------------------------//
    //=================================================================================//

    //===============Vehicle Specific=================================//

    void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            if (DamageSystem.crashClips.Length > 0)
            {
                if (collision.contacts[0].thisCollider.gameObject.layer != LayerMask.NameToLayer("Wheel") && collision.gameObject.layer != LayerMask.NameToLayer("Road") && collision.gameObject.layer != LayerMask.NameToLayer("Player") && collision.gameObject.layer != LayerMask.NameToLayer("Enemy") && collision.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
                {
                    if (DamageSystem.crashdebris.Count > 0)
                    {
                        ContactPoint contact = collision.contacts[0];
                        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
                        Vector3 pos = contact.point;
                        Instantiate(DamageSystem.crashdebris[UnityEngine.Random.Range(0, DamageSystem.crashdebris.Count)], pos, rot);
                    }
                    AudioComponents.DamageAudio.clip = DamageSystem.crashClips[UnityEngine.Random.Range(0, DamageSystem.crashClips.Length)];
                    AudioComponents.DamageAudio.pitch = UnityEngine.Random.Range(1.0f, 1.3f);
                    AudioComponents.DamageAudio.volume = UnityEngine.Random.Range(1.0f, 0.50f);
                    //
                    if (!AudioComponents.DamageAudio.isPlaying) { AudioComponents.DamageAudio.Play(); }
                    
                }
            }
        }
    }

    void SetupDriveControls()
    {
        s_gas = CarControllingSystem.Gas.ToString();
        s_brake = CarControllingSystem.Brake.ToString();
        s_turbo = CarControllingSystem.Turbo.ToString();
        s_gunstandard = CarControllingSystem.StandardGun.ToString();
        s_specialweapon = CarControllingSystem.SpecialWeapon.ToString();
        s_weapon1 = CarControllingSystem.SelectedWeapon1.ToString();
        s_weapon2 = CarControllingSystem.SelectedWeapon2.ToString();
        s_lightsonoff = CarControllingSystem.LightsOnOff.ToString();
        s_chasecam = CarControllingSystem.ChaseCameraOnOff.ToString();
        s_exitvehicle = CarControllingSystem.ExitVehicle.ToString();
        s_handbrake = CarControllingSystem.HandBrake.ToString();
    }

    void Chassis()
    {
        WheelHit CorrespondingGroundHit;
        ChassisSystem.RearRightWheel.GetGroundHit(out CorrespondingGroundHit);
        foreach (GameObject GO in ChassisSystem.chassis)
        {
            ChassisSystem.verticalLean = Mathf.Clamp(Mathf.Lerp(ChassisSystem.verticalLean, GetComponent<Rigidbody>().angularVelocity.x * ChassisSystem.chassisVerticalLean, Time.deltaTime * 5), -3.0f, 3.0f);
            ChassisSystem.horizontalLean = Mathf.Clamp(Mathf.Lerp(ChassisSystem.horizontalLean, GetComponent<Rigidbody>().angularVelocity.y * ChassisSystem.chassisHorizontalLean, Time.deltaTime * 3), -5.0f, 5.0f);
            Quaternion target = Quaternion.Euler(ChassisSystem.verticalLean, GO.transform.localRotation.y, ChassisSystem.horizontalLean);
            GO.transform.localRotation = target;
        }
    }

    void Nitro()
    {
        // if infinite nitro is enabled then allow boosting when nitro amount is more than half the max amount
        if (Input.GetButton(s_turbo))
        {
            if (NitrousSystem.CurrentNitroAmount > NitrousSystem.MaxNitroAmount / 2 && NitrousSystem.InfiniteNitro == true)
            {
                NitrousSystem.boosting = true;
            }
            if (NitrousSystem.CurrentNitroAmount >= 0.01f && NitrousSystem.InfiniteNitro == false)
            {
                NitrousSystem.boosting = true;
            }
        }
        // if nitro is out or player stopped using nitro then stop boosting speed
        if (NitrousSystem.CurrentNitroAmount < .01 || Input.GetButtonUp(s_turbo))
        {
            AudioComponents.NitroAudio.Stop();
            NitrousSystem.boosting = false;
        }

        // we are boosting our speed
        if (NitrousSystem.boosting)
        {
            NitrousSystem.CurrentNitroAmount -= NitrousSystem.ConsumeRate * Time.deltaTime;// decrease the bar's amount by the consumption rate var

            foreach (WheelCollider wc in NitrousSystem.wcs)
            {
                GetComponent<Rigidbody>().AddForce(transform.forward * NitrousSystem.boostPower, ForceMode.Acceleration);//only add force when we're on the ground, ok?
            }
            AudioComponents.NitroAudio.loop = true;
            if (!AudioComponents.NitroAudio.isPlaying) { AudioComponents.NitroAudio.Play(); } // activate nitro sound
            if (GUISystem.MPH_SPEED != null) { GUISystem.MPH_SPEED.color = Color.cyan; }

            foreach (GameObject GO in NitrousSystem.NitroFX)
            {
                GO.SetActive(true);
            }
            //set nitro current values
            float NitroPercent = NitrousSystem.CurrentNitroAmount / NitrousSystem.MaxNitroAmount;
            if (NitrousSystem.CurrentNitroAmount <= 0.01f) { NitrousSystem.boosting = false; }
            // if we are using a GUI
            if (NitrousSystem.NitroGauge != null)
            {
                // show graphic using fill image
                NitrousSystem.NitroGauge.fillAmount = NitroPercent;
            }
        }
        else if (!NitrousSystem.boosting) // we are no longer boosting
        {
            AudioComponents.NitroAudio.loop = false;
            AudioComponents.NitroAudio.Stop();
            if (GUISystem.MPH_SPEED != null) { GUISystem.MPH_SPEED.color = NitrousSystem.OriginalMPHColor; }
            foreach (WheelCollider wc in NitrousSystem.wcs)
            {
                if (NitrousSystem.InfiniteNitro == true)// && wc.isGrounded == true) // uncomment isGrounded if you want your refill to only happen when all 4 tires are grounded.
                {
                    NitrousSystem.CurrentNitroAmount += NitrousSystem.RefillRate * Time.deltaTime;
                    if (NitrousSystem.CurrentNitroAmount >= NitrousSystem.MaxNitroAmount) { NitrousSystem.CurrentNitroAmount = NitrousSystem.MaxNitroAmount; }
                    float NitroPercent = NitrousSystem.CurrentNitroAmount / NitrousSystem.MaxNitroAmount;
                    if (NitrousSystem.NitroGauge != null) { NitrousSystem.NitroGauge.fillAmount = NitroPercent; }
                }
            }

            foreach (GameObject GO in NitrousSystem.NitroFX) { GO.SetActive(false); }
        }
    }

    public void Armor()
    {
        if (!isInGarage)
        {

            if (ArmorSystem.Health_HUD != null)
            {
                float RPMPercent = (float)ArmorSystem.currentArmor / (float)ArmorSystem.MaxArmor;
                ArmorSystem.Health_HUD.fillAmount = RPMPercent;

                if (ArmorSystem.useColor == true)
                {
                    if (ArmorSystem.currentArmor < ArmorSystem.MaxArmor / 2 && ArmorSystem.currentArmor > ArmorSystem.MaxArmor / 3)
                    {
                        ArmorSystem.Health_HUD.color = ArmorSystem.MedHealth;
                    }
                    else if (ArmorSystem.currentArmor >= ArmorSystem.MaxArmor / 2) { ArmorSystem.Health_HUD.color = ArmorSystem.FullHealth; }
                    else { ArmorSystem.Health_HUD.color = ArmorSystem.LowHealth; }
                }
            }
            if ((Player.GetComponent<V_car_user>().driving) && (Player.GetComponent<V_car_user>().currCar == this.gameObject))
            {
                if (ArmorSystem.PlayerHealth_HUD != null)
                {
                    float RPMPercent = (float)ArmorSystem.currentArmor / (float)ArmorSystem.MaxArmor;
                    ArmorSystem.PlayerHealth_HUD.fillAmount = RPMPercent;

                    if (ArmorSystem.useColor == true)
                    {
                        if (ArmorSystem.currentArmor < ArmorSystem.MaxArmor / 2 && ArmorSystem.currentArmor > ArmorSystem.MaxArmor / 3)
                        {
                            ArmorSystem.PlayerHealth_HUD.color = ArmorSystem.MedHealth;
                        }
                        else if (ArmorSystem.currentArmor >= ArmorSystem.MaxArmor / 2) { ArmorSystem.PlayerHealth_HUD.color = ArmorSystem.FullHealth; }
                        else { ArmorSystem.PlayerHealth_HUD.color = ArmorSystem.LowHealth; }
                    }
                }
            }
            // destroy the vehicle
            if (ArmorSystem.currentArmor <= 0 && this.gameObject != null)
            {
                if (ArmorSystem.doOnce == false)
                {
                    PlayerHub.SetActive(false);

                    // if player is driving *this* vehicle, eject the player from car
                    if ((Player.GetComponent<V_car_user>().driving) && (Player.GetComponent<V_car_user>().currCar == this.gameObject))
                    {
                        // If youd like to destroy the player you may do so here, or 
                        // you can write a function to eject the player
                    }
                    if (ArmorSystem.DestructPrefab != null)
                    {
                        Instantiate(ArmorSystem.DestructPrefab, this.transform.position, this.transform.rotation);
                    }
                    //
                    ArmorSystem.doOnce = true;
                }

                // uncomment the below line to destroy the player/vehicle object
                //DestroyObject(this.gameObject, 2.1f);
                return;
            }
        }
    }

    //===============Door Specific====================================//

    public void AddJoint()
    {
        //Debug.Log("added joint");
        HingeJoint hinge2 = DoorSystem.DoorHinge.AddComponent<HingeJoint>();
        JointLimits hlimits;
        hinge2.anchor = Anchor;
        hinge2.axis = Axis;
        hinge2.autoConfigureConnectedAnchor = autojoint;
        hinge2.connectedBody = JoinedObject;
        hlimits = hinge2.limits;
        hinge2.useLimits = true;
        JointAdded = true;
    }

    IEnumerator DestroyHingesOnStart()
    {
        //collect and save information on door location/position
        doorPos = DoorSystem.DoorHinge.transform.localPosition;
        doorRot = DoorSystem.DoorHinge.transform.localRotation;
        //collect and save joint configuration
        HingeJoint hinge = DoorSystem.DoorHinge.GetComponent<HingeJoint>();
        JointMotor motor;
        JointLimits hlimits;
        motor = hinge.motor;
        Anchor = hinge.anchor;
        JoinedObject = hinge.connectedBody;
        Axis = hinge.axis;
        autojoint = hinge.autoConfigureConnectedAnchor;
        hinge.useLimits = true;
        hlimits = hinge.limits;
        yield return new WaitForSeconds(1.0f);
        //now destroy the joint and rigidbody
        //since we dont want our doors swinging open by accidental force
        Destroy(DoorSystem.DoorHinge.GetComponent<HingeJoint>());
        Destroy(DoorSystem.DoorHinge.GetComponent<Rigidbody>(), 0.01f);
    }

    IEnumerator DestroyHinges()
    {
        yield return new WaitForSeconds(0.01f);
        Destroy(DoorSystem.DoorHinge.GetComponent<HingeJoint>());
        Destroy(DoorSystem.DoorHinge.GetComponent<Rigidbody>(), .01f);
        JointAdded = false;
        //after we destroy our hinges lets reset the door
        //position and rotation back to its saved state
        DoorSystem.DoorHinge.transform.localPosition = doorPos;
        DoorSystem.DoorHinge.transform.localRotation = doorRot;

    }

    IEnumerator CCdoorOpen()
    {
        HingeJoint hinge2 = DoorSystem.DoorHinge.GetComponent<HingeJoint>();
        JointMotor motor;
        JointLimits hlimits;
        hlimits = hinge2.limits;
        motor = hinge2.motor;
        hlimits.min = 0;
        hlimits.max = 90;
        motor.force = 100;
        //motor.targetVelocity = DoorSystem.DoorOpenSpeed;
        motor.targetVelocity = 120;
        hinge2.motor = motor;
        hinge2.limits = hlimits;
        hinge2.useMotor = true;
        yield return new WaitForSeconds(3.6f);
        //motor.targetVelocity = -DoorSystem.DoorCloseSpeed;
        motor.targetVelocity = -140;
        if (hinge2 != null)
        {
            hinge2.motor = motor;
        }
    }

    public void DestroyJointOnDrive()
    {
        StartCoroutine(DestroyHinges());
    }

    //==============Motor Specific==================================//

    IEnumerator EngineStart()
    {
        MotorSystem.engineStopped = false;
        MotorSystem.engineStarted = true;
        AudioComponents.MotorAudio.clip = MotorSystem.engineStart;
        AudioComponents.MotorAudio.Play();
        yield return new WaitForSeconds(AudioComponents.MotorAudio.clip.length);
        AudioComponents.MotorAudio.Stop();
    }

    IEnumerator EngineStop()
    {
        MotorSystem.engineStopped = true;
        MotorSystem.engineStarted = false;
        AudioComponents.MotorAudio.clip = MotorSystem.engineStop;
        AudioComponents.MotorAudio.Play();
        yield return new WaitForSeconds(AudioComponents.MotorAudio.clip.length);
        AudioComponents.MotorAudio.Stop();
    }

    //==============Vehicle Lights Specific==================//

    IEnumerator HeadlightsOn()
    {
        foreach (Light VL in LightingSystem.VehicleLights) { VL.enabled = true; }
        foreach (GameObject VF in LightingSystem.VehicleFlares) { VF.SetActive(true); }
        // a little wait so we dont trip the on/off too soon.
        yield return new WaitForSeconds(.1f);
        OnOff = true;
        foreach (Renderer RL in LightingSystem.headlightObjects)
        {
            RL.material = LightingSystem.headlightsOnMaterial;
        }
    }

    IEnumerator HeadlightsOff()
    {
        foreach (Light VL in LightingSystem.VehicleLights) { VL.enabled = false; }
        foreach (GameObject VF in LightingSystem.VehicleFlares) { VF.SetActive(false); }
        yield return new WaitForSeconds(.1f);
        OnOff = false;
        foreach (Renderer RL in LightingSystem.headlightObjects) { RL.material = LightingSystem.headlightsOffMaterial; }
    }

    IEnumerator BrakelightsOn()
    {
        foreach (Light BL in LightingSystem.BrakeLights) { BL.enabled = true; }
        foreach (GameObject BF in LightingSystem.BrakeFlares) { BF.SetActive(true); }
        foreach (Renderer RL in LightingSystem.brakelightObjects)
        {
            RL.material = LightingSystem.brakelightsOnMaterial;
        }
        yield return new WaitForSeconds(.1f);
    }

    IEnumerator BrakelightsOff()
    {

        foreach (Light BL in LightingSystem.BrakeLights) { BL.enabled = false; }
        foreach (GameObject BF in LightingSystem.BrakeFlares) { BF.SetActive(false); }
        foreach (Renderer RL in LightingSystem.brakelightObjects)
        {
            RL.material = LightingSystem.brakelightsOffMaterial;
        }
        yield return new WaitForSeconds(.1f);
    }

    IEnumerator ReverselightsOn()
    {
        foreach (Light BL in LightingSystem.ReverseLights) { BL.enabled = true; }
        foreach (GameObject RF in LightingSystem.ReverseFlares) { RF.SetActive(true); }
        foreach (Renderer RL in LightingSystem.reverselightObjects)
        {
            RL.material = LightingSystem.reverselightsOnMaterial;
        }
        yield return new WaitForSeconds(.1f);
    }

    IEnumerator ReverselightsOff()
    {

        foreach (Light BL in LightingSystem.ReverseLights) { BL.enabled = false; }
        foreach (GameObject RF in LightingSystem.ReverseFlares) { RF.SetActive(false); }
        foreach (Renderer RL in LightingSystem.reverselightObjects)
        {
            RL.material = LightingSystem.reverselightsOffMaterial;
        }
        yield return new WaitForSeconds(.1f);
    }

    //================UI Specific=============================//

    IEnumerator SetupUI()
    {

        yield return new WaitForSeconds(.03f);
        myUI = Player.GetComponent<V_car_user>().myHub.GetComponent<v_vehicle_UI>();
        yield return new WaitForSeconds(.01f);
        PlayerHub = Player.GetComponent<V_car_user>().myHub;
        yield return new WaitForSeconds(.02f);
        PlayerHub.SetActive(false);
        ArmorSystem.PlayerHealth_HUD = myUI.Health_FillImage;
        GUISystem.MPH_SPEED = myUI.Mph_text;
        GUISystem.MPH_GAUGE = myUI.Mph_FillImage;
        FuelSystem.GasTankFill = myUI.Fuel_FillImage;
        NitrousSystem.NitroGauge = myUI.Nitro_FillImage;
        NitrousSystem.OriginalMPHColor = myUI.Mph_text.color;
        MyMessageMajor = myUI.MessageMajor;
        MyMessageMinor = myUI.MessageMinor;
        exitPaintShopIcon = myUI.ExitPaintIcon;
        exitPaintShopText = myUI.ExitPaintText;
        applyPaintShopIcon = myUI.ApplyPaintIcon;
        applyPaintShopText = myUI.ApplyPaintText;
        UIColorPicker = myUI.UIColorPicker;
        FadeInOut = myUI.FadeInOut;
        FadeInOut.CrossFadeAlpha(0, .01f, true);
    }

    public void EnableUI()
    {
        if (PlayerHub != null)
        {
            if (Player.GetComponent<V_car_user>().driving)
            {
                // if the vehicle UI is not active
                // and we are currently driving then set it to active
                if (PlayerHub.activeInHierarchy == false)
                {
                    PlayerHub.SetActive(true);
                    //-----------------------------//
                    myUI.Fuel_BackImage.enabled = true;
                    myUI.Fuel_FillImage.enabled = true;
                    myUI.Nitro_BackImage.enabled = true;
                    myUI.Nitro_FillImage.enabled = true;
                    myUI.Nitro_Icon.enabled = true;
                    myUI.Health_FillImage.enabled = true;
                    myUI.Health_BackImage.enabled = true;
                    myUI.Mph_text.enabled = true;
                    myUI.Mph_text_BackImage.enabled = true;
                    myUI.playerName.enabled = true;
                    //-----------------------------//
                    myUI.Health_Icon.enabled = false;
                    myUI.MessageMajor.enabled = false;
                    myUI.MessageMinor.enabled = false;
                    myUI.Rotor_BackImage.enabled = false;
                    myUI.Rotor_FillImage.enabled = false;
                    myUI.Rotor_Icon.enabled = false;
                    exitPaintShopIcon.SetActive(false);
                    exitPaintShopText.enabled = false;
                    myUI.boatStuff.SetActive(false);

                    applyPaintShopText.enabled = false;
                    if (!isInGarage)
                    {
                        FadeInOut.enabled = false;
                    }
                    if(!inPaintShop)
                    {
                        UIColorPicker.SetActive(false);
                        applyPaintShopIcon.SetActive(false);
                    }
                   
                    //OPTIONAL: Set the player name here to whatever, i currently use the car name, but if you have
                    //player names in your game, this is a good place to add it
                    myUI.playerName.text = Player.GetComponent<V_car_user>().currCar.name;
                }
            }

            else if (!Player.GetComponent<V_car_user>().driving && !Player.GetComponent<V_car_user>().gliding && !Player.GetComponent<V_car_user>().flying && !Player.GetComponent<V_car_user>().boating)

            {
                // if we are nor driving anymore, then disable the
                // vehicle UI only if it is still enabled.
                if (PlayerHub.activeInHierarchy == true)
                {
                    PlayerHub.SetActive(false);
                    // OPTIONAL: reset player name.
                    myUI.playerName.text = "--";
                }
            }
            
        }
    }

    //==============Garage Specific============================//

    public void InGarage()
    {

        if (inPaintShop)
        {
            PaintCar(UIColorPicker.GetComponent<CUIColorPicker>().Color);
        }

        if(inGarageShop)
        {
            UpgradeCar();
        }
        
    }



    public void PaintCar(Color color)
    {
        // change car color based on player choice
        if (PaintSystem.BodyPartsToColor.Length > 0 && PaintSystem.BodyPartsToColor != null)
        {
            foreach (GameObject GO in this.PaintSystem.BodyPartsToColor)
            {
                //Renderer[] rends = GO.GetComponentsInChildren<Renderer>();
                Renderer rends = GO.GetComponent<Renderer>();
                // foreach (Renderer r in rends) {
                rends.material.color = color;
              //  }
            }
        }

        if (Input.GetButtonDown(CarControllingSystem.Brake.ToString()))
        {
            
            // change car color based on player choice
            myUI.Nitro_BackImage.enabled = true;
            myUI.Nitro_FillImage.enabled = true;
            myUI.Nitro_Icon.enabled = true;
            myUI.Mph_text_BackImage.enabled = true;
            myUI.Mph_text.enabled = true;
            UIColorPicker.SetActive(false);
            isInGarage = false;
            inPaintShop = false;
            inGarageShop = false;
            Player.GetComponent<V_car_user>().inGarage = false;
            exitPaintShopIcon.SetActive(false);
            exitPaintShopText.enabled = false;
            applyPaintShopIcon.SetActive(false);
            applyPaintShopText.enabled = false;
            StartCoroutine(FadeIn(.2f));
            //transform.position = new Vector3(0, 1, 0);
            transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            transform.Rotate(0, 115, 0);
            transform.GetComponentInParent<CarUserControl>().enabled = true;
            transform.GetComponent<Rigidbody>().isKinematic = false;
            transform.GetComponentInParent<CarUserControl>().enabled = true;
            transform.GetComponentInParent<CarController>().enabled = true;
            transform.GetComponentInParent<CarController>().Move(0, 0, 1, 0);
            if (PaintShopExitSound != null)
            {
                AudioComponents.OtherAudio.clip = PaintShopExitSound;
                if (!AudioComponents.OtherAudio.isPlaying) { AudioComponents.OtherAudio.Play(); }
            }
            if (spraypaint_sfx != null)
            {
                AudioComponents.DamageAudio.clip = spraypaint_sfx;
                if (!AudioComponents.DamageAudio.isPlaying) { AudioComponents.OtherAudio.Play(); }
            }
            if (PaintShopExitPoint != null)
            {
                if (CarCamStateSystem.isHugeVehicle && trailerhitch != null)
                {
                    trailerhitch.SetActive(true);
                }
                transform.position = PaintShopExitPoint.position;
                transform.rotation = PaintShopExitPoint.rotation;
            }
            else
            {
                transform.position = new Vector3(0, 1, 0);
            }
        }

        if (Input.GetButtonDown(CarControllingSystem.HandBrake.ToString()))
        {
            //
            // change car color back to orginal color
            if (PaintSystem.BodyPartsToColor.Length > 0 && PaintSystem.BodyPartsToColor != null)
            {
                foreach (GameObject GO in this.PaintSystem.BodyPartsToColor)
                {
                    Renderer[] rends = GO.GetComponentsInChildren<Renderer>();
                    foreach (Renderer r in rends) { r.material.color = originalColor; }
                }
            }
        
            myUI.Nitro_BackImage.enabled = true;
            myUI.Nitro_FillImage.enabled = true;
            myUI.Nitro_Icon.enabled = true;
            myUI.Mph_text_BackImage.enabled = true;
            myUI.Mph_text.enabled = true;
            UIColorPicker.SetActive(false);
            isInGarage = false;
            inPaintShop = false;
            inGarageShop = false;
            Player.GetComponent<V_car_user>().inGarage = false;
            exitPaintShopIcon.SetActive(false);
            exitPaintShopText.enabled = false;
            applyPaintShopIcon.SetActive(false);
            applyPaintShopText.enabled = false;
            StartCoroutine(FadeIn(.2f));
            //transform.position = new Vector3(0, 1, 0);
            transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            transform.Rotate(0, 115, 0);
            transform.GetComponentInParent<CarUserControl>().enabled = true;
            transform.GetComponent<Rigidbody>().isKinematic = false;
            transform.GetComponentInParent<CarUserControl>().enabled = true;
            transform.GetComponentInParent<CarController>().enabled = true;
            transform.GetComponentInParent<CarController>().Move(0, 0, 1, 0);
            if (PaintShopExitSound != null)
            {
                AudioComponents.OtherAudio.clip = PaintShopExitSound;
                if (!AudioComponents.OtherAudio.isPlaying) { AudioComponents.OtherAudio.Play(); }
            }
            if (PaintShopExitPoint != null)
            {
                if (CarCamStateSystem.isHugeVehicle && trailerhitch != null)
                {
                    trailerhitch.SetActive(true);
                }
                transform.position = PaintShopExitPoint.position;
                transform.rotation = PaintShopExitPoint.rotation;
            }
            else
            {
                transform.position = new Vector3(0, 1, 0);
            }
        }
            transform.Rotate(Vector3.down * Time.deltaTime * 60);
    }

    public void UpgradeCar()
    {
        if (Input.GetButton(CarControllingSystem.Brake.ToString()))
        {
            if (ArmorSystem.currentArmor < ArmorSystem.MaxArmor)
            {
                ArmorSystem.currentArmor += 1;

            }
        }

        if (ArmorSystem.PlayerHealth_HUD != null)
        {
            float RPMPercent = (float)ArmorSystem.currentArmor / (float)ArmorSystem.MaxArmor;
            ArmorSystem.PlayerHealth_HUD.fillAmount = RPMPercent;

            if (ArmorSystem.useColor == true)
            {
                if (ArmorSystem.currentArmor < ArmorSystem.MaxArmor / 2 && ArmorSystem.currentArmor > ArmorSystem.MaxArmor / 3)
                {
                    ArmorSystem.PlayerHealth_HUD.color = ArmorSystem.MedHealth;
                }
                else if (ArmorSystem.currentArmor >= ArmorSystem.MaxArmor / 2) { ArmorSystem.PlayerHealth_HUD.color = ArmorSystem.FullHealth; }
                else { ArmorSystem.PlayerHealth_HUD.color = ArmorSystem.LowHealth; }
            }
        }

        if (Input.GetButtonDown(CarControllingSystem.HandBrake.ToString()))
        {
            //StartCoroutine(FadeOut(2.01f));
            //FadeInOut.CrossFadeAlpha(0, 2, true);
            //FadeInOut.enabled = false;
            inPaintShop = false;
            inGarageShop = false;
            isInGarage = false;
            Player.GetComponent<V_car_user>().inGarage = false;
            exitPaintShopIcon.SetActive(false);
            exitPaintShopText.enabled = false;
            applyPaintShopIcon.SetActive(false);
            applyPaintShopText.enabled = false;
            StartCoroutine(FadeIn(.2f));
            //transform.position = new Vector3(0, 1, 0);
            transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            transform.Rotate(0, 115, 0);
            transform.GetComponentInParent<CarUserControl>().enabled = true;
            transform.GetComponent<Rigidbody>().isKinematic = false;
            transform.GetComponentInParent<CarUserControl>().enabled = true;
            transform.GetComponentInParent<CarController>().enabled = true;
            transform.GetComponentInParent<CarController>().Move(0, 0, 1, 0);
            if (GarageExitSound != null)
            {
                AudioComponents.OtherAudio.clip = GarageExitSound;
                if (!AudioComponents.OtherAudio.isPlaying) { AudioComponents.OtherAudio.Play(); }
            }
            if (GarageExitPoint != null)
            {
                if (CarCamStateSystem.isHugeVehicle && trailerhitch != null)
                {
                    trailerhitch.SetActive(true);
                }
                transform.position = GarageExitPoint.position;
                transform.rotation = GarageExitPoint.rotation;
            }
            else
            {
                transform.position = new Vector3(0, 1, 0);
            }
        }
        transform.Rotate(Vector3.down * Time.deltaTime * 60);
    }

    void PortalToPaintShop(Vector3 position)
    {

        originalColor = PaintSystem.BodyPartsToColor[0].GetComponent<Renderer>().material.color;

        isInGarage = true;
        inPaintShop = true;
        inGarageShop = false;
        Player.GetComponent<V_car_user>().inGarage = true;
        myUI.Nitro_BackImage.enabled = false;
        myUI.Nitro_FillImage .enabled = false;
        myUI.Nitro_Icon.enabled = false;
        myUI.Mph_text_BackImage.enabled = false;
        myUI.Mph_text.enabled = false;
        UIColorPicker.SetActive(true);
        StartCoroutine( FadeIn(.4f));
        //myUI. FadeInOut.CrossFadeAlpha(1, 1,true);
        //StartCoroutine(FadeOut(5.1f));
        //transform.position = new Vector3(1500, 495, 1000);
        transform.position = position;
        transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        transform.rotation = new Quaternion(0, 0, 0, 0);
        transform.GetComponentInParent<CarUserControl>().enabled = false;
        transform.GetComponent<Rigidbody>().isKinematic = true;

    }

    void PortalToGarage(Vector3 position)
    {
        isInGarage = true;
        inPaintShop = false;
        inGarageShop = true;
        Player.GetComponent<V_car_user>().inGarage = true;
        //FadeInOut.enabled = true;
        //myUI.FadeInOut.CrossFadeAlpha(0.0f, 10.0f, false);
        StartCoroutine(FadeIn(.4f));
        //myUI. FadeInOut.CrossFadeAlpha(1, 1,true);
        //StartCoroutine(FadeOut(5.1f));
        //transform.position = new Vector3(1500, 495, 1000);
        transform.position = position;
        transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        transform.rotation = new Quaternion(0, 0, 0, 0);
        transform.GetComponentInParent<CarUserControl>().enabled = false;
        transform.GetComponent<Rigidbody>().isKinematic = true;

    }

    IEnumerator FadeIn(float waittime)
    {
        FadeInOut.CrossFadeAlpha(1.0f, 0.01f, false);
        yield return new WaitForSeconds(waittime);
        FadeInOut.CrossFadeAlpha(0.0f, 3.0f, false);

    }


    //=========================================================//

    //=================================================================================//
    // ------------------------------FUNCTIONS END-------------------------------------//
    //=================================================================================//

}
