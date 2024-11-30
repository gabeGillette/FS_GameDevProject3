using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [Header("-----Components-----")]
    [SerializeField] LayerMask _ignoreMask;
    [SerializeField] CharacterController _controller;

    [Header("-----Stats-----")]
    [SerializeField][Range(0, 100)] int _HP;
    [SerializeField][Range(1, 5)] int _moveSpeed;
    [SerializeField][Range(2, 5)] int _sprintMod;
    [SerializeField][Range(1, 3)] int _jumpMax;
    [SerializeField][Range(5, 20)] int _jumpSpeed;
    [SerializeField][Range(15, 40)] int _gravity;

    [Header("-----Guns-----")]
    [SerializeField] List<gunStats> _gunList = new List<gunStats>();
    [SerializeField] GameObject _gunModel;
    [SerializeField] GameObject _muzzleFlashLgt;

    [Header("-----Audio-----")]
    [SerializeField] AudioSource _aud;
    [SerializeField] AudioClip[] _audJump;
    [SerializeField] [Range(0, 1)] float _audJumpVol;
    [SerializeField] AudioClip[] _audHurt;
    [SerializeField][Range(0, 1)] float _audHurtVol;
    [SerializeField] AudioClip[] _audSteps;
    [SerializeField][Range(0, 1)] float _audStepsVol;

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

    int _jumpCount;
    int _HPOriginal;
    int _selectedGun;
    int _shootDamage;
    int _shootDist;

    float _shootRate;
    float _lastShotTime = 0f; // Time when the player last shot

    public int HP => _HP;
    public int HPOriginal => _HPOriginal;
    public List<gunStats> GunList => _gunList;
    public gunStats SelectedGun => GunList[_selectedGun];

    private GameManager _gameManager;

    void Start()
    {
        _gameManager = FindAnyObjectByType<GameManager>();

        _HPOriginal = _HP;

        if (_defaultGun != null && !_gunList.Contains(_defaultGun))  // Check if gunList doesn't already contain the default gun
        {
            // Add the default gun to the player's inventory (gunList)
            _gunList.Add(_defaultGun);
            _selectedGun = 0;  // Set the selected gun to the default gun
            getGunStats(_defaultGun);  // Set gun stats and model
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
        if(_controller.isGrounded)
        {
            _jumpCount = 0;
            _playerVel = Vector3.zero;
        }

        _moveDir = (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"));
        _controller.Move(_moveDir * _moveSpeed * Time.deltaTime);

        Jump();

        _controller.Move(_playerVel * Time.deltaTime);
        _playerVel.y -= _gravity * Time.deltaTime;

        if (Input.GetButton("Fire1")  && _gunList[_selectedGun].ammoCur > 0)
        {
            if (Time.time - _lastShotTime >= _shootRate && !_isShooting)
            {
                StartCoroutine(shoot());
            }
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

    public void takeDamage (int amount)
    {
        _HP -= amount;

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
        _shootDamage = _gunList[_selectedGun].shootDamage;
        _shootDist = _gunList[_selectedGun].shootDistance;
        _shootRate = _gunList[_selectedGun].shootRate;

        _gunModel.GetComponent<MeshFilter>().sharedMesh = _gunList[_selectedGun].gunModel.GetComponent<MeshFilter>().sharedMesh;
        _gunModel.GetComponent<MeshRenderer>().sharedMaterial = _gunList[_selectedGun].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    void reload()
    {
        if(Input.GetButtonDown("Reload") && _gunList.Count > 0)
        {
            _gunList[_selectedGun].ammoCur = _gunList[_selectedGun].ammoMax;
            _gameManager.UpdateUI();

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
       // _aud.PlayOneShot(_gunList[_selectedGun].shootSound[Random.Range(0, _gunList[_selectedGun].shootSound.Length)], _gunList[_selectedGun].shootVol);
        StartCoroutine(muzzleFlash());

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, _shootDist, ~_ignoreMask))
        {
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(_shootDamage);
            }
        }
        Instantiate(_gunList[_selectedGun].hitEffect, hit.point, Quaternion.identity);

        Debug.Log("Time between shots: " + (Time.time - _lastShotTime));

        _lastShotTime = Time.time;

        yield return new WaitForSeconds(_shootRate);

        _gameManager.UpdateUI();        
        
        _isShooting = false;
    }

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
