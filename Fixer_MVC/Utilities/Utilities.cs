using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System;
using System.Windows.Forms;

namespace Fixer_MVC.Utilities
{

    public static class Utility
    {
        /// <summary>Indicates whether the specified array is null or has a length of zero.</summary>
        /// <param name="array">The array to test.</param>
        /// <returns>true if the array parameter is null or has a length of zero; otherwise, false.</returns>
        public static bool IsNullOrEmpty(this Array array)
        {
            return (array == null || array.Length == 0);
        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().Location;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static Dictionary<string, string> LoadConfig(string settingfile)
        {
            var dic = new Dictionary<string, string>();

            if (File.Exists(settingfile))
            {
                var settingdata = File.ReadAllLines(settingfile);
                for (var i = 0; i < settingdata.Length; i++)
                {
                    var setting = settingdata[i];
                    var sidx = setting.IndexOf("=");
                    if (sidx >= 0)
                    {
                        var skey = setting.Substring(0, sidx);
                        var svalue = setting.Substring(sidx + 1);
                        if (!dic.ContainsKey(skey))
                        {
                            dic.Add(skey, svalue);
                        }
                    }
                }
            }

            return dic;
        }

    }

    public class GenericFactory<T> where T : MyAbstractType
    {
        public static T GetInstancePrivate()
        {
            return (T)Activator.CreateInstance(typeof(T), true);
        }

        public static T1 GetInstancePublic<T1>() where T1 : new()
        {
            return new T1();
        }
    }

    public class MyAbstractType
    {
    }

    public static class ExtensionUtilities
    {
        /// <summary>
        /// Get message recursive
        /// </summary>
        /// <param name="exc"></param>
        /// <returns></returns>
        public static string GetFullMessage(this Exception exc)
        {
            if (exc == null)
                return string.Empty;

            return GetMessageRecursive(exc);

        }

        private static string GetMessageRecursive(Exception exc)
        {
            if (exc.InnerException == null)
            {
                return exc.Message;
            }
            else
            {
                return exc.Message + ", " + GetMessageRecursive(exc.InnerException);
            }
        }
    }

    public static class LoggerExtensions
    {
        public static Serilog.ILogger Here(this Serilog.ILogger logger,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0) => logger
                .ForContext("MemberName", memberName)
                .ForContext("FilePath", sourceFilePath)
                .ForContext("LineNumber", sourceLineNumber);
    }

    public static class IEnumerableLinqExtensions
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int size)
        {
            T[] array = null;
            int count = 0;
            foreach (T item in source)
            {
                if (array == null)
                {
                    array = new T[size];
                }
                array[count] = item;
                count++;
                if (count == size)
                {
                    yield return new ReadOnlyCollection<T>(array);
                    array = null;
                    count = 0;
                }
            }
            if (array != null)
            {
                Array.Resize(ref array, count);
                yield return new ReadOnlyCollection<T>(array);
            }
        }

        public static IEnumerable<T> MyWhere<T>(this IEnumerable<T> data, Func<T, bool> predicate)
        {
            foreach (T value in data)
            {
                if (predicate(value)) yield return value;
            }
        }

        public static IEnumerable<T> GetNth<T>(this List<T> list, int n)
        {
            for (int i = 0; i < list.Count; i += n)
                yield return list[i];
        }

        public static IEnumerable<T> Sample<T>(this IEnumerable<T> sourceSequence, int interval)
        {
            int index = 0;
            foreach (T item in sourceSequence)
            {
                if (index++ % interval == 0)
                    yield return item;
            }
        }
    }

    public static class WinFormsExtensions
    {
        public static void InvokeIfRequired(this Control control, MethodInvoker action)
        {
            // See Update 2 for edits Mike de Klerk suggests to insert here.

            if (control.InvokeRequired)
            {
                control.Invoke(action);
            }
            else
            {
                action();
            }
        }
    }

    public static class Objects
    {
        /// <summary>
        /// Copies all the matching properties and fields from 'source' to 'destination'
        /// </summary>
        /// <param name="source">The source object to copy from</param>  
        /// <param name="destination">The destination object to copy to</param>
        public static void CopyPropsTo<T1, T2>(this T1 source, ref T2 destination)
        {
            var sourceMembers = GetMembers(source.GetType());
            var destinationMembers = GetMembers(destination.GetType());

            // Copy data from source to destination
            foreach (var sourceMember in sourceMembers)
            {
                if (!CanRead(sourceMember))
                {
                    continue;
                }
                var destinationMember = destinationMembers.FirstOrDefault(x => x.Name.ToLower() == sourceMember.Name.ToLower());
                if (destinationMember == null || !CanWrite(destinationMember))
                {
                    continue;
                }
                SetObjectValue(ref destination, destinationMember, GetMemberValue(source, sourceMember));
            }
        }

        private static void SetObjectValue<T>(ref T obj, System.Reflection.MemberInfo member, object value)
        {
            // Boxing method used for modifying structures
            var boxed = obj.GetType().IsValueType ? (object)obj : obj;
            SetMemberValue(ref boxed, member, value);
            obj = (T)boxed;
        }

        private static void SetMemberValue<T>(ref T obj, System.Reflection.MemberInfo member, object value)
        {
            if (IsProperty(member))
            {
                var prop = (System.Reflection.PropertyInfo)member;
                if (prop.SetMethod != null)
                {
                    prop.SetValue(obj, value);
                }
            }
            else if (IsField(member))
            {
                var field = (System.Reflection.FieldInfo)member;
                field.SetValue(obj, value);
            }
        }

        private static object GetMemberValue(object obj, System.Reflection.MemberInfo member)
        {
            object result = null;
            if (IsProperty(member))
            {
                var prop = (System.Reflection.PropertyInfo)member;
                result = prop.GetValue(obj, prop.GetIndexParameters().Count() == 1 ? new object[] { null } : null);
            }
            else if (IsField(member))
            {
                var field = (System.Reflection.FieldInfo)member;
                result = field.GetValue(obj);
            }
            return result;
        }

        private static bool CanWrite(System.Reflection.MemberInfo member)
        {
            return IsProperty(member) ? ((System.Reflection.PropertyInfo)member).CanWrite : IsField(member);
        }

        private static bool CanRead(System.Reflection.MemberInfo member)
        {
            return IsProperty(member) ? ((System.Reflection.PropertyInfo)member).CanRead : IsField(member);
        }

        private static bool IsProperty(System.Reflection.MemberInfo member)
        {
            return IsType(member.GetType(), typeof(System.Reflection.PropertyInfo));
        }

        private static bool IsField(System.Reflection.MemberInfo member)
        {
            return IsType(member.GetType(), typeof(System.Reflection.FieldInfo));
        }

        private static bool IsType(System.Type type, System.Type targetType)
        {
            return type.Equals(targetType) || type.IsSubclassOf(targetType);
        }

        private static List<System.Reflection.MemberInfo> GetMembers(System.Type type)
        {
            var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.NonPublic;
            var members = new List<System.Reflection.MemberInfo>();
            members.AddRange(type.GetProperties(flags));
            members.AddRange(type.GetFields(flags));
            return members;
        }
    }

}
