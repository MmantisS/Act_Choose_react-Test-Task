using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBackground : MonoBehaviour
{
    BackgroundController controller;
    public Texture texture;
    BackgroundController.Layer layer;  

    // Start is called before the first frame update
    void Start()
    {
        controller = BackgroundController.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            layer = controller.background;
            Debug.Log("Here");
        }

        if (Input.GetKey(KeyCode.T))
        {

        }
        else
        {
            if (Input.GetKey(KeyCode.A))
            {
                Debug.Log("there");
                Debug.Log(layer == null);
                layer.TransitionToTexture(texture);
            }
        }
        
    }
}
