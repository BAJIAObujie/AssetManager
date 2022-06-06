using System.Collections;
using System.Collections.Generic;
using Core.Asset;
using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        BundleManager.Instance.Init();
    }
}
