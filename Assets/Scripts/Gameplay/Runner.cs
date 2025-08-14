using System;
using UnityEngine;

namespace Game.Gameplay
{
    public class Runner : MonoBehaviour
    {
        [Min(0.1f)]
        [SerializeField] private float forwardSpeed = 10.0f;
        [Min(0.1f)]
        [SerializeField] private float swipeSpeed = 10.0f;
        [Min(0.1f)]
        [SerializeField] private float forwardPickupSpeed = 20.0f;
        [Min(0.1f)]
        [SerializeField] private float laneWidth = 10.0f;
        
        
        private Rigidbody runnerRB;
        private int currentLaneIndex = -1;
        private float targetX = 0.0f;
        private float startX = 0.0f;
        //private float previousX = 0.0f;
        private float lastKnownForwardSpeed = 0.0f;
        private float currentForwardSpeed = 0.0f;
        private Vector3 currentPosition = Vector3.zero;
        private bool shouldMove = true;
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

        //private void Update()
        //{
        //    previousX = currentPosition.x;
        //}

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
            
            runnerRB.MovePosition(currentPosition);
        }

        
    }
}
