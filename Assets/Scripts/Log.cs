using System.Runtime.CompilerServices;
using UnityEngine;

public static class Log
{
    /// <summary>
    /// 에러 로그 출력 함수
    /// </summary>
    /// <param name="message">에러 메시지 입력</param>
    /// <param name="gameObject">this 입력</param>
    /// <param name="filePath">입력 불필요</param>
    /// <param name="memberName">입력 불필요</param>
    public static void Error(string message, GameObject gameObject = null, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "")
    {
        string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
        Debug.LogError($"[{fileName}] {memberName}: {message}", gameObject);
    }
}