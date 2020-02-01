using UnityEngine;
using UnityEngine.Assertions;

namespace CliffLeeCL
{
    /// <summary>
    /// The class control how the player interact with other objects in the game.
    /// </summary>
    public class PlayerInteraction : MonoBehaviour
    { 
    
        /// <summary>
        /// Is used to retrieve current collision situation.
        /// </summary>
        PlayerCollision collision;

        /// <summary>
        /// Start is called once on the frame when a script is enabled.
        /// </summary>
        void Start()
        {
            collision = GetComponent<PlayerCollision>();
            Assert.IsTrue(collision, "Need \"PlayerCollision\" component on this gameObject");
        }

        [Header("General")]
        public KeyCode UseKey = KeyCode.Space;
        [Header("Pick Item")]
        public float PickingRadius = 1.0f;
        public LayerMask SearchForItemLayerMask;
        public GameObject CurrendHoldingItem;
        public Vector2 HoldingPosition;

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            // Pick up / drop items
            if (Input.GetKeyDown(UseKey))
            {
                if(CurrentLaunchItem!=null) //Launch item
                {
                    LauchItem(ref CurrentLaunchItem);
                }else if (CurrendHoldingItem == null)//Pick item
                {
                    var itemFound = SearchForItemInRange(PickingRadius);
                    CurrendHoldingItem = itemFound;

                    if(CurrendHoldingItem != null)
                        SetCarryingItem(CurrendHoldingItem,HoldingPosition);
                }
            }
        }


        public GameObject SearchForItemInRange(float radius)
        {
            var _collider = Physics2D.OverlapCircle(transform.position, radius, SearchForItemLayerMask);
            if (_collider == null)
                return null;
            else
                return _collider.gameObject;
        }

        public void SetCarryingItem(GameObject obj,Vector2 carryingPosition)
        {
            obj.transform.parent = transform;
            obj.transform.localPosition = carryingPosition;
            obj.GetComponent<Rigidbody2D>().isKinematic = true;
        }

        public void RemoveHoldingItem()
        {
            Destroy(CurrendHoldingItem);
            CurrendHoldingItem = null;
        }

        [Header("Launch Item")]
        public GameObject CurrentLaunchItem;
        public void SetLaunchingItem(GameObject objectToLaunch)
        {
            if (CurrentLaunchItem != null) {
                //Clean up current obj
                //Test
            }

            CurrentLaunchItem = objectToLaunch;
            SetCarryingItem(CurrentLaunchItem,LaunchReadyPosition);
        }

        public float LaunchForce = 10.0f;
        public Vector2 LaunchReadyPosition;
        public void LauchItem(ref GameObject itemToLaunch)
        {
            if (itemToLaunch.GetComponent<Rigidbody2D>())
            {
                itemToLaunch.transform.parent = null;
                itemToLaunch.GetComponent<Rigidbody2D>().isKinematic = false;
                itemToLaunch.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

                itemToLaunch.GetComponent<Rigidbody2D>().AddForce(Vector2.up * LaunchForce,ForceMode2D.Impulse);
                itemToLaunch = null;
            }
        }

        void OnDrawGizmosSelected()
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, PickingRadius);
        }
    }
}
