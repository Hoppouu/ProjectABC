using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class Log
{

    /// <summary>
    /// 일반 정보 로그 출력
    /// <para></para>
    /// 출력: [{스크립트 이름}] {실행 함수}: {메시지}<br/>
    /// 로그를 누르면 로그 발생 게임 오브젝트 하이어라키창에서 하이라이트<br/>
    /// 사용법: Func(message, this)형태로 사용<br/>
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
    public static void Info(string message, MonoBehaviour component = null, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "")
    {
        string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
        UnityEngine.Debug.Log(GetFormattedMessage(fileName, memberName, message), component);
    }
    /// <summary>
    /// 경고 로그 출력
    /// <para>기능 및 사용법은 Info 함수와 동일.</para>
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
    public static void Warning(string message, MonoBehaviour component = null, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "")
    {
        string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
        UnityEngine.Debug.LogWarning(GetFormattedMessage(fileName, memberName, message), component);
    }

    /// <summary>
    /// 에러 로그 출력
    /// <para>기능 및 사용법은 Info 함수와 동일.</para>
    /// </summary>
    public static void Error(string message, MonoBehaviour component = null, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "")
    {
        string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
        UnityEngine.Debug.LogError(GetFormattedMessage(fileName, memberName, message), component);
    }

    private static string GetFormattedMessage(string fileName, string memberName, string message)
    {
        return $"[{fileName}] {memberName}: {message}";
    }
}