using TMPro;
using UnityEngine;

public class PlayerAccountView : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _playerIdText;

    public void SetPlayerIdText(string playerId)
    {
        _playerIdText.SetText($"Player ID: {playerId}");
    }
}
