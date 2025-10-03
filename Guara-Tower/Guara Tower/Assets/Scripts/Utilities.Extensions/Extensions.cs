using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace MigalhaSystem.Extensions {

    [System.Serializable]
    public class Edges {

        [SerializeField] List<Transform> m_edgePoints;
        public List<Transform> m_EdgePoints => m_edgePoints;

        public bool ObjectInCamera() {
            return m_edgePoints.Exists(x => x.position.IsObjectInView());
        }

    }

    public static class MethodsExtension {

        public static MethodInfo GetMethodInfo(this object o, string m) {
            return o.GetType().GetMethod(m);
        }

        public static bool IsOverride(this object o, string m) {
            return o.GetMethodInfo(m).IsOverride();
        }

        public static bool IsOverride(this MethodInfo m) {
            return m.GetBaseDefinition().DeclaringType != m.DeclaringType;
        }

    }

    public static class QuaternionExtension {

        public static Quaternion DirectionFromDeg(Vector3 targetPosition, Vector3 spawnPosition) {

            Vector3 offsetTargetPosition = targetPosition + new Vector3(0f, 0f, 0f);
            Vector3 directionToTarget = offsetTargetPosition - spawnPosition;
            return Quaternion.LookRotation(directionToTarget, Vector3.up) * Quaternion.Euler(0f, -90f, 0f);

        }

        public static Transform LookAt2D(this Transform originPosition, Transform targetPosition) => originPosition.LookAt2D(targetPosition.position);
        public static Transform LookAt2D(this Transform originPosition, Vector3 targetPosition) {

            Vector2 vectorToTarget = targetPosition - originPosition.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
            originPosition.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            return originPosition;

        }

    }

    public static class VectorExtension {

        public static Vector2 With(this Vector2 _vector2, float? x = null, float? y = null) {
            float newX = x is null ? _vector2.x : (float)x;
            float newY = y is null ? _vector2.y : (float)y;
            _vector2.Set(newX, newY);
            return _vector2;
        }
        public static Vector3 With(this Vector3 _vector3, float? x = null, float? y = null, float? z = null) {
            float newX = x is null ? _vector3.x : (float)x;
            float newY = y is null ? _vector3.y : (float)y;
            float newZ = z is null ? _vector3.z : (float)z;
            _vector3.Set(newX, newY, newZ);
            return _vector3;
        }

        public static Vector2 Increase(this Vector2 _vector3, float? x = null, float? y = null) {
            float newX = x is null ? _vector3.x : _vector3.x + (float)x;
            float newY = y is null ? _vector3.y : _vector3.y + (float)y;
            _vector3.Set(newX, newY);
            return _vector3;
        }
        public static Vector3 Increase(this Vector3 _vector3, float? x = null, float? y = null, float? z = null) {
            float newX = x is null ? _vector3.x : _vector3.x + (float)x;
            float newY = y is null ? _vector3.y : _vector3.y + (float)y;
            float newZ = z is null ? _vector3.z : _vector3.z + (float)z;
            _vector3.Set(newX, newY, newZ);
            return _vector3;
        }

        public static Vector2 Decrease(this Vector2 _vector3, float? x = null, float? y = null) {
            float newX = x is null ? _vector3.x : _vector3.x - (float)x;
            float newY = y is null ? _vector3.y : _vector3.y - (float)y;
            _vector3.Set(newX, newY);
            return _vector3;
        }
        public static Vector3 Decrease(this Vector3 _vector3, float? x = null, float? y = null, float? z = null) {
            float newX = x is null ? _vector3.x : _vector3.x - (float)x;
            float newY = y is null ? _vector3.y : _vector3.y - (float)y;
            float newZ = z is null ? _vector3.z : _vector3.z - (float)z;
            _vector3.Set(newX, newY, newZ);
            return _vector3;
        }

        public static Vector3 DirectionFromDeg(this float angleInDeg, Vector3? rotationAxis = null, Vector3? startDirection = null) {
            rotationAxis ??= Vector3.forward;
            Quaternion myRotation = Quaternion.AngleAxis(angleInDeg, (Vector3)rotationAxis);

            startDirection ??= Vector3.right;
            Vector3 result = myRotation * (Vector3)startDirection;

            return result;
        }

        public static Vector3 DirectionFromRad(this float angleInRad, Vector3? rotationAxis = null, Vector3? startDirection = null) => DirectionFromDeg(angleInRad * Mathf.Rad2Deg, rotationAxis, startDirection);

        public static Vector2 AngleToDirection(this float angleInDegrees) {
            float angleInRadians = angleInDegrees * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
        }

        public static Vector2 GetRandomPosition(Vector2 minValue, Vector2 maxValue) {
            float x = Random.Range(minValue.x, maxValue.x);
            float y = Random.Range(minValue.y, maxValue.y);
            return new(x, y);
        }
        public static Vector3 GetRandomPosition(Vector3 minValue, Vector3 maxValue) {
            float x = Random.Range(minValue.x, maxValue.x);
            float y = Random.Range(minValue.y, maxValue.y);
            float z = Random.Range(minValue.z, maxValue.z);
            return new(x, y, z);
        }

    }

    public static class EnumExtension {

        public static int GetEnumCount<TEnum>(this TEnum _enum) where TEnum : struct, System.Enum => System.Enum.GetNames(typeof(TEnum)).Length;
        public static int GetEnumCount<TEnum>() where TEnum : struct, System.Enum => System.Enum.GetNames(typeof(TEnum)).Length;
        public static int GetEnumCount(this System.Type _type) => System.Enum.GetNames(_type.GetType()).Length;

    }

    public enum UpdateMethod { N, Update, FixedUpdate, LateUpdate }

    public enum DrawMethod { N, OnDrawGizmos, OnDrawGizmosSelected }

    public static class FloatExtend {

        public static float PlusPercentage(this float value, float percentage) {
            var result = value + (value * percentage);
            return result;
        }
        public static float PlusPercentage(this float value, int percentage) {
            var decimalFromInt = percentage / 100f;
            return PlusPercentage(value, decimalFromInt);
        }

        public static bool IsInteger(this float value) => value % 1 <= float.Epsilon * 1000;
        public static bool IsInteger(this double value) => value % 1 <= double.Epsilon * 1000;

        public static float GetPercentage(this float currentValue, float min = 0, float max = 1) => (currentValue - min) / max;

        public static TimeSpan GetTimeSpan(this float value) {

            int seconds = Mathf.FloorToInt(value);
            int milliseconds = (int)((value % 1) * 1000);
            return new TimeSpan(0, 0, 0, seconds, milliseconds);

        }

    }

    public static class RandomExtend {

        public static float RangeBy0(this float value) => Random.Range(0f, value);
        public static int RangeBy0(this int value, bool maxInclusive = false) => Random.Range(0, value + (maxInclusive ? 1 : 0));

        public static float VariateWithinRange(this float value) => Random.Range(-value, value);
        public static int VariateWithinRange(this int value, bool minValueInclusive = true, bool maxValueInclusive = false) => Random.Range(-value + (minValueInclusive ? 0 : 1), value + (maxValueInclusive ? 1 : 0));

        public static float ChooseAltNum(this float value) => Choose(-value, value);
        public static int ChooseAltNum(this int value) => Choose(-value, value);

        public static T Choose<T>(params T[] elements) => elements.GetRandom();

    }

    public static class StringExtend {

        public static string Color(this string _string, Color _textColor) => $"<color=#{ColorUtility.ToHtmlStringRGBA(_textColor)}>{_string}</color>";

        public static string Color(this string _string, string _textColorHex) {
            string _colorString = _textColorHex;
            if (_textColorHex.StartsWith('#')) {
                _colorString = _textColorHex.Split('#')[1];
            }
            return $"<color=#{_colorString}>{_string}</color>";
        }

        public static string Bold(this string _string) => $"<b>{_string}</b>";

        public static string Italic(this string _string) => $"<i>{_string}</i>";

        public static string Size(this string _string, float size = 15) => $"<size={size}>{_string}</size>";

        public static string Warning(this string _string) => $"{"WARNING:".Bold().Color(UnityEngine.Color.yellow)} {_string.Italic()}";

        public static string Error(this string _string) => $"{"ERROR:".Bold().Color(UnityEngine.Color.red)} {_string.Italic()}";

        public static string Capitalize(this string _string) {
            if (string.IsNullOrEmpty(_string)) return _string;
            string normalizedString = "";
            char lastChar = ' ';
            foreach (char c in _string) {
                char newChar = c;
                if (lastChar == ' ') {
                    newChar = char.ToUpper(c);
                }
                normalizedString += newChar;
                lastChar = newChar;
            }
            return normalizedString;
        }

    }

    public static class ListExtend {
        public static Task UniTask { get; private set; }

        public static T GetRandom<T>(this IEnumerable<T> enumerable) => enumerable.ElementAt(enumerable.Count().RangeBy0());
        public static T GetRandom<T>(this IEnumerable<T> enumerable, params T[] excluded) {
            if (enumerable.Count() == 1) {
                return enumerable.ElementAt(enumerable.Count().RangeBy0());
            }
            var a = enumerable.ToList();
            if (excluded.Count() > 0) {
                foreach (var ex in excluded) {
                    a.Remove(ex);
                }
            }

            return a[a.Count().RangeBy0()];
        }

        public static void Shuffle<T>(this IList<T> list) {

            int n = list.Count;

            while (n > 1) {

                n--;
                int k = UnityEngine.Random.Range(0, n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;

            }

        }

        public static List<T> Reverse<T>(this List<T> list) {
            if (list.IsNullOrEmpty()) return list;
            List<T> newList = new(list);
            for (int i = 0; i < list.Count; i++) {
                list[i] = newList[list.Count - i - 1];
            }
            return list;
        }

        public static TType FindType<TList, TType>(this List<TList> list) where TType : class, TList where TList : class => list?.Find(x => x is TType) as TType;
        public static List<TType> FindAllType<TList, TType>(this List<TList> list) where TType : class, TList => list.OfType<TType>().ToList();

        public static bool TryFind<T>(this List<T> list, System.Predicate<T> match, out T foundItem) where T : class {
            foundItem = list.Find(match);
            return foundItem != null;
        }
        public static bool TryFind<T>(this List<T> list, System.Predicate<T> match, out int foundItem) {
            foundItem = list.FindIndex(match);
            return foundItem != -1;
        }

        public static List<T> Without<T>(this List<T> list, params T[] item) {
            List<T> newList = new List<T>(list);
            newList.RemoveAll(item);
            return newList;
        }
        public static List<T> Without<T>(this List<T> list, IEnumerable<T> itens) {
            List<T> newList = new List<T>(list);
            newList.RemoveAll(itens);
            return newList;
        }

        public static List<T> RemoveAll<T>(this List<T> list, params T[] item) {
            if (item is null) return list;
            if (item.Length <= 0) return list;
            list.RemoveAll(x => item.Contains(x));
            return list;
        }
        public static List<T> RemoveAll<T>(this List<T> list, IEnumerable<T> itens) {
            if (itens is null) return list;
            list.RemoveAll(x => itens.Contains(x));
            return list;
        }

        public static List<T> RemoveDoubleItens<T>(this List<T> list) {
            List<T> newList = new();
            for (int i = 0; i < list.Count; i++) {
                T item = list[i];
                if (newList.Contains(item)) continue;
                newList.Add(item);
            }
            list.Clear();
            list.AddRange(newList);
            return list;
        }

        public static List<T> Override<T>(this List<T> list, T itemToOverride, T newItem) {
            if (list.IsNullOrEmpty()) return list;
            if (list.Contains(itemToOverride)) {
                int index = list.IndexOf(itemToOverride);
                list[index] = newItem;
            }
            return list;
        }

        public static List<T> AddOnce<T>(this List<T> list, T item) {
            if (!list.Contains(item)) list.Add(item);
            return list;
        }

        public static bool TryAddOnce<T>(this List<T> list, T item)
        {
            if (!list.Contains(item)) { list.Add(item); return true; }
            return false;
        }

        public static List<T> AddRangeOnce<T>(this List<T> list, IEnumerable<T> item) {
            foreach (T i in item) {
                list.AddOnce(i);
            }
            return list;
        }

        public static int CountItens<T>(this List<T> list, System.Predicate<T> match) => list.FindAll(match).Count;

        public static bool IsNullOrEmpty<T>(this List<T> list) => list.IsNull() || list.IsEmpty();

        public static bool IsEmpty<T>(this List<T> list) => list.Count <= 0;

        public static bool IsNull<T>(this List<T> list) => list == null;

        public static bool IsNullOrEmpty<T>(this T[] array) {
            if (array == null) return true;
            if (array.Length <= 0) return true;
            return false;
        }

        public static int FixIndex<T>(this List<T> list, int index)
        {
            if (list == null) return 0;
            int count = list.Count;
            if (count == 0) return 0; // ou lançar exceção
            return ((index % count) + count) % count;
        }

        public static async void AddAndRemoveAfter<T>(this List<T> list, T item, float removeDelay, CancellationToken token)
        {
            list.Add(item);
            if (await Cysharp.Threading.Tasks.UniTask.WaitForSeconds(removeDelay, cancellationToken: token).SuppressCancellationThrow()) return;
            list.Remove(item);
        }

        public static async void RemoveAfter<T>(this List<T> list, T item, float delay, CancellationToken token)
        {
            if (await Cysharp.Threading.Tasks.UniTask.WaitForSeconds(delay, cancellationToken: token).SuppressCancellationThrow()) return;
            list.Remove(item);
        }
    }

    public static class ColorExtend {

        public static Color GetRandomColor() => Random.ColorHSV();

        public static Color SetAlpha(this Color color, float alpha) {
            color.a = Mathf.Clamp01(alpha);
            return color;
        }

        public static Color Set(this Color color, float? r = null, float? g = null, float? b = null, float? a = null) {
            color.r = Mathf.Clamp01(r ?? color.r);
            color.g = Mathf.Clamp01(g ?? color.g);
            color.b = Mathf.Clamp01(b ?? color.b);
            color.a = Mathf.Clamp01(a ?? color.a);
            return color;
        }

        public static string ToHexString(this Color color) {
            return
                ((byte)(color.r * 255)).ToString("X2") +
                ((byte)(color.g * 255)).ToString("X2") +
                ((byte)(color.b * 255)).ToString("X2") +
                ((byte)(color.a * 255)).ToString("X2");
        }

    }

    public static class GameObjectExtensions {

        private static bool Requires(System.Type obj, System.Type requirement) {
            return System.Attribute.IsDefined(obj, typeof(RequireComponent)) &&
                   System.Attribute.GetCustomAttributes(obj, typeof(RequireComponent)).OfType<RequireComponent>()
                   .Any(rc => rc.m_Type0.IsAssignableFrom(requirement));
        }

        public static bool CanDestroy(this GameObject go, System.Type t) {
            return !go.GetComponents<Component>().Any(c => Requires(c.GetType(), t));
        }

        public static T GetOrAdd<T>(this GameObject gameObject) where T : Component {
            T component = gameObject.GetComponent<T>();
            if (!component) component = gameObject.AddComponent<T>();
            return component;
        }

        public static T OrNull<T>(this T obj) where T : Object => obj ? obj : null;

        public static List<GameObject> SortByDistance(this List<GameObject> components, Vector3 position) => components.OrderBy(x => Vector3.Distance(x.transform.position, position)).ToList();

    }

    public static class TransformExtensions {

        public static IEnumerable<Transform> Children(this Transform parent) {
            foreach (Transform child in parent) {
                yield return child;
            }
        }

        public static IEnumerable<RectTransform> ChildrenRectransform(this Transform parent) {
            foreach (Transform child in parent) {
                yield return child as RectTransform;
            }
        }

        public static void DestroyChildren(this Transform parent) => parent.PerfomActionOnChildren(child => Object.Destroy(child.gameObject));

        public static void SetChildrenActive(this Transform parent, bool active) => parent.PerfomActionOnChildren(child => child.gameObject.SetActive(active));

        public static void PerfomActionOnChildren(this Transform parent, System.Action<Transform> action) {
            for (int i = parent.childCount - 1; i >= 0; i--) {
                action?.Invoke(parent.GetChild(i));
            }
        }

        public static Transform GetMainParent(this Transform transform) {
            Transform mainParent = transform;
            while (mainParent.parent is not null) {
                mainParent = mainParent.parent;
            }
            return mainParent;
        }

        public static void ResetLocalTransformation(this Transform transform) {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public static void ResetTransformation(this Transform transform) {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public static float ReflectionAngle(this Transform _Transform, Vector2 _TargetPosition) {

            Vector2 normal = ((Vector2)_Transform.position - _TargetPosition).normalized;
            Vector2 direction = new Vector2(
                Mathf.Cos(_Transform.eulerAngles.z * Mathf.Deg2Rad),
                Mathf.Sin(_Transform.eulerAngles.z * Mathf.Deg2Rad)
            );
            Vector2 newDirection = direction - 2 * Vector2.Dot(direction, normal) * normal;

            return Mathf.Atan2(newDirection.y, newDirection.x) * Mathf.Rad2Deg;

        }

        public static void Ricochete(this Transform _Transform, Vector2 _TargetPosition) {

            _Transform.Ricochete(_TargetPosition, new FloatRange(0,0));

        }

        public static void Ricochete(this Transform _Transform, Vector2 _TargetPosition, FloatRange _ErrorOffSet) {

            _Transform.rotation = Quaternion.Euler(0, 0, _Transform.ReflectionAngle(_TargetPosition) + _ErrorOffSet.GetRandomValue());

        }

    }

    public static class ComponentExtension {
        public static List<T> SortByDistance<T>(this List<T> components, Vector3 position) where T : Component => components.OrderBy(x => Vector3.Distance(x.transform.position, position)).ToList();
    }

    [System.Serializable]
    public struct FloatRange {

        [SerializeField] float m_minValue;
        [SerializeField] float m_maxValue;

        public float m_MinValue => m_minValue;
        public float m_MaxValue => m_maxValue;

        public float this[int i] {
            get {
                switch (i) {
                    case 0: return m_minValue;
                    case 1: return m_maxValue;
                    default: throw new System.IndexOutOfRangeException();
                }
            }
        }

        public float deepness => m_maxValue - m_minValue;
        public float value => Random.Range(m_minValue, m_maxValue);

        public FloatRange(Vector2 value) {
            m_minValue = value.x;
            m_maxValue = value.y;
        }
        public FloatRange(Vector3 value) {
            m_minValue = value.x;
            m_maxValue = value.y;
        }
        public FloatRange(IntRange range) {
            m_minValue = range.m_MinValue;
            m_maxValue = range.m_MaxValue;
        }
        public FloatRange(float value) {
            m_minValue = value;
            m_maxValue = value;
        }
        public FloatRange(float _minValue, float _maxValue) {
            m_minValue = _minValue;
            m_maxValue = _maxValue;
        }

        public void Set(float? minValue = null, float? maxValue = null) {
            this.m_minValue = minValue ?? this.m_minValue;
            this.m_maxValue = maxValue ?? this.m_maxValue;
        }

        public bool InRange(float value) => value >= m_minValue && value <= m_maxValue;
        public bool InRange(int value) => value >= m_minValue && value <= m_maxValue;

        public float GetRandomValue() => value;
        public float Clamp(float value) => Mathf.Clamp(value, m_minValue, m_maxValue);
        public float Lerp(float t) => Mathf.Lerp(m_minValue, m_maxValue, t);
        public float InverseLerp(float value) => Mathf.InverseLerp(m_minValue, m_maxValue, value);
        public float LerpUnclamped(float t) => Mathf.LerpUnclamped(m_minValue, m_maxValue, t);
        public float Deepness() => deepness;

        public static FloatRange operator +(FloatRange a, FloatRange b) => new(a.m_minValue + b.m_minValue, a.m_maxValue + b.m_maxValue);
        public static FloatRange operator +(FloatRange a, float value) => new(a.m_minValue + value, a.m_maxValue + value);
        public static FloatRange operator -(FloatRange a, FloatRange b) => new(a.m_minValue - b.m_minValue, a.m_maxValue - b.m_maxValue);
        public static FloatRange operator -(FloatRange a, float value) => new(a.m_minValue - value, a.m_maxValue - value);
        public static FloatRange operator *(FloatRange a, float value) => new(a.m_minValue * value, a.m_maxValue * value);
        public static FloatRange operator /(FloatRange a, float value) => new(a.m_minValue / value, a.m_maxValue / value);

        public static bool operator <(FloatRange a, FloatRange b) => a.Deepness() < b.Deepness();
        public static bool operator >(FloatRange a, FloatRange b) => a.Deepness() > b.Deepness();
        public static bool operator <=(FloatRange a, FloatRange b) => a.Deepness() <= b.Deepness();
        public static bool operator >=(FloatRange a, FloatRange b) => a.Deepness() >= b.Deepness();

        public static implicit operator Vector2(FloatRange a) => new Vector2(a.m_minValue, a.m_maxValue);
        public static implicit operator Vector3(FloatRange a) => new Vector3(a.m_minValue, a.m_maxValue);

        public static implicit operator FloatRange(Vector2 vector) => new FloatRange(vector);
        public static implicit operator FloatRange(Vector3 vector) => new FloatRange(vector);
        public static implicit operator FloatRange(IntRange range) => new FloatRange(range);
    }

    [System.Serializable]
    public struct IntRange {
        [SerializeField] int m_minValue;
        [SerializeField] int m_maxValue;
        public int m_MinValue => m_minValue;
        public int m_MaxValue => m_maxValue;

        public IntRange(int _minValue, int _maxValue) {
            m_minValue = _minValue;
            m_maxValue = _maxValue;
        }

        public int GetRandomValue(bool _maxInclusive = false) {
            return Random.Range(m_minValue, m_maxValue + (_maxInclusive ? 1 : 0));
        }
        public bool InRange(float value) {
            return value >= m_minValue && value <= m_maxValue;
        }
        public void ChangeMinValue(int newValue) {
            m_minValue = newValue;
        }
        public void ChangeMaxValue(int newValue) {
            m_maxValue = newValue;
        }

        public int Lerp(float t) => Mathf.RoundToInt(Mathf.Lerp(m_minValue, m_maxValue, t));

    }

    [System.Serializable]
    public class Timer {

        [Header("Timer Settings")]
        [SerializeField] bool m_countdown = true;
        [SerializeField] bool m_repeater = true;
        [SerializeField] FloatRange m_startTimer = new(1, 1);
        float m_currentTimerValue;
        public float m_CurrentTimerValue => m_currentTimerValue;
        
        [SerializeField] protected UnityEvent m_OnTimerElapsed;
        [SerializeField] protected UnityEvent<float> m_OnTimePassed;

        public Timer() {
            m_OnTimerElapsed = new UnityEvent();
            m_OnTimePassed = new UnityEvent<float>();
        }

        public Timer(float time)
        {
            m_OnTimerElapsed = new UnityEvent();
            m_OnTimePassed = new UnityEvent<float>();

            m_startTimer = new FloatRange(time, time);
            SetupTimer();
            
            ActiveTimer(true);
        }

        public UnityEvent OnTimerElapsed => m_OnTimerElapsed;
        public UnityEvent<float> OnTimePassed => m_OnTimePassed;
        public FloatRange m_StartTimer => m_startTimer;
        public bool m_Countdown => m_countdown;
        public virtual void ActiveTimer(bool active) {
            m_countdown = active;
            if (active) {
                SetupTimer();
            }
        }


        public void SetStartTimer(FloatRange newStartTimer) {
            m_startTimer = newStartTimer;
        }

        public void ChangeMinStartTimerValue(float newMinValue) {
            m_startTimer.Set(minValue: newMinValue);
        }

        public void ChangeMaxStartTimerValue(float newMaxValue) {
            m_startTimer.Set(maxValue: newMaxValue);
        }

        public void ChangeStartTimerValue(float newMinValue, float newMaxValue) {
            m_startTimer.Set(minValue: newMinValue);
            m_startTimer.Set(maxValue: newMaxValue);
        }

        public void DecreaseStartTimerValue(float decreaseValue) {
            m_startTimer.Set(minValue: m_startTimer.m_MinValue - decreaseValue);
            m_startTimer.Set(m_startTimer.m_MaxValue - decreaseValue);
        }

        public void DecreaseStartTimerValue(float decreaseMinValue, float decreaseMaxValue) {
            m_startTimer.Set(minValue: m_startTimer.m_MinValue - decreaseMinValue);
            m_startTimer.Set(m_startTimer.m_MaxValue - decreaseMaxValue);
        }

        public virtual void SetupTimer() {
            m_currentTimerValue = m_startTimer.GetRandomValue();
        }

        public virtual bool TimerElapse(float deltaTime) 
        {
            if (!m_countdown) return false;
            m_currentTimerValue -= deltaTime;
            m_OnTimePassed?.Invoke(m_currentTimerValue);

            if (m_currentTimerValue <= 0) 
            {
                TimerElapsedAction();
                return true;
            }

            return false;
        }

        protected virtual void TimerElapsedAction() {
            ActiveTimer(m_repeater);
            m_OnTimerElapsed?.Invoke();
        }

    }

    public static class MigalhazHelper {
        #region Cinema Camera
        private static CinemachineBrain m_TargetCamera;
        public static CinemachineBrain TargetCamera {

            get {

                if (m_TargetCamera == null) m_TargetCamera = GameObject.FindFirstObjectByType<CinemachineBrain>();
                return m_TargetCamera;

            }
            set {
                m_TargetCamera = value;
            }

        }

        #endregion

        #region Camera
        static Camera m_mainCamera;
        public static Camera m_MainCamera {
            get {
                if (m_mainCamera == null) {
                    m_mainCamera = Camera.main;
                }
                return m_mainCamera;
            }
        }

        static Camera m_currentCamera;
        public static Camera m_CurrentCamera {
            get {
                if (m_currentCamera == null) {
                    m_currentCamera = Camera.current;
                }
                return m_currentCamera;
            }
        }
        #endregion

        #region WaitForSeconds
        static Dictionary<float, WaitForSeconds> m_waitForSecondsDictionary = new Dictionary<float, WaitForSeconds>();
        public static WaitForSeconds GetWaitForSeconds(float seconds) {
            if (m_waitForSecondsDictionary.TryGetValue(seconds, out WaitForSeconds wait)) return wait;
            m_waitForSecondsDictionary[seconds] = new WaitForSeconds(seconds);
            return m_waitForSecondsDictionary[seconds];
        }
        #endregion

        #region MouseOverUI
        static PointerEventData m_eventDataCurrentPos;
        static List<RaycastResult> m_results;
        static EventSystem m_currentEventSystem;

        public static EventSystem m_CurrentEventSystem {
            get {
                if (m_currentEventSystem is null) m_currentEventSystem = EventSystem.current;
                return m_currentEventSystem;
            }
        }
        public static bool IsOverUI() {
            m_eventDataCurrentPos = new PointerEventData(m_CurrentEventSystem) {
                position = Input.mousePosition
            };
            m_results = new List<RaycastResult>();
            m_CurrentEventSystem.RaycastAll(m_eventDataCurrentPos, m_results);
            return m_results.Count > 0;
        }
        #endregion

        #region RectTransformPosition
        public static Vector2 GetWorldPositionOfCanvasElement(RectTransform element) {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, m_MainCamera, out Vector3 worldPoint);
            return worldPoint;
        }
        public static Vector2 GetCanvasPositionOfWorldElement(GameObject element) {
            return RectTransformUtility.WorldToScreenPoint(m_MainCamera, element.transform.position);
        }

        #endregion

        #region CacheGetComponent
        public static T CacheGetComponent<T>(this GameObject obj, ref T component) {
            if (component is not null) return component;
            bool get = obj.TryGetComponent(out T getComponent);
            if (get) {
                component = getComponent;
            }
            return component;
        }

        public static T CacheGetComponent<T>(this Component obj, ref T component) {
            if (component is not null) return component;
            bool get = obj.TryGetComponent(out T getComponent);
            if (get) {
                component = getComponent;
            }
            return component;
        }
        #endregion
    }
}