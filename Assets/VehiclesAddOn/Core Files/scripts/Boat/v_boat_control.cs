using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;


public class v_boat_control : MonoBehaviour
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

    #region PlayerPositioning System   
    [System.Serializable]
    public class vPlayerPositionSystem
    {//Player Position systems
        public Transform Seat;
        public GameObject sitPoint;
        public Transform com;
    }
    [Tooltip("This handles all the Player Positioning components")]
    [Header("______Player Positioning System ___________________________")]
    public vPlayerPositionSystem PlayerPositionSystem;
    #endregion

    #region Fuel System   
    [System.Serializable]
    public class vFuelSystem
    {//fuel systems
        public bool InfiniteFuel = false;
        public float MaxFuelAmount = 100;
        public float CurrentFuelAmount;
        public float ConsumeRate = 0.05f;
        [HideInInspector]
        public Image GasTankFill;
    }
    [Header("______ Fuel System ___________________________")]
    public vFuelSystem FuelSystem;
    #endregion

    public GameObject Player;
    private v_vehicle_UI myUI;
    private GameObject PlayerHub;
    private Text MyMessageMinor;
    private Text MyMessageMajor;
    private Text exitPaintShopText;
    private GameObject exitPaintShopIcon;
    private Image UISpeedImage;
    private Text UISpeedText;
    private float steervolumeL;
    private float steervolumeR;
    private float gas;
    private float reverse;
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
    [HideInInspector]
    public Text applyPaintShopText;
    private GameObject applyPaintShopIcon;
    private Image FadeInOut;
    private GameObject UIColorPicker;
    private Color originalColor;
    [HideInInspector]
    public AudioClip spraypaint_sfx;
    private bool inPaintShop;
    private bool inGarageShop;
    [HideInInspector]
    public bool isInGarage; // is car currently is Garage
                            //

    private Image RotorFillImage;
    private Text RotorFillText;

  


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


    #region Controller System   
    [System.Serializable]
    public class vControlSystem
    {//controller setups        
        public ControllerSetup Propeller = ControllerSetup.X;
        public ControllerSetup Brake = ControllerSetup.A;
        public ControllerSetup StandardGun = ControllerSetup.LB;
        public ControllerSetup SpecialWeapon = ControllerSetup.RB;
        public ControllerSetup SelectedWeapon1 = ControllerSetup.RT;
        public ControllerSetup SelectedWeapon2 = ControllerSetup.LT;
        public ControllerSetup LightsOnOff = ControllerSetup.Back;
        public ControllerSetup ChaseCameraOnOff = ControllerSetup.RightStickClick;
        public ControllerSetup ExitVehicle = ControllerSetup.Start;
        public ControllerSetup Horn_Hydraulics = ControllerSetup.B;
    }
    [Header("______ Car Controller Setup ___________________________")]
    [Tooltip("Setup the controller input/setup here")]
    public vControlSystem BoatControllingSystem;
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

    #region Audio Components System   
    [System.Serializable]
    public class vAudioSystem
    {//Audio system
        public AudioSource ComboAudio;
        public AudioSource OtherAudio;
        public AudioSource NitroAudio;
        public AudioSource BoatAudio;
        public AudioSource MotorAudio;
        public AudioSource DamageAudio;
    }
    [Header("______ Audio Components System ___________________________")]
    [Tooltip("This handles all the Audio system components")]
    public vAudioSystem AudioComponents;
    #endregion


    public GameObject BoatExitPoint;
    public bool canControl = true;

    //Engine sound
    public AudioClip engineSound;
    //Particle system used for foam from the boat's propeller
    public Transform[] engineSpume;
    //Boat Mass
    public float mass = 3000.0f;
    //Boat motor force
    public float motorForce = 10000.0f;
    //Rudder sensivity
    public int rudderSensivity = 45;
    //Angular drag coefficient
    public float angularDrag = 0.8f;
    //Center of mass offset
    public float cogY = -0.5f;
    //Volume of boat in liters (the higher the volume, the higher the boat will floar)
    public int volume = 9000;
    //Max width, height and length of the boat (used for water dynamics)
    public Vector3 size = new Vector3(3, 3, 6);

    //Drag coefficients along x,y and z directions
    private Vector3 drag = new Vector3(6.0f, 4.0f, 0.2f);
    private float rpmPitch = 0.0f;
    private v_water_surface waterSurface;

    // Use this for initialization
    void Start()
    {
        if (!isInGarage)
        {
            StartCoroutine(SetupUI());
        }
        //Setup rigidbody
        if (!GetComponentInParent<Rigidbody>())
        {
            gameObject.AddComponent<Rigidbody>();
        }
        GetComponentInParent<Rigidbody>().mass = mass;
        GetComponentInParent<Rigidbody>().drag = 0.1f;
        GetComponentInParent<Rigidbody>().angularDrag = angularDrag;
        //GetComponentInParent<Rigidbody>().centerOfMass = PlayerPositionSystem.com.position;
        GetComponentInParent<Rigidbody>().centerOfMass = new Vector3(0, cogY, 0);
        GetComponentInParent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;

        //start engine noise
        if (!GetComponent<AudioSource>())
        {
            gameObject.AddComponent<AudioSource>();
        }
        GetComponentInParent<AudioSource>().clip = engineSound;
        GetComponentInParent<AudioSource>().loop = true;
  
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //If there is no water surface we are colliding with, no boat physics	
       // if (waterSurface == null)
       //     return;



        float motor = 0.0f;
        float steer = 0.0f;

        if (canControl)
        {
            // if using keyboard/mouse
            // if (player.GetComponent<InvectorCarCamera>().inputType == InvectorCarCamera.InputType.MouseKeyboard)
            // {
           
            if (Input.GetAxis("LeftAnalogHorizontal") > 0 || Input.GetAxis("LeftAnalogHorizontal") < 0 || Input.GetAxis("LeftAnalogVertical") > 0 || Input.GetAxis("LeftAnalogVertical") < 0)
            {
                
                motor = Input.GetAxis("LeftAnalogVertical");
                steer = Input.GetAxis("LeftAnalogHorizontal");
              
            }

            else if (Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Horizontal") < 0 || Input.GetAxis("Vertical") > 0 || Input.GetAxis("Vertical") < 0)
            {

                motor = Input.GetAxis("Vertical");
                steer = Input.GetAxis("Horizontal");
            }

           if (Input.GetButton(BoatControllingSystem.Propeller.ToString()))
            {
                motor = motor + 1 * Time.deltaTime;
            }
            if (Input.GetButton(BoatControllingSystem.Brake.ToString()))
            {
                motor = motor - 1 * Time.deltaTime;
            }

        }

        //Get water level and percent under water
        if (waterSurface != null)
        {
            float waterLevel = waterSurface.GetComponent<Collider>().bounds.max.y;
            float distanceFromWaterLevel = transform.position.y - waterLevel;
            float percentUnderWater = Mathf.Clamp01((-distanceFromWaterLevel + 0.5f * size.y) / size.y);


            //BUOYANCY (the force which keeps the boat floating above water)
            //_______________________________________________________________________________________________________

            //the point the buoyancy force is applied onto is calculated based 
            //on the boat's picth and roll, so it will always tilt upwards:
            Vector3 buoyancyPos = new Vector3();
            buoyancyPos = transform.TransformPoint(-new Vector3(transform.right.y * size.x * 0.5f, 0, transform.forward.y * size.z * 0.5f));

            //then it is shifted arcording to the current waves
            buoyancyPos.x += waterSurface.waveXMotion1 * Mathf.Sin(waterSurface.waveFreq1 * Time.time)
                        + waterSurface.waveXMotion2 * Mathf.Sin(waterSurface.waveFreq2 * Time.time)
                        + waterSurface.waveXMotion3 * Mathf.Sin(waterSurface.waveFreq3 * Time.time);
            buoyancyPos.z += waterSurface.waveYMotion1 * Mathf.Sin(waterSurface.waveFreq1 * Time.time)
                        + waterSurface.waveYMotion2 * Mathf.Sin(waterSurface.waveFreq2 * Time.time)
                        + waterSurface.waveYMotion3 * Mathf.Sin(waterSurface.waveFreq3 * Time.time);

            //apply the force
            GetComponentInParent<Rigidbody>().AddForceAtPosition(-volume * percentUnderWater * Physics.gravity, buoyancyPos);

            //ENGINE
            //_______________________________________________________________________________________________________

            //calculate propeller position
            Vector3 propellerPos = new Vector3(0, -size.y * 0.5f, -size.z * 0.5f);
            Vector3 propellerPosGlobal = transform.TransformPoint(propellerPos);

            //apply force only if propeller is under water
            if (propellerPosGlobal.y < waterLevel)
            {

                //direction propeller force is pointing to.
                //mostly forward, rotated a bit according to steering angle
                float steeringAngle = steer * rudderSensivity / 100 * Mathf.Deg2Rad;
                Vector3 propellerDir = transform.forward * Mathf.Cos(steeringAngle) - transform.right * Mathf.Sin(steeringAngle);

                //apply propeller force


                GetComponentInParent<Rigidbody>().AddForceAtPosition(propellerDir * motorForce * motor, propellerPosGlobal);





            }

            //DRAG
            //_______________________________________________________________________________________________________

            //calculate drag force
            Vector3 dragDirection = transform.InverseTransformDirection(GetComponentInParent<Rigidbody>().velocity);
            Vector3 dragForces = -Vector3.Scale(dragDirection, drag);

            //depth of the boat under water (used to find attack point for drag force)
            float depth = Mathf.Abs(transform.forward.y) * size.z * 0.5f + Mathf.Abs(transform.up.y) * size.y * 0.5f;

            //apply force
            Vector3 dragAttackPosition = new Vector3(transform.position.x, waterLevel - depth, transform.position.z);
            GetComponentInParent<Rigidbody>().AddForceAtPosition(transform.TransformDirection(dragForces) * GetComponentInParent<Rigidbody>().velocity.magnitude * (1 + percentUnderWater * (waterSurface.waterDragFactor - 1)), dragAttackPosition);

            //linear drag (linear to velocity, for low speed movement)
            GetComponentInParent<Rigidbody>().AddForce(transform.TransformDirection(dragForces) * 500);

            //rudder torque for steering (square to velocity)
            float forwardVelo = Vector3.Dot(GetComponentInParent<Rigidbody>().velocity, transform.forward);
            GetComponentInParent<Rigidbody>().AddTorque(transform.up * forwardVelo * forwardVelo * rudderSensivity * steer);

            //SOUND
            //_______________________________________________________________________________________________________
            if (Player.GetComponent<V_car_user>().boating == true)
            {
                if (GetComponentInParent<AudioSource>().isPlaying == false)
                {
                    GetComponentInParent<AudioSource>().Play();
                }
                GetComponentInParent<AudioSource>().volume = 0.3f + Mathf.Abs(motor) / 5;

                //slowly adjust pitch to power input
                rpmPitch = Mathf.Lerp(rpmPitch, Mathf.Abs(motor), Time.deltaTime * 0.4f);
                GetComponentInParent<AudioSource>().pitch = 0.3f + 0.7f * rpmPitch;
            }
            //Debug.Log(GetComponent<AudioSource>().pitch.ToString());
            //reset water surface, so we have to stay in contact for boat physics.
            waterSurface = null;
        }


    }

    void Update()
    {
        if (Player.GetComponent<V_car_user>().boating == true)
        {
            canControl = true;
            
            if (Input.GetAxis("LeftAnalogHorizontal") < 0)
            {
                steervolumeL = Input.GetAxis("LeftAnalogHorizontal") / 1;
                
            }
            else if (Input.GetAxis("LeftAnalogHorizontal") > 0)
            {
                steervolumeR = Input.GetAxis("LeftAnalogHorizontal") / 1;
                
            }
            else
            {
                steervolumeL = 0;
                steervolumeR = 0;
            }

            if (Input.GetAxis("LeftAnalogVertical") < 0)
            {
                reverse = Input.GetAxis("LeftAnalogVertical") / 1;

            }
            else if (Input.GetAxis("LeftAnalogVertical") > 0)
            {
                gas = Input.GetAxis("LeftAnalogVertical") / 1;

            }
            else
            {
                reverse = 0;
                gas = 0;
            }
            myUI.BoatGas.fillAmount = Mathf.Abs(gas);
            myUI.BoatReverse.fillAmount = Mathf.Abs(reverse);
            myUI.BoatSteerL.fillAmount = Mathf.Abs(steervolumeL);
            myUI.BoatSteerR.fillAmount = Mathf.Abs(steervolumeR);
        }
        else if (Player.GetComponent<V_car_user>().boating == false)
        {
            canControl = false;
            GetComponentInParent<AudioSource>().Stop();
        }
        if (!isInGarage)
        {

            

            if (Player.GetComponent<V_car_user>().currCar == this.gameObject && Player.GetComponent<V_car_user>().boating == true)
            {
                EnableUI();
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
            myUI.boatStuff.SetActive(false);
            InGarage();
        }

    }
            //Check if we inside water area
            void OnTriggerStay(Collider col)
    {
        if (col.GetComponentInParent<v_water_surface>() != null)
            waterSurface = col.GetComponent<v_water_surface>();
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
            if (Player.GetComponent<V_car_user>().boating)
            {

                if (PlayerHub.activeInHierarchy == false)
                {
                    PlayerHub.SetActive(true);

                    myUI.boatStuff.SetActive(true);
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
                    myUI.Rotor_BackImage.enabled = false;
                    myUI.Rotor_FillImage.enabled = false;
                    myUI.Speedometer_Gauge.SetActive(false);
                    myUI.Rotor_Icon.enabled = false;

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
            PaintCar( UIColorPicker.GetComponent<CUIColorPicker>().Color);
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

        if (Input.GetButtonDown(BoatControllingSystem.Brake.ToString()))
        {

            // change car color based on player choice
            myUI.boatStuff.SetActive(true);
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

        if (Input.GetButtonDown(BoatControllingSystem.Horn_Hydraulics.ToString()))
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
            myUI.boatStuff.SetActive(true);
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
       this.transform.Rotate(Vector3.up * Time.deltaTime * 60);
    }
}

