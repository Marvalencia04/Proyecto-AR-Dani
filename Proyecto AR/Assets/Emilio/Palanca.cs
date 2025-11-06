using UnityEngine;
using Vuforia;


public class Palanca : MonoBehaviour
{
    public GameObject vbBtnObj;
    public Animator cubeAni;

    public Giro2 giro;
    // Start is called before the first frame update
    void Start()
    {
        vbBtnObj = GameObject.Find("BotonPalanca");
        vbBtnObj.GetComponent<VirtualButtonBehaviour>().RegisterOnButtonPressed(OnButtonPressed);
        vbBtnObj.GetComponent<VirtualButtonBehaviour>().RegisterOnButtonReleased(OnButtonReleased);
        cubeAni.GetComponent<Animator>();

    }

    public void OnButtonPressed(VirtualButtonBehaviour vb)
    {
        cubeAni.Play("Cube|Scene");
        int[] generated = giro.GenerateWeightedArray();
        giro.PlayRequested(generated);
        Debug.Log("BTN Presionado");
        Invoke(nameof(ChangeToNone), 3f);

    }
    
    void ChangeToNone()
    {
        cubeAni.Play("none");
    }

    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        
        Debug.Log("BTN Soltado");
    }
    // Update is called once per frame
    void Update()
    {

    }
}
