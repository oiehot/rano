#if false

using UnityEngine;

namespace Rano.PoolSystem
{
    // TODO: PoolSystem과 긴밀하게 통합시킬것.
    [DisallowMultipleComponent]
    public class PoolableEntity : MonoBehaviour
    {
        [SerializeField] private bool _autoPushOnDisable = true;
        
        private void OnDisable()
        {
            // 게임오브젝트가 Disable되면 풀에 회수한다.
            if (_autoPushOnDisable)
            {
                Log.Info("AutoPush가 활성화되어 있어 오브젝트 풀에 Push합니다");
                
                // 여기서 문제가 발생한다.
                // 아직 OnDisable이 끝나지 않아 GameObject는 활성화 상태다.
                // 그런데 Push 메서드에서 다시 SetAtcive(false)를 호출하고 있다.
                // 함수가 무한반복되는 상황이 발생해야 하지만 유니티가 다음 에러를
                // 발생시키면서 중단한다.
                //
                // GameObject is already being activated or deactivated.
                //
                Game.Pool.Push(gameObject);
            }
        }
    }
}

#endif