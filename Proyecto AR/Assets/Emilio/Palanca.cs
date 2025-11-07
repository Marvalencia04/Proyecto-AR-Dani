using UnityEngine;
using Vuforia;


public class Palanca : MonoBehaviour
{
    public GameObject vbBtnObj;
    public Animator cubeAni;
    public Giro giro;

    public float tiempoReactivacion = 4f;

    void Start()
    {
        vbBtnObj = GameObject.Find("BotonPalanca");
        vbBtnObj.GetComponent<VirtualButtonBehaviour>().RegisterOnButtonPressed(OnButtonPressed);
        vbBtnObj.GetComponent<VirtualButtonBehaviour>().RegisterOnButtonReleased(OnButtonReleased);
    }

    public void OnButtonPressed(VirtualButtonBehaviour vb)
    {
        // DESACTIVO BOTON
        vbBtnObj.SetActive(false);

        cubeAni.Play("Cube|Scene");


        giro.IntentarGiro();

        Debug.Log("BTN Presionado");

        Invoke(nameof(ReactivarBoton), tiempoReactivacion);
        Invoke(nameof(ChangeToNone), tiempoReactivacion);
    }

    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        Debug.Log("BTN Soltado");
    }

    void ChangeToNone()
    {
        cubeAni.Play("none");
    }

    void ReactivarBoton()
    {
        vbBtnObj.SetActive(true);
        Debug.Log("BOTON REACTIVADO");
    }
}
