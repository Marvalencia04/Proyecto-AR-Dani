using UnityEngine;
using UnityEngine.SceneManagement;

public class SalirApp : MonoBehaviour
{
    public void Salir() => Application.Quit();



    public void IrJuego()
    {
        // Cargar la escena especificada
        SceneManager.LoadScene("PruebaEmilio");
    }
}
