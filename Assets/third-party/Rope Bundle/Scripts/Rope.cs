using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// TODO: Exert force on pinned objects?
// If rope segment between two pinned nodes is stretched too much, a pinned node 
// that is marked dynamic vs static maybe can be pulled.

namespace Kotorman
{
    namespace Rope
    {
        [RequireComponent(typeof(LineRenderer))]
        public class Rope : MonoBehaviour
        {
            LineRenderer LineRenderer;
            Vector3[] DrawPositions; //draw positions
            List<RopeNode> RopeNodes = new List<RopeNode>(); //nodes positions


            [SerializeField] GameObject ropeNode;

            // Deprecated
            [SerializeField] Transform attachTo;
            [SerializeField] Transform endAttachTo;

            [SerializeField] int _collisionLayers;

            public float NodeDistance
            {
                get => _nodeDistance;
                set => _nodeDistance = value;
            }
            [SerializeField] float _nodeDistance = 0.2f;

            [Range(0f, 0.9f)]
            [SerializeField] float _correction = 0.5f;

            [SerializeField] int _totalNodes = 50;

            public bool NodesVisible => _nodesVisible;
            [SerializeField] bool _nodesVisible = true;


            public bool Collision
            {
                get => _collision;
                set => _collision = value;
            }
            [SerializeField] bool _collision = true;

            public int SimulationIterations
            {
                get => _simulationIterations;
                set => _simulationIterations = value;
            }
            [SerializeField] int _simulationIterations = 80;


            public bool Gravity
            {
                get => _gravity;
                set => _gravity = value;
            }
            [SerializeField] bool _gravity = false;

            public Vector3 GravityForce
            {
                get => _gravityForce;
                set => _gravityForce = value;
            }
            [SerializeField] Vector3 _gravityForce = new Vector2(0f, -1f);
            [SerializeField] bool _rotateNodes = false;

            public float MaxRopeSize
            {
                get {
                    return RopeNodes.Count * NodeDistance;
                }
            }
            public float Stretch 
            {
                get {
                    return GetRopeSize() / MaxRopeSize;
                }
            }

            //int layerMask = 1;
            ContactFilter2D ContactFilter;
            RaycastHit2D[] RaycastHitBuffer = new RaycastHit2D[10];
            Collider2D[] ColliderHitBuffer = new Collider2D[10]; 


            void Awake()
            {
                ContactFilter = new ContactFilter2D
                {
                    layerMask = _collisionLayers,
                    useTriggers = false,
                };

                bool tmp = _gravity;
                _gravity = false;

                Clear();
                Generate();

                SetNodesVisible(_nodesVisible);

                _gravity = tmp;
            }


            //----INIT----//
            public void Generate()
            {
                LineRenderer = this.GetComponent<LineRenderer>();

                Vector3 startPosition = Vector3.zero;
                if (attachTo != null) {
                    startPosition = attachTo.position;
                }
                
                for (int i = 0; i < _totalNodes; i++)
                {
                    Debug.Log(startPosition);
                    AddNodeAt(startPosition, false); //update it once in the end
                    startPosition.y -= _nodeDistance;
                }

                if (attachTo != null) {
                    startPosition = attachTo.position;
                    PinNodeTo(GetFirstNode(), attachTo);
                }

                if (endAttachTo != null) {
                    // PinNodeTo(GetLastNode(), endAttachTo);
                }

                //for Line Renderer Data
                DrawPositions = new Vector3[RopeNodes.Count];
                DrawRope();
            }

            private void ClearLocalNodes(bool editor = false)
            {
                foreach (Transform t in this.transform)
                {
                    if (editor)
                        DestroyImmediate(t.gameObject);
                    else
                    {
                        if (t != null)
                            Destroy(t.gameObject);
                    }
                }

                if (editor)
                {
                    foreach (var t in RopeNodes)
                    {
                        if (t != null)
                        {
                            DestroyImmediate(t.gameObject);
                        }
                    }

                    RopeNodes.Clear();
                }
            }


