using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using Rano;

namespace Rano.Addressable
{
    /// <summary>
    /// 스프라이트 아틀라스를 빌트인되지 않기 위해서 Include in Build 를 활성화 하는 경우,
    /// 특별한 처리없이 아틀라스내에 있는 스프라이트 사용시 에러가 난다.
    /// 아틀라스에 들어가 있는 스프라이트를 로드하기 위해서 아틀라스에 처음 접근할 때 SpriteAtlasManager.atlasRequested가 호출되는데
    /// 여기서 어드레서블을 통해 로드한 아틀라스를 전달해주면 정상적으로 스프라이트가 로드된다.
    /// </summary>
    public class AddressableSpriteAtlasManager : MonoSingleton<AddressableSpriteAtlasManager>
    {
        private Dictionary<string, System.Action<SpriteAtlas>> _atlasRequests = new Dictionary<string, System.Action<SpriteAtlas>>();
        private Dictionary<string, SpriteAtlas> _spriteAtlases = new Dictionary<string, SpriteAtlas>();

        /// <summary>
        /// SpriteAtlasManager의 요청이 들어오면 즉시 로드할지 여부. 활성화시 즉시 로드되고
        /// 비활성화하면 수동으로 로드해야 한다. (LoadAllRequstedSpriteAtlas를 사용하면 된다)
        /// </summary>
        public bool IsAutoLoad { get; private set; } = false;

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

        private void HandleRequestAtlas(string name, System.Action<SpriteAtlas> callback)
        {
            SpriteAtlas spriteAtlas;

            if (TryGetLoadedSpriteAtlas(name, out spriteAtlas))
            {
                Log.Important($"SpriteAtlas Requested: {name} => AlreadyLoaded => SendToCallback");
                callback(spriteAtlas);
                return;
            }
            else
            {
                if (IsAutoLoad)
                {
                    Log.Important($"SpriteAtlas Requested: {name} => AutoLoad => LoadImmediately");
                    LoadRequestedSpriteAtlasAsync(name, callback).ConfigureAwait(false);
                }
                else
                {
                    Log.Important($"SpriteAtlas Requested: {name}");
                    _atlasRequests[name] = callback;
                }
            }
        }

        private void HandleAtlasRegistered(SpriteAtlas spriteAtlas)
        {
            Log.Info($"SpriteAtlas Registered: {spriteAtlas}");
        }

        /// <summary>
        /// SpriteAtlasManager에 의해 요청된 SpriteAtlas를 즉시 로딩시작하고 완료되는 즉시 콜백으로 전달하게 한다.
        /// </sumamry>
        private async Task LoadRequestedSpriteAtlasAsync(string name, System.Action<SpriteAtlas> callback)
        {
            var asyncOperation = Addressables.LoadAssetAsync<SpriteAtlas>(name);
            await asyncOperation.Task;
            if (asyncOperation.Status == AsyncOperationStatus.Succeeded)
            {
                Log.Info($"SpriteAtlas Loaded: {name}");
                var spriteAtlas = asyncOperation.Result;
                _spriteAtlases[name] = spriteAtlas;
                callback(spriteAtlas);
            }
        }

        /// <summary>
        /// SpriteAtlasManager에 의해 요청된 모든 SpriteAtlas들을 로딩하고 콜백으로 전달한다.
        /// </sumamry>
        public async Task LoadAllRequstedSpriteAtlasAsync()
        {
            var tasks = new List<Task>();

            Log.Info("Load All Requested SpriteAtlas Start");

            foreach (var request in _atlasRequests)
            {
                var name = request.Key;
                var callback = request.Value;

                var loadSpriteAtlas = Addressables.LoadAssetAsync<SpriteAtlas>(name);
                loadSpriteAtlas.Completed += (handle) => {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        Log.Info($"SpriteAtlas Loaded: {name}");
                        SpriteAtlas spriteAtlas = handle.Result;
                        callback(spriteAtlas);
                        _spriteAtlases[name] = spriteAtlas;
                    }
                };

                tasks.Add(loadSpriteAtlas.Task); // AsyncOperationHandle 의 Task
            }

            // 모든 스프라이트 로딩작업이 완료될 때까지 대기.
            await Task.WhenAll(tasks);

            Log.Info($"All Requested SpriteAtlas Loaded");

            _atlasRequests.Clear();
        }

        public async Task LoadSpriteAtlasAsync(string name)
        {
            if (IsSpriteAtlasLoaded(name))
            {
                Log.Warning($"SpriteAtlas Already Loaded: {name}");
                return;
            }

            var handle = Addressables.LoadAssetAsync<SpriteAtlas>(name);
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Log.Info($"SpriteAtlas Loaded: {name}");
                _spriteAtlases[name] = handle.Result;
            }
        }

        public void UnloadSpriteAtlas(string name)
        {
            SpriteAtlas spriteAtlas;
            if (TryGetLoadedSpriteAtlas(name, out spriteAtlas))
            {
                Log.Info($"Unload SpriteAtlas: {name}");
                Addressables.Release(spriteAtlas);
                _spriteAtlases.Remove(name);
            }
        }

        public void UnloadAllSpriteAtlas()
        {
            Log.Info("Unload All SpriteAtlas");
            foreach (var kv in _spriteAtlases)
            {
                Log.Info($"Unload SpriteAtlas: {kv.Key}");
                Addressables.Release(kv.Value);
            }
            _spriteAtlases.Clear();        
            _atlasRequests.Clear();
        }

        public bool TryGetLoadedSpriteAtlas(string name, out SpriteAtlas value)
        {
            return _spriteAtlases.TryGetValue(name, out value);
        }

        public bool IsSpriteAtlasLoaded(string name)
        {
            return _spriteAtlases.ContainsKey(name);
        }    
    }
}