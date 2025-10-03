using GuaraTower.Core.Interface;
using MigalhaSystem.Extensions;
using MigalhaSystem.Pool;
using System.Collections.Generic;
using UnityEngine;

namespace GuaraTower.Arena.Spawner {

    public class Spawner : MonoBehaviour {

        public Vector3 m_VerticalSize;
        public float m_HorizontalWidth;
        Unity.Mathematics.Random m_Random = new Unity.Mathematics.Random(12345);
        private float m_CurrentTime;

        [System.Serializable]
        public struct SpawnData {

            public PoolData m_EnemyPool;
            public float m_StartSpawnTime;
            public float m_EndSpawnTime;
            public AnimationCurve m_SpawnCurve;

        }
        public List<SpawnData> m_SpawnData = new List<SpawnData>();

        public class SpawnEnemyData {

            SpawnData m_SpawnData;
            public float m_SpawnTime;

            public void Initilize(SpawnData _SpawnData) {

                m_SpawnData = _SpawnData;
                m_SpawnTime = m_SpawnData.m_StartSpawnTime;

            }

            public bool Elapse(float _DeltaTime, float _TotalTime) {

                m_SpawnTime -= _DeltaTime;
                if(m_SpawnTime <= 0) {

                    return true;

                }

                return false;

            }

            public bool Spawn(Vector3 _SpawnPosition, float _TotalTime, out GameObject _Enemy) {

                _Enemy = null;

                if (_TotalTime < m_SpawnData.m_StartSpawnTime || _TotalTime >= m_SpawnData.m_EndSpawnTime) return false;

                if (m_SpawnTime <= 0) {

                    float spawnActiveTime = _TotalTime - m_SpawnData.m_StartSpawnTime;

                    _Enemy = m_SpawnData.m_EnemyPool.PullObject(_SpawnPosition, Quaternion.identity);
                    _Enemy.GetComponent<IEnemyTarget>().GetLifeSystem().Initialize(1 + (spawnActiveTime/10f) * .2f);

                    m_SpawnTime += 1f/m_SpawnData.m_SpawnCurve.Evaluate(spawnActiveTime);
                    return true;

                }

                return false;

            }

        }
        private List<SpawnEnemyData> m_SpawnEnemy = new List<SpawnEnemyData>();

        private void Start() {

            for (int i = 0; i < m_SpawnData.Count; i++) {

                var spawnEnemy = new SpawnEnemyData();
                spawnEnemy.Initilize(m_SpawnData[i]);
                m_SpawnEnemy.Add(spawnEnemy);

            }

        }

        private void Update() {

            SpawnEnemy();

        }

        public void SpawnEnemy() {

            m_CurrentTime += Time.deltaTime;

            for (int i = 0; i < m_SpawnEnemy.Count; i++) {

                if (m_SpawnEnemy[i].Elapse(Time.deltaTime, m_CurrentTime)) {

                    while (m_SpawnEnemy[i].Spawn(GetSpawnPosition(), m_CurrentTime, out _)) { }

                }

            }

        }

        #region SpawnPosi

        public Vector3 GetSpawnPosition() {

            int side = (m_Random.NextDouble() > 0.5f) ? 1 : -1;
            int upDown = m_Random.NextInt(-1,2);

            Vector3 a = new Vector3(m_VerticalSize.x * side, m_VerticalSize.y, m_VerticalSize.z * (upDown == 0 ? 1 : upDown));
            Vector3 b = new Vector3((m_VerticalSize.x * side) + (m_HorizontalWidth * (upDown == 0 ? 0 : 1) * side * -1), m_VerticalSize.y, m_VerticalSize.z * (upDown == 0? -1 : upDown));

            return new Vector3(m_Random.NextFloat(a.x, b.x), a.y, m_Random.NextFloat(a.z, b.z));

        }
        #endregion

        private void OnDrawGizmosSelected() {

            Gizmos.color = Color.red;

            Gizmos.DrawLine(m_VerticalSize, Vector3.Scale(m_VerticalSize, new Vector3(1,1,-1)));
            Gizmos.DrawLine(Vector3.Scale(m_VerticalSize, new Vector3(-1, 1, 1)), Vector3.Scale(m_VerticalSize, new Vector3(-1,1,-1)));

            Gizmos.DrawLine(m_VerticalSize, m_VerticalSize - new Vector3(m_HorizontalWidth, 0, 0));
            Gizmos.DrawLine(Vector3.Scale(m_VerticalSize, new Vector3(1, 1, -1)), Vector3.Scale(m_VerticalSize, new Vector3(1,1,-1)) - new Vector3(m_HorizontalWidth, 0, 0));

            Gizmos.DrawLine(Vector3.Scale(m_VerticalSize, new Vector3(-1, 1, 1)), Vector3.Scale(m_VerticalSize, new Vector3(-1,1,1)) + new Vector3(m_HorizontalWidth, 0, 0));
            Gizmos.DrawLine(Vector3.Scale(m_VerticalSize, new Vector3(-1, 1, -1)), Vector3.Scale(m_VerticalSize, new Vector3(-1,1,-1)) + new Vector3(m_HorizontalWidth, 0, 0));

        }

    }

}
