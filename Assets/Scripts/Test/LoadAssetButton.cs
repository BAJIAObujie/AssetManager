using System.Collections;
using System.Collections.Generic;
using Core.Asset;
using UnityEngine;
using UnityEngine.UI;

public class LoadAssetButton : MonoBehaviour
{
    public string bundleName;
    public string assetName;
    public bool sync;
    private AbstractAsset particleAsset;
    private GameObject particleObj;
    
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            //Release Last
            if (particleObj != null)
            {
                Destroy(particleObj);
            }
            particleAsset?.Release();
            
            //Load
            particleAsset = AssetManager.Instance.GetAsset(bundleName, assetName);
            if (sync)
            {
                particleAsset.LoadSync();
                particleObj = Instantiate(particleAsset.asset as GameObject);
            }
            else
            {
                particleAsset.SetCallback(() =>
                {
                    particleObj = Instantiate(particleAsset.asset as GameObject);
                });
                particleAsset.LoadAsync();
            }
        });
    }
}
