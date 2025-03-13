using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // 👈 Necesario para cambiar de escena

public class npc_dialogue : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public Button continueButton; // Botón para avanzar diálogo
    public Button acceptButton;   // 👈 Botón "Aceptar"
    public Button declineButton;  // 👈 Botón "No"

    public string[] dialogue;
    private int index = 0;

    public float wordSpeed = 0.06f;
    private bool isTyping = false;

    void Start()
    {
        dialogueText.text = "";
        dialoguePanel.SetActive(false); // Inicia oculto

        // Asegurar que los botones están ocultos al inicio
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(NextLine);
            continueButton.gameObject.SetActive(false);
        }

        if (acceptButton != null && declineButton != null)
        {
            acceptButton.onClick.AddListener(AcceptDialogue);
            declineButton.onClick.AddListener(DeclineDialogue);
            acceptButton.gameObject.SetActive(false);
            declineButton.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Jugador ha entrado en el área del NPC");
            dialoguePanel.SetActive(true);
            StartCoroutine(Typing());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Jugador ha salido del área del NPC");
            RemoveText();
        }
    }

    IEnumerator Typing()
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in dialogue[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }

        isTyping = false;

        if (index == dialogue.Length - 1) // 👈 Si es el último diálogo, mostrar botones de decisión
        {
            continueButton.gameObject.SetActive(false);
            acceptButton.gameObject.SetActive(true);
            declineButton.gameObject.SetActive(true);
        }
        else
        {
            continueButton.gameObject.SetActive(true);
        }
    }

    public void NextLine()
    {
        if (index < dialogue.Length - 1)
        {
            index++;
            dialogueText.text = "";
            continueButton.gameObject.SetActive(false); // Ocultar mientras escribe
            StartCoroutine(Typing());
        }
        else
        {
            RemoveText();
        }
    }

    public void RemoveText()
    {
        if (dialoguePanel != null && dialoguePanel.activeInHierarchy)
        {
            StopAllCoroutines();
            dialogueText.text = "";
            index = 0;
            dialoguePanel.SetActive(false);

            continueButton.gameObject.SetActive(false);
            acceptButton.gameObject.SetActive(false);
            declineButton.gameObject.SetActive(false);
        }
    }

    public void AcceptDialogue()
    {
        Debug.Log("Jugador aceptó la oferta. Cambiando de escena...");
        SceneManager.LoadScene("Game2");
    }

    public void DeclineDialogue()
    {
        Debug.Log("Jugador rechazó la oferta. Cerrando diálogo.");
        RemoveText();
    }
}
