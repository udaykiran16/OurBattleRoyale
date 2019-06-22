using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;
using Invector.CharacterController;

public class v_car_enter_paintshop : MonoBehaviour
{
    public AudioClip PaintShopEnterSound;
    public AudioClip PaintShopExitSound;
    public Transform PaintShopEnterPoint;
    public Transform PaintShopExitPoint;
    private AudioSource audioSource;
    private GameObject trailerback;
    public AudioClip spraypaint_sfx;
    public bool CarMayUse;
    public bool HelicopterMayUse;
    public bool BoatMayUse;
    public bool GliderMayUse;
    public bool JetMayUse;
    private GameObject trailer;



    void Start()
    {
        audioSource = GetComponent<AudioSource>();

    }

    void OnTriggerEnter(Collider hit)
    {
        if (CarMayUse) { CarPortal(hit); }

        if (HelicopterMayUse) { HelicopterPortal(hit); }

        if (GliderMayUse) { GliderPortal(hit); }

        if (BoatMayUse) { BoatPortal(hit); }

        if (JetMayUse) { JetPortal(hit); }

    }


    public void HelicopterPortal(Collider hit)
    {
        if (hit.gameObject.tag == "choppaDoor")
        {
            if (hit.GetComponent<v_helicopter_control>() != null)
            {
                hit.GetComponent<v_helicopter_control>().PaintShopExitPoint = PaintShopExitPoint;


                if (PaintShopExitSound != null)
                {
                    hit.GetComponent<v_helicopter_control>().PaintShopExitSound = PaintShopExitSound;
                }
                if (spraypaint_sfx != null)
                {
                    hit.GetComponent<v_helicopter_control>().spraypaint_sfx = spraypaint_sfx;
                }


            }
            if (PaintShopEnterSound != null)
            {
                audioSource.clip = PaintShopEnterSound;
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            if (hit.GetComponent<v_helicopter_control>().PaintShopExitPoint != null)
            {
                //hit.GetComponent<v_helicopter_control>().NitrousSystem.boosting = false;

                hit.GetComponent<v_helicopter_control>().applyPaintShopText.text = "Apply Changes";
                //hit.GetComponent<v_helicopter_control>().exitPaintShopText.text = "Cancel";
                hit.gameObject.SendMessage("PortalToPaintShop", PaintShopEnterPoint.position);
            }
        }

    }

    public void CarPortal(Collider hit)
    {
        if (hit.gameObject.tag == "carDoor")
        {
            if (hit.GetComponent<V_CarControl>() != null)
            {
                hit.GetComponent<V_CarControl>().PaintShopExitPoint = PaintShopExitPoint;
                if (hit.GetComponent<V_CarControl>().PlayerDriving)
                {
                    if (hit.GetComponent<V_CarControl>().CarCamStateSystem.isHugeVehicle)
                    {
                      
                         trailer = hit.transform.Find("trailer").gameObject;
                        if (trailer != null)
                        {
                            hit.GetComponent<V_CarControl>().trailerhitch = trailer;



                            trailer.SetActive(false);
                        }
                        //else Debug.Log("no Trailer found on this vehicle");
                        
                    }
                    if (PaintShopExitSound != null)
                    {
                        hit.GetComponent<V_CarControl>().PaintShopExitSound = PaintShopExitSound;
                    }
                    if (spraypaint_sfx != null)
                    {
                        hit.GetComponent<V_CarControl>().spraypaint_sfx = spraypaint_sfx;
                    }
                }

            }
            if (PaintShopEnterSound != null)
            {
                audioSource.clip = PaintShopEnterSound;
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            if (hit.GetComponent<V_CarControl>().PaintShopExitPoint != null)
            {
                hit.GetComponent<V_CarControl>().NitrousSystem.boosting = false;
                hit.GetComponentInParent<CarController>().Move(0, 0, 1, 0);
                hit.gameObject.GetComponentInParent<CarUserControl>().enabled = false;
                hit.gameObject.GetComponentInParent<CarController>().enabled = false;
                hit.GetComponent<V_CarControl>().applyPaintShopText.text = "Apply Changes";
                hit.GetComponent<V_CarControl>().exitPaintShopText.text = "Cancel";
                hit.gameObject.SendMessage("PortalToPaintShop", PaintShopEnterPoint.position);
            }
        }

}

    public void BoatPortal(Collider hit)
    {
        if (hit.gameObject.tag == "boatDoor")
        {
            if (hit.GetComponent<v_boat_control>() != null)
            {
                hit.GetComponent<v_boat_control>().PaintShopExitPoint = PaintShopExitPoint;


                if (PaintShopExitSound != null)
                {
                    hit.GetComponent<v_boat_control>().PaintShopExitSound = PaintShopExitSound;
                }
                if (spraypaint_sfx != null)
                {
                    hit.GetComponent<v_boat_control>().spraypaint_sfx = spraypaint_sfx;
                }


            }
            if (PaintShopEnterSound != null)
            {
                audioSource.clip = PaintShopEnterSound;
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            if (hit.GetComponent<v_boat_control>().PaintShopExitPoint != null)
            {
                //hit.GetComponent<v_helicopter_control>().NitrousSystem.boosting = false;

               hit.GetComponent<v_boat_control>().applyPaintShopText.text = "Apply Changes";
                //hit.GetComponent<v_helicopter_control>().exitPaintShopText.text = "Cancel";
                hit.gameObject.SendMessage("PortalToPaintShop", PaintShopEnterPoint.position);
            }
        }
    }

    public void GliderPortal(Collider hit)
    {
        if (hit.gameObject.tag == "glider")
        {
            Debug.Log("glider hit");
            if (hit.GetComponent<V_GliderControl>() != null)
            {
                hit.GetComponent<V_GliderControl>().PaintShopExitPoint = PaintShopExitPoint;


                if (PaintShopExitSound != null)
                {
                    hit.GetComponent<V_GliderControl>().PaintShopExitSound = PaintShopExitSound;
                }
                if (spraypaint_sfx != null)
                {
                    hit.GetComponent<V_GliderControl>().spraypaint_sfx = spraypaint_sfx;
                }


            }
            if (PaintShopEnterSound != null)
            {
                audioSource.clip = PaintShopEnterSound;
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            if (hit.GetComponent<V_GliderControl>().PaintShopExitPoint != null)
            {
                //hit.GetComponent<v_helicopter_control>().NitrousSystem.boosting = false;

                hit.GetComponent<V_GliderControl>().applyPaintShopText.text = "Apply Changes";
                //hit.GetComponent<v_helicopter_control>().exitPaintShopText.text = "Cancel";
                hit.gameObject.SendMessage("PortalToPaintShop", PaintShopEnterPoint.position);
            }
        }

    }

    public void JetPortal(Collider hit)
    {
        //
    }
}
