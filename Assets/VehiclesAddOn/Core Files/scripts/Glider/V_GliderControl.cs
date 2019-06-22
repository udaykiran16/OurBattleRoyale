using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class V_GliderControl : MonoBehaviour {

    public GameObject Player;//Player
    private v_vehicle_UI myUI;
    private GameObject PlayerHub;
    private Text MyMessageMinor;
    private Text MyMessageMajor;
    private Text exitPaintShopText;
    private GameObject exitPaintShopIcon;
    [HideInInspector]
    public Text applyPaintShopText;
    private GameObject applyPaintShopIcon;
    private Image FadeInOut;
    private GameObject UIColorPicker;
    private Color originalColor;
    private bool inPaintShop;
    private bool inGarageShop;
    [HideInInspector]
    public bool isInGarage; // is car currently is Garage

    #region PlayerPositioning System   
    [System.Serializable]
    public class vPlayerPositionSystem
    {//Player Position systems
        public GameObject sitPoint;
    }
    [Tooltip("This handles all the Player Positioning components")]
    [Header("______Player Positioning System ___________________________")]
    public vPlayerPositionSystem PlayerPositionSystem;
    #endregion

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
        public ControllerSetup ExitVehicle = ControllerSetup.Start;
        public ControllerSetup Brake = ControllerSetup.A;
        public ControllerSetup Horn_Hydraulics = ControllerSetup.B;
    }
    [Header("______ Glider Controller Setup ___________________________")]
    [Tooltip("Setup the controller input/setup here")]
    public vControlSystem GliderControllingSystem;
    #endregion

    #region Controls System   
    [System.Serializable]
    public class vControlsSystem
    {//control setups     
        public float moveSpeed;
        public float maxSpeed;
        public float rotSpeed;
    }
    [Header("______ Glider Control Setup ___________________________")]
    [Tooltip("Setup the control here")]
    public vControlsSystem GliderControlSystem;
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

    #region Audio Components System   
    [System.Serializable]
    public class vAudioSystem
    {//Audio system
        public AudioSource ComboAudio;
        public AudioSource OtherAudio;
        public AudioSource NitroAudio;
        public AudioSource DamageAudio;
    }
    [Header("______ Audio Components System ___________________________")]
    [Tooltip("This handles all the Audio system components")]
    public vAudioSystem AudioComponents;
    #endregion

    public bool gliderBroken;
    public bool applyGravity;
    [HideInInspector]
    public Image UISpeedImage;
    [HideInInspector]
    public Text UISpeedText;
    private float deadZone = .1f;
   
    private bool OnOff;
    public string SafeLandTag;

    // Controller setup
    private string s_exitvehicle;
    private string s_brakevehicle;
    private string s_sirenHydraulics;
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
    public AudioClip spraypaint_sfx;

    // Use this for initialization
    void Start () {
        if (!isInGarage)
        {
            StartCoroutine(SetupUI());
            SetupDriveControls();
        }

    }
	
	// Update is called once per frame
	void Update () {
        if (!isInGarage)
        {
            EnableUI();
            if (Player.GetComponent<V_car_user>().gliding == true && (Player.GetComponent<V_car_user>().currCar == this.gameObject && !gliderBroken))
            {

                if (Input.GetButtonDown(s_exitvehicle)) { StartCoroutine(Player.GetComponent<V_car_user>().SafeOutGlider()); }


                // if GUI is enabled show information
                if (UISpeedImage != null)
                {
                    UISpeedImage.enabled = true;
                    float Speed = GliderControlSystem.moveSpeed / GliderControlSystem.maxSpeed;
                    UISpeedImage.fillAmount = Speed;
                }
                if (UISpeedText != null)
                {
                    UISpeedText.enabled = true;
                    UISpeedText.text = GliderControlSystem.moveSpeed.ToString("###") + " MPH";
                }
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
        if (!isInGarage)
        {
            if (Player.GetComponent<V_car_user>().gliding == true && (Player.GetComponent<V_car_user>().currCar == this.gameObject && !gliderBroken))
            {
                GetLocomotionInput();
                if (!GetComponent<AudioSource>().isPlaying) { GetComponent<AudioSource>().Play(); }
            }
            else { GetComponent<AudioSource>().Stop(); }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player" && GliderControlSystem.moveSpeed >= 10 && Player.GetComponent<V_car_user>().gliding == true && Player.GetComponent<V_car_user>().GlideON == true)

        {
            StartCoroutine(Player.GetComponent<V_car_user>().GetOutGlider());

            gliderBroken = true;
        }

        if (collision.gameObject.tag != "Player" && GliderControlSystem.moveSpeed < 10 && Player.GetComponent<V_car_user>().gliding == true && Player.GetComponent<V_car_user>().GlideON == true && collision.gameObject.tag == SafeLandTag)
        {

            SafeLanding();
        }

    }

    void ApplyGrav()
    {
        applyGravity = true;
    }

    public void TurnGravityOn()
    {
        StartCoroutine(GravityOn());
    }

    public void TurnGravityOff()
    {
        StartCoroutine(GravityOff());
    }

    IEnumerator GravityOff()
    {
        Debug.Log("Gravity Turned Off");
        applyGravity = false;
        yield return new WaitForSeconds(.1f);
        OnOff = true;
    }

    IEnumerator GravityOn()
    {
        Debug.Log("Gravity Turned On");
        applyGravity = true;
        yield return new WaitForSeconds(.1f);
        OnOff = false;
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
            if (Player.GetComponent<V_car_user>().gliding)
            {

                // if the vehicle UI is not active
                // and we are currently driving then set it to active
                if (PlayerHub.activeInHierarchy == false)
                {
                    PlayerHub.SetActive(true);
                    myUI.boatStuff.SetActive(false);
                    myUI.Fuel_BackImage.enabled = false;
                    myUI.Fuel_FillImage.enabled = false;
                    myUI .Nitro_BackImage.enabled = false;
                    myUI.Nitro_FillImage.enabled = false;
                    myUI.Nitro_Icon.enabled = false;
                    myUI.MessageMajor.enabled = false;
                    myUI.MessageMinor.enabled = false;
                    myUI.Rotor_BackImage.enabled = false;
                    myUI.Rotor_FillImage.enabled = false;
                    myUI.Rotor_Icon.enabled = false;
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
                    myUI.Health_Icon.enabled = true;
                    myUI.Health_FillImage.enabled = true;
                    myUI.Health_BackImage.enabled = true;
                    myUI.Mph_text.enabled = true;
                    myUI.Mph_text_BackImage.enabled = true;
                    myUI.playerName.enabled = true;

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

    void GetLocomotionInput()
    {

        float gravityMultplr = (100 / GliderControlSystem.moveSpeed) * 2;
        if (applyGravity)
        {
            this.GetComponent<Rigidbody>().AddForce(Vector3.up * -GliderControlSystem.moveSpeed * gravityMultplr);
        }
        if (Input.GetAxisRaw("LeftAnalogVertical") > 0 || Input.GetAxisRaw("Vertical") > 0)
        {
            GliderControlSystem.moveSpeed = Mathf.Lerp(GliderControlSystem.moveSpeed, GliderControlSystem.maxSpeed, Time.deltaTime * 0.5f);
            GetComponent<AudioSource>().pitch = Mathf.Lerp(GetComponent<AudioSource>().pitch, 2f, Time.deltaTime * 0.5f);
        }
        else if (Input.GetAxisRaw("LeftAnalogVertical") < 0 || Input.GetAxisRaw("Vertical") < 0)
        {
            GliderControlSystem.moveSpeed = Mathf.Lerp(GliderControlSystem.moveSpeed, 1, Time.deltaTime);
            GetComponent<AudioSource>().pitch = Mathf.Lerp(GetComponent<AudioSource>().pitch, 0.5f, Time.deltaTime * 0.5f);
        }

        else if (Input.GetButton(GliderControllingSystem.Brake.ToString()) )
        {
            GliderControlSystem.moveSpeed = Mathf.Lerp(GliderControlSystem.moveSpeed, .01f, Time.deltaTime);
            GetComponent<AudioSource>().pitch = Mathf.Lerp(GetComponent<AudioSource>().pitch, 0.5f, Time.deltaTime * 0.5f);
        }

        else
        {
            GliderControlSystem.moveSpeed = Mathf.Lerp(GliderControlSystem.moveSpeed, 10, Time.deltaTime * 0.1f);
        }

        Vector3 ahead = this.transform.forward;

        this.GetComponent<Rigidbody>().AddForce(ahead * GliderControlSystem.moveSpeed);

        if (Input.GetAxis("Horizontal") > deadZone || Input.GetAxis("Horizontal") < -deadZone )
        {
            this.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.forward * -GliderControlSystem.rotSpeed * 2 * Input.GetAxis("Horizontal"));
            GetComponent<Rigidbody>().AddRelativeTorque(Vector3.up * GliderControlSystem.rotSpeed * Input.GetAxis("Horizontal"));
        }

        if (Input.GetAxis("LeftAnalogHorizontal") > deadZone || Input.GetAxis("LeftAnalogHorizontal") < -deadZone)
        {
            this.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.forward * -GliderControlSystem.rotSpeed * 2 * Input.GetAxis("LeftAnalogHorizontal"));
            GetComponent<Rigidbody>().AddRelativeTorque(Vector3.up * GliderControlSystem.rotSpeed * Input.GetAxis("LeftAnalogHorizontal"));
        }
        if (Input.GetAxisRaw("Vertical") > deadZone || Input.GetAxis("Vertical") < -deadZone)
        {
            this.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.right * GliderControlSystem.rotSpeed / 4 * Input.GetAxis("Vertical"));
        }
        if (Input.GetAxisRaw("LeftAnalogVertical") > deadZone || Input.GetAxis("LeftAnalogVertical") < -deadZone)
        {
            this.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.right * GliderControlSystem.rotSpeed / 4 * Input.GetAxis("LeftAnalogVertical"));
        }
        
        Quaternion _lookRotation = Quaternion.LookRotation(ahead);
        _lookRotation.z = 0;
        _lookRotation.x = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * 2f);
        GetComponent<Rigidbody>().velocity = ahead * 10;
    }

    void SafeLanding()
    {
        StartCoroutine(Player.GetComponent<V_car_user>().SafeOutGlider());
    }

    void SetupDriveControls()
    {
        s_exitvehicle = GliderControllingSystem.ExitVehicle.ToString();
        s_brakevehicle = GliderControllingSystem.Brake.ToString();
        s_sirenHydraulics = GliderControllingSystem.Horn_Hydraulics.ToString();
    }

    void PortalToPaintShop(Vector3 position)
    {
        Debug.Log("glider triggered");
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

        if (Input.GetButtonDown(GliderControllingSystem.Brake.ToString()))
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

        if (Input.GetButtonDown(GliderControllingSystem.Horn_Hydraulics.ToString()))
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
