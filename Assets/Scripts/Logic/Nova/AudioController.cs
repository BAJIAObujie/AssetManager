using UnityEngine;
using Core.Asset;

namespace Nova
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioController : MonoBehaviour
    {
        private AudioSource _audioSource;

        private AbstractAsset audioClipAsset;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
        }

        //同步加载
        public void Play(string bundleName, string assetName)
        {
            _audioSource.Stop();
            _audioSource.clip = null;
            
            audioClipAsset?.Release();
            audioClipAsset = AssetManager.Instance.GetAsset(bundleName, assetName);
            audioClipAsset.LoadSync();
            _audioSource.clip = audioClipAsset.asset as AudioClip;
            _audioSource.Play();
        }

        //异步加载
        public void Play2(string bundleName, string assetName)
        {
            _audioSource.Stop();
            _audioSource.clip = null;
            
            audioClipAsset?.Release();
            audioClipAsset = AssetManager.Instance.GetAsset(bundleName, assetName);
            audioClipAsset.SetCallback(() =>
            {
                _audioSource.clip = audioClipAsset.asset as AudioClip;
                _audioSource.Play();
            });
            audioClipAsset.LoadAsync();
        }
    }
}