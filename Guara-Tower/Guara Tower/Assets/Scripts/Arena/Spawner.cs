using Unity.Mathematics;
using UnityEngine;

namespace GuaraTower.Arena.Spawner {

    public class Spawner : MonoBehaviour {

        public Vector3 m_VerticalSize;
        public float m_HorizontalWidth;
        Unity.Mathematics.Random m_Random = new Unity.Mathematics.Random(12345);

        public Vector3 GetSpawnPosition() {

            int side = (m_Random.NextDouble() > 0.5f) ? 1 : -1;
            int upDown = m_Random.NextInt(-1,2);

            Vector3 a = new Vector3(m_VerticalSize.x * side, m_VerticalSize.y, m_VerticalSize.z * (upDown == 0 ? 1 : upDown));
            Vector3 b = new Vector3((m_VerticalSize.x * side) + (m_HorizontalWidth * (upDown == 0 ? 0 : 1) * side * -1), m_VerticalSize.y, m_VerticalSize.z * (upDown == 0? -1 : upDown));

            return new Vector3(m_Random.NextFloat(a.x, b.x), a.y, m_Random.NextFloat(a.z, b.z));

        }

        Vector3 m_LastSpawnedPosi;

        private void OnDrawGizmosSelected() {

            Gizmos.color = Color.red;

            Gizmos.DrawLine(m_VerticalSize, Vector3.Scale(m_VerticalSize, new Vector3(1,1,-1)));
            Gizmos.DrawLine(Vector3.Scale(m_VerticalSize, new Vector3(-1, 1, 1)), Vector3.Scale(m_VerticalSize, new Vector3(-1,1,-1)));

            Gizmos.DrawLine(m_VerticalSize, m_VerticalSize - new Vector3(m_HorizontalWidth, 0, 0));
            Gizmos.DrawLine(Vector3.Scale(m_VerticalSize, new Vector3(1, 1, -1)), Vector3.Scale(m_VerticalSize, new Vector3(1,1,-1)) - new Vector3(m_HorizontalWidth, 0, 0));

            Gizmos.DrawLine(Vector3.Scale(m_VerticalSize, new Vector3(-1, 1, 1)), Vector3.Scale(m_VerticalSize, new Vector3(-1,1,1)) + new Vector3(m_HorizontalWidth, 0, 0));
            Gizmos.DrawLine(Vector3.Scale(m_VerticalSize, new Vector3(-1, 1, -1)), Vector3.Scale(m_VerticalSize, new Vector3(-1,1,-1)) + new Vector3(m_HorizontalWidth, 0, 0));

            if (m_LastSpawnedPosi == Vector3.zero) return;

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(m_LastSpawnedPosi, 0.5f);

        }

    }

}
