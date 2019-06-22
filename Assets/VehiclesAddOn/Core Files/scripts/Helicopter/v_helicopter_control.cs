using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class v_helicopter_control : MonoBehaviour {

    public GameObject Player;
    private v_vehicle_UI myUI;
    private GameObject PlayerHub;
    private Text MyMessageMinor;
    private Text MyMessageMajor;
    private bool inPaintShop;
    private bool inGarageShop;
    [HideInInspector]
    public bool isInGarage; // is car currently is Garage
    private Image FadeInOut;
    private GameObject UIColorPicker;
    [HideInInspector]
    public Text applyPaintShopText;
    private GameObject applyPaintShopIcon;
    private Color originalColor;
    [HideInInspector]
    public Transform PaintShopExitPoint;
    [HideInInspector]
    public AudioClip PaintShopExitSound;




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
        public ControllerSetup RotorSpeedUp = ControllerSetup.X;
        public ControllerSetup RotorSpeedDown = ControllerSetup.A;
        public ControllerSetup Turbo = ControllerSetup.Y;
        public ControllerSetup StandardGun = ControllerSetup.LB;
        public ControllerSetup SpecialWeapon = ControllerSetup.RB;
        public ControllerSetup SelectedWeapon1 = ControllerSetup.RT;
        public ControllerSetup SelectedWeapon2 = ControllerSetup.LT;
        public ControllerSetup LightsOnOff = ControllerSetup.Back;
        public ControllerSetup ChaseCamera = ControllerSetup.RightStickClick;
        public ControllerSetup ExitVehicle = ControllerSetup.Start;
        public ControllerSetup Horn_Hydraulics = ControllerSetup.B;
    }
    [Header("______ Helicopter Controller Setup ___________________________")]
    [Tooltip("Setup the controller input/setup here")]
    public vControlSystem HelicopterControllingSystem;
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

    #region Door System   
    [System.Serializable]
    public class vDoorSystem
    {//door system
        public GameObject DoorHinge;
        //public float DoorOpenSpeed;
        //public float DoorCloseSpeed;
        public bool openDoor = false;
        [Tooltip("This is the soundclip used for Custom Open Door sound, leave blank if you wish to use default sounds (setup on player )")]
        public AudioClip OpenDoorSound;
        [Tooltip("This is the soundclip used for Custom Close Door sound, leave blank if you wish to use default sounds (setup on player )")]
        public AudioClip CloseDoorSound;
    }
    [Header("______ Door System ___________________________")]
    public vDoorSystem DoorSystem;
    #endregion

    #region Armor System   
    [System.Serializable]
    public class vArmorSystem
    {//armor system
        [HideInInspector]
        public int currentArmor;
        public int MaxArmor;
        [HideInInspector]
        public bool doOnce;
        [HideInInspector]
        public int DamageValue;
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
        public Color FullHealth = new Color(40f, 255f, 0f, 255f);
        public Color MedHealth = new Color(255f, 0f, 0f, 255f);
        public Color LowHealth = new Color(255f, 0f, 0f, 255f);
        public bool AlwaysShowHUD;
        [HideInInspector]
        public bool isPlayerDriving;
    }
    [Header("______ Armor System ___________________________")]
    [Tooltip("This handles all the Armor system components")]
    public vArmorSystem ArmorSystem;
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

    #region Grounding System   
    [System.Serializable]
    public class vGroundSystem
    {//grounding system
        public string GroundTag;
        public GameObject DebrisParticles;
        public float dist = 3f;
        public GameObject RaycastObject;
        public bool IsOnGround = true;
    }
    [Header("______ Grounding System ___________________________")]
    public vGroundSystem GroundingSystem;
    #endregion

    #region Lighting System   
    [System.Serializable]
    public class vLightingSystem
    {//lighting system
        public Light[] HelicopterLights;
        [Header("=================================")]
        public GameObject[] VehicleFlares;
    }
    [Header("______ Lighting System ___________________________")]
    public vLightingSystem LightingSystem;
    #endregion

    #region Fuel System   
    [System.Serializable]
    public class vFuelSystem
    {//fuel systems
        public bool InfiniteFuel = false;
        public float MaxFuelAmount=100;
        public float CurrentFuelAmount;
        public float ConsumeRate = 0.05f;
        [HideInInspector]
        public Image GasTankFill;
    }
    [Header("______ Fuel System ___________________________")]
    public vFuelSystem FuelSystem;
    #endregion

    #region Motor System   
    [System.Serializable]
    public class vMotorSystem
    {
        [HideInInspector]
        public bool OnOff;
        [HideInInspector]
        public Vector3 dir;
        public AudioSource HelicopterSound;
        public Rigidbody HelicopterModel;
        public GameObject[] Blades;
        public GameObject[] RotorBlurImage;
        [HideInInspector]
        public bool BlurOn;
        public HeliRotorController MainRotorController;
        public HeliRotorController SubRotorController;
    }
    [Header("______ Motor System ___________________________")]
    public vMotorSystem MotorSystem;
    #endregion

    #region Audio Components System   
    [System.Serializable]
    public class vAudioSystem
    {//Audio system
        public AudioSource ComboAudio;
        public AudioSource OtherAudio;
        public AudioSource NitroAudio;
        public AudioSource MotorAudio;
        public AudioSource DamageAudio;
    }
    [Header("______ Audio Components System ___________________________")]
    [Tooltip("This handles all the Audio system components")]
    public vAudioSystem AudioComponents;
    #endregion

    [Header("______ Controls System ___________________________")]
    public float TurnForce = 10f;
    public float ForwardForce = 10f;
    public float ForwardTiltForce = 20f;
    public float TurnTiltForce = 30f;
    public float EffectiveHeight = 100f;
    public float turnTiltForcePercent = 4.2f;
    public float turnForcePercent = 2.54f;
    public float _engineForce;
    public float EngineForce
    {
        get { return _engineForce; }
        set
        {
            MotorSystem.MainRotorController.RotarSpeed = value * 80;
            MotorSystem.SubRotorController.RotarSpeed = value * 40;
            MotorSystem.HelicopterSound.pitch = Mathf.Clamp(value / 40, 0, 1.2f);
            _engineForce = value;
        }
    }

    private Vector2 hMove = Vector2.zero;
    private Vector2 hTilt = Vector2.zero;
    private float hTurn = 0f;
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
    //door original position/rotation
    private Vector3 doorPos;
    private Quaternion doorRot;

    private HingeJoint hinge;
    private JointMotor motor;
    private JointLimits hlimits;
    // Controller setup
    private string s_gas;
    private string s_brake;
    private string s_turbo;
    private string s_gunstandard;
    private string s_specialweapon;
    private string s_weapon1;
    private string s_weapon2;
    private string s_lightsonoff;
    private string s_chasecam;
    private string s_exitvehicle;
    private string s_sirenHydraulics;
    //
    private Image UISpeedImage;
    private Text UISpeedText;
    private Image RotorFillImage;
    private Text RotorFillText;
    private Text exitPaintShopText;
    private GameObject exitPaintShopIcon;
    [HideInInspector]
    public AudioClip spraypaint_sfx;

    // Use this for initialization
    void Start () {
        if (!isInGarage)
        {
            StartCoroutine(SetupUI());
            SetupDriveControls();
            //intialize door hinges
            if (DoorSystem.DoorHinge != null) { StartCoroutine(DestroyHingesOnStart()); }

            MotorSystem.BlurOn = false;
            MotorSystem.dir = new Vector3(0, -1, 0);
            // objPlayer = (GameObject)GameObject.FindWithTag("Player");
            foreach (GameObject GO in MotorSystem.RotorBlurImage) { GO.SetActive(false); }
            foreach (Light VL in LightingSystem.HelicopterLights) { VL.enabled = false; }
            foreach (GameObject VF in LightingSystem.VehicleFlares) { VF.SetActive(false); }
            // Fuel system intialize values
            FuelSystem.CurrentFuelAmount = FuelSystem.MaxFuelAmount;
        }
    }

    // Update is called once per frame
    void Update() {

        if (!isInGarage) {

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

            if (Player.GetComponent<V_car_user>().currCar == this.gameObject && Player.GetComponent<V_car_user>().flying == true)
        {
            EnableUI();

            
                RaycastHit hit = new RaycastHit();


            if (EngineForce > 9 && MotorSystem.BlurOn == false)
            {
                foreach (GameObject GO in MotorSystem.RotorBlurImage) { GO.SetActive(true); }
                foreach (GameObject GB in MotorSystem.Blades)
                {
                    GB.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                }
                MotorSystem.BlurOn = true;
            }

            if (EngineForce < 9 && MotorSystem.BlurOn == true)
            {
                foreach (GameObject GO in MotorSystem.RotorBlurImage) { GO.SetActive(false); }
                foreach (GameObject GB in MotorSystem.Blades)
                {
                    GB.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                }
                MotorSystem.BlurOn = false;
            }


            #region Fuel System
            // if we are not using infinite fuel and have run out of fuel, stop the car from running
            if (FuelSystem.InfiniteFuel == false && FuelSystem.CurrentFuelAmount <= 0)
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
            // flying
            float tempY = 0;
            float tempX = 0;

            // stable forward
            if (hMove.y > 0)
                tempY = -Time.fixedDeltaTime;
            else
                if (hMove.y < 0)
                tempY = Time.fixedDeltaTime;

            // stable lurn
            if (hMove.x > 0)
                tempX = -Time.fixedDeltaTime;
            else
                if (hMove.x < 0)
                tempX = Time.fixedDeltaTime;
            float h;
            float v;
            if (Input.GetAxis("LeftAnalogHorizontal") > 0 || Input.GetAxis("LeftAnalogVertical") > 0 || Input.GetAxis("LeftAnalogHorizontal") < 0 || Input.GetAxis("LeftAnalogVertical") < 0)
            {
                h = Input.GetAxis("LeftAnalogHorizontal");
                v = Input.GetAxis("LeftAnalogVertical");
            }
            else if (Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Vertical") > 0 || Input.GetAxis("Horizontal") < 0 || Input.GetAxis("Vertical") < 0)
            {
                h = Input.GetAxis("Horizontal");
                v = Input.GetAxis("Vertical");
            }
            else
            {
                //== let helicopter drift at last value if neither joystick or keyboard is being used==//
                h = 0.0f;
                v = 0.0f;
            }

            if (h > 0) { if (!GroundingSystem.IsOnGround) tempX = Time.fixedDeltaTime; } //right
            if (h < 0) { if (!GroundingSystem.IsOnGround) tempX = -Time.fixedDeltaTime; } //left
            if (v > 0) { if (!GroundingSystem.IsOnGround) tempY = Time.fixedDeltaTime; } //forward
            if (v < 0) { if (!GroundingSystem.IsOnGround) tempY = -Time.fixedDeltaTime; } //back


            if (Input.GetButton(HelicopterControllingSystem.RotorSpeedUp.ToString()))
            {
                if (EngineForce < 50)
                {
                    EngineForce += 0.1f;
                }
            } //engine force --

            if (Input.GetButton(HelicopterControllingSystem.RotorSpeedDown.ToString())) { EngineForce -= 0.12f; if (EngineForce < 0) EngineForce = 0; } //engine force --
            if (Input.GetButton(HelicopterControllingSystem.LightsOnOff.ToString()) && MotorSystem.OnOff == false) { StartCoroutine(HeadlightsOn()); }
            else if (Input.GetButton(HelicopterControllingSystem.LightsOnOff.ToString()) && MotorSystem.OnOff == true) { StartCoroutine(HeadlightsOff()); }

            if (RotorFillImage != null)
            {
                float fillpercent = EngineForce / 50;
                RotorFillImage.fillAmount = fillpercent;
            }

            hMove.x += tempX;
            hMove.x = Mathf.Clamp(hMove.x, -1, 1);

            hMove.y += tempY;
            hMove.y = Mathf.Clamp(hMove.y, -1, 1);
        }
    }
        else
        {
            exitPaintShopIcon.SetActive(true);
            exitPaintShopText.enabled = true;
            applyPaintShopIcon.SetActive(true);
            applyPaintShopText.enabled = true;
            FadeInOut.enabled = true;
            if (!Cursor.visible)
                Cursor.visible = true;

            //start the Garage function which includes
            //customizing your car by paint color
            InGarage();
        }
    }

    void FixedUpdate()
    {
        LiftProcess();
        MoveProcess();
        TiltProcess();
    }

    void SetupDriveControls()
    {
        s_gas = HelicopterControllingSystem.RotorSpeedUp.ToString();
        s_brake = HelicopterControllingSystem.RotorSpeedDown.ToString();
        s_turbo = HelicopterControllingSystem.Turbo.ToString();
        s_gunstandard = HelicopterControllingSystem.StandardGun.ToString();
        s_specialweapon = HelicopterControllingSystem.SpecialWeapon.ToString();
        s_weapon1 = HelicopterControllingSystem.SelectedWeapon1.ToString();
        s_weapon2 = HelicopterControllingSystem.SelectedWeapon2.ToString();
        s_lightsonoff = HelicopterControllingSystem.LightsOnOff.ToString();
        s_chasecam = HelicopterControllingSystem.ChaseCamera.ToString();
        s_exitvehicle = HelicopterControllingSystem.ExitVehicle.ToString();
        s_sirenHydraulics = HelicopterControllingSystem.Horn_Hydraulics.ToString();
    }

    private void MoveProcess()
    {
        var turn = TurnForce * Mathf.Lerp(hMove.x, hMove.x * (turnTiltForcePercent - Mathf.Abs(hMove.y)), Mathf.Max(0f, hMove.y));
        hTurn = Mathf.Lerp(hTurn, turn, Time.fixedDeltaTime * TurnForce);
        MotorSystem.HelicopterModel.AddRelativeTorque(0f, hTurn * MotorSystem.HelicopterModel.mass, 0f);
        MotorSystem.HelicopterModel.AddRelativeForce(Vector3.forward * Mathf.Max(0f, hMove.y * ForwardForce * MotorSystem.HelicopterModel.mass));
    }

    private void LiftProcess()
    {
        var upForce = 1 - Mathf.Clamp(MotorSystem.HelicopterModel.transform.position.y / EffectiveHeight, 0, 1);
        upForce = Mathf.Lerp(0f, EngineForce, upForce) * MotorSystem.HelicopterModel.mass;
        MotorSystem.HelicopterModel.AddRelativeForce(Vector3.up * upForce);
    }

    private void TiltProcess()
    {
        hTilt.x = Mathf.Lerp(hTilt.x, hMove.x * TurnTiltForce, Time.deltaTime);
        hTilt.y = Mathf.Lerp(hTilt.y, hMove.y * ForwardTiltForce, Time.deltaTime);
        MotorSystem.HelicopterModel.transform.localRotation = Quaternion.Euler(hTilt.y, MotorSystem.HelicopterModel.transform.localEulerAngles.y, -hTilt.x);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == GroundingSystem.GroundTag)
        {
            GroundingSystem.IsOnGround = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == GroundingSystem.GroundTag)
        {
            GroundingSystem.IsOnGround = false;
        }
    }

    IEnumerator HeadlightsOn()
    {
        foreach (Light VL in LightingSystem.HelicopterLights) { VL.enabled = true; }
        yield return new WaitForSeconds(.1f);
        MotorSystem.OnOff = true;
        foreach (GameObject VF in LightingSystem.VehicleFlares) { VF.SetActive(true); }
    }

    IEnumerator HeadlightsOff()
    {
        foreach (Light VL in LightingSystem.HelicopterLights) { VL.enabled = false; }
        yield return new WaitForSeconds(.1f);
        MotorSystem.OnOff = false;
        foreach (GameObject VF in LightingSystem.VehicleFlares) { VF.SetActive(false); }
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
        motor.targetVelocity = 140;
        hinge2.motor = motor;
        hinge2.limits = hlimits;
        hinge2.useMotor = true;
        yield return new WaitForSeconds(4.6f);
        //motor.targetVelocity = -DoorSystem.DoorCloseSpeed;
        motor.targetVelocity = -160;
        if (hinge2 != null)
        {
            hinge2.motor = motor;
        }
    }

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

    public void DestroyJointOnDrive()
    {
        StartCoroutine(DestroyHinges());
    }

    IEnumerator SetupUI()
    {

        yield return new WaitForSeconds(.01f);
        myUI = Player.GetComponent<V_car_user>().myHub.GetComponent<v_vehicle_UI>();
        yield return new WaitForSeconds(.01f);
        PlayerHub = Player.GetComponent<V_car_user>().myHub;
        yield return new WaitForSeconds(.01f);
        PlayerHub.SetActive(false);
        myUI = Player.GetComponent<V_car_user>().myHub.GetComponent<v_vehicle_UI>();
        PlayerHub = Player.GetComponent<V_car_user>().myHub;
        ArmorSystem.PlayerHealth_HUD = myUI.Health_FillImage;
        UISpeedImage = myUI.Mph_FillImage;
        UISpeedText = myUI.Mph_text;
        FuelSystem.GasTankFill = myUI.Fuel_FillImage;
        MyMessageMajor = myUI.MessageMajor;
        MyMessageMinor = myUI.MessageMinor;
        RotorFillImage = myUI.Rotor_FillImage;
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
            if (Player.GetComponent<V_car_user>().flying)
            {

                // if the vehicle UI is not active
                // and we are currently driving then set it to active
                if (PlayerHub.activeInHierarchy == false)
                {
                    PlayerHub.SetActive(true);

                    myUI.boatStuff.SetActive(false);
                    myUI.Nitro_BackImage.enabled = false;
                    myUI.Nitro_FillImage.enabled = false;
                    myUI.Nitro_Icon.enabled = false;
                    myUI.MessageMajor.enabled = false;
                    myUI.MessageMinor.enabled = false;
                    exitPaintShopIcon.SetActive(false);
                    exitPaintShopText.enabled = false;
                    applyPaintShopIcon.SetActive(false);
                    applyPaintShopText.enabled = false;
                    if (!isInGarage)
                    {
                        FadeInOut.enabled = false;
                    }
                    if (!inPaintShop)
                    {
                        UIColorPicker.SetActive(false);
                    }

                    //-----------------------------//
                    myUI.Fuel_BackImage.enabled = true;
                    myUI.Fuel_FillImage.enabled = true;
                    myUI.Health_Icon.enabled = true;
                    myUI.Health_FillImage.enabled = true;
                    myUI.Health_BackImage.enabled = true;
                    myUI.Mph_text.enabled = true;
                    myUI.Mph_text_BackImage.enabled = true;
                    myUI.playerName.enabled = true;
                    myUI.Rotor_BackImage.enabled = true;
                    myUI.Rotor_FillImage.enabled = true;
                    myUI.Speedometer_Gauge.SetActive(false);
                    myUI.Rotor_Icon.enabled = true;

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

    void PortalToPaintShop(Vector3 position)
    {

        originalColor = PaintSystem.BodyPartsToColor[0].GetComponent<Renderer>().material.color;

        isInGarage = true;
        inPaintShop = true;
        inGarageShop = false;
        Player.GetComponent<V_car_user>().inGarage = true;
        myUI.boatStuff.SetActive(false);
        myUI.Nitro_BackImage.enabled = false;
        myUI.Nitro_FillImage.enabled = false;
        myUI.Nitro_Icon.enabled = false;
        myUI.Mph_text_BackImage.enabled = false;
        myUI.Mph_text.enabled = false;
        UIColorPicker.SetActive(true);
        StartCoroutine(FadeIn(.4f));
        //myUI. FadeInOut.CrossFadeAlpha(1, 1,true);
        //StartCoroutine(FadeOut(5.1f));
        //transform.position = new Vector3(1500, 495, 1000);
        transform.position = position;
        transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        transform.rotation = new Quaternion(0, 0, 0, 0);
        //transform.GetComponentInParent<CarUserControl>().enabled = false;
        transform.GetComponent<Rigidbody>().isKinematic = true;

    }

    IEnumerator FadeIn(float waittime)
    {
        FadeInOut.CrossFadeAlpha(1.0f, 0.01f, false);
        yield return new WaitForSeconds(waittime);
        FadeInOut.CrossFadeAlpha(0.0f, 3.0f, false);

    }

    public void InGarage()
    {

        if (inPaintShop)
        {
            PaintCar(UIColorPicker.GetComponent<CUIColorPicker>().Color);
        }

        if (inGarageShop)
        {
           // UpgradeCar();
        }

    }

    public void PaintCar(Color color)
    {
        // change car color based on player choice
        if (PaintSystem.BodyPartsToColor.Length > 0 && PaintSystem.BodyPartsToColor != null)
        {
            foreach (GameObject GO in this.PaintSystem.BodyPartsToColor)
            {
                Renderer[] rends = GO.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in rends) { r.material.color = color; }
            }
        }

        if (Input.GetButtonDown(HelicopterControllingSystem.RotorSpeedDown.ToString()))
        {

            // change car color based on player choice
            myUI.boatStuff.SetActive(false);
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
            //transform.GetComponentInParent<CarUserControl>().enabled = true;
            transform.GetComponent<Rigidbody>().isKinematic = false;
           // transform.GetComponentInParent<CarUserControl>().enabled = true;
            //transform.GetComponentInParent<CarController>().enabled = true;
            //transform.GetComponentInParent<CarController>().Move(0, 0, 1, 0);
            if (PaintShopExitSound != null)
            {
                AudioComponents.OtherAudio.clip = PaintShopExitSound;
                if (!AudioComponents.OtherAudio.isPlaying) { AudioComponents.OtherAudio.Play(); }
            }
            if (spraypaint_sfx != null)
            {
                AudioComponents.ComboAudio.clip = spraypaint_sfx;
                if (!AudioComponents.ComboAudio.isPlaying) { AudioComponents.ComboAudio.Play(); }
            }
            if (PaintShopExitPoint != null)
            {
                //if (isHugeVehicle && trailerhitch != null)
                //{
                //    trailerhitch.SetActive(true);
                //}
                transform.position = PaintShopExitPoint.position;
                transform.rotation = PaintShopExitPoint.rotation;
            }
            else
            {
                transform.position = new Vector3(0, 1, 0);
            }
        }

        if (Input.GetButtonDown(HelicopterControllingSystem.Horn_Hydraulics.ToString()))
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
            myUI.boatStuff.SetActive(false);
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
            //transform.GetComponentInParent<CarUserControl>().enabled = true;
            transform.GetComponent<Rigidbody>().isKinematic = false;
            //transform.GetComponentInParent<CarUserControl>().enabled = true;
            //transform.GetComponentInParent<CarController>().enabled = true;
            //transform.GetComponentInParent<CarController>().Move(0, 0, 1, 0);
            if (PaintShopExitSound != null)
            {
                AudioComponents.OtherAudio.clip = PaintShopExitSound;
                if (!AudioComponents.OtherAudio.isPlaying) { AudioComponents.OtherAudio.Play(); }
            }
            if (PaintShopExitPoint != null)
            {
                //if (isHugeVehicle && trailerhitch != null)
                //{
                //    trailerhitch.SetActive(true);
                //}
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
}
