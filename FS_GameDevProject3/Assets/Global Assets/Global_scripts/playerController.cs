using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [Header("-----Components-----")]
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] CharacterController controller;

    [Header("-----Stats-----")]
    [SerializeField][Range(0, 100)] int HP;
    [SerializeField][Range(1, 5)] int moveSpeed;
    [SerializeField][Range(2, 5)] int sprintMod;
    [SerializeField][Range(1, 3)] int jumpMax;
    [SerializeField][Range(5, 20)] int jumpSpeed;
    [SerializeField][Range(15, 40)] int gravity;

    [Header("-----Guns-----")]
    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    [SerializeField] GameObject gunModel;
    [SerializeField] GameObject muzzleFlashLgt;

    [Header("-----Audio-----")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audJump;
    [SerializeField] [Range(0, 1)] float audJumpVol;
    [SerializeField] AudioClip[] audHurt;
    [SerializeField][Range(0, 1)] float audHurtVol;
    [SerializeField] AudioClip[] audSteps;
    [SerializeField][Range(0, 1)] float audStepsVol;

    [Header("-----Default Gun-----")]
    [SerializeField] gunStats defaultGun;

    /// <summary>
    /// Contain all Vector2 and Vector3 in this section to keep them organized
    /// </summary>
    Vector3 moveDir;
    Vector3 playerVel;

    /// <summary>
    /// Contain all private variables in this section that will determine controlling capabilities of the character to including future plans for shooting and sounds
    /// </summary>
    bool isSprinting;
    bool isShooting;
    bool isPlayingSteps;

    int jumpCount;
    int HPOriginal;
    int selectedGun;
    int shootDamage;
    int shootDist;

    float shootRate;

    void Start()
    {
        HPOriginal = HP;

        if (defaultGun != null && !gunList.Contains(defaultGun))  // Check if gunList doesn't already contain the default gun
        {
            // Add the default gun to the player's inventory (gunList)
            gunList.Add(defaultGun);
            selectedGun = 0;  // Set the selected gun to the default gun
            getGunStats(defaultGun);  // Set gun stats and model
        }
    }

    void Update()
    {
        ///Uncommment once we get gameManager implemented and remove the duplication in Update below///
        //if(!gameManager.instance.isPause)
        //{
        //    movement();
        //    selectGun();
        //    reload();
        //}
        
        
        
        
        movement();
        selectGun();
        reload();
        sprint();
    }

    /// <summary>
    /// Uncomment when the gameManger is implemented
    /// </summary>
    //public void spawnPlayer()
    //{
    //    controller.enabled = false;
    //    transform.position = gameManager.instance.playerSpawnPos.transform.position;
    //    controller.enabled = true;
    //    HP = HPOriginal;
    //    updatePlayerUI();
    //}

    void movement()
    {
        //Reset the jumpCount on the ground and reset the playerVelocity so gravity doesn't keep building
        if(controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        moveDir = (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"));
        controller.Move(moveDir * moveSpeed * Time.deltaTime);

        Jump();

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        
    }
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
        }
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            moveSpeed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            moveSpeed /= sprintMod;
            isSprinting = false;
        }
    }

    public void takeDamage (int amount)
    {
        HP -= amount;

        ////What will the game Manager do if you hit 0 HP
        //if(HP <= 0)
        //{

        //}
    }

    /// <summary>
    /// Uncomment to activate the playerUI update once gameManager is implemented
    /// </summary>
    /// <returns></returns>
    //public void updatedPlayerUI()
    //{
    //    gameManager.instance.playerHPBar.fillAmount = (float)HP / HPOriginal;
    //}

    public void getGunStats(gunStats gun)
    {
        if (!gunList.Contains(gun)) // Avoid adding the gun again if it's already in the list
        {
            gunList.Add(gun);
        }

       

        //Stats
        shootDamage = gun.shootDamage;
        shootDist = gun.shootDistance;
        shootRate = gun.shootRate;

        //Visual
        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    void selectGun()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunList.Count - 1)
        {
            selectedGun++;
            changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
        {
            selectedGun--;
            changeGun();
        }
    }

    void changeGun()
    {
        shootDamage = gunList[selectedGun].shootDamage;
        shootDist = gunList[selectedGun].shootDistance;
        shootRate = gunList[selectedGun].shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    void reload()
    {
        if(Input.GetButtonDown("Reload") && gunList.Count > 0)
        {
            gunList[selectedGun].ammoCur = gunList[selectedGun].ammoMax;
        }
    }

    IEnumerator playStep()
    {
        isPlayingSteps = true;
        aud.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);

        if (!isSprinting)
        {
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
        }
        isPlayingSteps = false;
    }
    IEnumerator shoot()
    {
        isShooting = true;
        gunList[selectedGun].ammoCur--;
        aud.PlayOneShot(gunList[selectedGun].shootSound[Random.Range(0, gunList[selectedGun].shootSound.Length)], gunList[selectedGun].shootVol);
        StartCoroutine(muzzleFlash());

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreMask))
        {
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
        }
        Instantiate(gunList[selectedGun].hitEffect, hit.point, Quaternion.identity);

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    IEnumerator muzzleFlash()
    {
        muzzleFlashLgt.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        muzzleFlashLgt.SetActive(false);
    }

    ///Uncomment when gameManager is implemented for flash damage on the player///
    //IEnumerator flashDamage()
    //{
    //    gameManager.instance.playerDamageScreen.SetActive(true);
    //    yield return new WaitForSeconds(0.1f);
    //    gameManager.instance.playerDamageScreen.SetActive(true);
    //}
}
