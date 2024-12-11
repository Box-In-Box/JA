using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class Spawn
{
    [field : SerializeField] public Transform targetTransform { get; set; } // 스폰 위치
    [field : SerializeField] public bool isRandomSpawnInRange { get; set; } = false; // 원 범위 랜덤 스폰 여부
    [field : SerializeField, Range(0, 10f), ShowIf(nameof(isRandomSpawnInRange), false)] public float radius { get; set; } = 5f; // 스폰 원 범위
    [field : SerializeField, ReadOnly, ShowIf(nameof(isRandomSpawnInRange), false)] public Vector3 position { get; set; } // 계산된 스폰 위치
    [field : SerializeField, ReadOnly, ShowIf(nameof(isRandomSpawnInRange), false)] public Quaternion rotation { get; set; } // 계산된 스폰 회전
}

public class MapInfo : MonoBehaviour
{
    public static MapInfo instance;

    [field : Title("[ BGM ]")]
    [field : SerializeField] public AudioClip bgmClip { get; set; }

    [field : Title("[ Spawn Position ]", "Key : 이전 씬 이름, Root : 씬 바로 이동 될 때")]
    [field : SerializeField] public SerializedDictionary<string, Spawn> spawnDictionary { get; set; } = new SerializedDictionary<string, Spawn>();
    
    private void Awake()
    {
        instance = this;
        foreach (var spawn in spawnDictionary) {
            SetRandomRangeSpawnInCircle(spawn.Value);
        }
    }

    private void Start()
    {
        if (bgmClip != null) {
            SoundManager.instance.PlayBGM(bgmClip);
        }
    }

    private void SetRandomRangeSpawnInCircle(Spawn spawn)
    {
        if (spawn.isRandomSpawnInRange) {
            spawn.position = spawn.targetTransform.position + (Random.insideUnitSphere * spawn.radius);
            spawn.position = new Vector3(spawn.position.x, spawn.targetTransform.position.y, spawn.position.z);
            spawn.rotation = spawn.targetTransform.rotation;
        }
        else {
            spawn.position = spawn.targetTransform.position;
            spawn.rotation = spawn.targetTransform.rotation;
        }
    }

    public Spawn GetSpawnTransform(string sceneName)
    {
        spawnDictionary.TryGetValue(sceneName.ToString(), out Spawn spawn);

        return spawn ?? spawnDictionary.GetValueOrDefault("Root");
    }
}