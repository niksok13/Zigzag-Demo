using NSTools;
using UnityEngine;

namespace zigzag
{
    public class GemController : MonoBehaviour
    {
        
        public GameplayController gameplayController;

        private EZ ez;
        private bool isTouched;
        
        public void Touch()
        {
            if (isTouched) return;
            isTouched = true;
            
            var rend = GetComponent<SpriteRenderer>();
            ez = EZ.Spawn().Add(0.3f, t =>
            {
                transform.Translate(Vector3.up*t/50);

                var col = rend.color;
                col.a = 1 - t;
                rend.color = col;
            }).Add(gameplayController.PickGem);
        }
        
        
        private void OnDestroy() => ez?.Clear();
    }
}
