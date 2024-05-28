using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpcontroller : MonoBehaviour
{
    public GameObject PopUpOpciones;
    public GameObject PopUpcreditos;

    public void Mostrarpopup()
    {
        PopUpOpciones.SetActive(true);

    }

    public void Ocultarpopup()
    {
        PopUpOpciones.SetActive(false);

    }

    public void Mostrarcreditos()
    {
        PopUpcreditos.SetActive(true);
    }

    public void Ocultarcreditos()
    {
        PopUpcreditos.SetActive(false);
    }
}
