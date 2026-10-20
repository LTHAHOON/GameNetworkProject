using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerStatController : NetworkBehaviour
{
    private readonly NetworkVariable<int> _curScore = new(0);
    private PlayerStatView _playerStatView;
    private PlayerAccount _playerAccount;

    public override void OnNetworkSpawn()
    {
        if(!IsOwner)
        {
            return;
        }

        if(!_playerStatView)
        {
            _playerStatView = FindAnyObjectByType<PlayerStatView>();
            if(!_playerStatView)
            {
                Debug.LogError("PlayerStatView를 찾을 수 없습니다.");
                return;
            }
        }
        _playerStatView.SetScoreText(_curScore.Value);
        _curScore.OnValueChanged += _playerStatView.SetScoreText;
        _curScore.OnValueChanged += OnScoreChanged;

        if(!_playerAccount)
        {
            _playerAccount = FindAnyObjectByType<PlayerAccount>();
        }
        if(NetworkPlayerManager.Instance.TryGetScoreData(_playerAccount.ID, out int curScore))
        {
            SetScoreRpc(curScore);
        }
    }

    public override void OnNetworkDespawn()
    {
        if(!IsOwner)
        {
            return;
        }
        if(!_playerStatView)
        {
            return;
        }

        _playerStatView.SetScoreText(0);
        _curScore.OnValueChanged -= _playerStatView.SetScoreText;
        _curScore.OnValueChanged -= OnScoreChanged;
    }

    [Rpc(SendTo.Server)]
    public void AddScoreRpc(int addAmount)
    {
        _curScore.Value += addAmount;
    }
    
    [Rpc(SendTo.Server)]
    public void SetScoreRpc(int curScore)
    {
        _curScore.Value = curScore;
    }

    public void OnScoreChanged(int previousValue, int newValue)
    {
        NetworkPlayerManager.Instance.SetScoreData(_playerAccount.ID, newValue);
    }
}
