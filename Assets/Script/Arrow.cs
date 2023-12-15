using UnityEngine;

public class Arrow : MonoBehaviour
{
    private float speed = 10f;
    private BoxCollider2D myCollider;
    private Rigidbody2D myRigidbody;
    private GameObject target;
    
    void Awake() {
        myCollider = GetComponent<BoxCollider2D>();
        myCollider.isTrigger = true;

        myRigidbody = GetComponent<Rigidbody2D>();
        myRigidbody.isKinematic = true;

        target = GameObject.FindGameObjectWithTag("Player");
        
    }

    private void FixedUpdate() {
        Quaternion toRotation = Quaternion.LookRotation(Vector3.left, target.transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 10 * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            Destroy(this.gameObject);
        }
    }

    public float GetTime(Vector3 _start, Vector3 _end) {
        return Vector3.Distance(_start, _end) / speed;
    }
}
