using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NagaisoraFramework
{
	public class AssemblySystem
	{
		public Assembly Assembly;

		public AssemblyName Name => Assembly.GetName();

		public List<TypeInfo> TypeInfos;

		public AssemblySystem(Assembly assembly)
		{
			Assembly = assembly;

			TypeInfos = new List<TypeInfo>();

			foreach (Type type in ListAllExportedType())
			{
				TypeInfos.Add(new TypeInfo(type));
			}
		}

		public AssemblySystem(byte[] binary) : this(AppDomain.CurrentDomain.Load(binary))
		{

		}

		public object EntryPointRun(object obj, params object[] args)
		{
			return Assembly.EntryPoint.Invoke(obj, args);
		}

		public Type[] ListAllExportedType()
		{
			return Assembly.GetExportedTypes();
		}

		public Type GetAssemblyType(string name)
		{
			return Assembly.GetType(name);
		}

		public static void SetProperty(Type type, object instance, string name, object value, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
		{
			PropertyInfo info = type.GetProperty(name, bindingFlags) ?? throw new Exception($"获取不到属性 {name}");

			info.SetValue(instance, value);

			return;
		}

		public static object GetProperty(Type type, object instance, string name, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
		{
			PropertyInfo info = type.GetProperty(name, bindingFlags);

			return info is null ? throw new Exception($"获取不到属性 {name}") : info.GetValue(instance);
		}

		public static void SetField(Type type, object instance, string name, object value, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
		{
			FieldInfo info = type.GetField(name, bindingFlags) ?? throw new Exception($"获取不到字段 {name}");

			info.SetValue(instance, value);

			return;
		}

		public static object GetField(Type type, object instance, string name, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
		{
			FieldInfo info = type.GetField(name, bindingFlags);

			return info is null ? throw new Exception($"获取不到字段 {name}") : info.GetValue(instance);
		}

		public static MethodInfo GetMethod(Type type, string name)
		{
			return type.GetMethod(name);
		}

		public static T RunMethod<T>(Type type, string name, object obj, params object[] args)
		{
			return (T)GetMethod(type,name)?.Invoke(obj, args);
		}

		public T RunMethod<T>(string typename, string name, object obj, params object[] args)
		{
			Type type = GetAssemblyType(typename);
			return RunMethod<T>(type, name, obj, args);
		}

		public static List<Type> GetSubClasses(Type parentType)
		{
			return [.. parentType.Assembly.GetTypes().Where(t => t.BaseType == parentType)];
		}

		public static object CreateInstance(Type type, object[] args = null)
		{
			return Activator.CreateInstance(type, args);
		}

		public object CreateInstance(string name, object[] args = null)
		{
			return Activator.CreateInstance(Assembly.GetType(name), args);
		}
	}

	public class TypeInfo
	{
		public Type Type;

		public List<PropertyInfo> Propertys;

		public List<MethodInfo> MethodInfos;

		public TypeInfo(Type type)
		{
			Type = type;

			Propertys = [.. Type.GetProperties()];
			MethodInfos = [.. Type.GetMethods()];
		}

		public bool SetProperty(string name, object value)
		{
			PropertyInfo info = Propertys.Find(r => r.Name == name);

			if (info is null)
			{
				return false;
			}

			info.SetValue(Type, value, null);

			return true;
		}

		public object GetProperty(string name)
		{
			PropertyInfo info = Propertys.Find(r => r.Name == name);

			if (info is null)
			{
				return null;
			}

			return info.GetValue(Type, null);
		}
	}
}
