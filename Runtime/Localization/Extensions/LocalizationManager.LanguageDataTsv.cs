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
    /// <summary>
    /// LanaugeData.tsv는 모든 언어 데이터들이 다 들어있는 텍스트 파일이다.
    /// 헤더의 경우 [key, value_en, value_kr, ...] 등으로 구성되어 있다.
    /// 여기서 원하는 언어 데이터만 추출하하여 [key, value] 형태의 json 데이터로 변환하고
    /// json 데이터를 JsonUtility 를 통하여 스크립터블 오브젝트인 LocalizationData 로 변환해서
    /// 로드한다.
    /// </summary>
    public static partial class LocalizationManagerExtensions
    {
        public static void LoadFromLocalizationDataTsv(this LocalizationManager manager, string path, LocalizationLanguage language)
        {
            TextAsset data;
            data = Resources.Load<TextAsset>(path);
            if (!data)
            {
                throw new Exception($"Unable to find resource: {path}");
            }

            List<string> includeHeaders = new List<string>();
            List<string> excludeHeaders = new List<string>();
            Dictionary<string, string> renameHeaders = new Dictionary<string, string>();

            string languageColumnName = $"value_{language.code.ToLower()}";

            includeHeaders.Add("key");
            includeHeaders.Add(languageColumnName);
            renameHeaders.Add(languageColumnName, "value");

            string jsonString = Rano.Tsv.ToJson(data.text, includeHeaders, excludeHeaders, renameHeaders);
            
            // 주의: ScriptableObject 는 new 로 인스턴스를 만들 수 없고 CreateInstance 를 통해야한다.
            // LocalizationData localizationData = new LocalizationData();
            LocalizationData localizationData = ScriptableObject.CreateInstance<LocalizationData>();
            
            // 주의: FromJson을 쓰면 에러가 나는 이유는, 스크립터블 오브젝트는 new 로 인스턴스를 만들지 못하기 때문이다.
            // 그렇기 때문에 CreateInstance로 인스턴스를 만들고 JsonUtility.FromJsonOverwrite를 통해서
            // 데이터를 저장하는 것임. 이 함수는 인스턴스를 새로 만들지 않고 이미 있는 인스턴스를 사용한다.
            // localizationData = JsonUtility.FromJson<LocalizationData>(jsonString);
            JsonUtility.FromJsonOverwrite(jsonString, localizationData);

            manager.Load(localizationData);
        }
    }
}