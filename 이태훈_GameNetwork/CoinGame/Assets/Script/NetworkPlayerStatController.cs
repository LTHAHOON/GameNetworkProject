using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerStatController : NetworkBehaviour
{
    private readonly NetworkVariable<int> _curScore = new(0);
    private PlayerStatView _playerStatView;
    

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

        if(NetworkPlayerManager.Instance.TryGetScoreData(PlayerAccount.ID, out int curScore))
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

    private const int MaxAddScoreAmount = 10000;

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Server)]
    public void AddScoreRpc(int addAmount)
    {
        if (addAmount <= 0 || addAmount > MaxAddScoreAmount)
        {
            Debug.LogWarning($"잘못된 점수 증가량: {addAmount}");
            return;
        }

        _curScore.Value += addAmount;
    }
    
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void SetScoreRpc(int curScore)
    {
        _curScore.Value = curScore;
    }

    public void OnScoreChanged(int previousValue, int newValue)
    {
        NetworkPlayerManager.Instance.SetScoreData(PlayerAccount.ID, newValue);
    }
}
