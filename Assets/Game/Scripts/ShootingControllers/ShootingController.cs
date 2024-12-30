using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class ShootingController : MonoBehaviour
{
   Animator animator;
    InputManager inputManager;
    PlayerMovement playerMovement;

    [Header("Shooting Var")]
    public Transform firePoint;
    public float fireRate = 0f;
    public float fireRange = 100f;
    public float fireDamage = 15f;
    private float nextFireTime = 0f;

    [Header("Reloading")]
    public int maxAmmo = 30;
    private int currentAmmo;
    public float reloadTime = 1.5f;

    [Header("Shooting  Flags")]
    public bool isShooting;
    public bool isWalking;
    public bool isShootingInput;
    public bool isReloading = false;
    public bool isScopeInput;

    [Header("Sound Effects")]
    public AudioSource soundAudioSource;
    public AudioClip shootingSoundClip;
    public AudioClip reloadingSoundClip;

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public ParticleSystem bloodEffect;

    PhotonView view;

    public int playerTeam;


    private void Start()
    {
        view = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
        inputManager = GetComponent<InputManager>();
        playerMovement = GetComponent<PlayerMovement>();
        currentAmmo =maxAmmo;

        if (view.Owner.CustomProperties.ContainsKey("Team"))
        {
            int team = (int)view.Owner.CustomProperties["Team"];
            playerTeam = team;
        }
    }
    private void Update()
    {
        if (!view.IsMine)
        {
            return;
        }
        if(isReloading || playerMovement.isSprinting)
        {
            animator.SetBool("Shoot", false);
            animator.SetBool("ShootingMovement", false);
            animator.SetBool("ShootWalk", false);
            return;
        }
        isWalking= playerMovement.isMoving;
        isShootingInput=inputManager.fireInput;
        isScopeInput=inputManager.scopeInput;
        if (isShootingInput && isWalking)
        {
            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + 1f / fireRate;
                Shoot();
                animator.SetBool("ShootWalk", true);
            }
            animator.SetBool("Shoot",false);
            animator.SetBool("ShootingMovement", true);
            isShooting=true;
        }
         else if (isShootingInput)
        {
            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + 1f / fireRate;
                Shoot();
               
            }
            animator.SetBool("Shoot", true);
            animator.SetBool("ShootingMovement", false);
            animator.SetBool("ShootWalk", false);
            isShooting = true;
        }
        else if (isScopeInput)
        {
            animator.SetBool("Shoot", false);
            animator.SetBool("ShootingMovement", true);
            animator.SetBool("ShootWalk", false);
            isShooting = false;
        }
        else
        {
            animator.SetBool("Shoot", false);
            animator.SetBool("ShootingMovement", false);
            animator.SetBool("ShootWalk", false);
           isShooting = false;
        }
        if(inputManager.reloadInput && currentAmmo < maxAmmo)
        {
            Reload();
        }
    }
    private void Shoot()
    {
        if (currentAmmo > 0)
        {


            RaycastHit hit;
            if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, fireRange))
            {
                Debug.Log(hit.transform.name);

                //Extract hit information

                Vector3 hitPoint=hit.point;
                Vector3 hitNormal=hit.normal;

                //Apply damage to the players
                PlayerMovement playerMovementDamage= hit.collider.GetComponent<PlayerMovement>();
                Debug.DrawRay(firePoint.position, firePoint.forward * 7f, Color.red);
                if (playerMovementDamage != null && playerMovementDamage.playerTeam != playerTeam   ) 
                {

                    //apply damage
                    Debug.Log("Co vo day");
                    playerMovementDamage.ApplyDamage(fireDamage);
                    view.RPC("RPC_Shoot", RpcTarget.All, hitPoint, hitNormal);
                
                }
            }

            muzzleFlash.Play();
            soundAudioSource.PlayOneShot(shootingSoundClip);
            currentAmmo--;
        }
        else
        {
            Reload ();
        }

    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPoint, Vector3 hitNormal)
    {
        ParticleSystem blood = Instantiate(bloodEffect, hitPoint, Quaternion.LookRotation(hitNormal));
        Destroy(blood.gameObject, blood.main.duration );
    }
    private void Reload()
    {
        if (!isReloading && currentAmmo<maxAmmo)
        {
            if (isShootingInput && isWalking)
            {
                animator.SetTrigger("ShootReload");

            }
            else
            {
                animator.SetTrigger("Reload");
            }
            isReloading = true;
            soundAudioSource.PlayOneShot(reloadingSoundClip);
            Invoke("FinishReloading", reloadTime);
        }
    }
    private  void FinishReloading()
    {
        currentAmmo=maxAmmo;
        isReloading = false;
        if (isShootingInput && isWalking)
        {
            animator.ResetTrigger("ShootReload");

        }
        else
        {
            animator.ResetTrigger("Reload");
        }
    }
}
