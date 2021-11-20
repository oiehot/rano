// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

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
                Log.Warning("이 코루틴에서 WaitForSeconds 객체를 생성하고 있습니다. 성능이 저하될 수 있습니다.");
                yield return new WaitForSeconds(this.data[i].delay / this.speed);
            }
            this.Close();        
        }
    }
}