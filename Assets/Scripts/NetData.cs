using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

public class NetData : NetworkBehaviour //Team members that contributed to this script: Ian Bunnel
{
    /// 
    /// These variables must be public so they can be accessed by GameStateManager, which will distribute them to other classes
    /// 
    public NetworkVariable<int> SyncedMode;
    public NetworkVariable<int> GenSeed;
    public NetworkVariable<bool> HasSpawnedPlayers;
    public NetworkVariable<int> StartingPlayerCount;
    public static NetworkVariable<float> WorldSizeOverride;
    private void Awake()
    {
        GameStateManager.NetData = this;
        StartingPlayerCount = new NetworkVariable<int>(-1);
        HasSpawnedPlayers = new NetworkVariable<bool>(false);
        SyncedMode = new NetworkVariable<int>(0);
        GenSeed = new NetworkVariable<int>(Random.Range(0, int.MaxValue), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        WorldSizeOverride = new NetworkVariable<float>(World.DefaultChunkRadius);
    }
    private void LateUpdate()
    {
        if(GameStateManager.NetData != this)
        {
            if(IsServer)
            {
                Destroy(gameObject);
            }
        }
    }
    public void ResetValues()
    {
        StartingPlayerCount.Value = -1;
        HasSpawnedPlayers.Value = false;
        SyncedMode.Value = 0;
        if(GenSeed.Value <= 0)
            GenSeed.Value = Random.Range(0, int.MaxValue);
        if (WorldSizeOverride.Value <= 0)
            WorldSizeOverride.Value = World.DefaultChunkRadius;
    }
    [Rpc(SendTo.SpecifiedInParams)]
    public void SetBlockRpc(float x, float y, float z, int type, float particleMultiplier, bool generateSound, RpcParams rpcParams)
    {
        World.SetBlock(x, y, z, type, particleMultiplier, true, generateSound);
    }
    [Rpc(SendTo.SpecifiedInParams)]
    public void TileFillRpc(int x, int y, int z, int x2, int y2, int z2, int blockID, float particleMultiplier, bool generateSound, RpcParams rpcParams)
    {
        World.FillBlock(x, y, z, x2, y2, z2, blockID, particleMultiplier, true, generateSound);
    }
    [Rpc(SendTo.Server)]
    public void SpawnProjectileRpc(int Type, Vector3 pos, Quaternion rot, Vector3 velo)
    {
        Projectile.NewProjectile(Type, pos, rot, velo);
    }
    [Rpc(SendTo.Server)]
    public void DespawnNetworkPlayerRpc(int NetworkPlayerIndex)
    {
        NetHandler.LoggedPlayers[NetworkPlayerIndex].GetComponent<NetworkObject>().Despawn();
    }
    [Rpc(SendTo.NotMe)]
    public void SyncInventoryItemRpc(int playerID, int itemSlot, int count)
    {
        Player player = GameStateManager.Players[playerID];
        Item item = player.Inventory.Get(itemSlot);
        item.SetCount(count);
    }
}
