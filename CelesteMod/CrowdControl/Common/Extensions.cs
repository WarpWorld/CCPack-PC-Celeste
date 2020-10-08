using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CrowdControl.Common
{
    [UsedImplicitly]
    internal static class Extensions
    {
        [UsedImplicitly]
        public static bool TryGetValue<T>([NotNull] this SerializationInfo info, [NotNull] string name, [NotNull] Type type, [CanBeNull] out T value)
        {
            try
            {
                foreach (SerializationEntry entry in info)
                {
                    if (entry.Name?.Equals(name, StringComparison.Ordinal) ?? false)
                    {
                        value = (T)info.GetValue(name, type);
                        return true;
                    }
                }
            }
            catch (Exception ex) { Log.Error(ex); } // it still might throw if the type is wrong
            value = default;
            return false;
        }

        [UsedImplicitly]
        public static bool IsNumeric([NotNull] this string value) => long.TryParse(value, out long result) && result >= 0;

        [UsedImplicitly, CanBeNull]
        public static V Get<K, V>([NotNull] this Dictionary<K, V> dictionary, [NotNull] K key, [CanBeNull] V defaultValue)
            => dictionary.ContainsKey(key) ? dictionary[key] : defaultValue;

        [UsedImplicitly, CanBeNull]
        public static byte[] ToUTF8EncodedJsonObject<T>([CanBeNull] this T obj)
            => obj == null ? null : Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj)??string.Empty);

        [UsedImplicitly, CanBeNull]
        public static T FromUTF8EncodedJSONObject<T>([CanBeNull] this byte[] bytes)
            => bytes == null ? default : JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes));

        public static T[] ToArray<T>(this ArraySegment<T> values)
        {
            List<T> result = new List<T>();
            T[] array = values.Array;
            int offset = values.Offset;
            int count = values.Count;
            for (int i = 0; i < count; i++) { result.Add(array[offset + i]); }
            return result.ToArray();
        }

        public static bool HasFlag(this Enum variable, Enum value)
        {
            if (variable.GetType() != value.GetType())
            {
                throw new ArgumentException("The checked flag is not from the same type as the checked variable.");
            }

            ulong num = Convert.ToUInt64(value);
            ulong num2 = Convert.ToUInt64(variable);

            return (num2 & num) == num;
        }

        private static readonly DateTimeOffset EPOCH = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        public static long ToUnixTimeSeconds(this DateTimeOffset value) => ((long)(value.ToUniversalTime() - EPOCH).TotalSeconds);

        public static DateTimeOffset FromUnixTimeSeconds(this long value) => (EPOCH + TimeSpan.FromSeconds(value));

        [UsedImplicitly, CanBeNull]
        public static T MaxBy<T, R>([NotNull, ItemNotNull] this IEnumerable<T> en, [NotNull] Func<T, R> evaluate) where R : IComparable<R>
        {
            return en.Select(t => new Tuple<T, R>(t, evaluate(t)))
                     .Aggregate((max, next) => next.Item2.CompareTo(max.Item2) > 0 ? next : max).Item1;
        }

        [UsedImplicitly, CanBeNull]
        public static T MinBy<T, R>([NotNull, ItemNotNull] this IEnumerable<T> en, [NotNull] Func<T, R> evaluate) where R : IComparable<R>
        {
            return en.Select(t => new Tuple<T, R>(t, evaluate(t)))
                     .Aggregate((max, next) => next.Item2.CompareTo(max.Item2) < 0 ? next : max).Item1;
        }

        [NotNull]
        public static byte[] GetBytes([NotNull] this Stream stream)
        {
            try { if (stream.CanSeek) { stream.Seek(0, SeekOrigin.Begin); } }
            catch { /**/ }
            using MemoryStream temp = new MemoryStream();
            stream.CopyTo(temp);
            return temp.ToArray();
        }
    }
}