            void Update()
            {
                DrawRope();
            }

            private void FixedUpdate()
            {
                if (_gravity)
                    Simulate();

                // More iterations - more stable simulation
                for (int i = 0; i < _simulationIterations; i++)
                {
                    ApplyConstraint();

                    if (_collision)
                    {
                        if (i % 2 == 1)
                            AdjustCollisions();
                    }
                }
            }

            private void Simulate()
            {
                Debug.Log("Simulate");
                // step each node in rope
                for (int i = 0; i < RopeNodes.Count; i++)
                {
                    // derive the velocity from previous frame
                    Vector3 velocity = RopeNodes[i].transform.position - RopeNodes[i].PreviousPosition;
                    RopeNodes[i].PreviousPosition = RopeNodes[i].transform.position;

                    Vector3 newPos = RopeNodes[i].transform.position + velocity;
                    newPos += GravityForce * RopeNodes[i].Weight * Time.fixedDeltaTime;
                    Vector3 direction = RopeNodes[i].transform.position - newPos;

                    //rotate each node
                    if (_rotateNodes)
                    {
                        if (i > 0)
                        {
                            Transform previous = RopeNodes[i - 1].transform;
                            Transform current = RopeNodes[i].transform;

                            Vector2 dir = current.position - previous.position;
                            dir.Normalize();

                            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                            // transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                            //current.rotation = Quaternion.Euler(0, 0, angle+90);
                            //or
                            float offset = 90;
                            Quaternion q = Quaternion.AngleAxis(angle + offset, Vector3.forward);
                            current.rotation = q;
                            //current.rotation = Quaternion.Lerp(transform.rotation, q, 1 * Time.fixedDeltaTime);

                        }
                    }


                    int result = -1;
                    result = Physics2D.CircleCast(RopeNodes[i].transform.position, RopeNodes[i].transform.localScale.x / 2f, -direction.normalized, ContactFilter, RaycastHitBuffer, direction.magnitude);

                    if (result > 0)
                    {
                        for (int n = 0; n < result; n++)
                        {
                            if (RaycastHitBuffer[n].collider.gameObject.layer == -1)
                            {
                                Vector2 collidercenter = new Vector2(RaycastHitBuffer[n].collider.transform.position.x, RaycastHitBuffer[n].collider.transform.position.y);
                                Vector2 collisionDirection = RaycastHitBuffer[n].point - collidercenter;
                                // adjusts the position based on a circle collider
                                Vector2 hitPos = collidercenter + collisionDirection.normalized * (RaycastHitBuffer[n].collider.transform.localScale.x / 2f + RopeNodes[i].transform.localScale.x / 2f);
                                //Vector2 hitPos = collidercenter + collisionDirection.normalized * (RaycastHitBuffer[n].collider.transform.localScale.x / 2f + RopeNodes[i].transform.localScale.x / 2f);
                                newPos = hitPos;


                                break;              //Just assuming a single collision to simplify the model
                            }
                        }
                    }

                    RopeNodes[i].transform.position = newPos;
                }
            }

            private void AdjustCollisions()
            {
                // Loop rope nodes and check if currently colliding
                for (int i = 0; i < RopeNodes.Count - 1; i++)
                {
                    RopeNode node = this.RopeNodes[i];

                    int result = -1;
                    //result = Physics2D.OverlapCircle(node.transform.position, node.transform.localScale.x / 2f);
                    result = Physics2D.OverlapCircleNonAlloc(node.transform.position, node.transform.localScale.x / 2f, ColliderHitBuffer);
                    //result = Physics2D.OverlapBoxNonAlloc(node.transform.position, new Vector2(node.transform.localScale.x / 2f, node.transform.localScale.y / 2f), 0, ColliderHitBuffer);


                    if (result > 0)
                    {
                        for (int n = 0; n < result; n++)
                        {
                            //if (CollisionAllowed(RaycastHitBuffer[n].collider.gameObject.layer)) 
                            if (CollisionAllowed(ColliderHitBuffer[n].gameObject.layer))
                            //if (ColliderHitBuffer[n].gameObject.layer == LayerMask.NameToLayer("Default"))
                            //if (ColliderHitBuffer[n].gameObject.layer != 8)
                            {
                                // Adjust the rope node position to be outside collision
                                Vector3 collidercenter = ColliderHitBuffer[n].transform.position;
                                Vector3 collisionDirection = node.transform.position - collidercenter;

                                Vector3 hitPos = collidercenter + collisionDirection.normalized * ((ColliderHitBuffer[n].transform.localScale.x / 2f) + (node.transform.localScale.x / 2f));
                                node.transform.position = hitPos;
                                break;
                            }
                        }
                    }
                }
            }

