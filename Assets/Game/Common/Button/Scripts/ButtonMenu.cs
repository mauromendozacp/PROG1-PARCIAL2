using UnityEngine;

public class ButtonMenu : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform = null;
    [SerializeField] private float normalSize = 0f;
    [SerializeField] private float selectedSize = 0f;

    public void ToggleSelected(bool status)
    {
        float size = status ? selectedSize : normalSize;
        rectTransform.localScale = new Vector3(size, size, size);
    }
}
