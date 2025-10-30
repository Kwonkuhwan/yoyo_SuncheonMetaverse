using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Suncheon
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private SpawnerPos spawnerPos;
        public SpawnerPos SpawnerPos => spawnerPos;
    }
}
