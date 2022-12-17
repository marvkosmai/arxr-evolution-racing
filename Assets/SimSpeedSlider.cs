using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;

public class SimSpeedSlider : MonoBehaviour
{
    public Slider sldr_SimSpeed;
    public GameObject txt_val_SimSpeed;

   public void Start()
   {
      //Adds a listener to the main input field and invokes a method when the value changes.
      sldr_SimSpeed.onValueChanged.AddListener(delegate {updateValue(); });
   }
    public void updateValue()
     {
        int value = (int)sldr_SimSpeed.value;
        txt_val_SimSpeed.GetComponent<TextMeshProUGUI>().text = value + "";
        Time.timeScale = value;
     }
}
