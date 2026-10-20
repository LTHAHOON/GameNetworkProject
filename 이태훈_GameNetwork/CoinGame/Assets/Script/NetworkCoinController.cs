using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Unity.Netcode;
using UnityEngine;

public class NetworkCoinController : NetworkBehaviour
{
    private CancellationTokenSource _triggerEnterCts;

    public override void OnNetworkSpawn()
    {
        if(!IsServer)
        {
            return;
        }

        if(_triggerEnterCts is null)
        {
            _triggerEnterCts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
            OnTriggerEnterAsync(_triggerEnterCts.Token).Forget();
        }
        
    }

    public override void OnNetworkDespawn()
    {
        if(_triggerEnterCts is not null)
        {
            _triggerEnterCts.Cancel();
            _triggerEnterCts.Dispose();
            _triggerEnterCts = null;
        }
    }

    private async UniTaskVoid OnTriggerEnterAsync(CancellationToken token)
    {
        AsyncTriggerEnterTrigger triggerEnter = this.GetAsyncTriggerEnterTrigger();

        try
        {
            while(true)
            {
                token.ThrowIfCancellationRequested();
                Collider collider = await triggerEnter.OnTriggerEnterAsync(token);
                if(!collider || !collider.TryGetComponent(out NetworkPlayerStatController playerStatController))
                {
                    continue;
                }
                playerStatController.AddScoreRpc(1);
                NetworkObject.Despawn();
                break;
            }
        }
        catch(OperationCanceledException) { }
        catch(Exception ex)
        {
           Debug.LogError(ex.Message);
        }
    }
}
