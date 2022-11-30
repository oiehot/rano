using System;
using System.Collections;
using UnityEngine;

namespace Rano.Animation
{
    [Serializable]
    public struct SpriteAnimationData
    {
        [SerializeField] public Sprite sprite;
        [SerializeField] public float delay;
    }

    public class SpriteAnimation : MonoBehaviour
    {
        private bool _isPlaying;
        private SpriteRenderer _spriteRenderer;
        private Coroutine _playCoroutine;
        
        public SpriteAnimationData[] data;
        
        [Min(0.1f)] public float speed = 1.0f;
        
        void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _isPlaying = false;
        }

        public void Play()
        {
            if (_isPlaying)
            {
                Stop();
            }
            _isPlaying = true;
            _spriteRenderer.sprite = null;
            _playCoroutine = StartCoroutine(this.PlayCoroutine());
        }

        public void Stop()
        {
            if (_playCoroutine != null)
            {
                StopCoroutine(_playCoroutine);
            }
        }

        private void Close()
        {
            _isPlaying = false;
            _playCoroutine = null;
            _spriteRenderer.sprite = null;
            gameObject.SetActive(false);
        }

        private IEnumerator PlayCoroutine()
        {
            for (int i=0; i<this.data.Length; i++)
            {
                _spriteRenderer.sprite = this.data[i].sprite;
                Log.Warning("이 코루틴에서 WaitForSeconds 객체를 생성하고 있습니다. 성능이 저하될 수 있습니다.");
                yield return new WaitForSeconds(this.data[i].delay / this.speed);
            }
            Close();        
        }
    }
}