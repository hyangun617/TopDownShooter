using System.Collections.Generic;
using UnityEngine;

public static class AudioClipListExtensions
{
    // 리스트에서 무작위 클립을 반환. 리스트가 없거나 비어있으면 null (SoundManager.PlaySfx 는 null 클립을 무시함).
    public static AudioClip PickRandom(this IReadOnlyList<AudioClip> clips)
    {
        if (clips == null || clips.Count == 0) return null;
        return clips[Random.Range(0, clips.Count)];
    }
}
