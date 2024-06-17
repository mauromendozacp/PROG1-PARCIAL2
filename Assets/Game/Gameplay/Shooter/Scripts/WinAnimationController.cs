using System;
using System.Collections;

using UnityEngine;

using Cinemachine;

public class WinAnimationController : MonoBehaviour
{
    [SerializeField] private float focusRuneTransitionTime = 0f;
    [SerializeField] private float focusRuneExtraTime = 0f;
    [SerializeField] private CinemachineVirtualCamera virtualCamera = null;
    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private AudioEvent winEvent = null;

    [SerializeField] private float radius = 0f;
    [SerializeField] private int spawnCount = 0;
    [SerializeField] private float spawnDelay = 0f;
    [SerializeField] private GameObject[] chibiPrefabs = null;

    public void PlayWinAnimation(Vector3 focusPosition, Action onFinish)
    {
        virtualCamera.Follow = null;
        virtualCamera.LookAt = null;

        GameManager.Instance.AudioManager.StopCurrentMusic(
            onSuccess: () =>
            {
                GameManager.Instance.AudioManager.PlayAudio(winEvent);
            });

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

            for (int i = 0; i < spawnCount; i++)
            {
                float angle = i * Mathf.PI * 2 / spawnCount;

                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;

                Vector3 spawnPosition = new Vector3(focusPosition.x + x, focusPosition.y, focusPosition.z + z);

                int randomIndex = UnityEngine.Random.Range(0, chibiPrefabs.Length);
                GameObject chibiGO = Instantiate(chibiPrefabs[randomIndex], transform);
                chibiGO.transform.position = spawnPosition;
                chibiGO.transform.forward = Vector3.back;

                yield return new WaitForSeconds(spawnDelay);
            }

            yield return new WaitForSeconds(focusRuneExtraTime);

            onFinish?.Invoke();
        }
    }
}
