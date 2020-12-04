using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace DialogueEditor
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject
    {
        [SerializeField]
        List<NodeBase> nodes = new List<NodeBase>();
        [SerializeField]
        Vector2 newNodeOffset = new Vector2(250, 0);

        Dictionary<string, NodeBase> nodeLookup = new Dictionary<string, NodeBase>();

        private void OnValidate()
        {
            nodeLookup.Clear();
            foreach (NodeBase node in GetAllNodes())
            {
                nodeLookup[node.name] = node;
            }
        }


        public IEnumerable<NodeBase> GetAllNodes()
        {
            return nodes;
        }

        public NodeBase GetRootNode()
        {
            return nodes[0];
        }

        public IEnumerable<NodeBase> GetAllChildren(NodeBase parentNode)
        {
            foreach (string childID in parentNode.GetChildren())
            {
                if (nodeLookup.ContainsKey(childID))
                {
                    yield return nodeLookup[childID];
                }
            }
        }

        public IEnumerable<NodeBase> GetPlayerChildren(NodeBase currentNode)
        {
            foreach (NodeBase node in GetAllChildren(currentNode))
            {
                if (node.IsPlayerSpeaking())
                {
                    yield return node;
                }
            }
        }


        public IEnumerable<NodeBase> GetAIChildren(NodeBase currentNode)
        {
            foreach (NodeBase node in GetAllChildren(currentNode))
            {
                if (!node.IsPlayerSpeaking())
                {
                    yield return node;
                }
            }
        }

#if UNITY_EDITOR
        public void CreateNode(NodeBase parent)
        {
            NodeBase newNode = MakeNode(parent);
            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
            Undo.RecordObject(this, "Added Dialogue Node");
            AddNode(newNode);
        }

        public void DeleteNode(NodeBase nodeToDelete)
        {
            Undo.RecordObject(this, "Deleted Dialogue Node");
            nodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private NodeBase MakeNode(NodeBase parent)
        {
            NodeBase newNode = CreateInstance<NodeBase>();
            newNode.name = Guid.NewGuid().ToString();
            if (parent != null)
            {
                parent.AddChild(newNode.name);
                newNode.SetPlayerSpeaking(!parent.IsPlayerSpeaking());
                newNode.SetPosition(parent.GetRect().position + newNodeOffset);
            }

            return newNode;
        }

        private void AddNode(NodeBase newNode)
        {
            nodes.Add(newNode);
            OnValidate();
        }

        private void CleanDanglingChildren(NodeBase nodeToDelete)
        {
            foreach (NodeBase node in GetAllNodes())
            {
                node.RemoveChild(nodeToDelete.name);
            }
        }
#endif

        public void OnBeforeSerialize()
        {
            Debug.Log("Is gonna serialize");
#if UNITY_EDITOR
            if (nodes.Count == 0)
            {
                NodeBase newNode = MakeNode(null);
                AddNode(newNode);
            }

            if (AssetDatabase.GetAssetPath(this) != "")
            {
                foreach (NodeBase node in GetAllNodes())
                {
                    if (AssetDatabase.GetAssetPath(node) == "")
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize()
        {
        }

    }

}