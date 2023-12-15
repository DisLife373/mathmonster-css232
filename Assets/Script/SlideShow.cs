using UnityEngine;

public class SlideShow : MonoBehaviour
{
    [SerializeField] private AnimationCurve curve;

    private float current;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        current = Mathf.MoveTowards(current, 1, 0.5f * Time.deltaTime);

        transform.position = Vector3.Lerp(new Vector3(-17.73f, 0, 0), Vector3.zero, curve.Evaluate(current));
    }
}
