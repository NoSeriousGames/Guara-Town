using Cysharp.Threading.Tasks;
using Game.Utilities.Singleton;
using GuaraTower.Arena.Observer;
using GuaraTower.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Profiling;

namespace GuaraTower.Arena.Enemies {

    public static class PlayerHelper {

        public static IPlayerTarget GetPlayerTarget() {

            if (EnemyManager.Instance == null || EnemyManager.Instance.PlayerTargetList.Count == 0) return null;
            return EnemyManager.Instance.PlayerTargetList[0];

        }

    }

    public static class EnemiesHelper {

        static float3[] s_Pos;
        static byte[] s_Aim;
        static int[] s_Ids;
        static int s_Count;

        public static void BuildSnapshot() {
            var mgr = EnemyManager.Instance;
            if (mgr == null) { s_Count = 0; return; }

            var list = mgr.EnemyTargetList;
            int n = list.Count;
            Ensure(ref s_Pos, n);
            Ensure(ref s_Aim, n);
            Ensure(ref s_Ids, n);

            Profiler.BeginSample("EnemiesHelper.BuildSnapshot");
            int w = 0;
            for (int i = 0; i < n; i++) {
                var e = list[i];
                if (!e.GetLifeSystem().IsAlive()) continue;
                s_Pos[w] = (float3)e.GetTransform().position;
                s_Aim[w] = (byte)(e is not IIgnoreAim ? 1 : 0);
                s_Ids[w] = i;
                w++;
            }
            s_Count = w;
            Profiler.EndSample();
        }

        static void Ensure<T>(ref T[] arr, int n) {
            if (arr == null || arr.Length < n) arr = new T[Mathf.NextPowerOfTwo(Mathf.Max(1, n))];
        }

        static void CopyToResultByIds(IList<int> ids, List<IEnemyTarget> result) {
            var list = EnemyManager.Instance.EnemyTargetList;
            for (int i = 0; i < ids.Count; i++) result.Add(list[ids[i]]);
        }

        static readonly List<IEnemyTarget> m_EnemiesToUse = new List<IEnemyTarget>(256);

        static readonly Dictionary<int, List<int>> s_Grid = new Dictionary<int, List<int>>(256);
        static float s_Cell = 8f;

        static int Hash(int cx, int cy) => (cy * 73856093) ^ (cx * 19349663);

        public static void BuildGrid() {
            Profiler.BeginSample("EnemiesHelper.BuildGrid");
            s_Grid.Clear();
            for (int i = 0; i < s_Count; i++) {
                var p = s_Pos[i];
                int cx = Mathf.FloorToInt(p.x / s_Cell);
                int cy = Mathf.FloorToInt(p.y / s_Cell);
                int h = Hash(cx, cy);
                if (!s_Grid.TryGetValue(h, out var cell)) s_Grid[h] = cell = new List<int>(8);
                cell.Add(i);
            }
            Profiler.EndSample();
        }

        public static IEnumerable<IEnemyTarget> GetAllInArea(Bounds bounds) {
            var res = new List<IEnemyTarget>();

            if (s_Count == 0) return res;
            var list = EnemyManager.Instance.EnemyTargetList;
            for (int i = 0; i < s_Count; i++) {
                if (bounds.Contains((Vector3)s_Pos[i])) res.Add(list[s_Ids[i]]);
            }
            return res;
        }

        public static List<IEnemyTarget> GetAll(Func<IEnemyTarget, bool> _Filter = null) {
            var enemies = new List<IEnemyTarget>();
            GetAllNonAlloc(enemies, _Filter);
            return enemies;
        }

        public static void GetAllNonAlloc(List<IEnemyTarget> _Result, Func<IEnemyTarget, bool> _Filter = null) {
            _Result.Clear();
            var mgr = EnemyManager.Instance; if (mgr == null) return;
            var list = mgr.EnemyTargetList;

            Profiler.BeginSample("EnemiesHelper - Get All Non Alloc");
            if (_Filter == null) {
                for (int i = 0; i < s_Count; i++) _Result.Add(list[s_Ids[i]]);
            } else {
                for (int i = 0; i < s_Count; i++) {
                    var e = list[s_Ids[i]];
                    if (_Filter(e)) _Result.Add(e);
                }
            }
            Profiler.EndSample();
        }

