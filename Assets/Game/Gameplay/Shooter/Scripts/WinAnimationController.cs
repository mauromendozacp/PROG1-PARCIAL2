using System;
using System.Collections;

using UnityEngine;

using Cinemachine;

public class WinAnimationController : MonoBehaviour
{
    [SerializeField] private GameObject chibiPrefab = null;
    [SerializeField] private float focusRuneTransitionTime = 0f;
    [SerializeField] private float focusRuneExtraTime = 0f;
    [SerializeField] private CinemachineVirtualCamera virtualCamera = null;
    [SerializeField] private Vector3 offset = Vector3.zero;

    public void PlayWinAnimation(Vector3 focusPosition, Action onFinish)
    {
        virtualCamera.Follow = null;
        virtualCamera.LookAt = null;

        StartCoroutine(FocusRuneTransitionCoroutine());
        IEnumerator FocusRuneTransitionCoroutine()
        {
            float timer = 0f;
            Vector3 startPosition = virtualCamera.transform.position;
            Vector3 targetPosition = new Vector3(focusPosition.x, startPosition.y, focusPosition.z) + offset;

            while (timer < focusRuneTransitionTime)
            {
                timer += Time.deltaTime;

                virtualCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, timer / focusRuneTransitionTime);

                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForSeconds(focusRuneExtraTime);

            onFinish?.Invoke();
        }
    }
}
