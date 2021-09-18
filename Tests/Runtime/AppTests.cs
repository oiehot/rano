// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TestTools;  
using Rano.App;
using Rano.Store;

namespace Rano.Tests
{   
    public class AppTests
    {
        [Test]
        public void VersionStruct_Test()
        {
            Version v = new Version("1.2.3");
            Assert.AreEqual(v.major, 1);
            Assert.AreEqual(v.minor, 2);
            Assert.AreEqual(v.build, 3);
            Assert.AreEqual(v.buildVersionCode, 12003);
            Assert.AreEqual(v.ToString(), "1.2.3");            
            Assert.AreEqual(v.fullVersion, "1.2.3 (12003)");
            Assert.AreEqual(v.GetHashCode(), 12003);

            Assert.IsTrue(v == new Version("1.2.3"));
            Assert.IsTrue(v != new Version("1.2.4"));

            Assert.IsTrue(v >= new Version("1.2.3"));
            Assert.IsTrue(v >= new Version("1.2.0"));
            Assert.IsTrue(v > new Version("1.2.0"));

            Assert.IsTrue(v <= new Version("1.2.3"));
            Assert.IsTrue(v <= new Version("1.2.5"));
            Assert.IsTrue(v < new Version("1.2.5"));

            Version v2 = new Version(1,2,3);
            Assert.AreEqual(v2.ToString(), "1.2.3");
        }

        [UnityTest]
        public IEnumerator UpdateCheckerGoogle_Test()
        {
            GameObject go = new GameObject();
            UpdateChecker updateChecker = go.AddComponent<UpdateChecker>();
            updateChecker.Initialize(
                "com.oiehot.afo2",
                RuntimePlatform.Android,
                new Version("1.0.6")
            );

            updateChecker.onUpdateRequired = new UnityEvent();
            updateChecker.onUpdateRequired.AddListener(() => {
                // Done
            });

            updateChecker.onUpdateCheckFailed = new UnityEvent();
            updateChecker.onUpdateCheckFailed.AddListener(() => {
                throw new System.Exception("구글 플레이스토어 업데이트 체크 실패");
            });

            yield return updateChecker.CheckUpdate();
        }

        [UnityTest]
        /// <comments>
        /// 테스트용 bundleId: com.ninecatsgames.logalaxy
        /// 테스트용 trackId: 1350067922
        /// </comments>
        public IEnumerator UpdateCheckerApple_Test()
        {
            GameObject go = new GameObject();
            UpdateChecker updateChecker = go.AddComponent<UpdateChecker>();
            updateChecker.Initialize(
                "com.ninecatsgames.logalaxy",
                RuntimePlatform.IPhonePlayer,
                new Version("1.0.0")
            );

            updateChecker.onUpdateRequired = new UnityEvent();
            updateChecker.onUpdateRequired.AddListener(() => {
                // Done
            });

            updateChecker.onUpdateCheckFailed = new UnityEvent();
            updateChecker.onUpdateCheckFailed.AddListener(() => {
                throw new System.Exception("애플 앱스토어 업데이트 체크 실패");
            });

            yield return updateChecker.CheckUpdate();
        }
    }
}