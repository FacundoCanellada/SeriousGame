using UnityEngine;

public class SpinWheelFixedTarget : MonoBehaviour
{
    [SerializeField] private float minSpinSpeed = 500f;
    [SerializeField] private float maxSpinSpeed = 1000f;
    [SerializeField] private float deceleration = 300f;
    [SerializeField] private float targetAngle = 90f; // Ángulo donde está la planta

    private bool isSpinning = false;
    private float currentSpeed;
    private float totalRotation;

    void Update()
    {
        if (isSpinning)
        {
            // Rota según la velocidad
            transform.Rotate(0f, 0f, currentSpeed * Time.deltaTime);
            totalRotation += currentSpeed * Time.deltaTime;

            // Desacelera gradualmente
            currentSpeed -= deceleration * Time.deltaTime;

            if (currentSpeed <= 0)
            {
                isSpinning = false;

                // Ajusta el ángulo final para que termine en la planta
                Vector3 finalRotation = transform.eulerAngles;
                finalRotation.z = targetAngle;
                transform.eulerAngles = finalRotation;

                Debug.Log("Ruleta detenida en el icono de la planta 🌿");
            }
        }
    }

    public void Spin()
    {
        if (!isSpinning)
        {
            currentSpeed = Random.Range(minSpinSpeed, maxSpinSpeed);
            totalRotation = 0f;
            isSpinning = true;
        }
    }
}
