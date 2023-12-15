using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Spell : MonoBehaviour
{
    [SerializeField] private Animator anim;
    public SpellScriptableObject SpellToCast;

    private BoxCollider2D myCollider;
    private Rigidbody2D myRigidbody;
    private GameObject target;
    private string spellSound = "";
    
    void Awake() {
        spellSound = SpellToCast.spellElement == "Physical" ? "Earth" : SpellToCast.spellElement;
        FindObjectOfType<AudioManager>().PlaySound(spellSound);
        myCollider = GetComponent<BoxCollider2D>();
        myCollider.isTrigger = true;

        myRigidbody = GetComponent<Rigidbody2D>();
        myRigidbody.isKinematic = true;

        anim.Play(SpellToCast.spellElement);

        target = GameObject.FindGameObjectWithTag("Selected");
    }

    private void FixedUpdate() {
        if (SpellToCast.Speed > 0) {
            Quaternion toRotation = Quaternion.LookRotation(Vector3.left, target.transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 10 * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, SpellToCast.Speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Selected") {
            anim.enabled = false;
            Destroy(this.gameObject);
        }
    }

    public float GetTime(Vector3 _start, Vector3 _end) {
        return Vector3.Distance(_start, _end) / SpellToCast.Speed;
    }
    
}
