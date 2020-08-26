using UnityEngine;

public class ColorHsv2RGB : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float m_Hue;
    [Range(0.0f, 1.0f)]
    public float m_Saturation;
    [Range(0.0f, 1.0f)]
    public float m_Value;
    //Make sure your GameObject has a Renderer component in the Inspector window
    Renderer m_Renderer;

    void Start()
    {
        //Fetch the Renderer component from the GameObject with this script attached
        m_Renderer = GetComponent<Renderer>();

    }

    void Update()
    {
        //Change the Color of your GameObject to the new Color
        m_Renderer.material.color = Color.HSVToRGB(m_Hue, m_Saturation, m_Value);
    }
}