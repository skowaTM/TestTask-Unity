using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private HealthBar healthbar;

    [SerializeField] private GameObject stickMovement;
    [SerializeField] private GameObject stickLook;
    public GameObject StickMovement { get => stickMovement; }
    public GameObject StickLook { get => stickLook; }
    public float StickRange { get; private set; }

    [SerializeField] private Text textBooster;
    [SerializeField] private Text textDeath;

    [SerializeField] private Image menuPanel;

    [SerializeField] private Canvas noteCanvas;


    private void Awake()
    {
        StickRange = Mathf.Max(stickMovement.GetComponentInChildren<OnScreenStick>().movementRange,
                               stickLook.GetComponentInChildren<OnScreenStick>().movementRange);
    }


    public void UpdateHealthBar(float curHP, float maxHP)
    {
        healthbar.UpdateHealthBar(curHP, maxHP);
    }


    // Displaying remaining time of booster effect
    public void UpdateTextBooster(float val)
    {
        if (val > 0f)
        {
            if (!textBooster.enabled)
            {
                textBooster.enabled = true;
            }

            textBooster.text = $"Speed Boost: {val.ToString("0.0")}";
        }
        else
        {
            textBooster.enabled = false;
        }
    }


    // Switching sticks enabled value
    public void SwitchSticksEnabled(bool enabled)
    {
        stickMovement.GetComponentInChildren<OnScreenStick>().enabled = enabled;
        stickMovement.transform.GetChild(0).localPosition = stickMovement.transform.GetChild(1).localPosition;

        stickLook.GetComponentInChildren<OnScreenStick>().enabled = enabled;
        stickLook.transform.GetChild(0).localPosition = stickLook.transform.GetChild(1).localPosition;
    }


    // Show/Hide menu panel
    public void ShowMenuPanel(bool isActive)
    {
        menuPanel.gameObject.SetActive(isActive);
    }

    // Show/Hide the note with a text
    public void ShowNoteWithText(bool isActive, string text)
    {
        if (isActive)
        {
            noteCanvas.GetComponentInChildren<Text>().text = text;
        }
        noteCanvas.gameObject.SetActive(isActive);
    }


    // Just simple parody on 'YOU DIED' like in Dark Souls
    public IEnumerator TextDeathEffect()
    {
        float targetAlpha = 255f;
        float targetScale = 1.5f;

        textDeath.enabled = true;

        Color color = textDeath.color;
        Vector3 scale = textDeath.transform.localScale;
        while (color.a != targetAlpha && scale.x != targetScale)
        {
            if (color.a != targetAlpha)
            {
                color.a = Mathf.MoveTowards(color.a, targetAlpha, 0.5f * Time.deltaTime);
                textDeath.color = color;
            }
            if (scale.x != targetScale)
            {
                scale.x = Mathf.MoveTowards(scale.x, targetScale, 0.5f * Time.deltaTime);
                scale.y = Mathf.MoveTowards(scale.y, targetScale, 0.5f * Time.deltaTime);
                textDeath.transform.localScale = scale;
            }

            yield return null;
        }

        yield return new WaitForSeconds(1f);
        textDeath.enabled = false;
    }
}
