using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagDollMaterialChanger : MonoBehaviour
{

    [SerializeField] private Material[] _materials;
    [SerializeField] private GameObject[] _bodies;
    // Start is called before the first frame update
    public void MaterialChange(int matNum)
    {
        foreach(var body in _bodies)
        {
            body.GetComponent<SkinnedMeshRenderer>().material = _materials[matNum];
        }
    }
}
