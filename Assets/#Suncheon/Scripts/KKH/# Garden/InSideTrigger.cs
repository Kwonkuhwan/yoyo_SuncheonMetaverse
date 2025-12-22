using UnityEngine;

namespace Suncheon.UI
{
    public class InSideTrigger : MonoBehaviour
    {
        [SerializeField] private SpawnerPos spPos;
        [SerializeField] private LibName libName;
        public LibName LibaryName => libName;

        private void Start()
        {
            if(spPos == SpawnerPos.어린이실 || spPos == SpawnerPos.시청각실 || spPos == SpawnerPos.자료실)
            {
                libName = NetworkManager.Instance.LibPos;
            }
        }
    }
}
