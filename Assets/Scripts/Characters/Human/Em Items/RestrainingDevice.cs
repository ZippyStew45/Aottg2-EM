using UnityEngine;
using Characters;
using Photon.Pun;
using Utility;
using Unity.VisualScripting;
using System;

// item deploys after a delay (give it 5 seconds for now)
// item has a fixed amount that can be used set as the same number for everybody by the MC. if someone uses one, number goes down for everyone
// item can't be deployed within 500 units of another of the same item
// item can require the wagon to be within 200 units (the MC has to set this to true or false)
// item can only be deployed if the player is grounded
// item user can move while deploying the device but it interrupts the deployment
// item triggers a load bar appearing
// item being deployed doesn't mean it's active. After it's deployed, logistician has to activate it
// logistician has to be within 200 units to activate it
// item activates after 500ms of delay
// titans caught in aoe are restrained
// humans caught in aoe are killed
// if there are no traps found to activate (none exist or already activated) game should somehow send an error message
// keybinds are done both by a wheel and basic key hits
// some custom logic commands to set numbers for the mc (look for them at the end of the docs https://docs.google.com/document/d/1AsUV7SZjOZdz0_tmaKX8EbOonjlgYXvwcroZeCzoYHg/edit)

class RestrainingDevice : MonoBehaviour
{
    // MasterClient //
    public int AvailableAmount = 9999;
    public bool RequiresWagon = false;
    private float WagonDistance = 200;

    // Numbers //
    private int DeploymentDelay = 5;
    private float ActivationDelay = 0.5f;
    private float ActivationDistance = 200;
    private float OtherDeviceDistance = 500;

    // Computed Numbers //
    private float DeploymentTimeLeft;

    // State //
    public bool Deploying = false;
    public bool Activating = false;
    public bool IsDeployed = false;
    public bool IsActivated = false;
    private bool PlayingAnim = false;

    // Interaction //
    private string[] Errors = new string[] { "WagonFar", "DeviceNearby", "DeviceFar", "NoDeviceLeft", "Common", "HumanMove", "Deploying" };

    public Human _owner { get; set; }

    private void Start()
    {

    }

    private void Update()
    {
        if (Deploying == true)
        {
            UpdateDeployment();
            if (IsHumanMoving())
                CancelDeployment();
        }
    }



    public void StartDeploying(Human human)
    {
        _owner = human;
        DeploymentTimeLeft = DeploymentDelay;
        bool canDeploy = (AvailableAmount > 0) && (_owner.Grounded == true) && (HasOtherDevice() == false) && (Deploying == false);

        if (canDeploy == true)
        {
            if (RequiresWagon == true)
            {
                if (IsWagonNearby())
                {
                    UpdateDeployment();
                }
                else
                {
                    PrintDeploymentError(Errors[0]);
                }
            }
            else
            {
                UpdateDeployment();
            }
        }
        else
        {
            if (AvailableAmount <= 0)
            {
                PrintDeploymentError(Errors[3]);
            }
            else if (HasOtherDevice())
            {
                PrintDeploymentError(Errors[1]);
            }
            else if (Deploying == true)
            {
                PrintDeploymentError(Errors[6]);
            }
            else if (_owner.Grounded == false)
                PrintDeploymentError(Errors[5]);
            else
            {
                PrintDeploymentError(Errors[4]);
            }
        }
    }

    public bool HasOtherDevice()
    {
        // use OtherDeviceDistance here to check for the same prefab around the radius
        return false;
    }

    public bool IsWagonNearby()
    {
        // use WagonDistance here to check for wagon around the radius
        return true;
    }
    private void UpdateDeployment()
    {
        bool ownerMoving = false; // get velocity of _owner
        if (ownerMoving == true)
            CancelDeployment();

        Debug.Log("DEPLOYMENT TIME: " + DeploymentTimeLeft);
        if (DeploymentTimeLeft > 0)
        {
            if(PlayingAnim == false)
                PlayRefillAnimation();
            
            PlayingAnim = false;
            Deploying = true;
            DeploymentTimeLeft -= Time.deltaTime;
        }
        else
        {
            FinishDeployment();
        }
    }

    private void PlayRefillAnimation()
    {
        PlayingAnim = true;
        _owner.PlayAnimation(HumanAnimations.Refill);
    }

    private void PrintDeploymentError(string reason)
    {
        Debug.LogError(reason);
    }

    private void FinishDeployment()
    {
        // instantiate the prefab and aoe //
        GameObject go = PhotonNetwork.Instantiate(ResourcePaths.EE + "/Functionality/RestrainingDevice/Prefabs/RestrainingDevice", _owner.transform.position, _owner.transform.rotation, 0);
        _owner.Idle();
        IsDeployed = true;
        Deploying = false;
    }

    private void CancelDeployment()
    {
        Deploying = false;
        IsDeployed = false; // set this to false just in case of potential bugs
        PrintDeploymentError(Errors[5]);
    }

    private bool IsHumanMoving()
    {
        Vector3 velocity = _owner.GetComponent<Rigidbody>().velocity;
        float moveThreshold = 0.2f;

        bool isMoving = Math.Abs(velocity.x) > moveThreshold && Math.Abs(velocity.y) > moveThreshold && Math.Abs(velocity.z) > moveThreshold;
        return isMoving;
    }
}