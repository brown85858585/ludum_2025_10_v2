using UnityEngine;

public class AIAnimation : MonoBehaviour
{
    [Tooltip("Velocidad minima para ver al enemigo correr en pantalla (con su animacion de corrr).")]
    public float minimumRunSpeed = 1f;

    private Animation aiAnimation;

    void Start()
    {
        aiAnimation = GetComponent<Animation>();

        // Poner todas las animaciones en Loop
        aiAnimation.wrapMode = WrapMode.Loop;

        // El disparo no es Lopp, solo se ejecuta una vez.
        aiAnimation["shoot"].wrapMode = WrapMode.Once;

        // Poner idle y Run en un layer menor. S�lo se animaran si las animaciones de accion no se estan viendo.
        aiAnimation["idle"].layer = -1;
        aiAnimation["walk"].layer = -1;
        aiAnimation["run"].layer = -1;
        aiAnimation["idle"].speed = 1;
        aiAnimation["walk"].speed = 2;
        aiAnimation["run"].speed = 2;
        aiAnimation.Stop();
    }

    public void SetSpeed(float speed)
    {
        if (speed > minimumRunSpeed)
        {
            aiAnimation.CrossFade("run");
        }
        else if (speed > 0)
        {
            aiAnimation.CrossFade("walk");
        }
        else
        {
            aiAnimation.CrossFade("idle");
        }
    }

}