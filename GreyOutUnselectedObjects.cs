using UnityEngine;
using System.Collections.Generic;

public class GreyOutUnselectedObjects : MonoBehaviour
{
    public GameObject model; // Reference to the MODEL GameObject
    public GameObject[] selectedChildren; // Array of selected children to keep unchanged
    public Material greyedOutMaterial; // Material to apply for greyed-out objects
    public bool isVisibilityMode = false; // Set to OFF by default

    private Dictionary<Renderer, Material> originalMaterials = new Dictionary<Renderer, Material>();
    private bool previousVisibilityMode; // Store the previous state of isVisibilityMode

    void Start()
    {
        // Store original materials for all children of the model
        StoreOriginalMaterials(model.transform);
        UpdateVisibility(); // Initial visibility based on the toggle state
        previousVisibilityMode = isVisibilityMode; // Initialize the previous state
    }

    void StoreOriginalMaterials(Transform parent)
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(parent);

        while (queue.Count > 0)
        {
            Transform current = queue.Dequeue();
            Renderer renderer = current.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Store the original material
                originalMaterials[renderer] = renderer.material;
            }

            // Enqueue all children of the current transform
            foreach (Transform child in current)
            {
                queue.Enqueue(child);
            }
        }
    }

    void Update()
    {
        // Check if isVisibilityMode has changed
        if (isVisibilityMode != previousVisibilityMode)
        {
            UpdateVisibility(); // Call to update visibility based on the new state
            previousVisibilityMode = isVisibilityMode; // Update the previous state
        }
    }

    public void UpdateVisibility()
    {
        if (isVisibilityMode)
        {
            RestoreVisibility();
        }
        else
        {
            GreyOutUnselectedChildren();
        }
    }

    public void GreyOutUnselectedChildren()
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(model.transform);

        while (queue.Count > 0)
        {
            Transform current = queue.Dequeue();

            // Check if the current transform is one of the selected children or their descendants
            if (!IsSelectedOrDescendant(current))
            {
                GreyOutChild(current);
            }
            else
            {
                RestoreChildVisibility(current); // Restore the selected children's visibility
            }

            // Enqueue all children of the current transform
            foreach (Transform child in current)
            {
                queue.Enqueue(child);
            }
        }
    }

    void GreyOutChild(Transform child)
    {
        Renderer renderer = child.GetComponent<Renderer>();
        if (renderer != null)
        {
            // Store the original material if it's not already stored
            if (!originalMaterials.ContainsKey(renderer))
            {
                originalMaterials[renderer] = renderer.material; // Store the original material
            }
            // Apply the greyed-out material
            renderer.material = greyedOutMaterial;
        }
    }

    void RestoreVisibility()
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(model.transform);

        while (queue.Count > 0)
        {
            Transform current = queue.Dequeue();
            RestoreChildVisibility(current);

            // Enqueue all children of the current transform
            foreach (Transform child in current)
            {
                queue.Enqueue(child);
            }
        }
    }

    void RestoreChildVisibility(Transform parent)
    {
        Renderer renderer = parent.GetComponent<Renderer>();
        if (renderer != null && originalMaterials.ContainsKey(renderer))
        {
            renderer.material = originalMaterials[renderer]; // Restore original material
        }
    }

    bool IsSelectedOrDescendant(Transform current)
    {
        foreach (GameObject selectedChild in selectedChildren)
        {
            if (current.gameObject == selectedChild || IsDescendantOf(current, selectedChild.transform))
            {
                return true;
            }
        }
        return false;
    }

    bool IsDescendantOf(Transform child, Transform parent)
    {
        Transform current = child.parent;
        while (current != null)
        {
            if (current == parent)
            {
                return true;
            }
            current = current.parent;
        }
        return false;
    }
}






// using UnityEngine;
// using System.Collections.Generic;

