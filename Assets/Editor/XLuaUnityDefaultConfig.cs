using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XLua;

/// <summary>
/// xLua 配置
/// </summary>
static class XLuaUnityDefaultConfig
{
    static List<string> exclude = new List<string> {
        "HideInInspector", "ExecuteInEditMode",
        "AddComponentMenu", "ContextMenu",
        "RequireComponent", "DisallowMultipleComponent",
        "SerializeField", "AssemblyIsEditorAssembly",
        "Attribute", "Types",
        "UnitySurrogateSelector", "TrackedReference",
        "TypeInferenceRules", "FFTWindow",
        "RPC", "Network", "MasterServer",
        "BitStream", "HostData",
        "ConnectionTesterStatus", "GUI", "EventType",
        "EventModifiers", "FontStyle", "TextAlignment",
        "TextEditor", "TextEditorDblClickSnapping",
        "TextGenerator", "TextClipping", "Gizmos",
        "ADBannerView", "ADInterstitialAd",
        "Android", "Tizen", "jvalue",
        "iPhone", "iOS", "Windows", "CalendarIdentifier",
        "CalendarUnit", "CalendarUnit",
        "ClusterInput", "FullScreenMovieControlMode",
        "FullScreenMovieScalingMode", "Handheld",
        "LocalNotification", "NotificationServices",
        "RemoteNotificationType", "RemoteNotification",
        "SamsungTV", "TextureCompressionQuality",
        "TouchScreenKeyboardType", "TouchScreenKeyboard",
        "MovieTexture", "UnityEngineInternal",
        "Terrain", "Tree", "SplatPrototype",
        "DetailPrototype", "DetailRenderMode",
        "MeshSubsetCombineUtility", "AOT", "Social", "Enumerator",
        "SendMouseEvents", "Cursor", "Flash", "ActionScript",
        "OnRequestRebuild", "Ping",
        "ShaderVariantCollection", "SimpleJson.Reflection",
        "CoroutineTween", "GraphicRebuildTracker",
        "Advertisements", "UnityEditor", "WSA",
        "EventProvider", "Apple",
        "ClusterInput", "Motion",
        "UnityEngine.UI.ReflectionMethodsCache", "NativeLeakDetection",
        "NativeLeakDetectionMode", "WWWAudioExtensions", "UnityEngine.Experimental",

        "UnityEngine.DrivenRectTransformTracker", "UnityEngine.AnimatorControllerParameter", "UnityEngine.AudioSettings",
        "UnityEngine.AudioSource", "UnityEngine.CanvasRenderer", "UnityEngine.Caching",
        "DrivenRectTransformTracker", "UnityEngine.LightProbeGroup", "UnityEngine.LightingSettings", 
        "UnityEngine.MeshRenderer", "UnityEngine.Material", "UnityEngine.ParticleSystemRenderer",
        "UnityEngine.UI.DefaultControls", "UnityEngine.TextureMipmapLimitGroups", "UnityEngine.Input",
        "UnityEngine.TextureMipmapLimitGroups", "UnityEngine.Texture", "UnityEngine.ParticleSystemForceField",
        "UnityEngine.Texture2D", "UnityEngine.QualitySettings", "UnityEngine.Light",
        "UnityEngine.MonoBehaviour", "UnityEngine.UI.Text", "UnityEngine.UI.Graphic",

        "FengSheng.EventManagerEditor", "FengSheng.GameManagerEditor", "FengSheng.HotfixManagerEditor",
        "FengSheng.LuaManagerEditor", "FengSheng.NetManagerEditor", "FengSheng.ProtosManagerEditor",
        "FengSheng.ResourcesManagerEditor", "FengSheng.UIManagerEditor", "FengSheng.UIEventManagerEditor"
    };

    static bool isExcluded(Type type)
    {
        var fullName = type.FullName;
        for (int i = 0; i < exclude.Count; i++)
        {
            if (fullName.Contains(exclude[i]))
            {
                return true;
            }
        }
        return false;
    }

    [LuaCallCSharp]
    public static IEnumerable<Type> LuaCallCSharp
    {
        get
        {
            List<string> namespaces = new List<string>()
            {
                "UnityEngine",
                "UnityEngine.UI",
                "DG.Tweening",
                "FengSheng"
            };
            var unityTypes = (from assembly in AppDomain.CurrentDomain.GetAssemblies()//获取全部程序集
                              where !(assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder)//过滤动态生成程序集
                              from type in assembly.GetExportedTypes()//获取程序集中公共非嵌套类型
                              where type.Namespace != null //命名空间非空
                                && namespaces.Contains(type.Namespace) //包含指定的命名空间
                                && !isExcluded(type) //不在列表中
                                && type.BaseType != typeof(MulticastDelegate) //非委托类型
                                && !type.IsInterface //不是接口
                                && !type.IsEnum //不是枚举
                              select type);

            return unityTypes;
        }
    }

