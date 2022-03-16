#if false

public class AssetManager
{
    private Dictionary<SpritePreloadTag, List<SpriteAtlas>> _loadedSpriteSheets = new Dictionary<SpritePreloadTag, List<SpriteAtlas>>();
    private Dictionary<string, SpriteAtlas> _guidToSpriteSheet = new Dictionary<string, SpriteAtlas>();
    private Dictionary<string, Action<SpriteAtlas>> _lateBindingCallbacks = new Dictionary<string, Action<SpriteAtlas>>();
 
     public AssetManager()
    {
        SpriteAtlasManager.atlasRegistered += (spriteAtlas =>
        {
            UnityEngine.Debug.Log("spriteAtlas loaded: " + spriteAtlas.name);
        });
        SpriteAtlasManager.atlasRequested += ((name, bindAction) =>
        {
            UnityEngine.Debug.Log("spriteAtlas requested: " + name);
            _lateBindingCallbacks[name] = bindAction;
        });
    }
 
    public async Task PreloadSprites(SpritePreloadTag tag)
    {
        Log.Debug("Start loading sprites tag: " + tag);
        var preloadSet = new HashSet<AssetReference>();
 
        var spriteSheetGuids = new HashSet<string>();
        foreach (var guid in PreloadableUtility.GetSpriteSheets(tag))
        {
            spriteSheetGuids.Add(guid);
        }
 
        // this is here for sprites that are serialized in client packed prefabs.
        // unity will strip them out at build time, and these callbacks will load them back
        // into the UI prefabs
        // we also keep track of these so we can explicetly unload them
        foreach (var guid in spriteSheetGuids)
        {
            if (_guidToSpriteSheet.ContainsKey(guid))
            {
                Log.Debug(_guidToSpriteSheet[guid].name + " already loaded");
                continue;
            }
 
            var result = await Addressables.LoadAssetAsync<SpriteAtlas>(guid).Task;
            if (result != null)
            {
                if (!_loadedSpriteSheets.ContainsKey(tag))
                {
                    _loadedSpriteSheets[tag] = new List<SpriteAtlas>();
                }
 
                _loadedSpriteSheets[tag].Add(result);
                _guidToSpriteSheet[guid] = result;
                Log.Debug("load " + result.name);
 
                if (_lateBindingCallbacks.ContainsKey(result.name))
                {
                    _lateBindingCallbacks[result.name].Invoke(result);
                }
                else
                {
#if !UNITY_EDITOR
            Log.Warn("no callback registered for spritesheet " + result.name);
#endif
                }
            }
        }
 
        stopwatch.Stop();
        Log.Debug("sprite loading took " + stopwatch.Elapsed);
    }
 
    public void UnloadSpriteSheets(SpritePreloadTag tag)
    {
        Log.Debug("release sprite sheets: " + tag);
        if (_loadedSpriteSheets.TryGetValue(tag, out var sheets))
        {
            _loadedSpriteSheets.Remove(tag);
            foreach (var s in sheets)
            {
                foreach (var kvp in _guidToSpriteSheet)
                {
                    if (kvp.Value == s)
                    {
                        _guidToSpriteSheet.Remove(kvp.Key);
                        break;
                    }
                }
                Addressables.Release(s);
            }
        }
        else
        {
            Log.Error("no sprite sheets to unload for " + tag);
        }
    }
 

}
#endif