// public class GreyOutUnselectedObjects : MonoBehaviour
// {
//     public GameObject model; // Reference to the MODEL GameObject
//     public GameObject selectedChild; // The child to keep unchanged
//     public Material greyedOutMaterial; // Material to apply for greyed out objects
//     public bool isVisibilityMode = true; // Inspector toggle

//     private Dictionary<Renderer, Material> originalMaterials = new Dictionary<Renderer, Material>();
//     private bool previousVisibilityMode; // Store the previous state of isVisibilityMode

//     void Start()
//     {
//         // Store original materials for all children of the model
//         StoreOriginalMaterials(model.transform);
//         UpdateVisibility(); // Initial visibility based on the toggle state
//         previousVisibilityMode = isVisibilityMode; // Initialize the previous state
//     }

//     void StoreOriginalMaterials(Transform parent)
//     {
//         Queue<Transform> queue = new Queue<Transform>();
//         queue.Enqueue(parent);

//         while (queue.Count > 0)
//         {
//             Transform current = queue.Dequeue();
//             Renderer renderer = current.GetComponent<Renderer>();
//             if (renderer != null)
//             {
//                 // Store the original material
//                 originalMaterials[renderer] = renderer.material;
//             }

//             // Enqueue all children of the current transform
//             foreach (Transform child in current)
//             {
//                 queue.Enqueue(child);
//             }
//         }
//     }

//     void Update()
//     {
//         // Check if isVisibilityMode has changed
//         if (isVisibilityMode != previousVisibilityMode)
//         {
//             UpdateVisibility(); // Call to update visibility based on the new state
//             previousVisibilityMode = isVisibilityMode; // Update the previous state
//         }
//     }

//     public void UpdateVisibility()
//     {
//         if (isVisibilityMode)
//         {
//             RestoreVisibility();
//         }
//         else
//         {
//             GreyOutUnselectedChildren();
//         }
//     }

//     public void GreyOutUnselectedChildren()
//     {
//         Queue<Transform> queue = new Queue<Transform>();
//         queue.Enqueue(model.transform);

//         while (queue.Count > 0)
//         {
//             Transform current = queue.Dequeue();

//             // Check if the current transform is the selected child or its descendants
//             if (current.gameObject != selectedChild && !IsDescendantOf(current, selectedChild.transform))
//             {
//                 GreyOutChild(current);
//             }
//             else
//             {
//                 RestoreChildVisibility(current); // Restore the selected child's visibility
//             }

//             // Enqueue all children of the current transform
//             foreach (Transform child in current)
//             {
//                 queue.Enqueue(child);
//             }
//         }
//     }

//     void GreyOutChild(Transform child)
//     {
//         Renderer renderer = child.GetComponent<Renderer>();
//         if (renderer != null)
//         {
//             // Store the original material if it's not already stored
//             if (!originalMaterials.ContainsKey(renderer))
//             {
//                 originalMaterials[renderer] = renderer.material; // Store the original material
//             }
//             // Apply the greyed-out material
//             renderer.material = greyedOutMaterial;
//         }
//     }

//     void RestoreVisibility()
//     {
//         Queue<Transform> queue = new Queue<Transform>();
//         queue.Enqueue(model.transform);

//         while (queue.Count > 0)
//         {
//             Transform current = queue.Dequeue();
//             RestoreChildVisibility(current);

//             // Enqueue all children of the current transform
//             foreach (Transform child in current)
//             {
//                 queue.Enqueue(child);
//             }
//         }
//     }

//     void RestoreChildVisibility(Transform parent)
//     {
//         Renderer renderer = parent.GetComponent<Renderer>();
//         if (renderer != null && originalMaterials.ContainsKey(renderer))
//         {
//             renderer.material = originalMaterials[renderer]; // Restore original material
//         }
//     }

//     bool IsDescendantOf(Transform child, Transform parent)
//     {
//         Transform current = child.parent;
//         while (current != null)
//         {
//             if (current == parent)
//             {
//                 return true;
//             }
//             current = current.parent;
//         }
//         return false;
//     }
// }