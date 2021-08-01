using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class DirecitonTest : MonoBehaviour
{
    NavMeshAgent _navMeshAgent;

    public GameObject TestDest1;
    public GameObject TestDest2;


    public GameObject Target;

    public float distacne;
    public float angle;
    // Use this for initialization
    void Start()
    {
        _navMeshAgent.SetDestination(Vector3.zero);
    }

    private void OnEnable()
    {
        var direction = ( this.transform.position - Target.transform.position).normalized;

        if(TestDest1 == null)
        {
           TestDest1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            TestDest1.transform.SetParent(this.transform);
        }
        if (TestDest2 == null)
        {
            TestDest2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            TestDest2.transform.SetParent(this.transform);
        }

        TestDest1.transform.position = this.transform.position + direction * distacne;
        var quaternion = Quaternion.Euler(0, angle, 0);
        var newDirection = quaternion * direction;
        TestDest2.transform.position = this.transform.position + newDirection * distacne;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
