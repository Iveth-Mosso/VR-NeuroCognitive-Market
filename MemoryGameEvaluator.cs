using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class MemoryGameEvaluator : MonoBehaviour
{
    public List<GameObject> selectedObjects; // Lista de objetos seleccionados por el sistema
    private HashSet<string> scannedObjectNames = new HashSet<string>(); // Nombres de objetos ya escaneados
    public TMP_Text resultText; // Texto para mostrar resultados

    public AudioClip successSound; // Sonido para objetos correctos
    public AudioClip failureSound; // Sonido para objetos incorrectos

    public string successMessage = "¡Escaneo correcto!";
    public string failureMessage = "¡Objeto incorrecto!";
    public string allScannedMessage = "¡Todos los objetos escaneados correctamente!";

    private string lastMessage = ""; // Último mensaje mostrado

    void Start()
    {
        if (selectedObjects == null)
        {
            selectedObjects = new List<GameObject>();
            Debug.LogError("La lista 'selectedObjects' no estaba inicializada. Se ha creado una nueva lista vacía.");
        }

        // Limpia elementos nulos
        selectedObjects.RemoveAll(obj => obj == null);

        if (selectedObjects.Count == 0)
        {
            Debug.LogWarning("La lista 'selectedObjects' está vacía. Asegúrate de asignar los objetos seleccionados.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"OnTriggerEnter llamado con: {other.gameObject.name}");

        string objectName = other.gameObject.name.Replace("(Clone)", "").Trim(); // Limpia el nombre del objeto
        Debug.Log($"Objeto detectado: {objectName}");

        // Verifica si el objeto ya fue escaneado
        if (scannedObjectNames.Contains(objectName))
        {
            UpdateText($"El objeto {objectName} ya fue escaneado.");
            return;
        }

        // Verifica si el objeto está en la lista de nombres seleccionados
        if (IsObjectSelected(objectName))
        {
            scannedObjectNames.Add(objectName); // Marca el objeto como escaneado

            // Cambia el color del objeto a verde si tiene Renderer
            Renderer renderer = other.gameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = new Material(renderer.material); // Clona el material
                renderer.material.color = Color.green;
                Debug.Log($"Color cambiado a verde para {objectName}");
            }
            else
            {
                Debug.LogWarning($"El objeto {objectName} no tiene un Renderer.");
            }

            // Reproduce el sonido de éxito
            if (successSound != null)
            {
                AudioSource.PlayClipAtPoint(successSound, transform.position);
            }

            UpdateText($"Objeto correcto: {objectName}");
            CheckCompletion();
        }
        else
        {
            Debug.Log($"Objeto incorrecto detectado: {objectName}");

            // Cambia el color del objeto a rojo si tiene Renderer
            Renderer renderer = other.gameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = new Material(renderer.material); // Clona el material
                renderer.material.color = Color.red;
                Debug.Log($"Color cambiado a rojo para {objectName}");
            }
            else
            {
                Debug.LogWarning($"El objeto {objectName} no tiene un Renderer.");
            }

            // Reproduce el sonido de error
            if (failureSound != null)
            {
                AudioSource.PlayClipAtPoint(failureSound, transform.position);
            }

            UpdateText($"Objeto incorrecto: {objectName}");
        }
    }

    private bool IsObjectSelected(string objectName)
    {
        // Limpia elementos nulos de la lista antes de procesar
        selectedObjects.RemoveAll(obj => obj == null);

        foreach (var obj in selectedObjects)
        {
            if (obj.name.Replace("(Clone)", "").Trim() == objectName)
            {
                return true;
            }
        }
        return false;
    }

    void CheckCompletion()
    {
        // Verifica si todos los objetos seleccionados fueron escaneados
        if (scannedObjectNames.Count == selectedObjects.Count)
        {
            UpdateText(allScannedMessage);
        }
    }

    public void EvaluateSelection()
    {
        foreach (var obj in selectedObjects)
        {
            if (obj == null)
            {
                Debug.LogError("Un objeto en 'selectedObjects' es nulo.");
                continue;
            }

            string cleanName = obj.name.Replace("(Clone)", "").Trim();
            if (!scannedObjectNames.Contains(cleanName))
            {
                UpdateText("Faltan objetos por escanear.");
                return;
            }
        }

        UpdateText(allScannedMessage);
    }

    private void UpdateText(string message)
    {
        if (message != lastMessage) // Solo actualiza si el mensaje cambia
        {
            lastMessage = message;

            if (resultText != null)
            {
                resultText.text = message; // Actualiza el texto en pantalla
            }

            Debug.Log(message); // También lo imprime en la consola
        }
    }
}
