using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Typer : MonoBehaviour
{
    [SerializeField]
    private string text;
    public string Text { get { return text; } set { text = value; } }

    [SerializeField]
    private Text textComponent;

    [SerializeField]
    private float typingDelay;

    [SerializeField]
    private float startDelay;

    [SerializeField]
    private Color textColor;
    public Color TextColor { get { return textColor; } set { textColor = value; } }

    private void OnEnable()
    {
        textComponent.color = textColor;

        StartCoroutine(TypeRoutine());
    }

    private IEnumerator TypeRoutine()
    {
        yield return new WaitForSeconds(startDelay);

        for (int i = 0; i < text.Length + 1; i++)
        {
            string newText = Substitute(i);

            textComponent.text = newText;

            yield return new WaitForSeconds(typingDelay);
        }
    }

    private string Substitute(int charactersAmount)
    {
        StringBuilder stringBuilder = new StringBuilder(text);

        int replacementLength = text.Length - charactersAmount;

        stringBuilder.Length -= replacementLength;

        return stringBuilder.ToString();
    }

    private void OnDisable()
    {
        textComponent.text = string.Empty;
    }
}
