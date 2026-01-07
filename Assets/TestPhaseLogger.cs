using UnityEngine;
using System;
using System.IO;
using System.Text;

/// <summary>
/// [Singleton] Pre-test/Post-test 버튼 클릭 Logger
/// - Unix time 기준 CSV 기록
/// - 버튼 클릭 시마다 횟수 증가 및 기록
/// </summary>
public class EMSTestPhaseLogger : MonoBehaviour
{
    public static EMSTestPhaseLogger Instance { get; private set; }

    [Header("Log Output Folder")]
    public string folderPath = @"D:\hyunjin\Research\You don't Swing at all\[MainTest] Logs";

    private StreamWriter csvWriter;
    private string logFilePath;

    // 클릭 카운터
    private int preTestClickCount = 0;
    private int postTestClickCount = 0;
    private int transferTestClickCount = 0;

    private static readonly DateTime UnixEpoch =
        new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("[EMSTestPhaseLogger] Instance created");
    }

    void Start()
    {
        InitializeLogger();
    }

    private void InitializeLogger()
    {
        try
        {
            // 폴더 생성
            Directory.CreateDirectory(folderPath);

            // 파일명: TestPhase_Log_YYYYMMDD_HHmmss.csv
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            logFilePath = Path.Combine(folderPath, $"TestPhase_Log_{timestamp}.csv");

            csvWriter = new StreamWriter(logFilePath, false, Encoding.UTF8);

            // CSV 헤더
            string header = "UnixTimestamp(s),ButtonType,ClickCount,LocalTime";
            csvWriter.WriteLine(header);
            csvWriter.Flush();

            Debug.Log($"[EMSTestPhaseLogger] ✅ Log file created: {logFilePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[EMSTestPhaseLogger] ❌ Failed to create log: {e.Message}");
        }
    }

    /// <summary>
    /// Pre-test 버튼 클릭 기록
    /// </summary>
    public void LogPreTestClick()
    {
        preTestClickCount++;
        LogButtonClick("Pre-test", preTestClickCount);
        Debug.Log($"[EMSTestPhaseLogger] Pre-test clicked (Total: {preTestClickCount})");
    }

    /// <summary>
    /// Post-test 버튼 클릭 기록
    /// </summary>
    public void LogPostTestClick()
    {
        postTestClickCount++;
        LogButtonClick("Post-test", postTestClickCount);
        Debug.Log($"[EMSTestPhaseLogger] Post-test clicked (Total: {postTestClickCount})");
    }

    public void LogTransferTestClick()
    {
        transferTestClickCount++;
        LogButtonClick("Post-test", transferTestClickCount);
        Debug.Log($"[EMSTestPhaseLogger] transfer-test clicked (Total: {transferTestClickCount})");
    }

    /// <summary>
    /// CSV에 버튼 클릭 기록
    /// Format: UnixTimestamp, ButtonType, ClickCount, LocalTime
    /// </summary>
    private void LogButtonClick(string buttonType, int clickCount)
    {
        if (csvWriter == null)
        {
            Debug.LogWarning("[EMSTestPhaseLogger] ⚠️ CSV writer is null!");
            return;
        }

        try
        {
            // Unix timestamp (초 단위, 소수점 3자리)
            double unixTimestamp = (DateTime.UtcNow - UnixEpoch).TotalSeconds;

            // 로컬 시간 (읽기 쉬운 형식)
            string localTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // CSV 라인 작성
            string line = $"{unixTimestamp:F3},{buttonType},{clickCount},{localTime}";

            csvWriter.WriteLine(line);
            csvWriter.Flush();

            Debug.Log($"[EMSTestPhaseLogger] Logged: {line}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[EMSTestPhaseLogger] ❌ Error writing to CSV: {e.Message}");
        }
    }

    /// <summary>
    /// 현재 클릭 횟수 조회 (디버깅/UI 표시용)
    /// </summary>
    public int GetPreTestClickCount() => preTestClickCount;
    public int GetPostTestClickCount() => postTestClickCount;

    /// <summary>
    /// 카운터 초기화 (새 세션 시작 시 사용)
    /// </summary>
    public void ResetCounters()
    {
        preTestClickCount = 0;
        postTestClickCount = 0;
        Debug.Log("[EMSTestPhaseLogger] Counters reset to 0");
    }

    void OnApplicationQuit()
    {
        if (csvWriter != null)
        {
            csvWriter.Close();
            Debug.Log($"[EMSTestPhaseLogger] Log saved and closed: {logFilePath}");
            Debug.Log($"[EMSTestPhaseLogger] Final counts - Pre-test: {preTestClickCount}, Post-test: {postTestClickCount} Transfer-test: {transferTestClickCount}");
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            csvWriter?.Close();
        }
    }
}