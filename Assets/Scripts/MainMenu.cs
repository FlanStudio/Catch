using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Text ip_address;
    public Text port;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Host_Click()
    {
        PlayerPrefs.DeleteAll();
        
        PlayerPrefs.SetInt("is_host", 0);
        
        PlayerPrefs.Save();

        Application.LoadLevel(1);

    }

    public void Connect_Click()
    {
        PlayerPrefs.DeleteAll();
        
        PlayerPrefs.SetInt("is_host", 1);
        PlayerPrefs.SetString("ip_address", ip_address.text);
        PlayerPrefs.SetString("port", port.text);

        PlayerPrefs.Save();
        if(ip_address.text.Length > 0 && port.text.Length > 0)
        {

            Application.LoadLevel(1);
        }

    }
}
