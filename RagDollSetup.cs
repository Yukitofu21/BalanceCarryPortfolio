using UnityEngine;

public class RagdollSetup : MonoBehaviour
{
    [SerializeField] private Transform rootBone;

    public void Setup(Transform originalRootBone)
    {
        Debug.Log("a");
        CloneTransforms(originalRootBone, rootBone);
        //Explosion(250f,transform.position,10f);
    }
    private void CloneTransforms(Transform root, Transform clone)
    {
        foreach (Transform child in root)
        {
            Transform cloneChild = clone.Find(child.name);
            if (cloneChild != null)
            {
                cloneChild.position = child.position;
                cloneChild.rotation = child.rotation;

                CloneTransforms(child, cloneChild);
                
            }
        }
    }
}