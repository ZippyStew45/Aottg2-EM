using ApplicationManagers;
using Characters;
using GameManagers;
using Map;
using Photon.Pun;
using Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Projectiles;
using Effects;
using UnityEngine.UIElements;

class CannoneerCannon : MonoBehaviourPun
{
    [Header("Cannon Parts")]
    [SerializeField] private Transform CanBase;
    [SerializeField] private Transform Barrel;
    [SerializeField] private Transform BarrelEnd;
    [SerializeField] private Transform HumanMount;

    private PhotonView PV;

    private GameObject Hero;
    private Human _human;

    protected GeneralInputSettings _input;
    protected HumanInputSettings _humanInput;
    protected InteractionInputSettings _interactionInput;

    private float currentRot = 0f;
    private float RotateSpeed = 20f;
    private float BallSpeed = 300f;

    public float interval = 3.0f; // Time interval in seconds
    private float timer; // Timer to track time

    public LineRenderer myCannonLine;

    private void Awake()
    {
        PV = gameObject.GetComponent<PhotonView>();
        Hero = PhotonExtensions.GetPlayerFromID(PV.Owner.ActorNumber);
        _human = Hero.GetComponent<Human>();
        this.myCannonLine = this.BarrelEnd.GetComponent<LineRenderer>();
    }

    void Start()
    {
        Hero.transform.position = HumanMount.transform.position;
        Hero.transform.SetParent(HumanMount.transform);
        _human.MountState = HumanMountState.MapObject;
        _human.MountedTransform = HumanMount.transform;

        if (PV.IsMine)
        {
            _input = SettingsManager.InputSettings.General;
            _humanInput = SettingsManager.InputSettings.Human;
            _interactionInput = SettingsManager.InputSettings.Interaction;
        }

        timer = 3.0f;
    }

    void Shoot()
    {
        if(timer >= interval)
        {
            timer = 0.0f;

            Vector3 position = BarrelEnd.transform.position;
            Vector3 velocity = Barrel.forward.normalized * BallSpeed;
            Vector3 gravity = new Vector3(0, -20, 0);

            EffectSpawner.Spawn(EffectPrefabs.Boom2, position, gameObject.transform.rotation, 0.5f);
            ProjectileSpawner.Spawn(ProjectilePrefabs.CannonBall, position, Quaternion.Euler(Vector3.zero), velocity, gravity, 2.0f, _human.GetComponent<PhotonView>().ViewID, _human.Team);
        }
    }

    public void UnMount() //Gotta Send RPC Here
    {
        RPCManager.PhotonView.RPC("UnMountCannoneer", RpcTarget.All, _human, Hero, gameObject, HumanMount.transform);
    }

    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        DrawLine();
        Controls(); 
        CheckHuman();
    }

    private void Controls()
    {
        if (_interactionInput.Interact.GetKeyDown()) { UnMount(); }
        if (_humanInput.AttackDefault.GetKeyDown()) { Shoot(); }

        if (_input.Forward.GetKey())
        {
            if (this.currentRot <= 32f)
            {
                this.currentRot += Time.deltaTime * RotateSpeed;
                this.Barrel.Rotate(new Vector3(Time.deltaTime * RotateSpeed, 0f, 0f));
            }
        }
        else if (_input.Back.GetKey() && (this.currentRot >= -18f))
        {
            this.currentRot += Time.deltaTime * -RotateSpeed;
            this.Barrel.Rotate(new Vector3(Time.deltaTime * -RotateSpeed, 0f, 0f));
        }
        if (_input.Left.GetKey())
        {
            base.transform.Rotate(new Vector3(0f, Time.deltaTime * -RotateSpeed, 0f));
        }
        else if (_input.Right.GetKey())
        {
            base.transform.Rotate(new Vector3(0f, Time.deltaTime * RotateSpeed, 0f));
        }
    }

    private void DrawLine()
    {
        Vector3 vector = new Vector3(0f, -30f, 0f);
        Vector3 position = this.BarrelEnd.position;
        Vector3 vector3 = (Vector3)(this.BarrelEnd.forward * 300f);
        float num = 40f / vector3.magnitude;
        this.myCannonLine.SetWidth(0.5f, 40f);
        this.myCannonLine.SetVertexCount(100);
        for (int i = 0; i < 100; i++)
        {
            this.myCannonLine.SetPosition(i, position);
            position += (Vector3)((vector3 * num) + (((0.5f * vector) * num) * num));
            vector3 += (Vector3)(vector * num);
        }
    }

    private void CheckHuman()
    {
        if (Hero == null)
        {
            Destroy(gameObject);
        }
    }
}
