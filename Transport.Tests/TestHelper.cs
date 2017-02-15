using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Transport.Tests
{
    public partial class TestHelper
    {
        private static volatile object _syncRoot = new object();
        private static List<Type> _types;
        private static Random _random = new Random((int)DateTime.Now.Ticks);

        #region Common часть TestHelper'а

        private static readonly DateTime DateDefault = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        private static readonly DateTime TimeDefault = new DateTime(DateTime.MinValue.Year, DateTime.MinValue.Month, DateTime.MinValue.Day);

        private static int _nextInt32 = 1;
        private static byte _nextByte;
        private static ushort _nextUShort;
        private static int _nextVersion;
        private static DateTime _dateTime = DateTime.Now;
        private static DateTime _date = DateDefault;
        private static DateTime _time = TimeDefault;
        private static bool _bool;

        public static long NextInt64()
        {
            return NextInt32();
        }

        public static long NextInt64(int min, int max)
        {
            return NextInt32(min, max);
        }

        public static decimal NextDecimal()
        {
            return (decimal)NextInt32();
        }

        public static decimal NextDecimal(int min, int max)
        {
            return (decimal)NextInt32(min, max);
        }

        public static uint? NextNullableUInt32()
        {
            return (uint)NextInt32();
        }

        public static uint? NextNullableUInt32(int min, int max)
        {
            return (uint)NextInt32(min, max);
        }

        public static uint NextUInt32()
        {
            return (uint)NextInt32();
        }

        public static uint NextUInt32(int min, int max)
        {
            return (uint)NextInt32(min, max);
        }

        public static int NextInt32()
        {
            unchecked
            {
                return ++_nextInt32;
            }
        }

        public static int NextInt32(int min, int max)
        {
            if (min == max)
                return min;

            return ((NextInt32() % (max - min)) + min);
        }

        public static int NextInt32(IList<int> ints)
        {
            if (ints.Count == 0)
                return NextInt32();

            var index = NextInt32(0, ints.Count - 1);

            return ints[index];
        }

        public static byte NextByte()
        {
            unchecked
            {
                return ++_nextByte;
            }
        }

        public static byte NextByte(byte min, byte max)
        {
            return (byte)((NextByte() % (max - min)) + min);
        }

        public static ushort NextUShort()
        {
            unchecked
            {
                return ++_nextUShort;
            }
        }

        public static ushort NextUShort(ushort min, ushort max)
        {
            return (ushort)((NextUShort() % (max - min)) + min);
        }

        public static Version NextVersion()
        {
            return new Version(++_nextVersion, ++_nextVersion, ++_nextVersion, ++_nextVersion);
        }

        public static DateTime NextDate()
        {
            _date = DateDefault.AddDays(NextInt32());
            return _date;
        }

        public static DateTime NextTime()
        {
            _time = TimeDefault.AddSeconds(NextInt32(1, 24 * 60 * 60 - 1));
            return _time;
        }

        public static DateTime NextDateTime()
        {
            _dateTime = _dateTime.Add(NextTimeSpan());
            return _dateTime;
        }

        public static bool NextBool()
        {
            _bool = !_bool;
            return _bool;
        }

        public static Guid NextGuid()
        {
            return Guid.NewGuid();
        }

        public static TimeSpan NextTimeSpan()
        {
            return TimeSpan.FromSeconds(NextInt32());
        }

        public static string NextString()
        {
            return "Неважная строка #" + NextInt32();
        }

        public static string NextString(int seed)
        {
            return "Неважная сидированная строка #" + seed;
        }

        public static string NextString(string format)
        {
            return string.Format(format, NextInt32());
        }

        public static string NextMailBox()
        {
            return String.Format("somerec{0}@somehost{1}.com", NextInt32(), NextInt32());
        }

        public static int RandomInt32(int min, int max)
        {
            return _random.Next(min, max);
        }

        private static T NextEnum<T>()
        {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new InvalidOperationException(String.Format("Тип '{0}' не является перечислимым.", type.FullName));

            var values = type.GetEnumValues();
            var index = RandomInt32(0, values.Length);
            var value = values.GetValue(index);

            return (T)value;
        }

        public static object RunStaticMethod(Type type, string methodName, object[] objParams)
        {
            BindingFlags eFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

            return RunMethod(type, methodName, null, objParams, eFlags);
        }

        public static object RunInstanceMethod(Type type, string methodName, object instance, object[] objParams)
        {
            BindingFlags eFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            return RunMethod(type, methodName, instance, objParams, eFlags);
        }

        public static object RunMethod(Type type, string methodName, object instance, object[] objParams, BindingFlags bindingFlags)
        {
            Type[] types = objParams.Select(obj => obj == null ? (Type)null : obj.GetType()).ToArray();
            var method = type.GetMethod(methodName, bindingFlags, Type.DefaultBinder, types, new ParameterModifier[] { });
            if (method == null)
                throw new ArgumentException(String.Format("У типа '{0}' отсутствует метод '{1}'.'", type, methodName));

            object objRet = method.Invoke(instance, objParams);

            return objRet;
        }

        public static string NextTempFullFileName()
        {
            return Path.GetTempFileName();
        }

        public static void DeleteIfExits(string fullFileName)
        {
            if (File.Exists(fullFileName))
                File.Delete(fullFileName);
        }

        public static byte[] JoinBytes(params byte[][] bytes)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream, Encoding.UTF8))
            {
                foreach (var b in bytes)
                {
                    writer.Write(b);
                }

                return stream.ToArray();
            }
        }

        public static void MustThrowArgumentNullException(Action action, string variableName)
        {
            try
            {
                action.Invoke();
                Assert.Fail("Must throw!");
            }
            catch (ArgumentNullException exception)
            {
                Assert.IsTrue(exception.Message.Contains(variableName), exception.Message);
            }
        }

        public static void MustThrowArgumentOutOfRangeException(Action action, string variableName)
        {
            try
            {
                action.Invoke();
                Assert.Fail("Must throw!");
            }
            catch (ArgumentOutOfRangeException exception)
            {
                Assert.IsTrue(exception.Message.Contains(variableName), exception.Message);
            }
        }

        public static void MustThrowException(Action action, string variableName)
        {
            try
            {
                action.Invoke();
                Assert.Fail("Must throw!");
            }
            catch (Exception exception)
            {
                Assert.IsTrue(exception.Message.Contains(variableName), exception.Message);
            }

        }

        #region ObjectDumper

        public static string PrintObjects0(object expected, object actual)
        {
            return string.Format("Expected: <{0}>,\r\nActual: <{1}>.", DumpObject(expected, 0), DumpObject(actual, 0));
        }

        public static string PrintObjects1(object expected, object actual)
        {
            return string.Format("Expected: <{0}>,\r\nActual: <{1}>.", DumpObject(expected, 1), DumpObject(actual, 1));
        }

        public static string PrintObjects10(object expected, object actual)
        {
            return string.Format("Expected: <{0}>,\r\nActual: <{1}>.", DumpObject(expected, 10), DumpObject(actual, 10));
        }

        public static string DumpObject(object element, int depth)
        {
            if (element == null)
                return null;

            using (var writer = new StringWriter())
            {
                ObjectDumper.Write(element, depth, writer);

                return writer.ToString();
            }
        }

        private class ObjectDumper
        {
            ////public static void Write(object element)
            ////{
            ////    Write(element, 0);
            ////}

            ////public static void Write(object element, int depth)
            ////{
            ////    Write(element, depth, Console.Out);
            ////}

            public static void Write(object element, int depth, TextWriter log)
            {
                ObjectDumper dumper = new ObjectDumper(depth);
                dumper.writer = log;
                dumper.WriteObject(null, element);
            }

            TextWriter writer;
            int pos;
            int level;
            int depth;

            private ObjectDumper(int depth)
            {
                this.depth = depth;
            }

            private void Write(string s)
            {
                if (s != null)
                {
                    writer.Write(s);
                    pos += s.Length;
                }
            }

            private void WriteIndent()
            {
                for (int i = 0; i < level; i++) writer.Write("  ");
            }

            private void WriteLine()
            {
                writer.WriteLine();
                pos = 0;
            }

            private void WriteTab()
            {
                Write("  ");
                while (pos % 8 != 0) Write(" ");
            }

            private void WriteObject(string prefix, object element)
            {
                if (element == null || element is ValueType || element is string)
                {
                    WriteIndent();
                    Write(prefix);
                    WriteValue(element);
                    WriteLine();
                }
                else
                {
                    IEnumerable enumerableElement = element as IEnumerable;
                    if (enumerableElement != null)
                    {
                        foreach (object item in enumerableElement)
                        {
                            if (item is IEnumerable && !(item is string))
                            {
                                WriteIndent();
                                Write(prefix);
                                Write("...");
                                WriteLine();
                                if (level < depth)
                                {
                                    level++;
                                    WriteObject(prefix, item);
                                    level--;
                                }
                            }
                            else
                            {
                                WriteObject(prefix, item);
                            }
                        }
                    }
                    else
                    {
                        MemberInfo[] members = element.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
                        WriteIndent();
                        Write(prefix);
                        bool propWritten = false;
                        foreach (MemberInfo m in members)
                        {
                            FieldInfo f = m as FieldInfo;
                            PropertyInfo p = m as PropertyInfo;
                            if (f != null || p != null)
                            {
                                if (propWritten)
                                {
                                    WriteTab();
                                }
                                else
                                {
                                    propWritten = true;
                                }
                                Write(m.Name);
                                Write("=");
                                Type t = f != null ? f.FieldType : p.PropertyType;
                                if (t.IsValueType || t == typeof(string))
                                {
                                    WriteValue(f != null ? f.GetValue(element) : p.GetValue(element, null));
                                }
                                else
                                {
                                    if (typeof(IEnumerable).IsAssignableFrom(t))
                                    {
                                        Write("...");
                                    }
                                    else
                                    {
                                        Write("{ }");
                                    }
                                }
                            }
                        }
                        if (propWritten) WriteLine();
                        if (level < depth)
                        {
                            foreach (MemberInfo m in members)
                            {
                                FieldInfo f = m as FieldInfo;
                                PropertyInfo p = m as PropertyInfo;
                                if (f != null || p != null)
                                {
                                    Type t = f != null ? f.FieldType : p.PropertyType;
                                    if (!(t.IsValueType || t == typeof(string)))
                                    {
                                        object value = f != null ? f.GetValue(element) : p.GetValue(element, null);
                                        if (value != null)
                                        {
                                            level++;
                                            WriteObject(m.Name + ": ", value);
                                            level--;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            private void WriteValue(object o)
            {
                if (o == null)
                {
                    Write("null");
                }
                else if (o is DateTime)
                {
                    Write(((DateTime)o).ToShortDateString());
                }
                else if (o is ValueType || o is string)
                {
                    Write(o.ToString());
                }
                else if (o is IEnumerable)
                {
                    Write("...");
                }
                else
                {
                    Write("{ }");
                }
            }
        }

        #endregion

        #endregion

        public static object Create(Type type)
        {
            var types = _types;
            if (types == null)
            {
                lock (_syncRoot)
                {
                    if (_types == null)
                        _types = GetTypes();

                    types = _types;
                }
            }

            var method = types.SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
                              .Where(o => (o.ReturnType == type && o.Name.StartsWith("Next"))
                                          || (o.ReturnType == type && o.Name.StartsWith("Create") && o.GetParameters()
                                                                                                      .Length == 0))
                              .FirstOrDefault();

            if (method == null)
                return null;

            return method.Invoke(null, BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder, null, CultureInfo.InvariantCulture);
        }

        private static List<Type> GetTypes()
        {
            var assembly = Assembly.GetAssembly(typeof(TestHelper));

            var a = assembly.GetTypes().ToList();

            return assembly.GetTypes()
                           .Where(o => o.FullName.Contains("TestHelper"))
                           .ToList();
        }
    }
}
