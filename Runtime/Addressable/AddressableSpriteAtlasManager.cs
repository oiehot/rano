#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

namespace Rano.Addressable
{
    /// <remarks>
    /// SpriteAtlasManager.atlasRequested 이슈:
    ///
    /// SpriteAtlas의 에셋관리를 위해서 Include In Build를 비활성화 하는 경우
    /// SpriteAtlas에 연결된 Sprite가 자동으로 로드되지 않아 백색사각형으로 나오는 경우가 있다.
    ///
    /// 이러한 문제를 해결하기 위해서 SpriteAtlasManager는 씬이 시작될 때
    /// 모든 Sprite(SpriteAtlas에 속한)를 SpriteAtlas의 Sprite로 교체하는데
    /// SpriteAtlas를 얻기 위해서 사용자에게 요청한다.
    ///
    /// 사용자는 이 요청에 콜백을 달아 응답할 수 있는데,
    /// SpriteAtlasManager.atlasRequested 에 이벤트 핸들러를 추가함으로 가능하다.
    /// 이벤트 핸들러에서 Addressables.AssetLoadAsync 등으로 SpriteAtlas 에셋을 로드하고,
    /// 넘어온 콜백에 SpriteAtlas를 매개변수로 담아 호출해주면 SpriteAtlasManager에 SpriteAtlas가 등록되고
    /// 모든 Sprite(SpriteAtlas에 속한)가 SpriteAtlas에 들어있는 Sprite로 교체된다.
    ///
    /// 또한,
    /// Addressables.AssetLoadAsync를 사용해서 SpriteAtlas를 로드하면
    /// 씬이 시작됨과 관계없이 다시한번 SpriteAtlasManager.atlasRequested가 호출된다.
    /// 여기서 로드시 미리 캐싱해둔 SpriteAtlas를 콜백에 전달하면 Sprite들이 정상적으로 출력된다.
    ///
    /// 그러므로 SpriteAtlasManager.atlasRequsted가 콜백되는 시점은 다음 두 가지다:
    /// 1) 씬이 시작될 때
    /// 2) Addressables.AssetLoadAsync로 SpriteAtlas 에셋이 로드될 때
    ///
    /// </remarks>
    // TODO: AddressableAssetManager에 포함시킬것.
    public class AddressableSpriteAtlasManager : ManagerComponent
    {
        // private Dictionary<string, System.Action<SpriteAtlas>> _atlasRequests = new Dictionary<string, System.Action<SpriteAtlas>>();
        private Dictionary<string, SpriteAtlas> _spriteAtlases = new Dictionary<string, SpriteAtlas>();

        protected override void OnEnable()
        {
            base.OnEnable();
            SpriteAtlasManager.atlasRequested += HandleRequestAtlas;
            SpriteAtlasManager.atlasRegistered += HandleAtlasRegistered;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            SpriteAtlasManager.atlasRequested -= HandleRequestAtlas;
            SpriteAtlasManager.atlasRegistered -= HandleAtlasRegistered;
        }

        /// <summary>
        /// SpriteAtlasManager의 아틀라스 요청 콜백에 의해 호출된다.
        /// 요청이 들어오면 로드된 SpriteAtlas가 있는지 찾고 있으면 콜백을 통해 응답, 전달한다.
        /// </summary>
        /// <remarks>
        /// 요청은 SpriteAtlas에셋의 이름을 사용한다. 어드레서블 키 값이 아님에 주의할것.
        /// </remarks>
        /// <param name="spriteAtlasName">SpriteAtlasManager가 요청할 때 사용하는 에셋의 이름. 어드레서블 키 값이 아니다.</param>
        /// <param name="callback">SpriteAtlasManager에 응답할 떄 사용. 얻은 SpriteAtlas를 매개변수로 담아 콜백하여 전달한다.</param>
        private void HandleRequestAtlas(string spriteAtlasName, System.Action<SpriteAtlas> callback)
        {
            SpriteAtlas spriteAtlas;

            if (TryGetLoadedSpriteAtlas(spriteAtlasName, out spriteAtlas))
            {
                Log.Info($"SpriteAtlas is requested. Sends the Loaded SpriteAtlas ({spriteAtlasName})");
                callback(spriteAtlas);
                return;
            }
            else
            {
                // 이 메시지가 출력되지 않게 하려면 SpriteAtlas를 미리 로드하면 된다.
                Log.Info($"SpriteAtlas is requested. But SpriteAtlas is not Loaded. ({spriteAtlasName})");
                // _atlasRequests[name] = callback;
            }
        }