            private void ApplyConstraint()
            {
                // Apply pinned position if needed
                for (int i = 0; i < RopeNodes.Count; i++) {
                    this.RopeNodes[i].MaybeUpdatePositionToPinnedTransform();
                }

                for (int i = 0; i < RopeNodes.Count - 1; i++)
                {
                    RopeNode node1 = this.RopeNodes[i];
                    RopeNode node2 = this.RopeNodes[i + 1];

                    // Get the current distance between rope nodes
                    float currentDistance = (node1.transform.position - node2.transform.position).magnitude;
                    float difference = Mathf.Abs(currentDistance - _nodeDistance);
                    Vector2 direction = Vector2.zero;

                    // determine what direction we need to adjust our nodes
                    if (currentDistance > _nodeDistance)
                    {
                        direction = (node1.transform.position - node2.transform.position).normalized;
                    }
                    else if (currentDistance < _nodeDistance)
                    {
                        direction = (node2.transform.position - node1.transform.position).normalized;
                    }

                    // calculate the movement vector
                    Vector3 movement = direction * difference;

                    // Apply correction if not pinned
                    if (!node1.IsPinned()) {
                        node1.transform.position -= (movement * _correction);
                    }
                    if (!node2.IsPinned()) {
                        node2.transform.position += (movement * _correction);
                    }
                }
            }

            public float GetRopeSize() {
                float ropeSize = 0;
                for (int i = 0; i < RopeNodes.Count - 1; i++) {
                    RopeNode node1 = this.RopeNodes[i];
                    RopeNode node2 = this.RopeNodes[i + 1];
                    ropeSize += (node1.transform.position - node2.transform.position).magnitude;
                }
                return ropeSize;
            }

            public float GetRopeStretchToEndFrom(RopeNode ropeNode) {
                float ropeSize = 0;
                int segments = 0;
                for (int i = ropeNode.NodeIndex; i < RopeNodes.Count - 1; i++) {
                    RopeNode nodeA = this.RopeNodes[i];
                    RopeNode nodeB = this.RopeNodes[i + 1];
                    ropeSize += (nodeA.transform.position - nodeB.transform.position).magnitude;
                    segments += 1;
                }
                return ropeSize / (segments * NodeDistance);
            }

            private void DrawRope()
            {
                //LineRenderer.startWidth = _ropeWidth;
                //LineRenderer.endWidth = _ropeWidth;

                for (int n = 0; n < RopeNodes.Count; n++)
                {
                    DrawPositions[n] = new Vector3(RopeNodes[n].transform.position.x, RopeNodes[n].transform.position.y, 0);
                    Debug.Log("Draw Y: " + RopeNodes[n].transform.position.y);
                }

                LineRenderer.positionCount = DrawPositions.Length;
                LineRenderer.SetPositions(DrawPositions);
            }

