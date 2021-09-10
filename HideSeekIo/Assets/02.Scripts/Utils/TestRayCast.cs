using System.Collections;

using UnityEngine;

namespace Assets._02.Scripts.Utils
{
    public class TestRayCast : MonoBehaviour
    {
        public int rayCastCount;
        public float angle;

        private void Update()
        {
            var angleInterval = angle / rayCastCount;

            Vector3 direction = this.transform.forward;
            for(int i =  -rayCastCount/2 ; i < rayCastCount/2 +1; i++)
            {
                RaycastHit hit;
                Quaternion quaternion = Quaternion.Euler(0, angleInterval * i , 0);
                var newDirection = quaternion * direction;

                //Physics.Raycast(this.transform.position ,Vector3.zero, out hit , 10){

                //}

                Debug.DrawRay(this.transform.position, newDirection, Color.green);

            }


        }
    }
}