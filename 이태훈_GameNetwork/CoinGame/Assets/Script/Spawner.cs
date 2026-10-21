using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public enum SpawnType
    {
        BaseObject,
        NetworkObject
    }

    [SerializeField]
    private SpawnType _spawnType;
    [SerializeField]
    private GameObject _spawnPrefab;
    [SerializeField]
    private bool _onAwakeSpawn;

    [Space(10f)]
    [SerializeField]
    private bool _isLoop = false;
    [SerializeField]
    private int _spawnCount = 10;
    [SerializeField]
    private float _spawnDelay = 1f;
    [SerializeField]
    private Vector3 _spawnArea = new(5f, 5f, 5f);
    [SerializeField]
    private bool _drawArea = false;
    [SerializeField]
    private float _checkRadius = 1f;
    [SerializeField]
    private bool _drawCheckRadius = false;
    [SerializeField]
    private LayerMask _blockingLayerMask;

    private List<CancellationToken> _spawnedTokens;
    private CancellationTokenSource _spawnCts;
    private const int MAX_ITER = 100;

    private void Awake()
    {
        _spawnCts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
        if (_isLoop)
        {
            _spawnedTokens = new();
            SpawnWhenDespawned_Loop(_spawnCts.Token).Forget();
        }
        if (_onAwakeSpawn)
        {
            SpawnAsync(_spawnCts.Token, _spawnCount).Forget();
        }
    }

    public void OnDestroy()
    {
        if(_spawnCts is not null)
        {
            _spawnCts.Cancel();
            _spawnCts.Dispose();
            _spawnCts = null;
        }
    }

    public void Spawn()
    {
        SpawnAsync(_spawnCts.Token, _spawnCount).Forget();
    }

    public void SpawnPlayer(ulong clientId)
    {
        NetworkManager networkManager = NetworkManager.Singleton;
        if (networkManager == null || !networkManager.IsServer)
        {
            return;
        }
        if (!networkManager.ConnectedClients.ContainsKey(clientId))
        {
            return;
        }
        if (!_spawnPrefab.TryGetComponent(out NetworkObject networkObject))
        {
            Debug.LogError("ERROR: SpawnPrefab에 NetworkObject가 포함되지않습니다.");
            return;
        }
        _spawnCts?.Cancel();
        _spawnCts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
        SpawnPlayerAsync(_spawnCts.Token, networkObject, clientId).Forget();
    }

    public void CancelSpawnPlayer()
    {
        _spawnCts?.Cancel();
        _spawnCts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
    }

    private async UniTask SpawnPlayerAsync(CancellationToken token, NetworkObject networkObject, ulong clientId)
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_spawnDelay), cancellationToken: token);
            token.ThrowIfCancellationRequested();
            await SpawnAsyncForNetwork(token, networkObject, clientId);
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        finally
        {
            _spawnCts?.Dispose();
            _spawnCts = null;
        }
    }

    private async UniTask SpawnWhenDespawned_Loop(CancellationToken token)
    {
        try
        {
            while (_isLoop)
            {
                token.ThrowIfCancellationRequested();
                if (_spawnedTokens.Count <= 0f)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update);
                    continue;
                }
                int canceledCount = 0;
                foreach (CancellationToken spawnedToken in _spawnedTokens)
                {
                    if (spawnedToken.IsCancellationRequested)
                    {
                        ++canceledCount;
                    }
                }
                if (canceledCount > 0)
                {
                    _spawnedTokens.RemoveAll(x => x.IsCancellationRequested);
                    await SpawnAsync(token, canceledCount);
                }
                await UniTask.Yield(PlayerLoopTiming.Update);

            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    private async UniTask<Vector3> GetSpawnPosition(CancellationToken token)
    {
        int curIter = 0;
        try
        {
            while (curIter < MAX_ITER)
            {
                token.ThrowIfCancellationRequested();
                ++curIter;

                Vector3 position = transform.position;
                float xRandomDelta = UnityEngine.Random.Range(-_spawnArea.x / 2f, _spawnArea.x / 2f);
                float yRandomDelta = UnityEngine.Random.Range(-_spawnArea.y / 2f, _spawnArea.y / 2f);
                float zRandomDelta = UnityEngine.Random.Range(-_spawnArea.z / 2f, _spawnArea.z / 2f);
                position += new Vector3(xRandomDelta, yRandomDelta, zRandomDelta);

                //Instantiate되기 전이므로 자신이 포함되지 않게 됨
                if (!Physics.CheckSphere(position, _checkRadius, _blockingLayerMask))
                {
                    return position;
                }

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        return transform.position;
    }

    private async UniTask<CancellationToken?> SpawnAsyncForNetwork(CancellationToken token, NetworkObject networkObject, ulong? clientId = null)
    {
        try
        {
            await UniTask.WaitUntil(() => NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening, cancellationToken: token);
            if (!NetworkManager.Singleton.IsServer)
            {
                return null;
            }
            Vector3 spawnPosition = await GetSpawnPosition(token);
            token.ThrowIfCancellationRequested();
            NetworkObject newNetworkObject = Instantiate(networkObject);

            newNetworkObject.transform.position = spawnPosition;
            if (clientId.HasValue)
            {
                token.ThrowIfCancellationRequested();
                newNetworkObject.SpawnAsPlayerObject(clientId.Value);
            }
            else
            {
                newNetworkObject.Spawn();
            }
            return newNetworkObject.destroyCancellationToken;
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            Debug.LogException(ex, this);
        }
        return null;
    }

    private async UniTask SpawnAsync(CancellationToken token, int spawnCount)
    {
        try
        {
            if (!_spawnPrefab)
            {
                Debug.LogError("ERROR: SpawnPrefab이 비어있습니다.");
                return;
            }
            await UniTask.Delay(TimeSpan.FromSeconds(_spawnDelay), cancellationToken: token);

            int curSpawnCount = 0;
            while (curSpawnCount < spawnCount)
            {
                ++curSpawnCount;
                CancellationToken spawnedToken = default;
                bool spawnSucceeded = false;
                switch (_spawnType)
                {
                    case SpawnType.BaseObject:
                        Vector3 spawnPosition = await GetSpawnPosition(token);
                        GameObject newSpawnObject = Instantiate(_spawnPrefab);
                        newSpawnObject.transform.position = spawnPosition;
                        spawnedToken = newSpawnObject.GetCancellationTokenOnDestroy();
                        spawnSucceeded = true;
                        break;
                    case SpawnType.NetworkObject:
                        if (!_spawnPrefab.TryGetComponent(out NetworkObject networkObject))
                        {
                            Debug.LogError("ERROR: SpawnPrefab에 NetworkObject가 포함되지않습니다.");
                            return;
                        }
                        var spawnedTokenRef = await SpawnAsyncForNetwork(token, networkObject);
                        if (spawnedTokenRef.HasValue)
                        {
                            spawnedToken = spawnedTokenRef.Value;
                            spawnSucceeded = true;
                        }
                        break;
                }
                if (_isLoop && spawnSucceeded)
                {
                    _spawnedTokens.Add(spawnedToken);
                }
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        finally
        {
            if(!_isLoop)
            {
                _spawnCts?.Dispose();
                _spawnCts = null;
            }
        }
    }


    private void OnDrawGizmos()
    {
        if (_drawArea)
        {
            Gizmos.DrawWireCube(transform.position, _spawnArea);
        }
        if (_drawCheckRadius)
        {
            Gizmos.DrawWireSphere(transform.position, _checkRadius);
        }
    }

}
