using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Elixir]

public class ChangePage : MonoBehaviour
{
    public bool back = false;
    public GameObject bookOpen;
    // Start is called before the first frame update
    void Start()
    {
        if(back){
            GetComponent<Renderer>().material.color = Color.red;
        }
        else{
            GetComponent<Renderer>().material.color = Color.blue;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if(back){
            bookOpen.GetComponent<bookTemp>().BackPage();
        }else{
            bookOpen.GetComponent<bookTemp>().ForwardPage();
        }
    }
}
