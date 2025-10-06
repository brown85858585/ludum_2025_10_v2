using Gleb;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

/*
    Documentation: https://mirror-networking.gitbook.io/docs/guides/networkbehaviour
    API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class Player : NetworkBehaviour
{
    public int Health = 100;

    [FormerlySerializedAs("slider")]
    public Slider hpBar;

    public GameObject EggPrefab;
    
    public TMP_Text HPText;

    public AudioSource DeathSound;
    public AudioSource HP100Sound;
    public AudioSource HP50Sound;


    [SyncVar(hook = nameof(SyncHealth))] //задаем метод, который будет выполняться при синхронизации переменной
    int _SyncHealth;

    //метод не выполнится, если старое значение равно новому
    void SyncHealth(int oldValue, int newValue) //обязательно делаем два значения - старое и новое. 
    {
        Health = newValue;
        hpBar.value = newValue;
        HPText.text = Health.ToString();
    }

    [Server] //обозначаем, что этот метод будет вызываться и выполняться только на сервере
    public void ChangeHealthValue(int newValue)
    {
        _SyncHealth = newValue;
        if (newValue <= 0)
        {
            RpcOnDead();
            SpawnEgg();
            return;
        }

        if (newValue <= 50)
        {
            RpcHit50();
            return;
        }

        RpcHit100();
    }

    [Server]
    public void SpawnEgg()
    {
        GameObject egg = Instantiate(EggPrefab, gameObject.transform.position, Quaternion.identity); //Создаем локальный объект пули на сервере
        NetworkServer.Spawn(egg); //отправляем информацию о сетевом объекте всем игрокам.
    }

    [ClientRpc] //обозначаем, что этот метод будет выполняться на клиенте по запросу сервера
    private void RpcOnDead() //обязательно ставим Rpc в начале названия метода
    {
        DeathSound.Play();
    }

    [ClientRpc]
    private void RpcHit50()
    {
        HP50Sound.Play();
    }

    [ClientRpc]
    private void RpcHit100()
    {
        HP100Sound.Play();
    }


    [Command] //обозначаем, что этот метод должен будет выполняться на сервере по запросу клиента
    public void CmdChangeHealth(int newValue) //обязательно ставим Cmd в начале названия метода
    {
        ChangeHealthValue(newValue); //переходим к непосредственному изменению переменной
    }

    public void OnBulletHit(Bullet bullet)
    {
        if (isServer) //если мы являемся сервером, то переходим к непосредственному изменению переменной
            ChangeHealthValue(Health - bullet.Damage);
        else
            CmdChangeHealth(Health - bullet.Damage); //в противном случае делаем на сервер запрос об изменении переменной
    }

    #region Unity Callbacks

    /// <summary>
    /// Add your validation code here after the base.OnValidate(); call.
    /// </summary>
    protected override void OnValidate()
    {
        base.OnValidate();
    }


    // NOTE: Do not put objects in DontDestroyOnLoad (DDOL) in Awake.  You can do that in Start instead.
    void Awake()
    {
    }

    void Start()
    {
        if (isOwned)
            hpBar.gameObject.SetActive(false);

        HPText.text = Health.ToString();
    }

    void Update()
    {
        if (Camera.main != null)
        {
            hpBar.transform.LookAt(Camera.main.transform);
            hpBar.transform.Rotate(0, 180, 0);
        }
    }

    #endregion

    #region Start & Stop Callbacks

    /// <summary>
    /// This is invoked for NetworkBehaviour objects when they become active on the server.
    /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
    /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
    /// </summary>
    public override void OnStartServer()
    {
    }

    /// <summary>
    /// Invoked on the server when the object is unspawned
    /// <para>Useful for saving object data in persistent storage</para>
    /// </summary>
    public override void OnStopServer()
    {
    }

    /// <summary>
    /// Called on every NetworkBehaviour when it is activated on a client.
    /// <para>Objects on the host have this function called, as there is a local client on the host. The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.</para>
    /// </summary>
    public override void OnStartClient()
    {
        NetworkIdentity identity = GetComponent<NetworkIdentity>();
        if (!identity.isLocalPlayer)
        {
            var cameras = GetComponentsInChildren<Camera>();
            foreach (var camera in cameras)
                camera.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// This is invoked on clients when the server has caused this object to be destroyed.
    /// <para>This can be used as a hook to invoke effects or do client specific cleanup.</para>
    /// </summary>
    public override void OnStopClient()
    {
    }

    /// <summary>
    /// Called when the local player object has been set up.
    /// <para>This happens after OnStartClient(), as it is triggered by an ownership message from the server. This is an appropriate place to activate components or functionality that should only be active for the local player, such as cameras and input.</para>
    /// </summary>
    public override void OnStartLocalPlayer()
    {
    }

    /// <summary>
    /// Called when the local player object is being stopped.
    /// <para>This happens before OnStopClient(), as it may be triggered by an ownership message from the server, or because the player object is being destroyed. This is an appropriate place to deactivate components or functionality that should only be active for the local player, such as cameras and input.</para>
    /// </summary>
    public override void OnStopLocalPlayer()
    {
    }

    /// <summary>
    /// This is invoked on behaviours that have authority, based on context and <see cref="NetworkIdentity.hasAuthority">NetworkIdentity.hasAuthority</see>.
    /// <para>This is called after <see cref="OnStartServer">OnStartServer</see> and before <see cref="OnStartClient">OnStartClient.</see></para>
    /// <para>When <see cref="NetworkIdentity.AssignClientAuthority">AssignClientAuthority</see> is called on the server, this will be called on the client that owns the object. When an object is spawned with <see cref="NetworkServer.Spawn">NetworkServer.Spawn</see> with a NetworkConnectionToClient parameter included, this will be called on the client that owns the object.</para>
    /// </summary>
    public override void OnStartAuthority()
    {
    }

    /// <summary>
    /// This is invoked on behaviours when authority is removed.
    /// <para>When NetworkIdentity.RemoveClientAuthority is called on the server, this will be called on the client that owns the object.</para>
    /// </summary>
    public override void OnStopAuthority()
    {
    }

    #endregion
}