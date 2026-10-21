using System.Text;
using TMPro;
using UnityEngine;

public class PlayerAccountView : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _playerIdText;
    [SerializeField]
    private TMP_Text _onlineStatusText;

    private readonly StringBuilder _sb = new();

    public void SetPlayerIdText(string playerId)
    {
        _sb.Clear();
        _sb.Append("Player ID: ");
        _sb.Append(playerId);
        _playerIdText.SetText(_sb);
    }

    public void SetOnlineStatusText(bool isOnline)
    {
        _sb.Clear();
        _sb.Append("Online Status: ");
        if(isOnline)
        {
            _sb.Append("Online");
        }
        else
        {
            _sb.Append("Offline");
        }
        _onlineStatusText.SetText(_sb);
    }
}