    //自动把LuaCallCSharp涉及到的delegate加到CSharpCallLua列表，后续可以直接用lua函数做callback
    [CSharpCallLua]
    public static List<Type> CSharpCallLua
    {
        get
        {
            var lua_call_csharp = LuaCallCSharp;
            var delegate_types = new List<Type>();
            var flag = BindingFlags.Public | BindingFlags.Instance
                | BindingFlags.Static | BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly;
            foreach (var field in (from type in lua_call_csharp select type).SelectMany(type => type.GetFields(flag)))
            {
                if (typeof(Delegate).IsAssignableFrom(field.FieldType))
                {
                    delegate_types.Add(field.FieldType);
                }
            }

            foreach (var method in (from type in lua_call_csharp select type).SelectMany(type => type.GetMethods(flag)))
            {
                if (typeof(Delegate).IsAssignableFrom(method.ReturnType))
                {
                    delegate_types.Add(method.ReturnType);
                }
                foreach (var param in method.GetParameters())
                {
                    var paramType = param.ParameterType.IsByRef ? param.ParameterType.GetElementType() : param.ParameterType;
                    if (typeof(Delegate).IsAssignableFrom(paramType))
                    {
                        delegate_types.Add(paramType);
                    }
                }
            }

            delegate_types.Add(typeof(UnityEngine.Events.UnityAction<bool>));

            var list = delegate_types.Where(t => t.BaseType == typeof(MulticastDelegate) && !hasGenericParameter(t) && !delegateHasEditorRef(t) && !deleteList.Contains(t)).Distinct().ToList();

            return list;
        }
    }

    public static List<Type> deleteList = new List<Type>()
    {
        typeof(UnityEngine.Application.MemoryUsageChangedCallback),
    };

    static bool hasGenericParameter(Type type)
    {
        if (type.IsGenericTypeDefinition) return true;
        if (type.IsGenericParameter) return true;
        if (type.IsByRef || type.IsArray)
        {
            return hasGenericParameter(type.GetElementType());
        }
        if (type.IsGenericType)
        {
            foreach (var typeArg in type.GetGenericArguments())
            {
                if (hasGenericParameter(typeArg))
                {
                    return true;
                }
            }
        }
        return false;
    }

    static bool typeHasEditorRef(Type type)
    {
        if (type.Namespace != null && (type.Namespace == "UnityEditor" || type.Namespace.StartsWith("UnityEditor.")))
        {
            return true;
        }
        if (type.IsNested)
        {
            return typeHasEditorRef(type.DeclaringType);
        }
        if (type.IsByRef || type.IsArray)
        {
            return typeHasEditorRef(type.GetElementType());
        }
        if (type.IsGenericType)
        {
            foreach (var typeArg in type.GetGenericArguments())
            {
                if (typeArg.IsGenericParameter)
                {
                    //skip unsigned type parameter
                    continue;
                }
                if (typeHasEditorRef(typeArg))
                {
                    return true;
                }
            }
        }
        return false;
    }

    static bool delegateHasEditorRef(Type delegateType)
    {
        if (typeHasEditorRef(delegateType)) return true;
        var method = delegateType.GetMethod("Invoke");
        if (method == null)
        {
            return false;
        }
        if (typeHasEditorRef(method.ReturnType)) return true;
        return method.GetParameters().Any(pinfo => typeHasEditorRef(pinfo.ParameterType));
    }

    static bool IsSpanType(Type type)
    {
        if (!type.IsGenericType)
            return false;

        var genericDefinition = type.GetGenericTypeDefinition();

        return
            genericDefinition == typeof(Span<>) ||
            genericDefinition == typeof(ReadOnlySpan<>);
    }

    static bool IsSpanMember(MemberInfo memberInfo)
    {
        switch (memberInfo)
        {
            case FieldInfo fieldInfo:
                return IsSpanType(fieldInfo.FieldType);

            case PropertyInfo propertyInfo:
                return IsSpanType(propertyInfo.PropertyType);

            case ConstructorInfo constructorInfo:
                return constructorInfo.GetParameters().Any(p => IsSpanType(p.ParameterType));

            case MethodInfo methodInfo:
                return methodInfo.GetParameters().Any(p => IsSpanType(p.ParameterType)) || IsSpanType(methodInfo.ReturnType);

            default:
                return false;
        }
    }

    [BlackList]
    public static Func<MemberInfo, bool> SpanMembersFilter = IsSpanMember;
}