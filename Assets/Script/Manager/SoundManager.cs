using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundManager
{
    private readonly AudioSource bgmSource;
    private readonly Queue<AudioSource> sfxPool = new();
    private readonly Transform sfxRoot;

    public float SFXVolume = 1f;
    public float BGMVolume = 1f;

    private const int INITIAL_POOL_SIZE = 8;

    public SoundManager(Transform root)
    {
        sfxRoot = root;

        // BGM 전용 소스 : 루프 재생, 풀에 안 들어감.
        GameObject bgmObj = new GameObject("BGM_Source");
        bgmObj.transform.SetParent(root);
        bgmSource = bgmObj.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;

        for (int i = 0; i < INITIAL_POOL_SIZE; i++)
        {
            sfxPool.Enqueue(CreateSfxSource());
        }
    }

    // 효과음 실행 객체 생성.
    // AudioRolloffMode
    // Logarithmic 기본값 - 현실적인 감쇠. 근처에서 급격히 줄어듬.
    // Linear - 거리에 비례해서 일정하게 줄어듦. 예측 가능해서 플레이 사운드(피격음, 발소리) 무난하게 사용함.
    // Custom - AnimationCurve를 직접 그린 커스텀 곡선, 
    private AudioSource CreateSfxSource()
    {
        GameObject obj = new GameObject("SFX_Source");
        obj.transform.SetParent(sfxRoot);
        AudioSource source = obj.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.minDistance = 2f;        // 이 거리 안에선 최대 볼륨
        source.maxDistance = 40f;       // 이 거리 밖에선 볼륨 0
        return source;
    }

    // 효과음 재생
    public void PlaySfx(AudioClip clip, Vector3? worldPosition = null, float pitch = 1f)
    {
        if (clip == null) return;

        AudioSource source = sfxPool.Count > 0 ? sfxPool.Dequeue() : CreateSfxSource();

        if(worldPosition.HasValue)
        {
            source.transform.position = worldPosition.Value;
            source.spatialBlend = 1f;           // 3D 효과음.
        }
        else
        {
            source.spatialBlend = 0f;           // 2D 효과음. (UI, 알림음 등)
        }

        source.clip = clip;
        source.volume = SFXVolume;
        source.pitch = pitch;
        source.Play();

        // 재생 끝나면 풀로 반환
        GameManager.Instance.StartCoroutine(ReturnAfterPlay(source, clip.length));
    }

    // sfx 풀로 반환하는 코루틴
    private System.Collections.IEnumerator ReturnAfterPlay(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        sfxPool.Enqueue(source);
    }

    // 배경음악 실행.
    public void PlayBGM(AudioClip clip)
    {
        // 이미 배경음악이 진행중이라면 중복 방지.
        if(bgmSource.clip == clip) return;

        bgmSource.clip = clip;
        bgmSource.volume = BGMVolume;
        bgmSource.Play();
    }

    // 배경음악 정지.
    public void StopBGM() => bgmSource.Stop();
}