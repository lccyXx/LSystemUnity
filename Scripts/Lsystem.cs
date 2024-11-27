using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Lsystem : MonoBehaviour
{
    private string axiom = "F";
    private string currentString;
    private Dictionary<char, string> rules = new Dictionary<char, string>();
    private Stack<TransformInfo> transformStack = new Stack<TransformInfo>();
    private List<GameObject> branches = new List<GameObject>();
    [SerializeField] private UIController UI;

    [System.Serializable]
    public struct LSystemBufferedData
    {
        public int title, iterations, randomAngleVariance;
        public float angle, branchLength, branchWidth, leafLength, leafStartWidth, leafEndWidth;
        public bool observeTreeGrow, useRandomMode, useRandomLeafColors;
    }
    private LSystemBufferedData bd;

    [System.Serializable]
    public class Variables
    {
        public int title = 1;
        [Range(1, 7)] public int Iterations = 5;
        [Range(0, 90)] public float angle = 25f;
        [Range(0f, 15f)] public float branchLength = 3f;
        [Range(0f, 3f)] public float branchWidth = 1f;
        [Range(0, 15)] public float leafLength = 1f;
        [Range(0, 10)] public float leafStartWidth = 3f;
        [Range(0, 10)] public float leafEndWidth = 2f;
        public bool observeTreeGrow = false;
        public bool useRandomMode = false;
        [Range(1, 100)] public int randomAngleVariance = 10;
        public List<Material> leafMaterials;
        public bool useRandomLeafColors = false;
    }
    [SerializeField] public Variables var;

    [System.Serializable]
    public class TreeModelSettings
    {
        public GameObject branchPrefab;
        public GameObject leafPrefab;
    }
    [SerializeField] private TreeModelSettings treeModel;

    public class TransformInfo
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    private float[] randomRotationValues = new float[100];
    private Material currentLeafMaterial;
    private Vector3 currentTipPosition;
    private Quaternion currentTipRotation;
    private float checkInterval = 0.2f, checkTimer = 0f;
    private Coroutine lSystemCoroutine;

    void Start()
    {
        transform.position = Vector3.zero;
        BufferLSData();
        RulesTitle();
        currentString = axiom;

        if (var.useRandomMode)
        {
            ResetRandomValues();
        }
        if (var.observeTreeGrow)
        {
            StopCurrentCoroutine();
            lSystemCoroutine = StartCoroutine(GenerateLSystem());
        }
        else
        {
            GenerateLSystemDIR(); 
        }
        if (var.useRandomLeafColors && var.leafMaterials.Count > 0)
        {
            currentLeafMaterial = var.leafMaterials[UnityEngine.Random.Range(0, var.leafMaterials.Count)];
        }
    }

    void Update()
    {
        checkTimer += Time.deltaTime;
        if (checkTimer >= checkInterval)
        {
            checkTimer = 0f;
            if (CheckInspectorChanges())
            {
                BufferLSData();
                if (var.useRandomLeafColors && var.leafMaterials.Count > 0)//random materials
                {
                    UpdateLeafMaterials();
                }
                RegenerateTree();
            }
        }
        if (bd.useRandomMode != var.useRandomMode)
        {
            bd.useRandomMode = var.useRandomMode; 
            ResetRandomValues();
        }
    }

    private bool CheckInspectorChanges()
    {
        bool hasChanged = bd.title != var.title ||
                          bd.iterations != var.Iterations ||
                          Mathf.Abs(bd.angle - var.angle) > 0.01f ||
                          Mathf.Abs(bd.branchLength - var.branchLength) > 0.01f ||
                          Mathf.Abs(bd.branchWidth - var.branchWidth) > 0.01f ||
                          Mathf.Abs(bd.leafLength - var.leafLength) > 0.01f ||
                          Mathf.Abs(bd.leafStartWidth - var.leafStartWidth) > 0.01f ||
                          Mathf.Abs(bd.leafEndWidth - var.leafEndWidth) > 0.01f ||
                          bd.observeTreeGrow != var.observeTreeGrow ||
                          bd.useRandomMode != var.useRandomMode ||
                          bd.randomAngleVariance != var.randomAngleVariance ||
                          bd.useRandomLeafColors != var.useRandomLeafColors;

        if (hasChanged)
        {
            BufferLSData();
            ToggleObserveMode(); 
        }
        return hasChanged;
    }

    private void BufferLSData()
    {
        bd.title = var.title;
        bd.iterations = var.Iterations;
        bd.angle = var.angle;
        bd.branchLength = var.branchLength;
        bd.branchWidth = var.branchWidth;
        bd.leafLength = var.leafLength;
        bd.leafStartWidth = var.leafStartWidth;
        bd.leafEndWidth = var.leafEndWidth;
        bd.observeTreeGrow = var.observeTreeGrow;
        bd.useRandomMode = var.useRandomMode;
        bd.randomAngleVariance = var.randomAngleVariance;
        bd.useRandomLeafColors = var.useRandomLeafColors;
    }
    
    private void ClearBranches()
    {
        foreach (GameObject branch in branches)
        {
            branch.SetActive(false);
        }
        branches.Clear();
    }

    public void RegenerateTree()
    {
        ClearBranches();
        transformStack.Clear();
        ResetRandomValues();
        RulesTitle();
        currentString = axiom;

        if (var.useRandomLeafColors && var.leafMaterials.Count > 0)
        {
            currentLeafMaterial = var.leafMaterials[UnityEngine.Random.Range(0, var.leafMaterials.Count)];
            UpdateLeafMaterials();
        }
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        if (var.observeTreeGrow)
        {
            StopCurrentCoroutine();
            lSystemCoroutine = StartCoroutine(GenerateLSystem());
        }
        else
        {
            GenerateLSystemDIR();
        }
    }

    private void ResetRandomValues()
    {
        for (int i = 0; i < randomRotationValues.Length; i++)
        {
            randomRotationValues[i] = UnityEngine.Random.Range(-1f, 1f);
        }
    }

    //two different mode
    void GenerateLSystemDIR()
    {
        for (int count = 0; count < var.Iterations; count++)
        {
            string newString = "";
            char[] stringCharacters = currentString.ToCharArray();
            foreach (char currentCharacter in stringCharacters)
            {
                if (rules.ContainsKey(currentCharacter))
                    newString += rules[currentCharacter];
                else
                    newString += currentCharacter.ToString();
            }
            currentString = newString;
        }
        GenerateBranchDIR();
    }
    void GenerateBranchDIR()
    {
        foreach (char currentCharacter in currentString)
        {
            GenerateBranch(currentCharacter);
        }
    }

    IEnumerator GenerateLSystem()
    {

        for (int count = 0; count < var.Iterations; count++)
        {
            string newString = "";
            char[] stringCharacters = currentString.ToCharArray();
            foreach (char currentCharacter in stringCharacters)
            {
                if (rules.ContainsKey(currentCharacter))
                    newString += rules[currentCharacter];
                else
                    newString += currentCharacter.ToString();
            }
            currentString = newString;
            yield return null;//generate step by step

        }
        foreach (char currentCharacter in currentString)//graphic structure
        {
            GenerateBranch(currentCharacter);
            yield return null;//dynamic way
        }
    }

    private void StopCurrentCoroutine()
    {
        if (lSystemCoroutine != null)
        {
            StopCoroutine(lSystemCoroutine);
            lSystemCoroutine = null;
        }
        ClearBranches();
    }
    private void ToggleObserveMode()
    {
        StopCurrentCoroutine();
        if (var.observeTreeGrow)
        {
            lSystemCoroutine = StartCoroutine(GenerateLSystem());
        }
        else if (!var.observeTreeGrow && lSystemCoroutine != null)
        {
            StopCoroutine(lSystemCoroutine);
            lSystemCoroutine = null;
        }
    }

    //core generate system
    void GenerateBranch(char currentCharacter)
    {
        if (currentCharacter == 'F' || currentCharacter == 'G')
        {
            Quaternion branchRotation = transform.rotation;

            Vector3 initialPosition = transform.position;
            transform.Translate(Vector3.up * var.branchLength); 

            //generate branch
            if (treeModel.branchPrefab != null)
            {
                GameObject branch = Instantiate(treeModel.branchPrefab, initialPosition, Quaternion.identity);
                branch.SetActive(true); 

                LineRenderer branchRenderer = branch.GetComponent<LineRenderer>();
                if (branchRenderer != null)
                {
                    branchRenderer.SetPosition(0, initialPosition);
                    branchRenderer.SetPosition(1, transform.position);
                    branchRenderer.startWidth = var.branchWidth;
                    branchRenderer.endWidth = var.branchWidth;
                }
                branches.Add(branch);
            }
            currentTipPosition = transform.position; //reset position
            currentTipRotation = transform.rotation; //reset rotation
        }
        //3d
        else if (currentCharacter == '*')
        {
            float randomVariance = var.useRandomMode ? var.randomAngleVariance : 0;
            transform.Rotate(Vector3.up * (120f + randomVariance * randomRotationValues[0]));
        }
        else if (currentCharacter == '/')
        {
            float randomVariance = var.useRandomMode ? var.randomAngleVariance : 0;
            transform.Rotate(Vector3.down * (120f + randomVariance * randomRotationValues[1]));
        }
        else if (currentCharacter == '[')
        {
            transformStack.Push(new TransformInfo { position = transform.position, rotation = transform.rotation });
        }
        else if (currentCharacter == ']')
        {
            if (transformStack.Count > 0)
            {
                TransformInfo ti = transformStack.Pop();
                transform.position = ti.position;
                transform.rotation = ti.rotation;
            }
            //generate leaf
            if (treeModel.leafPrefab != null && branches.Count > 0)
            {
                GenerateLeafAtBranchEnd(branches[^1]);
            }
        }
        else if (currentCharacter == '+')
        {
            float randomVariance = var.useRandomMode ? var.randomAngleVariance : 0;
            transform.Rotate(Vector3.forward * (var.angle + randomVariance * randomRotationValues[2]));
        }
        else if (currentCharacter == '-')
        {
            float randomVariance = var.useRandomMode ? var.randomAngleVariance : 0;
            transform.Rotate(Vector3.forward * (-var.angle + randomVariance * randomRotationValues[3]));
        }
    }

    void GenerateLeafAtBranchEnd(GameObject branch)
    {
        if (treeModel.leafPrefab != null)
        {
            GameObject leaf = Instantiate(treeModel.leafPrefab, currentTipPosition, Quaternion.identity);
            leaf.transform.position = currentTipPosition;//make the leaf position=branch position
            leaf.transform.rotation = currentTipRotation;
            LineRenderer leafRenderer = leaf.GetComponent<LineRenderer>();

            if (leafRenderer != null)
            {
                leafRenderer.startWidth = var.leafStartWidth;
                leafRenderer.endWidth = var.leafEndWidth;
                Vector3 leafDirection = currentTipRotation * Vector3.up * var.leafLength;
                leafRenderer.SetPosition(0, currentTipPosition);
                leafRenderer.SetPosition(1, currentTipPosition + leafDirection);

                if (currentLeafMaterial != null)
                {
                    leafRenderer.material = currentLeafMaterial;
                }
            }
        }
    }

    //linerenderer material
    void UpdateLeafMaterials()
    {
        foreach (GameObject branch in branches)
        {
            foreach (Transform leaf in branch.transform)
            {
                LineRenderer leafRenderer = leaf.GetComponent<LineRenderer>();
                if (leafRenderer != null && currentLeafMaterial != null)
                {
                    leafRenderer.material = currentLeafMaterial;
                }
                leaf.transform.SetParent(branches[^1].transform, true);
            }
        }
    }
    public void RulesTitle()
    {
        rules.Clear();
        switch (var.title)
        {
            case 1:
                rules.Add('F', "FF-[-F+F+F]+[+F-F-F]");
                break;
            case 2:
                rules.Add('F', "F[+F]F[-F]F");
                break;
            case 3:
                rules.Add('F', "F[+F]F[-F][F]");
                break;
            case 4:
                rules.Add('F', "F[+F][-F]F");
                break;
            case 5:
                rules.Add('F', "G[+F]G[-F]+F");
                rules.Add('G', "GG");
                break;
            case 6:
                rules.Add('F', "G[+F][-F]GF");
                rules.Add('G', "GG");
                break;
            case 7:
                rules.Add('F', "G-[[F]+F]+G[+GF]-F");
                rules.Add('G', "GG");
                break;
            case 8:
                rules.Add('F', "[G[-F+G[+GF]][*-F+G[+GF]][/-F+G[+GF]-F]]");//reference
                rules.Add('G', "GG");
                break;
            case 9:
                rules.Add('F', "[*+GF]F[+GF][/+G-GF]");//reference
                rules.Add('G', "GG");
                break;
        }
    }
}