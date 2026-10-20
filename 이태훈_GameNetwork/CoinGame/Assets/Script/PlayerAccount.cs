using System;
using UnityEngine;

public class PlayerAccount : MonoBehaviour
{
    [SerializeField]
    private PlayerAccountView _playerAccountView;

    private static Guid _id;

    private void Awake()
    {
        _id = Guid.NewGuid();
        _playerAccountView.SetPlayerIdText(_id.ToString());
    }

    public static Guid ID => _id;
}
