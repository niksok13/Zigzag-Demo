using NSTools;
using UnityEngine;

namespace zigzag
{
    public class TileController : MonoBehaviour
    {
        private EZ ez;
        private bool isFall;

        private void Touch()
        {
            if (isFall) return;
            isFall = true;
            var rend = GetComponent<SpriteRenderer>();
            ez = EZ.Spawn().Wait(1f).Add(0.3f, t =>
            {
                transform.Translate(Vector3.down*t/10);

                var col = rend.color;
                col.a = 1 - t;
                rend.color = col;
            });
        }

        private void OnDestroy() => ez?.Clear();
    }
}
