using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactiveWhenInvisible : MonoBehaviour
{
    // 화면 밖으로 나가 보이지 않게 되면 호출이 된다.
    // ! 주의. 게임 뷰포트에서도 작동된다. 게임화면에 나오더라도 뷰포트에 나오면 뮤효.
    // void OnBecameInvisible()
    // {
    //     // Destroy(this.gameObject); //객체를 삭제한다.
    //     Debug.Log("OnBecameInvisible()");
    //     this.gameObject.SetActive(false);
    // }
    
    // void OnTriggerEnter2D(Collider2D collider)
    // {
    //     Debug.Log("OnTriggerEnter()");
    // }    
    void OnTriggerExit2D(Collider2D collider)
    {
        this.gameObject.SetActive(false);
    }
}