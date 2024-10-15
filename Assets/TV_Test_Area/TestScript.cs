using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    ScreenResolution[] _screenResolution;
    // Start is called before the first frame update
    void Start()
    {
        _screenResolution = new ScreenResolution[3];
        _screenResolution[0] = new ScreenResolution(1920, 1080);
        _screenResolution[1] = new ScreenResolution(720, 480);
        _screenResolution[2] = new ScreenResolution(540, 540);
    }


    public void ChangeScreenSize(int index)
    {
        ScreenResolution x = _screenResolution[index];
        Screen.SetResolution(x.width, x.height, false);
    }
    struct ScreenResolution
    {
        public int width, height;
        public ScreenResolution(int w, int h)
        {
            width = w;
            height = h;
        }
    }
}
