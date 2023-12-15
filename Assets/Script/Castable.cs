using System.Threading.Tasks;
using UnityEngine;

public class Castable : MonoBehaviour
{
    public async Task<bool> Cast(Vector3 _targetPos, Spell _spell, Vector3 _castPos, Quaternion _castRotate) {
        Instantiate(_spell, _castPos, _castRotate);
        await Task.Delay(Mathf.RoundToInt(_spell.GetTime(_castPos, _targetPos) * 1000));
        return true;
    }
}
