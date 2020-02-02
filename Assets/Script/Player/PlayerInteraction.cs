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

        public KeyCode UseKey = KeyCode.Space;
        public float PickingRadius = 1.0f;
        public LayerMask interactLayerMask;
        public GameObject CurrendHoldingItem;
        public Vector2 HoldingPosition;
        public GameObject Arrow;


        void ClientSendHoldItemReq(BoltEntity item)
        {
            var pickReq = PlayerPickDropItemReq.Create(Bolt.GlobalTargets.OnlyServer);
            pickReq.Player = GetComponent<BoltEntity>();
            pickReq.DroppingItem = item;

            pickReq.Send();
        }

        public void ServerSetHoldingItem(GameObject itemFound)
        {
            CurrendHoldingItem = itemFound;
            OnItemPicked(CurrendHoldingItem);
        }


        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            // Pick up / drop items
            if (Input.GetKeyDown(UseKey))
            {
                if (CurrentLaunchItem != null) //Launch item
                {
                    Arrow.SetActive(true);
                    // LauchItem(ref CurrentLaunchItem);
                }
                else { 
                   
                    var itemFound = ItemInRange(PickingRadius);
                    if(itemFound != null)
                    {
                        if (itemFound.GetComponent<ItemBase>() != null)
                        {
                            ItemStateEnum itemState = itemFound.GetComponent<ItemBase>().itemSetting.ItemState;
                            if(itemState == ItemStateEnum.Garbage)
                            {
                                if(CurrendHoldingItem == null)
                                {
                                    Debug.Log("Sending Req");
                                    ClientSendHoldItemReq(itemFound.GetComponent<BoltEntity>());
                                }
                               
                            }
                            else if (itemState == ItemStateEnum.New)
                            {
                                if (CurrentLaunchItem == null)
                                {
                                    SetLaunchingItem(itemFound);
                                    Arrow.SetActive(true);
                                }
                            }
                        }
                    }
                }
            }
            if (Input.GetKeyUp(UseKey))
            {
                if (CurrentLaunchItem != null) //Launch item
                {
                    LauchItem(ref CurrentLaunchItem);
                }

            }
        }


        public GameObject ItemInRange(float radius)
        {
            var _collider = Physics2D.OverlapCircle(transform.position, radius, interactLayerMask);
            if (_collider == null)
                return null;
            else
                return _collider.gameObject;
        }

        public void OnItemPicked(GameObject obj)
        {
            obj.transform.parent = transform;
            obj.transform.localPosition = HoldingPosition;
            obj.GetComponent<Rigidbody2D>().isKinematic = true;
        }

        public void RemoveHoldingItem()
        {
            Destroy(CurrendHoldingItem);
            CurrendHoldingItem = null;
        }

        public GameObject CurrentLaunchItem;
        public void SetLaunchingItem(GameObject objectToLaunch)
        {
            if (CurrentLaunchItem != null) {
                //Clean up current obj
            }
            CurrentLaunchItem = objectToLaunch;
            OnItemPicked(CurrentLaunchItem);
        }

        public float LaunchForce = 10.0f;

        public void LauchItem(ref GameObject itemToLaunch)
        {
            Vector2 dir = new Vector2(Arrow.transform.up.x, Arrow.transform.up.y);
            if (itemToLaunch.GetComponent<Rigidbody2D>())
            {

                itemToLaunch.transform.parent = null;
                itemToLaunch.GetComponent<Rigidbody2D>().isKinematic = false;
                itemToLaunch.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

                itemToLaunch.GetComponent<Rigidbody2D>().AddForce(dir * LaunchForce,ForceMode2D.Impulse);
                itemToLaunch = null;
            }
            Arrow.SetActive(false);
        }

        void OnDrawGizmosSelected()
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, PickingRadius);
        }
    }
}