        private void HandleAtlasRegistered(SpriteAtlas spriteAtlas)
        {
            Log.Info($"SpriteAtlas Registered ({spriteAtlas})");
        }

        /// <summary>
        /// 어드레서블 SpriteAtlas 에셋을 비동기로 로드하여 캐싱한다.
        /// 로드되면 자동으로 SpriteAtlasManager.atlasRequested가 호출되어 전달된다.
        /// </summary>
        public async Task<bool> LoadSpriteAtlasAsync(string spriteAtlasName)
        {
            if (IsSpriteAtlasLoaded(spriteAtlasName) == true)
            {
                Log.Warning($"스프라이트 아틀라스가 이미 로드되어 있습니다 ({spriteAtlasName})");
                return false;
            }

            AsyncOperationHandle<SpriteAtlas> handle;
            try
            {
                handle = Addressables.LoadAssetAsync<SpriteAtlas>(spriteAtlasName);
            }
            catch (Exception e)
            {
                Log.Warning("스프라이트 아틀라스 로딩 실패 (예외 발생)");
                Log.Exception(e);
                return false;
            }
            
            SpriteAtlas spriteAtlas = await handle.Task;

            if (spriteAtlas == null)
            {
                Log.Warning("스프라이트 아틀라스 로딩 실패 (비어 있음)");
                return false;
            }

            Log.Info($"스프라이트 아틀라스 로드 성공 ({spriteAtlasName})");
            _spriteAtlases[spriteAtlasName] = spriteAtlas;

            return true;
        }

        // public bool LoadSpriteAtlas(string spriteAtlasName)
        // {
        //     if (IsSpriteAtlasLoaded(spriteAtlasName))
        //     {
        //         Log.Warning($"스프라이트 아틀라스가 이미 로드되어 있습니다 ({spriteAtlasName})");
        //         return false;
        //     }
        //
        //     AsyncOperationHandle<SpriteAtlas> handle;
        //     try
        //     {
        //         handle = Addressables.LoadAssetAsync<SpriteAtlas>(spriteAtlasName);
        //     }
        //     catch (Exception e)
        //     {
        //         Log.Warning("스프라이트 아틀라스 로딩 실패 (예외 발생)");
        //         Log.Exception(e);
        //         return false;
        //     }
        //
        //     // 완료 될 때까지 대기
        //     handle.WaitForCompletion();
        //     
        //     if (handle.Status != AsyncOperationStatus.Succeeded)
        //     {
        //         Log.Warning("스프라이트 아틀라스 로딩 실패 (비동기 작업 실패)");
        //         return false;
        //     }
        //
        //     SpriteAtlas spriteAtlas = handle.Result;
        //     
        //     if (spriteAtlas == null)
        //     {
        //         Log.Info($"스프라이트 아틀라스 로드 실패 (결과가 비어 있음)");
        //         return false;
        //     }
        //     
        //     Log.Info($"스프라이트 아틀라스 로드 성공 ({spriteAtlasName})");
        //     _spriteAtlases[spriteAtlasName] = spriteAtlas;
        //
        //     return true;
        // }

        public void UnloadSpriteAtlas(string spriteAtlasName)
        {
            SpriteAtlas spriteAtlas;
            
            if (TryGetLoadedSpriteAtlas(spriteAtlasName, out spriteAtlas))
            {
                Log.Info($"Unload SpriteAtlas ({spriteAtlasName})");
                Addressables.Release(spriteAtlas);
                _spriteAtlases.Remove(spriteAtlasName);
            }
        }

        public void UnloadAllSpriteAtlas()
        {
            Log.Info("Unload All SpriteAtlas");
            foreach (var kv in _spriteAtlases)
            {
                Log.Info($"Unload SpriteAtlas ({kv.Key})");
                Addressables.Release(kv.Value);
            }
            _spriteAtlases.Clear();
            // _atlasRequests.Clear();
        }

        public bool TryGetLoadedSpriteAtlas(string spriteAtlasName, out SpriteAtlas value)
        {
            return _spriteAtlases.TryGetValue(spriteAtlasName, out value);
        }

        public bool IsSpriteAtlasLoaded(string spriteAtlasName)
        {
            return _spriteAtlases.ContainsKey(spriteAtlasName);
        }    
    }
}