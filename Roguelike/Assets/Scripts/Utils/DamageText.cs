using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
	[Header("Damage Text")]
	public Text Text;

	private float duration;
	private RectTransform rectTransform;
	private Vector2 textStartPosition, textEndPosition;
	private Coroutine TextCoroutine;

	void Start()
	{
		rectTransform = Text.GetComponent<RectTransform>();
		textStartPosition = rectTransform.anchoredPosition;
		textEndPosition = new Vector2(textStartPosition.x, textStartPosition.y+0.5f);
		duration = 1f;
	}

	public void ShowDamage(string text)
	{
		if (Text.enabled)
			StopCoroutine(TextCoroutine);
		TextCoroutine = StartCoroutine(ShowText(text));
		
	}

	IEnumerator ShowText(string text)
	{
		Text.text = text;
		Text.enabled = true;

		float elapsedTime = 0;

		while (elapsedTime < duration)
		{
			float t = elapsedTime / duration; //0 means the animation just started, 1 means it finished
			rectTransform.anchoredPosition = Vector2.Lerp(textStartPosition, textEndPosition, t);
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		Text.enabled = false;
	}
}