using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class CharacterAppearance : MonoBehaviour
{
    [Header("Hombre")]
    public RuntimeAnimatorController maleController;
    public Sprite maleIdleSprite;

    [Header("Mujer")]
    public RuntimeAnimatorController femaleController;
    public Sprite femaleIdleSprite;

    void Awake()
    {
        ApplySelectedCharacter();
    }

    public void ApplySelectedCharacter()
    {
        string selected = PlayerPrefs.GetString("SelectedCharacter", "Male");
        Animator animator = GetComponent<Animator>();
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        bool isFemale = selected == "Female";

        if (isFemale)
        {
            if (femaleController != null)
                animator.runtimeAnimatorController = femaleController;
            if (femaleIdleSprite != null)
                spriteRenderer.sprite = femaleIdleSprite;
        }
        else
        {
            if (maleController != null)
                animator.runtimeAnimatorController = maleController;
            if (maleIdleSprite != null)
                spriteRenderer.sprite = maleIdleSprite;
        }
    }
}
