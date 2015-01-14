using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GenerateUI : MonoBehaviour {

	public TerrainGenerator Generator;

	public InputField DetailInput;
	public InputField RoughnessInput;
	public Scrollbar RoughnessSlider;

	void Start()
	{
		DetailInput.text = Generator.Detail.ToString();
		RoughnessInput.text = Generator.Roughness.ToString();
		RoughnessSlider.value = Generator.Roughness;

		DetailInput.onEndEdit.AddListener(OnSubmitDetail);
		RoughnessInput.onEndEdit.AddListener(OnSubmitRoughness);
		RoughnessSlider.onValueChanged.AddListener(OnRoughnessValueChanged);
	}

	public void OnSubmitDetail(string value)
	{
		int intValue = int.Parse(value);

		Generator.Detail = intValue;
		DetailInput.text = value;
	}

	public void OnSubmitRoughness(string value)
	{
		float floatValue = Mathf.Clamp(float.Parse(value), 0, 1);

		Generator.Roughness = floatValue;
		RoughnessInput.text = floatValue.ToString();
		RoughnessSlider.value = floatValue;
	}

	public void OnRoughnessValueChanged(float value)
	{
		float floatValue = Mathf.Clamp(value, 0, 1);

		Generator.Roughness = floatValue;
		RoughnessInput.text = floatValue.ToString();
		RoughnessSlider.value = floatValue;
	}
}
