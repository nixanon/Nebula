﻿using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerShooting : NetworkBehaviour
{
    [SerializeField] float shotCooldown = .3f;
    [SerializeField] Transform firePosition;
    [SerializeField] ShotEffectsManager shotEffects;

    // hook = callback
    // syncvar allows the client to be notified of changes,
    // by the sever.
    [SyncVar(hook ="OnScoreChanged")] int score;
    
    float ellapsedTime;
    bool canShoot;

    void Start()
    {
        shotEffects.Initialize();

        if (isLocalPlayer)
            canShoot = true;
    }
    
    [ServerCallback]
    void OnEnable()
    {
        score = 0;
    }

    void Update()
    {
        if (!canShoot)
            return;

        ellapsedTime += Time.deltaTime;

        if (CrossPlatformInputManager.GetButtonDown("Fire1") && ellapsedTime > shotCooldown)
        {
            ellapsedTime = 0f;
            CmdFireShot(firePosition.position, firePosition.forward);
        }
    }

    [Command]
    void CmdFireShot(Vector3 origin, Vector3 direction)
    {
        RaycastHit hit;

        Ray ray = new Ray(origin, direction);
        Debug.DrawRay(ray.origin, ray.direction * 50f, Color.red, 20f);

        bool result = Physics.Raycast(ray, out hit, 50f);

        if (result)
        {
            PlayerHealth enemy = hit.transform.GetComponent<PlayerHealth>();

            if (enemy != null)
            {
                
                bool wasKillShot = enemy.TakeDamage();
                if (wasKillShot)
                    score += 10;
                else
                    score++;
            }
        }

        RpcProcessShotEffects(result, hit.point);
    }

    [ClientRpc]
    void RpcProcessShotEffects(bool playImpact, Vector3 point)
    {
        shotEffects.PlayShotEffects();

        if (playImpact)
            shotEffects.PlayImpactEffect(point);
    }

    void OnScoreChanged(int value)
    {
        if (isLocalPlayer)
        {
            PlayerCanvas.canvas.SetKills(value);
        }
    }
}