            bool CollisionAllowed(int layer)
            {
                #region Explanation
                //Read more: http://bit.ly/3HBA9cn
                /*
                string bitLayerMask = System.Convert.ToString(_collisionLayers, 2); //all collision allowed layers to bit system
                int bitLayer = 1 << layer; //current collision layer to bit system
                string bitLayerStr = System.Convert.ToString(bitLayer, 2); ; //current collision layer to bit system

                Debug.Log($"collisionLayers = {_collisionLayers}\n" +
                 $"collisionLayers^2 = {bitLayerMask}\n" +
                 $"layer = {layer}\n" +
                 $"layer^2 = {bitLayerStr} ({bitLayer})\n" +
                 $"Result : {_collisionLayers & bitLayer}\n");
                */
                #endregion

                if ((_collisionLayers & (1 << layer)) == 0) //if they are crossed without differences (bitwise AND)
                {
                    return false;
                }
              
                return true;
            }

            #region User Operations
            void AddNodeAt(Vector2 pos, bool updateLinePos = false)
            {
                RopeNode node = Instantiate(ropeNode).GetComponent<RopeNode>();
                node.PreviousPosition = pos;
                node.transform.position = pos;
                node.transform.parent = transform;
                node.NodeIndex = RopeNodes.Count;
                RopeNodes.Add(node);

                if (updateLinePos)
                    DrawPositions = new Vector3[RopeNodes.Count];
            }

            public void AddNode()
            {
                if (RopeNodes.Count > 2)
                {
                    Vector2 t1 = RopeNodes[RopeNodes.Count - 1].transform.position;
                    Vector2 t2 = RopeNodes[RopeNodes.Count - 2].transform.position;

                    Vector2 dir = t1 - t2;
                    dir.Normalize(); 
                    dir *= _nodeDistance;

                    Vector2 pos = new Vector2(RopeNodes.Last().transform.position.x, RopeNodes.Last().transform.position.y) + dir;
                    AddNodeAt(pos, true);
                }
                else
                {
                    Vector2 pos = new Vector2(RopeNodes.Last().transform.position.x, RopeNodes.Last().transform.position.y - _nodeDistance);
                    AddNodeAt(pos, true);
                }

                OnRopeNodesCountChanged?.Invoke();
            }

            public void RemoveNode()
            {
                int n = RopeNodes.Count;
                if (n > 1)
                {
                    RopeNode node = RopeNodes[n - 1];
                    RopeNodes.RemoveAt(n - 1);

                    Destroy(node.gameObject);
                    DrawPositions = new Vector3[RopeNodes.Count];

                    OnRopeNodesCountChanged?.Invoke();
                }
            }

            public RopeNode GetClosestNode(Vector3 worldPosition) {
                float minDistance = float.MaxValue;
                RopeNode node = RopeNodes[0];
                for (int i = 0; i < RopeNodes.Count; i++) {
                    float distance = (worldPosition - RopeNodes[i].transform.position).magnitude;
                    if (distance < minDistance) {
                        minDistance = distance;
                        node = RopeNodes[i];
                    }
                }
                return node;
            }

            public RopeNode GetNodeAt(int index)
            {
                if (index < RopeNodes.Count && index > -1)
                {
                    return RopeNodes[index];
                }
                return null;
            }
            public RopeNode GetFirstNode()
            {
                return RopeNodes.FirstOrDefault();
            }
            public RopeNode GetLastNode()
            {
                return RopeNodes.Last();
            }
            public List<RopeNode> GetAllNodes()
            {
                return RopeNodes;
            }

            public void AttachTo(Transform obj)
            {
                attachTo = obj; 
            }

            public void PinNodeTo(RopeNode node, Transform obj) {
                node.pinnedTo = obj;
            }

            public void UnpinAllNodes() {
                for (int i = 0; i < RopeNodes.Count; i++) {
                    UnpinNode(RopeNodes[i]);
                }
            }

            public void UnpinNode(RopeNode node) {
                node.pinnedTo = null;
            }

            public void Clear(bool editor = false)
            {
                ClearLocalNodes(editor);
            }

            public void SetNodesVisible(bool visible)
            {
                _nodesVisible = visible;
                foreach (var n in RopeNodes)
                {
                    n.spriteRenderer.enabled = visible;
                }
            }

            //EVENTS
            public event Action OnRopeNodesCountChanged;

            #endregion
        }
    }
}