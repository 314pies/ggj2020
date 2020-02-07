using UnityEngine;
using UnityEngine.Assertions;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace CliffLeeCL
{
    /// <summary>
    /// The class control how the player interact with other objects in the game.
    /// </summary>
    public class PlayerInteraction : MonoBehaviour
    {
        public AudioClip impact;
        AudioSource audioSource;
        PlayerAgent playerAgent { get { return GetComponent<PlayerAgent>(); } }
        [ShowInInspector]
        public BoltEntity CurrentCarryingItem
        {
            get
            {
                if (playerAgent.entity.IsAttached)
                    return playerAgent.state.CarryingItem;
                else
                    return null;
            }
        }
        public KeyCode UseKey = KeyCode.Space;
        public float PickingRadius = 1.0f;
        public LayerMask interactLayerMask;
        public Vector2 CarryingPosition = new Vector2(0, 3);
        public GameObject Arrow;

        /// <summary>
        /// Start is called once on the frame when a script is enabled.
        /// </summary>
        void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }


        [Button]
        void ClientSendHoldItemReq(BoltEntity item)
        {
            var pickReq = PlayerPickItemReq.Create(Bolt.GlobalTargets.OnlyServer);
            pickReq.Player = playerAgent.entity;
            pickReq.Item = item;
            pickReq.Send();
        }

        [Button]
        public void ServerSetCarryingItem(BoltEntity itemFound)
        {
            if (playerAgent.state.CarryingItem != null) { return; }
            var itemAgent = itemFound.GetComponent<ItemAgent>();
            if (itemAgent == null) { return; }
            if (itemAgent.state.Holder.HoldBy != null) { return; }
            itemAgent.ServerSetHolder(playerAgent.entity, CarryingPosition);
            playerAgent.state.CarryingItem = itemFound;
        }

        //audioSource.PlayOneShot(impact, 0.7F);

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            // Pick up / drop items
            if (Input.GetKeyDown(UseKey))
            {
                if (CurrentCarryingItem == null)
                {
                    var item = GetItemInRange(PickingRadius);
                    Debug.Log("GetItemInRange result: " + item, item);
                    if (item != null)
                        ClientSendHoldItemReq(item.GetComponent<BoltEntity>());
                }
                if (CurrentCarryingItem != null)
                {
                    if (CurrentCarryingItem.GetComponent<ItemAgent>().itemState == ItemStateEnum.New)
                    {
                        //Ready to launch
                        Arrow.SetActive(true);
                    }
                }
            }
            if (Input.GetKeyUp(UseKey))
            {
                if (Arrow.activeSelf)
                {
                    //Launch item
                    ClientSentLaunchCarryingItemReq();
                    audioSource.PlayOneShot(impact);
                    Arrow.SetActive(false);
                }
            }
        }

        class ItemInRangePriorityComparer : IComparer<Collider2D>
        {
            public Vector3 center;
            public ItemInRangePriorityComparer(Vector3 center)
            {
                this.center = center;
            }
            public int Compare(Collider2D x, Collider2D y)
            {
                var xDistance = Vector3.Distance(center, x.transform.position);
                var yDistance = Vector3.Distance(center, y.transform.position);
                if (xDistance > yDistance)
                    return 1;
                else if (xDistance < yDistance)
                    return -1;
                else
                    return 0;
            }
        }
        public GameObject GetItemInRange(float radius)
        {
            ContactFilter2D filter = new ContactFilter2D();
            filter.layerMask = interactLayerMask;
            List<Collider2D> results = new List<Collider2D>();
            int count = Physics2D.OverlapCircle(transform.position, radius, filter, results);
            ItemInRangePriorityComparer comparer = new ItemInRangePriorityComparer(transform.position);
            results.Sort(comparer);
            //for (int i = 0; i < count; i++) { Debug.Log(i + "st itemFound: " + results[i], results[i]); }

            for (int i = 0; i < count; i++)
            {
                if (results[i].GetComponent<ItemAgent>() == null) { continue; }
                if (results[i].GetComponent<BoltEntity>() == null) { continue; }
                if (results[i].GetComponent<ItemAgent>().state.Holder.HoldBy == null)
                {
                    return results[i].gameObject;
                }
            }
            return null;
        }

        public GameObject CurrentLaunchItem;

        public float LaunchForce = 10.0f;
        [Button]
        public void ClientSentLaunchCarryingItemReq()
        {
            var launchReq = LaunchCarryingItem.Create(Bolt.GlobalTargets.OnlyServer);
            launchReq.Player = playerAgent.entity;
            launchReq.ArrowRotation = Arrow.transform.localRotation;
            launchReq.Send();
        }
        [Button]
        public void ServerLaunchCarryingItem(Quaternion arrowRotation)
        {
            if (CurrentCarryingItem != null)
            {
                Arrow.transform.localRotation = arrowRotation;
                CurrentCarryingItem.GetComponent<ItemAgent>().ServerSetHolder(null, Vector3.zero);
                var itemToLaunch = playerAgent.state.CarryingItem;
                playerAgent.state.CarryingItem = null;
                LauchItem(itemToLaunch);
            }
        }

        void LauchItem(GameObject itemToLaunch)
        {
            Vector2 dir = new Vector2(Arrow.transform.up.x, Arrow.transform.up.y);
            if (itemToLaunch.GetComponent<Rigidbody2D>())
            {
                itemToLaunch.transform.parent = null;
                itemToLaunch.GetComponent<Rigidbody2D>().isKinematic = false;
                itemToLaunch.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

                itemToLaunch.GetComponent<Rigidbody2D>().AddForce(dir * LaunchForce, ForceMode2D.Impulse);
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
