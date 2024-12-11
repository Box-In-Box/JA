using UnityEngine;

[CreateAssetMenu(fileName = "SoundClipData", menuName = "Scriptable Objects/Sound Clip Data")]
public class SoundClipData : ScriptableObject
{
    [System.Serializable]
    public class SoundClip
    {
        public string name;
        public AudioClip clip;
    }

    [field: SerializeField] public SoundClip[] SFX { get; set; }
    [field: SerializeField] public SoundClip[] Player { get; set; }

    public AudioClip GetSFXClip(string clipName)
    {
        return GetClip(SFX, clipName);
    }

    public AudioClip GetPlayerClip(string clipName)
    {
        return GetClip(Player, clipName);
    }

    private AudioClip GetClip(SoundClip[] soundClips, string clipName)
    {
        foreach (var soundClip in soundClips)
        {
            if (soundClip.name == clipName)
            {
                return soundClip.clip;
            }
        }
        return null;
    }
}