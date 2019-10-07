using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    
    private float z;
    
    void Awake()
    {
        z = transform.position.z;
    }
    
    void Update()
    {
        Vector3 distance = target.position - transform.position;
        
        transform.position += distance / 2f;
        
        transform.position = new Vector3(transform.position.x, transform.position.y, z);
    }
}