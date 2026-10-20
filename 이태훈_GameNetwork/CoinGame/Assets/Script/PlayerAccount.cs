using System;
using UnityEngine;

public class PlayerAccount : MonoBehaviour
{
    private Guid _id;

    private void Awake()
    {
        _id = Guid.NewGuid();
    }

    public Guid ID => _id;
}
