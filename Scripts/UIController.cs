using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{

    public Lsystem lSystem;

    // Title
    public Text titleText, ruleText;
    public int maxTitle = 9, minTitle = 1;
    // Iterations
    [Range(1, 6)] public int iterations; 
    public Text iterationsText;
    public Slider iterationsSlider;
    public int maxIterations = 7;
    public int minIterations = 1;
    // Angle
    [Range(0f, 90f)] public float angle;
    public Text angleText;
    public Slider angleSlider;
    public float maxAngle = 90f, minAngle = 0f;
    // branchLength
    [Range(0f, 15f)] public float branchLength;
    public Text branchLengthText;
    public Slider branchLengthSlider;
    public float maxbranchLength = 15f, minbranchLength = 0f;
    // branchWidth
    [Range(0f, 3f)] public float branchWidth;
    public Text branchWidthText;
    public Slider branchWidthSlider;
    public float maxbranchWidth = 3f, minbranchWidth = 0f;
    // leafLength
    [Range(0f, 15f)] public float leafLength;
    public Text leafLengthText;
    public Slider leafLengthSlider;
    public float maxLeafLength = 15f, minLeafLength = 0f;
    //leafstartwidth
    [Range(0f, 10f)] public float leafStartWidth;
    public Text leafStartWidthText;
    public Slider leafStartWidthSlider;
    public float maxLeafStartWidth = 10f, minLeafStartWidth = 0f;
    //leafendwidth
    [Range(0f, 10f)] public float leafEndWidth;
    public Text leafEndWidthText;
    public Slider leafEndWidthSlider;
    public float maxLeafEndWidth = 10f, minLeafEndWidth = 0f;
    //random
    public Toggle randomColorToggle, observeToggle, randomModeToggle;
    public Slider angleVarianceSlider; 
    public Text angleVarianceText; 

    void Start()
    {
        UpdateRuleText();

        randomColorToggle.isOn = false;
        observeToggle.isOn = false;
        randomModeToggle.isOn = false;

        InitializeTitle();
        InitializeIterationsSlider();
        InitializeAngleSlider();
        InitializebranchLengthSlider();
        InitializebranchWidthSlider();
        InitializeLeafLengthSlider();
        InitializeLeafStartWidthSlider();
        InitializeLeafEndWidthSlider();
        InitializeAngleVarianceSlider();

        randomColorToggle.onValueChanged.AddListener(OnRandomColorToggleChanged);
        observeToggle.onValueChanged.AddListener(OnObserveToggleChanged);
        randomModeToggle.onValueChanged.AddListener(OnRandomModeToggleChanged);
    }

    void Update()
    {
        //hotkeys
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            IncreaseTitle();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            DecreaseTitle();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            AdjustSliderValue(iterationsSlider, 1);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            AdjustSliderValue(iterationsSlider, -1); 
        }
        UpdateRuleText();
    }

    private void InitializeTitle()
    {
        UpdateTitleText();
    }
    private void UpdateRuleText()
    {
        if (ruleText != null && lSystem != null)
        {
            ruleText.text = GetRuleDescriptionForCurrentTitle(lSystem.var.title);
        }
    }

    private string GetRuleDescriptionForCurrentTitle(int title)
    {
        switch (title)
        {
            case 1: return "Axiom: F\nRule1: F ¡ú FF+[+F-F-F]-[-F+F+F]";
            case 2: return "Axiom: F\nRule1: F ¡ú F[+F]F[-F]F";
            case 3: return "Axiom: F\nRule1: F ¡ú F[+F]F[-F][F]";
            case 4: return "Axiom: F\nRule1: F ¡ú F[+F][-F]F";
            case 5: return "Axiom: F\nRule1: F ¡ú [-GF]F[+GF][+G-GF]\nRule2: G¡úGG";
            case 6: return "Axiom: F\nRule1: F ¡ú G[+F][-F]GF\nRule2: G¡úGG";
            case 7: return "Axiom: F\nRule1: F ¡ú G+[[F]-F]-G[-GF]+F\nRule2: G¡úGG";
            case 8: return "Axiom: F\nRule1: F ¡ú [G[-F+G[+GF]][*-F+G[+GF]][/-F+G[+GF]-F]]\nRule2: G¡úGG";
            case 9: return "Axiom: F\nRule1: F ¡ú [*+GF]F[+GF][/+G-GF]\nRule2: G¡úGG";
            default: return "Default Rules";
        }
    }

    private void InitializeIterationsSlider()
    {
        iterationsSlider.minValue = minIterations;
        iterationsSlider.maxValue = maxIterations;
        iterationsSlider.value = lSystem.var.Iterations;

        UpdateIterationsText();

        iterationsSlider.onValueChanged.AddListener(OnIterationsSliderValueChanged);
    }

    void AdjustSliderValue(Slider slider, float delta)
    {
        if (slider != null)
        {
            float newValue = Mathf.Clamp(slider.value + delta, slider.minValue, slider.maxValue);
            slider.value = newValue; 

            slider.onValueChanged.Invoke(newValue);
        }
    }

    private void InitializeAngleSlider()
    {
        angleSlider.minValue = minAngle;
        angleSlider.maxValue = maxAngle;
        angleSlider.value = lSystem.var.angle;

        UpdateAngleText();

        angleSlider.onValueChanged.AddListener(OnAngleSliderValueChanged);
    }

    private void InitializebranchLengthSlider()
    {
        branchLengthSlider.minValue = minbranchLength;
        branchLengthSlider.maxValue = maxbranchLength;
        branchLengthSlider.value = lSystem.var.branchLength;

        UpdatebranchLengthText();

        branchLengthSlider.onValueChanged.AddListener(OnbranchLengthSliderValueChanged);
    }

    private void InitializebranchWidthSlider()
    {
        branchWidthSlider.minValue = minbranchWidth;
        branchWidthSlider.maxValue = maxbranchWidth;
        branchWidthSlider.value = lSystem.var.branchWidth;

        UpdatebranchWidthText();

        branchWidthSlider.onValueChanged.AddListener(OnbranchWidthSliderValueChanged);
    }

    private void InitializeLeafLengthSlider()
    {
        leafLengthSlider.minValue = minLeafLength;
        leafLengthSlider.maxValue = maxLeafLength;
        leafLengthSlider.value = lSystem.var.leafLength;

        UpdateLeafLengthText();

        leafLengthSlider.onValueChanged.AddListener(OnLeafLengthSliderValueChanged);
    }

    private void InitializeLeafStartWidthSlider()
    {
        leafStartWidthSlider.minValue = minLeafStartWidth;
        leafStartWidthSlider.maxValue = maxLeafStartWidth;
        leafStartWidthSlider.value = lSystem.var.leafStartWidth;

        UpdateLeafStartWidthText();

        leafStartWidthSlider.onValueChanged.AddListener(OnLeafStartWidthSliderValueChanged);
    }

    private void InitializeLeafEndWidthSlider()
    {
        leafEndWidthSlider.minValue = minLeafEndWidth;
        leafEndWidthSlider.maxValue = maxLeafEndWidth;
        leafEndWidthSlider.value = lSystem.var.leafEndWidth;

        UpdateLeafEndWidthText();

        leafEndWidthSlider.onValueChanged.AddListener(OnLeafEndWidthSliderValueChanged);
    }

    public void ToggleTreeGrowthMode()
    {
        lSystem.var.observeTreeGrow = !lSystem.var.observeTreeGrow;
    }

    public void RefreshTree()
    {
        lSystem.RegenerateTree();
    }

    public void IncreaseTitle()
    {
        if (lSystem.var.title < maxTitle)
        {
            lSystem.var.title++;
            UpdateTitleText(); 
            UpdateRuleText();
        }
    }
    public void DecreaseTitle()
    {
        if (lSystem.var.title > minTitle)
        {
            lSystem.var.title--;
            UpdateTitleText(); 
            UpdateRuleText();
        }
    }
    private void UpdateTitleText()
    {
        titleText.text = lSystem.var.title.ToString();
    }


    public void OnIterationsSliderValueChanged(float value)
    {
        lSystem.var.Iterations = Mathf.RoundToInt(value); 
        UpdateIterationsText(); 
    }

    private void UpdateIterationsText()
    {
        iterationsText.text = lSystem.var.Iterations.ToString();
    }

    public void OnAngleSliderValueChanged(float value)
    {
        lSystem.var.angle = value; 
        UpdateAngleText(); 
    }

    private void UpdateAngleText()
    {
        angleText.text = lSystem.var.angle.ToString("F2") + "¡ã";
    }

    public void OnbranchLengthSliderValueChanged(float value)
    {
        lSystem.var.branchLength = value; 
        UpdatebranchLengthText(); 
    }

    private void UpdatebranchLengthText()
    {
        branchLengthText.text = lSystem.var.branchLength.ToString("F2"); 
    }

    public void OnbranchWidthSliderValueChanged(float value)
    {
        lSystem.var.branchWidth = value; 
        UpdatebranchWidthText(); 
    }

    private void UpdatebranchWidthText()
    {
        branchWidthText.text = lSystem.var.branchWidth.ToString("F2"); 
    }

    public void OnLeafLengthSliderValueChanged(float value)
    {
        lSystem.var.leafLength = value; 
        UpdateLeafLengthText(); 
    }

    private void UpdateLeafLengthText()
    {
        leafLengthText.text = lSystem.var.leafLength.ToString("F2");
    }

    public void OnLeafStartWidthSliderValueChanged(float value)
    {
        lSystem.var.leafStartWidth = value;
        UpdateLeafStartWidthText(); 
    }

    private void UpdateLeafStartWidthText()
    {
        leafStartWidthText.text = lSystem.var.leafStartWidth.ToString("F2");
    }

    public void OnLeafEndWidthSliderValueChanged(float value)
    {
        lSystem.var.leafEndWidth = value; 
        UpdateLeafEndWidthText(); 
    }

    private void UpdateLeafEndWidthText()
    {
        leafEndWidthText.text = lSystem.var.leafEndWidth.ToString("F2");
    }

    private void OnRandomColorToggleChanged(bool isOn)
    {
        lSystem.var.useRandomLeafColors = isOn;
        lSystem.RegenerateTree();
    }

    private void OnObserveToggleChanged(bool isOn)
    {
        if (isOn)
        {
            lSystem.var.observeTreeGrow = true;
        }
        else
        {
            lSystem.var.observeTreeGrow = false;
        }
    }

    //randommode
    private void OnRandomModeToggleChanged(bool isOn)
    {
        lSystem.var.useRandomMode = isOn;

        angleVarianceSlider.interactable = isOn;

        if (!isOn)
        {
            lSystem.var.randomAngleVariance = 0;
            UpdateAngleVarianceText();
        }

        lSystem.RegenerateTree();
    }

    private void InitializeAngleVarianceSlider()
    {
        angleVarianceSlider.value = lSystem.var.randomAngleVariance;

        UpdateAngleVarianceText();

        angleVarianceSlider.interactable = randomModeToggle.isOn;

        angleVarianceSlider.onValueChanged.AddListener(OnAngleVarianceSliderValueChanged);
    }

    private void OnAngleVarianceSliderValueChanged(float value)
    {
        if (randomModeToggle.isOn)
        {
            lSystem.var.randomAngleVariance = (int)value; 
            UpdateAngleVarianceText();
        }
    }

    private void UpdateAngleVarianceText()
    {
        angleVarianceText.text = lSystem.var.randomAngleVariance.ToString("F2") + "¡ã";
    }
}