        public static List<IEnemyTarget> GetRandomAmount(Vector3 position, float maxRange, int amount) {
            var result = new List<IEnemyTarget>(amount);
            GetRandomAmountNonAlloc(position, maxRange, amount, result);
            return result;
        }

        public static void GetRandomAmountNonAlloc(Vector3 position, float maxRange, int amount, List<IEnemyTarget> _Result, System.Random rng = null) {
            _Result.Clear();
            if (s_Count == 0 || amount <= 0) return;

            rng ??= new System.Random();
            float r2 = maxRange * maxRange;
            var pos = (float3)position;

            Profiler.BeginSample("EnemiesHelper - Get Random Amount Non Alloc");
            var reservoir = ListPool<int>.Get();
            try {
                int t = 0;
                for (int i = 0; i < s_Count; i++) {
                    if (math.lengthsq(s_Pos[i] - pos) > r2) continue;
                    int id = s_Ids[i];
                    t++;
                    if (reservoir.Count < amount) {
                        reservoir.Add(id);
                    } else {
                        int j = rng.Next(t);
                        if (j < amount) reservoir[j] = id;
                    }
                }
                CopyToResultByIds(reservoir, _Result);
            } finally { ListPool<int>.Release(reservoir); }
            Profiler.EndSample();
        }

        public static IEnemyTarget GetNearest(Vector3 position, Func<IEnemyTarget, bool> _Filter = null, params IEnemyTarget[] enemiesToIgnore) {
            return GetNearest(position, Mathf.Infinity, _Filter, enemiesToIgnore);
        }

        public static IEnemyTarget GetNearest(Vector3 position, float maxDistance, Func<IEnemyTarget, bool> _Filter = null, params IEnemyTarget[] enemiesToIgnore) {
            m_EnemiesToUse.Clear();
            GetNearestsNonAlloc(position, 1, maxDistance, m_EnemiesToUse, _Filter, enemiesToIgnore);
            return m_EnemiesToUse.Count > 0 ? m_EnemiesToUse[0] : null;
        }

        public static List<IEnemyTarget> GetNearests(Vector3 position, int amount, float maxDistance, Func<IEnemyTarget, bool> _Filter = null, params IEnemyTarget[] enemiesToIgnore) {
            var allEnemies = new List<IEnemyTarget>();
            GetNearestsNonAlloc(position, amount, maxDistance, allEnemies, _Filter, enemiesToIgnore);
            return allEnemies;
        }

