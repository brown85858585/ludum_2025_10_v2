using Gleb;
using UnityEngine;
using Mirror;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

/*
    Documentation: https://mirror-networking.gitbook.io/docs/guides/networkbehaviour
    API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class Player : NetworkBehaviour
{
    public int Health = 100;
    public int Score = 0;

    [FormerlySerializedAs("slider")]
    public Slider hpBar;

    public GameObject EggPrefab;

    public TMP_Text HPText;
    private TMP_Text ScoreText;

    public AudioSource DeathSound;
    public AudioSource HP100Sound;
    public AudioSource HP50Sound;
    public AudioSource RespawnSound;


    [SyncVar(hook = nameof(SyncHealth))] //задаем метод, который будет выполняться при синхронизации переменной
    int _SyncHealth;

    [SyncVar(hook = nameof(SyncScore))] 
    int _SyncScore;


    //метод не выполнится, если старое значение равно новому

    void SyncHealth(int oldValue, int newValue) //обязательно делаем два значения - старое и новое. 
    {
        Health = newValue;
        hpBar.value = newValue;
        HPText.text = Health.ToString();
    }

    void SyncScore(int oldValue, int newValue)
    {
        Score = newValue;
        ScoreText.text = Score.ToString();
    }

    [Server]
    public void SpawnEgg(uint owner)
    {
        GameObject egg = Instantiate(EggPrefab, gameObject.transform.position, Quaternion.identity); //Создаем локальный объект пули на сервере
        NetworkServer.Spawn(egg); //отправляем информацию о сетевом объекте всем игрокам.
        egg.GetComponent<Egg>().Init(owner); //инициализируем поведение пули

    }

    [ClientRpc] //обозначаем, что этот метод будет выполняться на клиенте по запросу сервера
    private void RpcOnDead(uint owner) //обязательно ставим Rpc в начале названия метода
    {
        DeathSound.Play();
        if (owner == netId)
            Respawn();
    }

    [ClientRpc]
    private void RpcHit50()
    {
        HP50Sound.Play();
    }

    private void RespawnPizdec()
    {
        var cm = GetComponent<CharacterMotor>();
        cm.SetVelocity(Vector3.zero);
        var respawns = GameObject.FindGameObjectsWithTag("Respawn");
        int randomIndex = Random.Range(0, respawns.Length);
        transform.position = respawns[randomIndex].transform.position;
        RespawnSound.Play();
    }

    [ClientRpc]
    private void RespawnDolbayob()
    {
        var cm = GetComponent<CharacterMotor>();
        cm.SetVelocity(Vector3.zero);
        var respawns = GameObject.FindGameObjectsWithTag("Respawn");
        int randomIndex = Random.Range(0, respawns.Length);
        transform.position = respawns[randomIndex].transform.position;
        RespawnSound.Play();
    }

    [ClientRpc]
    private void RpcHit100()
    {
        HP100Sound.Play();
    }

    public void OnBulletHit(Bullet bullet)
    {
        if (isServer) //если мы являемся сервером, то переходим к непосредственному изменению переменной
            ChangeHealthValue(Health - bullet.Damage);
        else
            CmdChangeHealth(Health - bullet.Damage); //в противном случае делаем на сервер запрос об изменении переменной
    }

    [Command] //обозначаем, что этот метод должен будет выполняться на сервере по запросу клиента
    public void CmdChangeHealth(int newValue) //обязательно ставим Cmd в начале названия метода
    {
        ChangeHealthValue(newValue); //переходим к непосредственному изменению переменной
    }

    [Server] //обозначаем, что этот метод будет вызываться и выполняться только на сервере
    public void ChangeHealthValue(int newValue)
    {
        _SyncHealth = newValue;
        if (newValue <= 0)
        {
            RpcOnDead(netId);
            SpawnEgg(netId);
            _SyncHealth = 100;
            return;
        }

        if (newValue <= 50)
        {
            RpcHit50();
            return;
        }

        RpcHit100();
    }

    public void OnPickupEgg()
    {
        if (isServer) //если мы являемся сервером, то переходим к непосредственному изменению переменной
            ChangeScore(Score + 2);
        else
            CmdPickup(Score + 2);
    }

    [Command]
    public void CmdPickup(int newValue)
    {
        ChangeScore(newValue);
    }

    [Server]
    public void ChangeScore(int newValue)
    {
        _SyncScore = newValue;
    }

    #region Unity Callbacks

    void Start()
    {
        if (isOwned)
            hpBar.gameObject.SetActive(false);

        HPText.text = Health.ToString();
        var hud = GameObject.FindGameObjectWithTag("HUD");
        var comp = hud.GetComponent<CanvasHUD>();
        ScoreText = comp.ScoreText;
        ScoreText.text = Score.ToString();
    }

    void Update()
    {
        if (Camera.main != null)
        {
            hpBar.transform.LookAt(Camera.main.transform);
            hpBar.transform.Rotate(0, 180, 0);
        }
        
        if (Input.GetKeyDown(KeyCode.T) || needResp)
        {
            RespawnPizdec();
            needResp = false;
        }
    }

    public void Respawn()
    {
        if (isServer)
            RespawnSuka();
        else
            CmdRespawn();
    }

    [Server]
    public void RespawnSuka()
    {
        RespawnDolbayob();
    }


    public bool needResp = false;

    [Command]
    public void CmdRespawn()
    {
        RespawnDolbayob();
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

    #endregion
    
}