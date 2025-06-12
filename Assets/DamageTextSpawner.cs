using UnityEngine;

public class DamageTextSpawner : MonoBehaviour
{
    public static DamageTextSpawner Instance { get; private set; }
    [SerializeField] private GameObject damageTextPrefab;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// worldPos: where the text should appear
    /// </summary>
    public void SpawnDamageText(int damage, Vector3 worldPos)
    {
        if (damageTextPrefab == null) return;
        var go = Instantiate(damageTextPrefab, worldPos, Quaternion.identity);
        var dt = go.GetComponent<DamageText>();
        dt.Initialize(damage);
    }
}
