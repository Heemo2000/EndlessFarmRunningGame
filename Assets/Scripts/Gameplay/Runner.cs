using System;
using UnityEngine;

namespace Game.Gameplay
{
    public class Runner : MonoBehaviour
    {
        [Header("Movement Settings: ")]
        [Min(0.1f)]
        [SerializeField] private float forwardSpeed = 10.0f;
        [Min(0.1f)]
        [SerializeField] private float swipeSpeed = 10.0f;
        [Min(0.1f)]
        [SerializeField] private float forwardPickupSpeed = 20.0f;
        [Min(0.1f)]
        [SerializeField] private float laneWidth = 10.0f;

        [Header("Jump Settings: ")]
        [Min(0.1f)]
        [SerializeField] private float jumpHeight = 5.0f;
        [Min(0.1f)]
        [SerializeField] private float gravity = 5.0f;
        [SerializeField] private LayerMask groundLayerMask;
        [SerializeField] private Transform groundCheck;
        [Min(0.03f)]
        [SerializeField] private float groundCheckRadius = 0.2f;
        [Min(0.1f)]
        [SerializeField] private float belowCheckDistance = 4.0f;

        private Rigidbody runnerRB;
        private int currentLaneIndex = -1;
        private float targetX = 0.0f;
        private float startX = 0.0f;
        //private float previousX = 0.0f;
        private float lastKnownForwardSpeed = 0.0f;
        private float currentForwardSpeed = 0.0f;
        private Vector3 currentPosition = Vector3.zero;
        private bool shouldMove = true;
        private bool isGrounded = false;
        private float velocityY = 0.0f;
        private float currentVelocityY = 0.0f;
        private float newVelocityY = 0.0f;
        private float averageVelocityY = 0.0f;

        public bool ShouldMove { get => shouldMove; set => shouldMove = value; }


        public void MoveBack(float distance)
        {
            if(shouldMove)
            {
                currentPosition.z -= distance;
            }
        }

        public void Swipe(int directionX)
        {
            //if (previousX != currentPosition.x)
            //{
            //    return;
            //}
            //Debug.Log("DirectionX: " + directionX);

            int newLaneIndex = currentLaneIndex + directionX;

            if (newLaneIndex >= -1 && newLaneIndex <= 1)
            {
                currentLaneIndex = newLaneIndex;
                Debug.Log("Current Lane Index: " +  currentLaneIndex);
                targetX = startX + (currentLaneIndex * laneWidth);
            }
        }

        private void Awake()
        {
            runnerRB = GetComponent<Rigidbody>();
            startX = transform.position.x;

            currentLaneIndex = 0;
            targetX = transform.position.x;
            currentForwardSpeed = 0.0f;
            currentPosition = transform.position;
            runnerRB.isKinematic = true;
        }

        private void Update()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayerMask.value);

            //We will compute gravity using verlet integration.
            if (isGrounded)
            {
                velocityY = 0.0f;
            }
            else
            {
                currentVelocityY = velocityY;
                newVelocityY = currentVelocityY - gravity * Time.deltaTime;
                averageVelocityY = (currentVelocityY + newVelocityY) / 2.0f;
                velocityY = averageVelocityY;
            }
        }

        void FixedUpdate()
        {
            

            //If movement allowed only then move
            if (shouldMove) 
            {
                //The below condition is done to allow seamless pausing and un-pausing.
                if (currentForwardSpeed == 0.0f) 
                {
                    currentForwardSpeed = lastKnownForwardSpeed;
                }
                currentForwardSpeed = Mathf.MoveTowards(currentForwardSpeed, 
                                                        forwardSpeed, 
                                                        forwardPickupSpeed * Time.fixedDeltaTime);
            }
            else
            {
                lastKnownForwardSpeed = currentForwardSpeed;
                currentForwardSpeed = 0.0f;
            }

            //Allow movement in Z-axis.
            currentPosition.z += currentForwardSpeed * Time.fixedDeltaTime;

            //And X-axis too.
            currentPosition.x = Mathf.Lerp(currentPosition.x, targetX, swipeSpeed * Time.fixedDeltaTime);
            currentPosition.y = transform.position.y + velocityY * Time.fixedDeltaTime;
            runnerRB.MovePosition(currentPosition);
        }

        private void OnDrawGizmosSelected()
        {
            //For showing ground check radius.
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, groundCheckRadius);

            
            //For showing lane width.
            Gizmos.color = Color.magenta;
            Vector3 drawPosition = Vector3.zero;
            if(Application.isPlaying)
            {
                drawPosition = new Vector3(startX, transform.position.y, transform.position.z); ;
            }
            else
            {
                drawPosition = transform.position;
            }

            Gizmos.DrawLine(drawPosition - Vector3.right * laneWidth, drawPosition + Vector3.right * laneWidth);

            //For showing below check distance for ground checking
            Gizmos.color = Color.black;
            Gizmos.DrawLine(drawPosition, drawPosition - Vector3.up * belowCheckDistance);
        }
    }
}
