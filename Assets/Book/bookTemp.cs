using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Elixir]
public class bookTemp : MonoBehaviour
{
    int index = 0;
    public Texture2D[] leftPages;
    public Texture2D[] RightPages;
    public Renderer left;
    public Renderer right;
    // Start is called before the first frame update
    void Start()
    {
        left.material.SetTexture("_BaseMap", leftPages[index]);
        right.material.SetTexture("_BaseMap", RightPages[index]);
    }

    public void ForwardPage(){
        if(index < leftPages.Length-1)
            index++;
        else{
            index = 0;
        }
        left.material.SetTexture("_BaseMap", leftPages[index]);
        right.material.SetTexture("_BaseMap", RightPages[index]);
    }
    public void BackPage(){
        if(index > 0)
            index--;
        else{
            index = leftPages.Length-1;
        }
        left.material.SetTexture("_BaseMap", leftPages[index]);
        right.material.SetTexture("_BaseMap", RightPages[index]);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
    
}
