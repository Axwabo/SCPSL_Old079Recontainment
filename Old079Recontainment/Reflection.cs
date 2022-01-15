using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace Axwabo.Util {
    public static class Reflection {
        public static T Get<T>(this object obj, string field) {
            return (T) Get(obj, field);
        }

        public static object Get(this object obj, string field) {
            return obj.GetType().Get(obj, field);
        }

        public static void Set(this object obj, string field, object val) {
            obj.GetType().Set(obj, field, val);
        }

        public static T Call<T>(this object obj, string methodName, params object[] args) {
            return obj.GetType().Call<T>(obj, methodName, args);
        }

        public static void Call(this object obj, string methodName, params object[] args) {
            Call<object>(obj, methodName, args);
        }

        public static T StaticGet<T>(this Type type, string name) {
            return (T) StaticGet(type, name);
        }

        public static object StaticGet(this Type type, string name) {
            var field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return field != null ? field.GetValue(null) : default;
        }

        public static void StaticSet(this Type type, string field, object val) {
            var fieldInfo = type.GetField(field, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (fieldInfo != null)
                fieldInfo.SetValue(null, val);
        }

        public static T StaticCall<T>(this Type type, string methodName, params object[] args) {
            var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return method != null ? (T) method.Invoke(null, args) : default;
        }

        public static void StaticCall(this Type type, string methodName, params object[] args) {
            StaticCall<object>(type, methodName, args);
        }


        public static T Get<T>(this Type type, object obj, string field) {
            return (T) Get(type, obj, field);
        }

        public static object Get(this Type type, object obj, string field) {
            var fieldInfo = type.GetField(field,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance);
            return fieldInfo?.GetValue(obj);
        }

        public static void Set(this Type type, object obj, string field, object val) {
            var fieldInfo = type.GetField(field,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance);
            if (fieldInfo != null)
                fieldInfo.SetValue(obj, val);
        }

        public static T Call<T>(this Type type, object obj, string methodName, params object[] args) {
            var method = type.GetMethod(methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance);
            return method != null ? (T) method.Invoke(obj, args) : default;
        }

        public static void Call(this Type type, object obj, string methodName, params object[] args) {
            Call<object>(type, obj, methodName, args);
        }

        public static IEnumerable<T> MapUnknownTypeArray<T>(this object obj, string fieldName, string innerField,
            Func<object, T> mapper) {
            return obj.MapUnknownTypeArray(fieldName, o => mapper(o.Get(innerField)));
        }

        public static IEnumerable<T> MapUnknownTypeArray<T>(this object obj, string fieldName, Func<object, T> mapper) {
            return (from object o in obj.Get<IEnumerable>(fieldName) select mapper(o)).ToList();
        }

        public static IEnumerable<T> Enums<T>(this Type type) {
            return Enum.GetValues(type).ToArray<T>();
        }

        public static T Construct<T>(this Type type, params object[] args) {
            var constructor = type.GetConstructors().FirstOrDefault(e => {
                var list = e.GetParameters().AsEnumerable().ToList();
                if (args.Length != list.Count)
                    return false;
                for (var i = 0; i < list.Count; i++) {
                    var o = args[i];
                    if (o != null && !list[i].ParameterType.IsInstanceOfType(o))
                        return false;
                }

                return true;
            });
            return constructor == null ? default : (T) constructor.Invoke(args);
        }
    }
}