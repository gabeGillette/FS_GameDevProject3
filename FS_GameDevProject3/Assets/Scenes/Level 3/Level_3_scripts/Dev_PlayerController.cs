using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace gabe_indev
{

    public class PlayerController : MonoBehaviour
    {

        /*-------------------------------------------------------- SERIALIZED FIELDS */
        [SerializeField] float _speed;

        [SerializeField] float _sprintMod;

        [SerializeField][Range(1, 5)] int _jumpMax;

        [SerializeField][Range(1, 20)] float _jumpSpeed;

        [SerializeField] float _gravity;

        [SerializeField] float _shootRate;

        [SerializeField] GameObject _feetCollider;


        /*---------------------------------------------------- PRIVATE CLASS MEMBERS */

        Vector3 _movDir;
        Vector3 _playerVelocity;
        bool _isSprinting;
        bool _isJumping;
        int _jumpCount;

        CharacterController _controller;
        PlayerFeet _feet;


        /*----------------------------------------------------- PUBLIC CLASS METHODS */

        void Start()
        {
            _controller = GetComponent<CharacterController>();
            _feet = _feetCollider.GetComponent<PlayerFeet>();
        }


        void Update()
        {
            // update player movement.
            Movement();
            Sprint();

        }


        void Sprint()
        {
            if (Input.GetButtonDown("Sprint"))
            {
                _speed *= _sprintMod;
                _isSprinting = true;
            }
            else if (Input.GetButtonUp("Sprint"))
            {
                _speed /= _sprintMod;
                _isSprinting = false;
            }
        }

        void Movement()
        {
            //if the player is on the ground then reset some values.
            if (_controller.isGrounded)
            {
                _jumpCount = 0;
                _playerVelocity.y = 0;
                if (_feet.IsColliding)
                {
                    transform.SetParent(_feet.What.transform, true);


                }

            }
            else
            {
                if (!_feet.IsColliding)
                {
                    transform.SetParent(null, true);
                }
            }


            // Determine a direction vector based on Input axis and 
            // update the player controller in the direction scaled by spped/deltatime
            _movDir = ((transform.forward * Input.GetAxis("Vertical")) +
              (transform.right * Input.GetAxis("Horizontal")));

            _controller.Move(_movDir * _speed * Time.deltaTime);

            // Check if the player is jumping and jump the player if so.
            Jump();

            // move the player's controller by playerVelocity
            _controller.Move(_playerVelocity * Time.deltaTime);

            // Decrease the y axis of the velocity by gravity scaled by deltatime
            _playerVelocity.y -= _gravity * Time.deltaTime;

        }

        void Jump()
        {
            if (Input.GetButtonDown("Jump") && _jumpCount < _jumpMax)
            {
                _jumpCount++;
                _playerVelocity.y = _jumpSpeed;
            }
        }

    }
}