        public static void GetNearestsNonAlloc(
Vector3 position, int amount, float maxDistance,
List<IEnemyTarget> _Result, Func<IEnemyTarget, bool> _Filter = null, params IEnemyTarget[] enemiesToIgnore) {
            _Result.Clear();
            var mgr = EnemyManager.Instance; if (mgr == null || s_Count == 0 || amount <= 0) return;

            Profiler.BeginSample("EnemiesHelper - Get Nearests Non Alloc (NoSort)");

            var list = mgr.EnemyTargetList;
            var pos = (float3)position;
            bool hasMax = !float.IsInfinity(maxDistance);
            float r2 = hasMax ? maxDistance * maxDistance : float.PositiveInfinity;

            // Ignorados: evite HashSet se poucos itens (linear check é mais barato até ~6-8)
            bool HasIgnore(IEnemyTarget e) {
                if (enemiesToIgnore == null || enemiesToIgnore.Length == 0) return false;
                if (enemiesToIgnore.Length <= 8) {
                    for (int i = 0; i < enemiesToIgnore.Length; i++) if (ReferenceEquals(enemiesToIgnore[i], e)) return true;
                    return false;
                }
                // fallback para muitos ignorados
                return new HashSet<IEnemyTarget>(enemiesToIgnore).Contains(e);
            }

            // Buffer dos k melhores (ordenado por d2 ASC)
            int bestCount = 0;
            // garante capacidade suficiente uma única vez
            Ensure(ref s_BestIds, amount);
            Ensure(ref s_BestD2, amount);

            void TryConsider(int snapIdx) {
                // filtro por alcance
                float d2 = math.lengthsq(s_Pos[snapIdx] - pos);
                if (d2 > r2) return;

                var e = list[s_Ids[snapIdx]];
                if (_Filter != null && !_Filter(e)) return;
                if (HasIgnore(e)) return;

                // Inserção em lista ordenada (tamanho máximo = amount)
                int insert = bestCount;
                // se cheio e este não é melhor que o pior, descarta rápido
                if (bestCount == amount && d2 >= s_BestD2[bestCount - 1]) return;

                // encontra posição de inserção (ASC)
                while (insert > 0 && d2 < s_BestD2[insert - 1]) insert--;

                // shift à direita
                int limit = Math.Min(bestCount, amount - 1);
                for (int j = limit; j > insert; j--) {
                    s_BestIds[j] = s_BestIds[j - 1];
                    s_BestD2[j] = s_BestD2[j - 1];
                }
                s_BestIds[insert] = s_Ids[snapIdx];
                s_BestD2[insert] = d2;
                if (bestCount < amount) bestCount++;
            }

            // 1) Se temos grid e maxDistance finito, varre só as células vizinhas
            if (s_Grid.Count > 0 && hasMax) {
                int cx = Mathf.FloorToInt(pos.x / s_Cell);
                int cy = Mathf.FloorToInt(pos.y / s_Cell);
                int cr = Mathf.CeilToInt(maxDistance / s_Cell);

                for (int ix = cx - cr; ix <= cx + cr; ix++) {
                    for (int iy = cy - cr; iy <= cy + cr; iy++) {
                        if (!s_Grid.TryGetValue(Hash(ix, iy), out var cell)) continue;
                        for (int j = 0; j < cell.Count; j++) {
                            TryConsider(cell[j]); // cell[j] é índice no snapshot
                        }
                    }
                }
            }
            // 2) Caso contrário, varre todo o snapshot
            else {
                for (int i = 0; i < s_Count; i++) TryConsider(i);
            }

            // Copia resultado (já em ordem)
            for (int i = 0; i < bestCount; i++) _Result.Add(list[s_BestIds[i]]);

            Profiler.EndSample();
        }

        // buffers estáticos para o método acima
        static int[] s_BestIds;
        static float[] s_BestD2;

        public static List<IEnemyTarget> GetAllInRange(Vector3 position, float range, Func<IEnemyTarget, bool> _Filter = null) {
            var enemies = new List<IEnemyTarget>();
            GetAllInRangeNonAlloc(position, range, enemies, _Filter);
            return enemies;
        }

        public static void GetAllInRangeNonAlloc(Vector3 position, float range, List<IEnemyTarget> _Result, Func<IEnemyTarget, bool> _Filter = null) {
            _Result.Clear();
            if (s_Count == 0) return;

            float r2 = range * range;
            var pos = (float3)position;
            var mgr = EnemyManager.Instance; if (mgr == null) return;
            var list = mgr.EnemyTargetList;

            Profiler.BeginSample("EnemiesHelper - Get All In Range Non Alloc");

            if (s_Grid.Count > 0) {
                int cx = Mathf.FloorToInt(pos.x / s_Cell);
                int cy = Mathf.FloorToInt(pos.y / s_Cell);
                int cr = Mathf.CeilToInt(range / s_Cell);

                var ids = ListPool<int>.Get();
                try {
                    for (int ix = cx - cr; ix <= cx + cr; ix++) {
                        for (int iy = cy - cr; iy <= cy + cr; iy++) {
                            if (!s_Grid.TryGetValue(Hash(ix, iy), out var cell)) continue;
                            for (int j = 0; j < cell.Count; j++) {
                                int snapIdx = cell[j];
                                if (math.lengthsq(s_Pos[snapIdx] - pos) > r2) continue;
                                var e = list[s_Ids[snapIdx]];
                                if (_Filter == null || _Filter(e)) ids.Add(s_Ids[snapIdx]);
                            }
                        }
                    }
                    CopyToResultByIds(ids, _Result);
                } finally { ListPool<int>.Release(ids); }
            } else {

                for (int i = 0; i < s_Count; i++) {
                    if (math.lengthsq(s_Pos[i] - pos) > r2) continue;
                    var e = list[s_Ids[i]];
                    if (_Filter != null && !_Filter(e)) continue;
                    _Result.Add(e);
                }
            }

            Profiler.EndSample();
        }

