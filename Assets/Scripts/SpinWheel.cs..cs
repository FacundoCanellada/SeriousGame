using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cargar escenas

public class SpinWheelFixedTarget : MonoBehaviour
{
    [SerializeField] private float minSpinSpeed = 500f;
    [SerializeField] private float maxSpinSpeed = 1000f;
    [SerializeField] private float deceleration = 300f;
    [SerializeField] private AudioSource audioSource; // 🔊 arrastrá tu AudioSource acá

    private bool isSpinning = false;
    private float currentSpeed;
    private float totalRotation;

    // 🔹 Ángulo fijo donde debe detenerse
    private float targetAngle = 270f;

    void Update()
    {
        if (isSpinning)
        {
            // Rota la ruleta
            transform.Rotate(0f, 0f, currentSpeed * Time.deltaTime);
            totalRotation += currentSpeed * Time.deltaTime;

            // Desacelera gradualmente
            currentSpeed -= deceleration * Time.deltaTime;

            // Arranca el sonido si no se está reproduciendo
            if (!audioSource.isPlaying)
                audioSource.Play();

            // Cuando la velocidad llega a 0, se detiene la ruleta
            if (currentSpeed <= 0)
            {
                isSpinning = false;
                currentSpeed = 0;
                audioSource.Stop();

                // 🔹 Ajusta el ángulo final exactamente en 90°
                Vector3 finalRotation = transform.eulerAngles;
                finalRotation.z = targetAngle;
                transform.eulerAngles = finalRotation;

                Debug.Log("✅ Ruleta detenida en 90° — Cargando nivel...");

                // 🔹 Carga el nivel (asegurate que esté en Build Settings)
                SceneManager.LoadScene("BiologiaScene");
            }
        }
    }

    // Llamá este método desde un botón o evento para girar
    public void Spin()
    {
        if (!isSpinning)
        {
            currentSpeed = Random.Range(minSpinSpeed, maxSpinSpeed);
            totalRotation = 0f;
            isSpinning = true;
            Debug.Log("🎡 Ruleta girando...");
        }
    }
}