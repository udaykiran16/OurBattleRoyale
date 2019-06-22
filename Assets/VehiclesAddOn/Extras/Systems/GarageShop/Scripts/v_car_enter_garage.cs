using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;
using Invector.CharacterController;

public class v_car_enter_garage : MonoBehaviour
{
    public AudioClip GarageEnterSound;
    public AudioClip GarageExitSound;
    public Transform GarageEnterPoint;
    public Transform GarageExitPoint;
    private AudioSource audioSource;
    private GameObject trailerback;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

    }
    void OnTriggerEnter(Collider hit)
    {

        if (hit.gameObject.tag == "carDoor")
        {
            if (hit.GetComponent<V_CarControl>() != null)
            {
                hit.GetComponent<V_CarControl>().GarageExitPoint = GarageExitPoint;
                if (hit.GetComponent<V_CarControl>().PlayerDriving)
                {
                    if (hit.GetComponent<V_CarControl>().CarCamStateSystem.isHugeVehicle)
                    {
                        GameObject trailer = hit.transform.Find("trailer").gameObject;
                        hit.GetComponent<V_CarControl>().trailerhitch = trailer;


                        trailer.SetActive(false);
                    }
                    if (GarageExitSound != null)
                    {
                        hit.GetComponent<V_CarControl>().GarageExitSound = GarageExitSound;
                    }
                }

            }
            if (GarageEnterSound != null)
            {
                audioSource.clip = GarageEnterSound;
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            if (hit.GetComponent<V_CarControl>().GarageExitPoint != null)
            {
                hit.GetComponent<V_CarControl>().NitrousSystem.boosting = false;
                hit.GetComponentInParent<CarController>().Move(0, 0, 1, 0);
                hit.gameObject.GetComponentInParent<CarUserControl>().enabled = false;
                hit.gameObject.GetComponentInParent<CarController>().enabled = false;
                hit.GetComponent<V_CarControl>().exitPaintShopText.text = "Exit Garage";
                hit.gameObject.SendMessage("PortalToGarage", GarageEnterPoint.position);
            }
        }

    }
}
