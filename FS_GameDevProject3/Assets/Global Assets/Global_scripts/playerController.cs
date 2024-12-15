using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerController : MonoBehaviour, IDamage
{
    [Header("-----Components-----")]
    [SerializeField] LayerMask _ignoreMask;
    [SerializeField] CharacterController _controller;

    [Header("-----Stats-----")]
    [SerializeField][Range(0, 100)] int _HP;
    [SerializeField][Range(1, 5)] float _moveSpeed;
    [SerializeField][Range(2, 5)] int _sprintMod;
    [SerializeField][Range(1, 3)] int _jumpMax;
    [SerializeField][Range(5, 20)] int _jumpSpeed;
    [SerializeField][Range(15, 40)] int _gravity;

    [Header("-----Crouch-----")]
    [SerializeField] float _crouchHeight = 0.5f; // Height of the character while crouching
    [SerializeField] float _normalHeight = 2.0f; // Normal height when standing
    [SerializeField] float _crouchSpeedModifier = 0.5f; // Speed modifier when crouching
    private bool _isCrouching = false; // Track crouch state

    [Header("-----Guns-----")]
    [SerializeField] List<gunStats> _gunList = new List<gunStats>();
    [SerializeField] GameObject _gunModel;
    [SerializeField] GameObject _muzzleFlashLgt;
    [SerializeField] Transform _muzzlePosition;

    [Header("-----Audio-----")]
    [SerializeField] AudioSource _aud;
    [SerializeField] AudioClip[] _audJump;
    [SerializeField] [Range(0, 1)] float _audJumpVol;
    [SerializeField] AudioClip[] _audHurt;
    [SerializeField][Range(0, 1)] float _audHurtVol;
    [SerializeField] AudioClip[] _audSteps;
    [SerializeField][Range(0, 1)] float _audStepsVol;
    [SerializeField] AudioClip[] _audHealthPickUp;
    [SerializeField][Range(0, 1)] float _audHealthPickUpVol;

    [Header("-----Default Gun-----")]
    [SerializeField] gunStats _defaultGun;

    /// <summary>
    /// Contain all Vector2 and Vector3 in this section to keep them organized
    /// </summary>
    Vector3 _moveDir;
    Vector3 _playerVel;

    /// <summary>
    /// Contain all private variables in this section that will determine controlling capabilities of the character to including future plans for shooting and sounds
    /// </summary>
    bool _isSprinting;
    bool _isShooting;
    bool _isPlayingSteps;
    bool hasPlayedEmptySound = false; // Flag to track if the empty sound has been played
    bool displayQuest;


    int _jumpCount;
    int _HPOriginal;
    int _HPMax = 100;
    public int _selectedGun;
    int _shootDamage;
    int _shootDist;

    float _shootRate;
    float _lastShotTime = 0f; // Time when the player last shot

    public int HP => _HP;
    public int HPMax => _HPMax;
    public int HPOriginal => _HPOriginal;
    public List<gunStats> GunList => _gunList;
    public gunStats SelectedGun => GunList[_selectedGun];

    public int ReserveAmmo => _gunList[_selectedGun].ammoRes;

    private GameManager _gameManager;
    public AudioClip reloadSound;
    public AudioClip emptySound;
    public AudioClip changeGunSound;

    private int _savedHP;
    private int _savedGunIndex;
    private int _savedAmmoCur;
    private int _savedAmmoRes;
    private GameObject _playerSpawn;




    void Awake()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Freeze rotation on all axes (prevents spinning)
            rb.freezeRotation = true; // This will prevent the player from rotating due to physics forces
        }
        _gameManager = FindAnyObjectByType<GameManager>();
        // Add the default gun to the player's inventory (gunList)
        _gunList.Add(_defaultGun);
        _selectedGun = _gunList.Count - 1;  // Set the selected gun to the default gun
        getGunStats(_defaultGun);  // Set gun stats and model
        
    }

    void Start()
    {
 
        if (_gunList[_selectedGun].ammoCur > _gunList[_selectedGun].ammoMax)
        {
            _gunList[_selectedGun].ammoCur = _gunList[_selectedGun].ammoMax;
        }




        _gameManager.UpdateUI();

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

        if (_gunList[_selectedGun].ammoRes < 0)
        {
            _gunList[_selectedGun].ammoRes = 0;
            _gameManager.UpdateUI();

        }


        movement();
      //  crouch();
        selectGun();
        reload();
        sprint();
    }

    public void SetHealth(int health)
    {
        _HP = health;
    }

    public void SetGun(int gunIndex)
    {
        _selectedGun = gunIndex;
        // Reload gun data
        getGunStats(_gunList[_selectedGun]);
    }

    public void SetAmmo(int ammoCur, int ammoRes)
    {
        _gunList[_selectedGun].ammoCur = ammoCur;
        _gunList[_selectedGun].ammoRes = ammoRes;
    }

    

    public void restoreHealth(int amount)
    {
        //Checks if the player has less than original amount
        if (_HP < _HPMax)
        {
            //Checks to see if the amount the player would pick up is greater than the original amount they started with/are allowed and then sets to max HP
            if (_HP + amount > _HPMax)
            {
                _HP = _HPMax;
            }
            //Otherwise adds the amount to the health
            else
            {
                _HP += amount;
            }
            _aud.PlayOneShot(_audHealthPickUp[Random.Range(0, _audHealthPickUp.Length)], _audHealthPickUpVol);

        }
        //If the player's health is already at max HP from the original it will return and do nothing.
        else
        {
            return;
        }
        
        _gameManager.UpdateUI();

       


    }
    public void returnAmmo(int amount, AmmoType ammoType)
    {
        foreach (var gun in _gunList)
        {
            // Check if the current gun uses the matching ammo type
            if (gun.ammoType == ammoType)
            {
                // Add ammo to the reserve of this gun
                gun.ammoRes += amount;

                

                // Update the UI with the new ammo count
                _gameManager.UpdateUI();

                Debug.Log($"Added {amount} ammo to {ammoType} reserves.");
                return; // Exit the method once the ammo is added to the matching gun
            }
        }
    }
    void movement()
    {
        //Reset the jumpCount on the ground and reset the playerVelocity so gravity doesn't keep building
        if(_controller.isGrounded)
        {
            _jumpCount = 0;
            _playerVel = Vector3.zero;
        }

        _moveDir = (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"));
        _controller.Move(_moveDir * _moveSpeed * Time.deltaTime);

      
        Jump();
       // crouch();

        _controller.Move(_playerVel * Time.deltaTime);
        _playerVel.y -= _gravity * Time.deltaTime;

        if (Input.GetButton("Fire1"))
        {
            // If the gun has ammo
            if (_gunList[_selectedGun].ammoCur > 0)
            {
                if (Time.time - _lastShotTime >= _shootRate && !_isShooting)
                {
                    StartCoroutine(shoot());
                }

                // Reset the empty sound flag if ammo is available
                hasPlayedEmptySound = false;
            }
            else
            {
                // Only play the empty sound once if it hasn't been played already
                if (!hasPlayedEmptySound)
                {
                    AudioSource.PlayClipAtPoint(emptySound, transform.position);
                    hasPlayedEmptySound = true; // Set the flag so it doesn't play again
                }
            }
        }
        else
        {
            // Reset the flag when the fire button is released
            hasPlayedEmptySound = false;
        }

    }
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && _jumpCount < _jumpMax)
        {
            _jumpCount++;
            _playerVel.y = _jumpSpeed;
        }
    }

   
    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            _moveSpeed *= _sprintMod;
            _isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            _moveSpeed /= _sprintMod;
            _isSprinting = false;
        }
    }

    //void crouch()
    //{
    //    // Toggle crouch when the player presses the crouch button (e.g., Left Control key)
    //    if (Input.GetKeyDown(KeyCode.LeftControl) && !_isCrouching)
    //    {
    //        // Start crouching
    //        _isCrouching = true;
    //        StartCoroutine(CrouchStand());
    //    }
    //    else if (Input.GetKeyDown(KeyCode.LeftControl) && _isCrouching)
    //    {
    //        // Stand up
    //        _isCrouching = false;
    //        StartCoroutine(CrouchStand());
    //    }
    //}

    //IEnumerator CrouchStand()
    //{
    //    // If crouching, gradually reduce the height
    //    if (_isCrouching)
    //    {
    //        // Smoothly transition to crouch height
    //        float timeElapsed = 0f;
    //        float targetHeight = _crouchHeight;
    //        float currentHeight = _controller.height;

    //        while (timeElapsed < 0.25f) // 0.25f is the smooth transition time
    //        {
    //            _controller.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / 0.25f);
    //            timeElapsed += Time.deltaTime;
    //            yield return null;
    //        }

    //        _controller.height = targetHeight; // Ensure final height is set
    //    }
    //    else
    //    {
    //        // If standing, gradually increase height to normal
    //        float timeElapsed = 0f;
    //        float targetHeight = _normalHeight;
    //        float currentHeight = _controller.height;

    //        // Check for obstruction (if the player is standing up into something)
    //        if (Physics.Raycast(transform.position, Vector3.up, 1f)) // Check if something is above the player
    //        {
    //            yield break; // If there's an obstruction, prevent standing
    //        }

    //        while (timeElapsed < 0.25f) // 0.25f is the smooth transition time
    //        {
    //            _controller.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / 0.25f);
    //            timeElapsed += Time.deltaTime;
    //            yield return null;
    //        }

    //        _controller.height = targetHeight; // Ensure final height is set
    //    }
    //}

    public void takeDamage(int amount)
    {
        _HP -= amount; // Deduct health

        _gameManager.UpdateUI(); // Update the UI to reflect the new health

        if (_HP <= 0) // Check if the player is dead
        {
            Death();
        }
    }

    public void Death()
    {
        // Save player data before respawn
        GameManager.Instance.SavePlayerData(this);
        _playerSpawn = GameObject.FindWithTag("PlayerSpawn");


       
        // Reload scene (you could also load a specific respawn scene if desired)
        GameManager.Instance.RespawnPlayer(_playerSpawn.transform);
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
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
        if (!_gunList.Contains(gun)) // Avoid adding the gun again if it's already in the list
        {
            _gunList.Add(gun);
        }

       

        //Stats
        _shootDamage = gun.shootDamage;
        _shootDist = gun.shootDistance;
        _shootRate = gun.shootRate;

        //Visual
        _gunModel.GetComponent<MeshFilter>().sharedMesh = gun.gunModel.GetComponent<MeshFilter>().sharedMesh;
        _gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.gunModel.GetComponent<MeshRenderer>().sharedMaterial;
     //   _gameManager.UpdateUI();

    }

    void selectGun()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0 && _selectedGun < _gunList.Count - 1)
        {
            _selectedGun++;
            changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && _selectedGun > 0)
        {
            _selectedGun--;
            changeGun();
        }
    }

    void changeGun()
    {

        AudioSource.PlayClipAtPoint(changeGunSound, transform.position);

        _shootDamage = _gunList[_selectedGun].shootDamage;
        _shootDist = _gunList[_selectedGun].shootDistance;
        _shootRate = _gunList[_selectedGun].shootRate;

        _gunModel.GetComponent<MeshFilter>().sharedMesh = _gunList[_selectedGun].gunModel.GetComponent<MeshFilter>().sharedMesh;
        _gunModel.GetComponent<MeshRenderer>().sharedMaterial = _gunList[_selectedGun].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
        _gameManager.UpdateUI();
    }

    void reload()
    {
        if(Input.GetButtonDown("Reload") && _gunList.Count > 0)
        {

            int currentRes = _gunList[_selectedGun].ammoRes;

            //Check to see if there is ammo in the reserves
            if (_gunList[_selectedGun].ammoRes != 0)
            {
                AudioSource.PlayClipAtPoint(reloadSound, transform.position);

                //Variable to hold the ammo currently in the gun and subtract that from the max so there is no wasted ammo
                int ammoToAdd = (_gunList[_selectedGun].ammoMax - _gunList[_selectedGun].ammoCur);
                Debug.Log(ammoToAdd);

                //Reload the gun to max ammo
                //Checks to see if the the current reserves are available but less than the max along with making sure the current ammo is not equal to or greater than the max.
                if (_gunList[_selectedGun].ammoCur < _gunList[_selectedGun].ammoMax)
                {
                    if (currentRes < _gunList[_selectedGun].ammoMax)
                    {
                        _gunList[_selectedGun].ammoCur += currentRes;
                        _gunList[_selectedGun].ammoRes -= currentRes;
                        _gameManager.UpdateUI();
                    }
                    else
                    {
                        _gunList[_selectedGun].ammoCur += ammoToAdd;
                        _gunList[_selectedGun].ammoRes -= ammoToAdd;
                        _gameManager.UpdateUI();
                    }
                }
            }
            if (_gunList[_selectedGun].ammoRes <= 0)
            {
                AudioSource.PlayClipAtPoint(emptySound, transform.position);

                _gunList[_selectedGun].ammoRes = 0;
                _gameManager.UpdateUI();

            }


        }
    }

    IEnumerator playStep()
    {
        _isPlayingSteps = true;
        _aud.PlayOneShot(_audSteps[Random.Range(0, _audSteps.Length)], _audStepsVol);

        if (!_isSprinting)
        {
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
        }
        _isPlayingSteps = false;
    }

    IEnumerator shoot()
    {
        _isShooting = true;
        _gunList[_selectedGun].ammoCur--;
        _gameManager.UpdateUI();
        _aud.PlayOneShot(_gunList[_selectedGun].shootSound[Random.Range(0, _gunList[_selectedGun].shootSound.Length)], _gunList[_selectedGun].shootVol);
        StartCoroutine(muzzleFlash());

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, _shootDist, ~_ignoreMask))
        {
            Debug.Log(hit.collider.name);
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(_shootDamage);
            }

        }
        Instantiate(_gunList[_selectedGun].hitEffect, hit.point, Quaternion.identity);

        yield return new WaitForSeconds(_shootRate);
        _isShooting = false;

    }
    //IEnumerator shoot()
    //{
    //    _isShooting = true;

    //    _gunList[_selectedGun].ammoCur--;
    //    _gameManager.UpdateUI();

    //    _aud.PlayOneShot(_gunList[_selectedGun].shootSound[Random.Range(0, _gunList[_selectedGun].shootSound.Length)], _gunList[_selectedGun].shootVol);
    //    StartCoroutine(muzzleFlash());

    //    GameObject bullet = Instantiate(_gunList[_selectedGun].bulletPrefab, _muzzlePosition.position, Camera.main.transform.rotation);
    //    Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
    //    if (bulletRb != null)
    //    {
    //        // Add forward force to the bullet to simulate its movement
    //        bulletRb.AddForce(Camera.main.transform.forward * _shootDist, ForceMode.VelocityChange);
    //    }

    //    RaycastHit hit;
    //    if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, _shootDist, ~_ignoreMask))
    //    {
    //        IDamage dmg = hit.collider.GetComponent<IDamage>();

    //        if (dmg != null)
    //        {
    //            dmg.takeDamage(_shootDamage);
    //        }
    //    }
    //    Instantiate(_gunList[_selectedGun].hitEffect, hit.point, Quaternion.identity);

    //   // Debug.Log(hit.collider.name);
    //   // Debug.Log("Time between shots: " + (Time.time - _lastShotTime));

    //    _lastShotTime = Time.time;

    //    yield return new WaitForSeconds(_shootRate);

        
    //    _isShooting = false;
    //}

    IEnumerator muzzleFlash()
    {
        _muzzleFlashLgt.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        _muzzleFlashLgt.SetActive(false);
    }

    ///Uncomment when gameManager is implemented for flash damage on the player///
    //IEnumerator flashDamage()
    //{
    //    gameManager.instance.playerDamageScreen.SetActive(true);
    //    yield return new WaitForSeconds(0.1f);
    //    gameManager.instance.playerDamageScreen.SetActive(true);
    //}
}
