using System;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using ETModel;
using System.Threading;

public class Init_2 : MonoBehaviour
{
    private readonly OneThreadSynchronizationContext contex = new OneThreadSynchronizationContext();

    public InputField pname;
    public InputField password;
    public Button loginBut;
    public Button enterMap;

    GameObject uiLogin;
    GameObject uiLobby;

    private async void Start()
    {
        try
        {
            SynchronizationContext.SetSynchronizationContext(this.contex);
            DontDestroyOnLoad(gameObject);
            Game.EventSystem.Add(DLLType.Model, typeof(Game).Assembly);


            Game.Scene.AddComponent<GlobalConfigComponent>();
            Game.Scene.AddComponent<NetOuterComponent>();
            Game.Scene.AddComponent<ResourcesComponent>();

            Game.Scene.AddComponent<PlayerComponent>();
            Game.Scene.AddComponent<UnitComponent>();
            Game.Scene.AddComponent<ClientFrameComponent>();

            await BundleHelper.DownloadBundle();

            // 加载配置
            Game.Scene.GetComponent<ResourcesComponent>().LoadBundle("config.unity3d");
            Game.Scene.AddComponent<ConfigComponent>();
            Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle("config.unity3d");
            Game.Scene.AddComponent<OpcodeTypeComponent>();

            //MessageDispatherComponent
            Game.Scene.AddComponent<MessageDispatherComponent>();


            uiLogin = GameObject.Find("UILogin");
            uiLobby = GameObject.Find("UILobby");
            uiLobby.SetActive(false);

            loginBut.onClick.AddListener(OnLogin);
            enterMap.onClick.AddListener(EnterMap);

            UnitConfig unitConfig = (UnitConfig)Game.Scene.GetComponent<ConfigComponent>().Get(typeof(UnitConfig), 1001);
            Log.Debug($"config {JsonHelper.ToJson(unitConfig)}");
            Game.EventSystem.Run(EventIdType.InitSceneStart);
        }
        catch (Exception e)
        {
            Log.Error(e);
        }
    }

    public async void OnLogin()
    {
        SessionWrap sessionWrap = null;
        try
        {
            IPEndPoint connetEndPoint = NetworkHelper.ToIPEndPoint(GlobalConfigComponent.Instance.GlobalProto.Address);
            string text = pname.text;
            string pass = password.text;

            Session session = Game.Scene.GetComponent<NetOuterComponent>().Create(connetEndPoint);
            sessionWrap = new SessionWrap(session);
            R2C_Login r2CLogin = (R2C_Login)await sessionWrap.Call(new C2R_Login() { Account = text, Password = pass });
            sessionWrap.Dispose();

            connetEndPoint = NetworkHelper.ToIPEndPoint(r2CLogin.Address);
            Session gateSession = Game.Scene.GetComponent<NetOuterComponent>().Create(connetEndPoint);
            Game.Scene.AddComponent<SessionWrapComponent>().Session = new SessionWrap(gateSession);
            Game.Scene.AddComponent<SessionComponent>().Session = gateSession;
            G2C_LoginGate g2CLoginGate = (G2C_LoginGate)await SessionWrapComponent.Instance.Session.Call(new C2G_LoginGate() { Key = r2CLogin.Key });

            Log.Info("登陆gate成功!");
            uiLogin.SetActive(false);
            uiLobby.SetActive(true);

            // 创建Player
            Player player = ComponentFactory.CreateWithId<Player>(g2CLoginGate.PlayerId);
            PlayerComponent playerComponent = Game.Scene.GetComponent<PlayerComponent>();
            playerComponent.MyPlayer = player;

            //Game.Scene.GetComponent<UIComponent>().Create(UIType.UILobby);
            //Game.Scene.GetComponent<UIComponent>().Remove(UIType.UILogin);
        }
        catch (Exception e)
        {
            sessionWrap?.Dispose();
            Log.Error(e);
        }
    }

    private async void EnterMap()
    {
        try
        {
            Debug.Log(SessionComponent.Instance.Session);
            //G2C_EnterMap g2CEnterMap = await SessionComponent.Instance.Session.Call<G2C_EnterMap>(new C2G_EnterMap());
            G2C_EnterMap g2CEnterMap = (G2C_EnterMap)await SessionComponent.Instance.Session.Call(new C2G_EnterMap());

            uiLobby.SetActive(false);
            Log.Info("EnterMap...");
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }
    }

    private void Update()
    {
        this.contex.Update();
        Game.Hotfix.Update?.Invoke();
        Game.EventSystem.Update();
    }

    private void LateUpdate()
    {
        Game.Hotfix.LateUpdate?.Invoke();
        Game.EventSystem.LateUpdate();
    }

    private void OnApplicationQuit()
    {
        Game.Hotfix.OnApplicationQuit?.Invoke();
        Game.Close();
    }
}
