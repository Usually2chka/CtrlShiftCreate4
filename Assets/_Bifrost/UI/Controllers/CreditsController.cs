using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class CreditsController : MonoBehaviour
{
    public UIDocument uiDocument;

    private VisualElement root;
    private Label creditsText;

    private float positionY;
    private float speed = 120f;
    private float colorHue = 0f;

    private float minDuration = 14f;
    private float timer = 0f;

    public bool IsFinished { get; private set; }

    private void Awake()
    {
        root = uiDocument.rootVisualElement;
        creditsText = root.Q<Label>("creditsText");

        root.style.display = DisplayStyle.None;
    }

    public void Show()
    {
        IsFinished = false;

        root.style.display = DisplayStyle.Flex;

        positionY = -800f; // Начальная позиция выше экрана
        creditsText.style.translate = new Translate(0, positionY);

        StartCoroutine(ScrollRoutine());
    }

    public void Hide()
    {
        root.style.display = DisplayStyle.None;
    }

    private IEnumerator ScrollRoutine()
    {
        Debug.Log("Credits scroll started");
        timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            colorHue = (colorHue + Time.deltaTime * 0.08f) % 1f;
            creditsText.style.color = new StyleColor(Color.HSVToRGB(colorHue, 0.8f, 1f));

            positionY += speed * Time.deltaTime;
            creditsText.style.translate = new Translate(0, positionY);

            bool textFinished = positionY > creditsText.resolvedStyle.height + 100;
            bool timeFinished = timer >= minDuration;

            if (timeFinished && textFinished)
            {
                IsFinished = true;
                Debug.Log("Credits finished!");
                yield break;
            }

            yield return null;
        }
    }
}