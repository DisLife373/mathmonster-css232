using System.Threading.Tasks;
using UnityEngine;

public class Shootable : MonoBehaviour
{
    public static Shootable instance;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Arrow arrow;

    void Start() {
        instance = this;
    }

    public async Task<bool> Shoot(Vector3 _targetPos) {
        Instantiate(arrow, shootPoint.position, shootPoint.rotation);
        await Task.Delay(Mathf.RoundToInt(arrow.GetTime(shootPoint.position, _targetPos) * 1000));
        return true;
    }
}