        public static bool AimFilter(IEnemyTarget _Enemy) {
            return _Enemy is not IIgnoreAim;
        }

        public static bool EmptyFilter(IEnemyTarget _Enemy) {
            return true;
        }
    }

    public class EnemyManager : Singleton<EnemyManager> {

        private List<IPlayerTarget> m_PlayerTargetList = new List<IPlayerTarget>();
        public List<IPlayerTarget> PlayerTargetList { get { return m_PlayerTargetList; } private set { } }

        bool m_TriggerEnemyUpdate = false;
        private List<IEnemyTarget> m_EnemyTargetList = new List<IEnemyTarget>();
        public List<IEnemyTarget> EnemyTargetList { get {
                if (m_TriggerEnemyUpdate) { m_TriggerEnemyUpdate = false; EnemiesHelper.BuildSnapshot(); }
                return m_EnemyTargetList; 
            } private set { } }


        CancellationTokenSource m_CancellationToken;

        private void OnEnable() {

            m_CancellationToken?.CancellHelper();
            m_CancellationToken = new CancellationTokenSource();
            UpdateList(m_CancellationToken.Token).Forget();

            ObserverPlayerTarget.AddPlayer += AddPlayer;
            ObserverPlayerTarget.RemovePlayer += RemovePlayer;

            ObserverEnemyTarget.AddEnemy += AddEnemy;
            ObserverEnemyTarget.RemoveEnemy += RemoveEnemy;

        }

        private void OnDisable() {

            ObserverPlayerTarget.AddPlayer -= AddPlayer;
            ObserverPlayerTarget.RemovePlayer -= RemovePlayer;

            ObserverEnemyTarget.AddEnemy -= AddEnemy;
            ObserverEnemyTarget.RemoveEnemy -= RemoveEnemy;

            m_CancellationToken?.CancellHelper();
            m_CancellationToken = null;

        }

        public void AddPlayer(IPlayerTarget _IPlayerTarget) {

            if (m_PlayerTargetList.Contains(_IPlayerTarget)) return;
            m_PlayerTargetList.Add(_IPlayerTarget);

        }

        public void RemovePlayer(IPlayerTarget _IPlayerTarget) {

            m_PlayerTargetList.Remove(_IPlayerTarget);

        }

        public void AddEnemy(IEnemyTarget _IEnemy) {

            if (m_EnemyTargetList.Contains(_IEnemy)) return;
            m_EnemyTargetList.Add(_IEnemy);
            m_TriggerEnemyUpdate = true;

        }

        public void RemoveEnemy(IEnemyTarget _IEnemy) {

            m_EnemyTargetList.Remove(_IEnemy);
            m_TriggerEnemyUpdate = true;

        }

        private void LateUpdate() {

            EnemiesHelper.BuildSnapshot();
            m_TriggerEnemyUpdate = false;

        }

        private async UniTask UpdateList(CancellationToken _CancellationToken) {

            while (true) {

                UpdateList();

                if (await UniTask.WaitForSeconds(1f, cancellationToken: _CancellationToken).SuppressCancellationThrow()) return;

            }

        }

        private void UpdateList() {

            var monoBehaviour = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            m_PlayerTargetList.Clear();
            m_PlayerTargetList.AddRange(monoBehaviour.OfType<IPlayerTarget>());

            m_EnemyTargetList.Clear();
            m_EnemyTargetList.AddRange(monoBehaviour.OfType<IEnemyTarget>());

            EnemiesHelper.BuildSnapshot();
            m_TriggerEnemyUpdate = false;

            ObserverPlayerTarget.CallTargetUpdate();
            ObserverEnemyTarget.CallTargetUpdate();

        }

    }

}