using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Kdd.UIKit.Forms.Extensions
{
    /// <summary>
    /// I know this is evil, but it's the only option to not fully use the source code instead of packages(
    /// </summary>
    public static class ReflectionExtensions
    {
        private static readonly Dictionary<string, MethodInfo> _methodInfoStorage = new Dictionary<string, MethodInfo>();

        public static TResult InvokeNonPublicStaticMethod<TResult>(this Type classOwnerType, (Type, object)[] parameters = null, [CallerMemberName] string methodName = null)
        {
            var result = InvokeNonPublicStaticMethodInternal(classOwnerType, parameters, methodName);

            if (result is null)
            {
                return default;
            }

            return (TResult)result;
        }

        public static void InvokeNonPublicStaticMethod(this Type classOwnerType, (Type, object)[] parameters = null, [CallerMemberName] string methodName = null)
        {
            InvokeNonPublicStaticMethodInternal(classOwnerType, parameters, methodName);
        }

        private static object InvokeNonPublicStaticMethodInternal(Type classOwnerType, (Type Type, object Obj)[] parameters, string methodName)
        {
            parameters ??= Array.Empty<(Type, object)>();
            var parameterTypes = parameters?.Select(parameter => parameter.Type).ToArray();
            var parameterTypesKeyPart = string.Join(", ", parameterTypes.Select(type => type.FullName));
            var currentMethodKey = $"{classOwnerType.FullName}.{methodName}({parameterTypesKeyPart})";

            if (!_methodInfoStorage.ContainsKey(currentMethodKey))
            {
                var newMethodInfo = classOwnerType.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic, null, parameterTypes, null);
                _methodInfoStorage[currentMethodKey] = newMethodInfo;
            }

            var parameterObjects = parameters.Select(parameter => parameter.Obj).ToArray();
            var methodInfo = _methodInfoStorage[currentMethodKey];
            var result = methodInfo.Invoke(null, parameterObjects);
            return result;
        }

        public static (Type, object)[] ProduceParameters(this Type _, params (Type, object)[] parameters)
        {
            return parameters;
        }
    }
}
