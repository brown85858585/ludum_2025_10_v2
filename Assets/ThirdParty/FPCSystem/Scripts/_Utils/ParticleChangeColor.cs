using UnityEngine;

public class ParticleChangeColor : MonoBehaviour
{
    private ParticleSystem myParticleSystem;
    private ParticleSystem.MainModule myMainModule;

    public void ChangeParticleColor(Color _color)
    {
        GetInternalComponents();
        if (myParticleSystem == null) { return; } // Security Sentence.

        myMainModule.startColor = _color;
    }

    void Awake()
    {
        GetInternalComponents();
    }

    private void GetInternalComponents()
    {
        if (myParticleSystem == null)
        {
            myParticleSystem = GetComponent<ParticleSystem>();
        }

        if (myParticleSystem != null)
        {
            myMainModule = myParticleSystem.main;
        }
    }
    
}