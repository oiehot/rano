// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

#if false

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rano
{        
    public class DigitNumbers : MonoBehaviour
    {
        public int maxDigit = 10;
        private float letterSpace = 0.0f;
        
        public GameObject numberPrefab;
        GameObject[] numberGameObjects;
        DigitNumber[] digitNumbers;
        float digitWidth;
        float digitHeight;
        
        string valueStr;
        int valueCharLength = 0;
        int val;

        public int Value
        {
            get
            {
                return val;
            }
            set
            {
                // 값
                val = value;
                valueStr = val.ToString();
                valueCharLength = valueStr.Length;
                
                // 표시될 숫자 스프라이트 변경.
                for (int i=0; i<valueCharLength; i++)
                {
                    digitNumbers[i].Value = valueStr[i] -'0'; // Char to Int
                    numberGameObjects[i].SetActive(true);
                }
                // 그외는 모두 끄기.
                for (int i=valueCharLength; i<maxDigit; i++)
                {
                    digitNumbers[i].Value = 0;
                    numberGameObjects[i].SetActive(false);
                }
                
                // 숫자 스프라이트들의 위치 변경 (중앙정렬)
                float curX;
                if (valueCharLength > 1)
                {
                    curX = -(width/2);
                }
                else
                {
                    curX = 0.0f;
                }
                for (int i=0; i<valueCharLength; i++)
                {
                    numberGameObjects[i].transform.localPosition = new Vector2(curX, 0);
                    curX += digitWidth + letterSpace;
                }
            }
        }
        
        public float width
        {
            get
            {
                return digitWidth * valueCharLength;
            }
        }
        
        public float height
        {
            get
            {
                return digitHeight;
            }
        }
        
        void Awake()
        {
            numberGameObjects = new GameObject[maxDigit];
            digitNumbers = new DigitNumber[maxDigit];
                    
            SpriteRenderer r = numberPrefab.GetComponent<SpriteRenderer>();
            digitWidth = r.size.x;
            digitHeight = r.size.y;

            for (int i=0; i<maxDigit; i++)
            {
                numberGameObjects[i] = Instantiate(numberPrefab, new Vector3(0,0,0), Quaternion.identity);
                numberGameObjects[i].SetActive(false);
                numberGameObjects[i].transform.parent = transform; // 숫자를 현재 트랜스폼의 Children으로.
                
                digitNumbers[i] = numberGameObjects[i].GetComponent<DigitNumber>();
            }
            
            Value = 0;
        }
    }
}

#endif