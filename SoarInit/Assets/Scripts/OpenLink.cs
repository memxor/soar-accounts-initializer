using TMPro;
using UnityEngine;

public class OpenLink : MonoBehaviour
{
    public void Open(TextMeshProUGUI val)
    {
        Application.OpenURL(val.text);
    }
}
