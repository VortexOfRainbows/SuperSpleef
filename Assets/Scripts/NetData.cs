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
    public NetworkVariable<bool> DataSentToClients = new NetworkVariable<bool>(false);

    public static NetworkVariable<float> WorldSize;
    public static NetworkVariable<int> WorldType;
    public static NetworkVariable<bool> WorldChaos;
    public static NetworkVariable<bool> WorldUCI;
    public static NetworkVariable<bool> WorldBorder;
    public static NetworkVariable<bool> WorldPadded;
    private void Awake()
    {
        Main.NetData = this;
        StartingPlayerCount = new NetworkVariable<int>(-1);
        HasSpawnedPlayers = new NetworkVariable<bool>(false);
        SyncedMode = new NetworkVariable<int>(0);
        GenSeed = new NetworkVariable<int>(Random.Range(0, int.MaxValue), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        WorldType = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        WorldSize = new NetworkVariable<float>(World.DefaultChunkRadius);
        WorldChaos = new NetworkVariable<bool>(false);
        WorldUCI = new NetworkVariable<bool>(false);
        WorldBorder = new NetworkVariable<bool>(false);
        WorldPadded = new NetworkVariable<bool>(false);
    }
    private void LateUpdate()
    {
        if(Main.NetData != this)
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
        HasSpawnedPlayers.Value = DataSentToClients.Value = false;
        SyncedMode.Value = 0;
        if(GenSeed.Value <= 0)
            GenSeed.Value = Random.Range(0, int.MaxValue);
        if (WorldSize.Value <= 0)
            WorldSize.Value = World.DefaultChunkRadius;
        ResetWorldgenValues();
    }
    private void ResetWorldgenValues()
    {
        if(IsServer)
        {
            WorldSize.Value = ClientData.WorldSize.Value;
            WorldType.Value = ClientData.WorldType.Value;
            WorldChaos.Value = ClientData.WorldGenChaos.Value > 0;
            WorldUCI.Value = ClientData.WorldGenUCI.Value > 0;
            WorldBorder.Value = ClientData.WorldGenBorder.Value > 0;
            WorldPadded.Value = ClientData.WorldGenPadding.Value > 0;

            SyncedMode.Value = ClientData.WorldApoc.Value > 0 ? GameModeID.LaserBattleApocalypse : GameModeID.LaserBattle;
        }
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
        Player player = Main.Players[playerID];
        Item item = player.Inventory.Get(itemSlot);
        item.SetCount(count);
    }
}
