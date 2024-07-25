using UnityEngine;
using UnityEngine.UI;

public class LoadMenuUI : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void ShowMenu(bool value) => gameObject.SetActive(value);

    public void UdateSlider(float value) => slider.value = value;
}
