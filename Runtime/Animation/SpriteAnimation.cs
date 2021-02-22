using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rano
{
    [Serializable]
    public struct SpriteAnimationData
    {
        [SerializeField] public Sprite sprite;
        [SerializeField] public float delay;
    }

    class SpriteAnimation : MonoBehaviour
    {
        bool playing;
        public SpriteAnimationData[] data;
        [Min(0.1f)] public float speed = 1.0f;
        private SpriteRenderer spriteRenderer;
        private Coroutine playCoroutine;

        void Awake()
        {
            this.spriteRenderer = GetComponent<SpriteRenderer>();
            this.playing = false;
        }

        public void Play()
        {
            if (this.playing)
            {
                this.Stop();
            }
            this.playing = true;
            this.spriteRenderer.sprite = null;
            this.playCoroutine = StartCoroutine(this.PlayCoroutine());
        }

        public void Stop()
        {
            if (this.playCoroutine != null)
            {
                StopCoroutine(this.playCoroutine);
            }
        }

        private void Close()
        {
            this.playing = false;
            this.playCoroutine = null;
            this.spriteRenderer.sprite = null;
            this.gameObject.SetActive(false);
        }

        IEnumerator PlayCoroutine()
        {
            for (int i=0; i<this.data.Length; i++)
            {
                this.spriteRenderer.sprite = this.data[i].sprite;
                yield return new WaitForSeconds(this.data[i].delay / this.speed);
            }
            this.Close();        
        }
    }
}