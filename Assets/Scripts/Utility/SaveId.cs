using System;
using UnityEngine;

[Serializable]
public struct SaveId
{
    [SerializeField] private string value;

    public string Value => value;

    public bool HasValue => !string.IsNullOrEmpty(value);

    public static implicit operator string(SaveId id) => id.value;

    public override string ToString() => value;
}
