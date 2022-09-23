#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rano.SaveSystem
{
    [DisallowMultipleComponent]
    public class SaveableEntity : MonoBehaviour
    {   
        [SerializeField] private string _id = string.Empty;
        
        /// <summary>
        /// 배치시 로드 순서. 작을수록 더 먼저 로드된다.
        /// </summary>
        [SerializeField] private int _order = 0;

        public string Id => _id;
        public int Order => _order;

        void Reset()
        {
            GenerateId();
        }

        public bool Initialize(string id, int order=0)
        {
            if (string.IsNullOrEmpty(id) == true)
            {
                Log.Warning($"초기화 실패 (잘못된 Id: {id})");
                return false;
            }
            
            _id = id;
            _order = order;

            return true;
        }
        
        public bool IsInitialized()
        {
            if (string.IsNullOrEmpty(_id) == true) return false;
            return true;
        }

        // 실수로 Id를 바꿀 수 있기 때문에 해제함.
        // [ContextMenu("Generate Id", false, 1401)]
        private void GenerateId()
        {
            _id = System.Guid.NewGuid().ToString();
        }

        [ContextMenu("Set to ClearState")]
        public bool ClearState()
        {
            foreach (var saveableComponent in GetComponents<ISaveLoadable>())
            {
                try
                {
                    saveableComponent.ClearState();
                }
                catch (Exception e)
                {
                    Log.Warning($"게임오브젝트를 초기상태로 만드는 중에 예외가 발생 (component:{saveableComponent})");
                    Log.Exception(e);
                    return false;
                }
            }
            return true;
        }

        [ContextMenu("Set to DefaultState")]
        public bool DefaultState()
        {
            foreach (var saveableComponent in GetComponents<ISaveLoadable>())
            {
                try
                {
                    saveableComponent.DefaultState();
                }
                catch (Exception e)
                {
                    Log.Warning($"게임오브젝트를 기본상태로 만드는 중에 예외가 발생 (component:{saveableComponent})");
                    Log.Exception(e);
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// 게임오브젝트에 달린 모든 ISaveLoadable 컴포넌트들의 상태를 캡쳐한다.
        /// </summary>
        /// <returns>상태 데이터들, 실패하면 null이 리턴됨</returns>
        public Dictionary<string, object>? CaptureState()
        {
            // TODO: 두 개 이상의 컴포넌트가 있는 예외상황 대처.
            var componentStates = new Dictionary<string, object>();
            foreach (var component in GetComponents<ISaveLoadable>())
            {
                object componentState;
                try
                {
                    componentState = component.CaptureState();
                }
                catch (Exception e)
                {
                    Log.Warning($"게임오브젝트의 상태를 캡쳐하는데 예외가 발생 (component:{component})");
                    Log.Exception(e);
                    return null;
                }
                var componentName = component.GetType().ToString();
                componentStates[componentName] = componentState;
            }
            return componentStates;
        }

        /// <summary>
        /// 게임오브젝트에 달린 모든 ISaveLoadable 컴포넌트들의 상태를 복구한다.
        /// </summary>
        /// <param name="dict">컴포넌트들의 데이터</param>
        /// <param name="useDefaultStateIfFailed">상태복구 실패시 기본상태를 사용할지 여부</param>
        /// <returns>복구 결과</returns>
        public bool RestoreState(object dict, bool useDefaultStateIfFailed)
        {
            // TODO: 두 개 이상의 동일컴포넌트가 있는 예외상황 대처.
            var componentStates = (Dictionary<string, object>)dict;
            
            // 게임오브젝트에 달린 모든 ISaveLoadable 인터페이스를 지원하는 컴포넌트들의 상태를 복구한다.
            foreach (ISaveLoadable component in GetComponents<ISaveLoadable>())
            {
                string componentName = component.GetType().ToString();
                
                // 게임오브젝트 상태 데이터에서 컴포넌트의 데이터를 찾는데 성공한 경우:
                if (componentStates.TryGetValue(componentName, out object componentState) == true)
                {
                    // 상태 데이터를 검증한다.
                    try
                    {
                        component.ValidateState(componentState);
                        component.RestoreState(componentState);
                    }
                    catch (Exception e)
                    {
                        Log.Info($"컴포넌트의 상태데이터검증 또는 상태복원 중 예외 발생 (component:{componentName}, exception:{e})");
                        if (useDefaultStateIfFailed)
                        {
                            Log.Info($"컴포넌트의 상태데이터검증 또는 상태복원에 실패하여 기본상태로 설정 (component:{componentName}");
                            try
                            {
                                component.DefaultState();
                            }
                            catch (Exception e2)
                            {
                                Log.Warning($"컴포넌트를 기본상태로 만드는 중 예외가 발생 (component:{componentName}, exception:{e2})");
                                return false;
                            }
                        }
                        else
                        {
                            Log.Warning($"컴포넌트의 상태데이터검증 또는 상태복원 실패 (component:{componentName})");
                            return false;
                        }
                    }
                }
                // 게임오브젝트 상태 데이터에서 컴포넌트의 데이터를 찾는데 실패한 경우:
                else
                {
                    // 상태복구 실패시 기본상태를 사용하기로 한 경우 컴포넌트를 기본상태로 만든다.
                    if (useDefaultStateIfFailed)
                    {
                        try
                        {
                            Log.Info($"컴포넌트의 상태데이터를 찾을 수 없어 기본상태로 설정 (component:{componentName}");
                            component.DefaultState();
                        }
                        catch (Exception e)
                        {
                            Log.Warning($"컴포넌트를 기본상태로 만드는 중 예외가 발생 (component:{component}, exception:{e})");
                            return false;
                        }
                    }
                    else
                    {
                        Log.Warning($"컴포넌트의 상태데이터를 찾을 수 없어 상태복원에 실패함 (component:{componentName})");
                        return false;
                    }
                }
            }
            return true;
        }
        
        public string? ToJsonString()
        {
            var dict = CaptureState();
            if (dict != null)
            {
                return Rano.Encoding.Json.ConvertObjectToString(dict);
            }
            else return null;
        }

        [ContextMenu("Print Json", false, 1301)]
        public void PrintJsonString()
        {
            Log.Info($"{_id}:");
            string? log = ToJsonString();
            if (log != null) Debug.Log(ToJsonString());
            else Log.Info("  (ToJson Failed)");
        }
    }
}