using UnityEngine;
using System.Collections;

public class v_car_enter_gas : MonoBehaviour {
    public AudioClip GasEnterSound;
    public AudioClip GasFinishedSound;
    public AudioClip GasPumpingSound;
    private bool playOnce;
    private AudioSource audioSource;
    public AudioSource EnterExitAudiosource;
    private bool fullTank;

    // Use this for initialization
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
                EnterExitAudiosource.clip = GasEnterSound;
                if (!EnterExitAudiosource.isPlaying)
                {
                    hit.transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    EnterExitAudiosource.Play();
                    playOnce = false;
                    hit.GetComponent<V_CarControl>().isPumpingGas = true;
                }
                
            }
        }
    }


    void OnTriggerExit(Collider hit)
    {

        if (hit.gameObject.tag == "carDoor")
        {
            if (hit.GetComponent<V_CarControl>() != null)
            {
                if (hit.GetComponent<V_CarControl>().PlayerDriving == true)
                {
                    fullTank = false;
                    audioSource.loop = false;
                    audioSource.Stop();
                    hit.GetComponent<V_CarControl>().PlayerHub.GetComponent<v_vehicle_UI>().MessageMinor.text = "";
                    hit.GetComponent<V_CarControl>().PlayerHub.GetComponent<v_vehicle_UI>().MessageMinor.enabled = false;
                    hit.GetComponent<V_CarControl>().isPumpingGas = false;
                }
            }
        }
    }                 

    void OnTriggerStay(Collider hit)
    {
        if (hit.gameObject.tag == "carDoor")
        {
            if (hit.GetComponent<V_CarControl>() != null)
            {
                if (hit.GetComponent<V_CarControl>().PlayerDriving == true)
                {

                    if (hit.GetComponent<V_CarControl>().FuelSystem.CurrentFuelAmount < hit.GetComponent<V_CarControl>().FuelSystem.MaxFuelAmount && !fullTank )
                    {
                        hit.GetComponent<V_CarControl>().PlayerHub.GetComponent<v_vehicle_UI>().MessageMinor.color = Color.green;
                        hit.GetComponent<V_CarControl>().PlayerHub.GetComponent<v_vehicle_UI>().MessageMinor.text = "Re-Fueling";
                        hit.GetComponent<V_CarControl>().PlayerHub.GetComponent<v_vehicle_UI>().MessageMinor.enabled = true;
                        
                        if (!audioSource.isPlaying )
                        {
                            audioSource.clip = GasPumpingSound;
                            audioSource.loop = true;
                            audioSource.Play();
                        }
                        hit.transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        hit.GetComponent<V_CarControl>().FuelSystem.CurrentFuelAmount = hit.GetComponent<V_CarControl>().FuelSystem.CurrentFuelAmount + 5 * Time.deltaTime;
                    }
                    else
                    {
                        fullTank = true;
                        audioSource.loop = false;
                        audioSource.Stop();
                        hit.GetComponent<V_CarControl>().PlayerHub.GetComponent<v_vehicle_UI>().MessageMinor.color = Color.red;
                        hit.GetComponent<V_CarControl>().PlayerHub.GetComponent<v_vehicle_UI>().MessageMinor.text = "Finished Re-Fueling";
                        hit.GetComponent<V_CarControl>().PlayerHub.GetComponent<v_vehicle_UI>().MessageMinor.enabled = true;
                        if (!playOnce)
                        {
                            EnterExitAudiosource.clip = GasFinishedSound;
                            if (!EnterExitAudiosource.isPlaying) { EnterExitAudiosource.Play(); }
                            playOnce = true;
                        }
                    }
                }
            }
        }
     }

 }
