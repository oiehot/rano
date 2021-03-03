// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

#if false

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using Rano;

namespace RanoEditor.Build
{
    [Serializable]
    class BuildConfig
    {
        public int releaseVersion = 0;
        public int majorVersion = 0;
        public int minorVersion = 0;
        public int buildVersion = 0;
        public string applicationName = "";
        public string applicationIdentifier = "";
        public string buildTarget = ""; // android" or "ios"
        public List<string> scenes = new List<string>{};

        [System.NonSerialized] public string outputPath;
    }

    static class Builder
    {
        static public bool Build(BuildConfig cfg)
        {
            string version;
            string outputPath;
            BuildTarget buildTarget;
            
            version = $"{cfg.releaseVersion}.{cfg.majorVersion}.{cfg.minorVersion}.{cfg.buildVersion}";
            outputPath = $"{cfg.outputPath}/{cfg.applicationName}_{version}.apk";
            
            // 플레이어 세팅 (버젼) 설정
            PlayerSettings.applicationIdentifier = cfg.applicationIdentifier; // ex) com.oiehot.flappybird
            PlayerSettings.bundleVersion = version;
            
            // 빌드 타겟 선택
            switch (cfg.buildTarget)
            {
                case "android":
                    buildTarget = BuildTarget.Android;
                    break;
                case "ios":
                    buildTarget = BuildTarget.iOS;
                    break;
                default:
                    Debug.Log($"Not supported platform: {cfg.buildTarget}");
                    return false;
            }
            
            // cfg.scenes에 Application.dataPath를 더해서 scene 배열을 만들고 이를 빌드시 넘기도록 한다.
            var scenes = cfg.scenes.Select(scenePath => Application.dataPath + scenePath).ToList();
            BuildPipeline.BuildPlayer(scenes.ToArray(), outputPath, buildTarget, BuildOptions.None );
            
            return true;
        }
    }

    public static class Client
    {
        public static void Test()
        {
            Console.WriteLine("* Test Done");
        }
        
        public static void Main()
        {
            string configPath = "";
            string outputPath = "";

            // 명령행으로부터 JSON 파일 경로를 얻는다.
            var args = Environment.GetCommandLineArgs();
            for (var i=0; i<args.Length; ++i)
            {
                if (args[i] == "--config")
                {
                    configPath = args[i+1];
                }
                else if (args[i] == "--output")
                {
                    outputPath = args[i+1];
                }
            }
            
            // Check configPath
            // TODO: Check exists json file
            if (configPath.Length <= 0)
            {
                Console.WriteLine("* No Configuration. ex) --config /var/u3d/cfg.json");
                EditorApplication.Exit(1); // TODO: Exception? Exit?
            }
            
            // Check outputPath
            // TODO: Check exists output path, (make)
            if (outputPath.Length <= 0)
            {
                Console.WriteLine("* No OutputPath. ex) --output /var/u3d");
                EditorApplication.Exit(1);
            }

            // JSON 파일로 부터 빌드 설정을 얻는다.
            var raw = System.IO.File.ReadAllText(configPath);
            var cfg = JsonUtility.FromJson<BuildConfig>(raw);

            // outputPath는 json 파일이 아닌 매개변수로 전달받는다.
            cfg.outputPath = outputPath;

            if (Builder.Build(cfg))
            {
                // 성공했다면, buildVersion을 1올리고 JSON을 다시 저장한다.
                Console.WriteLine("* Build Success");
            }
            else
            {
                Console.WriteLine("* Build Failed");
                EditorApplication.Exit(1); // TODO:
            }
        }
    }
}

#endif