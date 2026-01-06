using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

/// <summary>
/// [Singleton] MainTest 전용 EMS Logger
/// - Unix time 기준 CSV 기록
/// - Trial count를 MainTestController로부터 받아서 기록
/// </summary>
public class EMSMainTestLogger : MonoBehaviour
{
    public static EMSMainTestLogger Instance { get; private set; }

    [Header("Log Output Folder")]
    public string folderPath = @"D:\hyunjin\Research\You don't Swing at all\[MainTest] Logs";

    private StreamWriter csvWriter;

    // Trial context
    private string currentTrialID = "N/A";
    private int currentTrialIndex = -1;
    private string currentGuidanceType = "N/A";  // PressOnly, PressRelease, ElectricCue, Visual
    private string currentSequence = "N/A";      // D-E-F-G

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
    }

    void Start()
    {
        try
        {
            Directory.CreateDirectory(folderPath);

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string logPath = Path.Combine(folderPath, $"EMS_MainTest_Log_{timestamp}.csv");

            csvWriter = new StreamWriter(logPath, false, Encoding.UTF8);

            string header =
                "UnixTimestamp(s),TrialID,TrialIndex,GuidanceType,Sequence," +
                "EventType,TargetChannel,Intensity,TargetDuration(ms)";

            csvWriter.WriteLine(header);
            csvWriter.Flush();

            Debug.Log($"[EMSMainTestLogger] Log created: {logPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[EMSMainTestLogger] Failed to create log: {e.Message}");
        }
    }

    // =========================
    // Trial control
    // =========================
    /// <summary>
    /// MainTestController에서 호출하는 Trial 시작 메서드
    /// </summary>
    /// <param name="guidanceType">PressOnly, PressRelease, ElectricCue, Visual</param>
    /// <param name="sequence">D-E-F-G 등</param>
    /// <param name="trialCount">MainTestController에서 관리하는 Trial 번호</param>
    public void StartNewTrial(string guidanceType, string sequence, int trialCount)
    {
        currentGuidanceType = string.IsNullOrWhiteSpace(guidanceType) ? "N/A" : guidanceType.Trim();
        currentSequence = string.IsNullOrWhiteSpace(sequence) ? "N/A" : sequence.Trim();
        currentTrialIndex = trialCount;

        currentTrialID =
            $"MT_{currentGuidanceType}_{currentSequence}_{currentTrialIndex}_{DateTime.Now:HHmmss}";

        Debug.Log($"[EMSMainTestLogger] Trial started: {currentTrialID}");
        LogEvent("TRIAL_START", 0, 0, 0);
    }

    public void StopTrial()
    {
        if (currentTrialIndex < 0) return;

        Debug.Log($"[EMSMainTestLogger] Trial ended: {currentTrialID}");
        LogEvent("TRIAL_END", 0, 0, 0);

        currentTrialID = "N/A";
        currentTrialIndex = -1;
        currentGuidanceType = "N/A";
        currentSequence = "N/A";
    }

    // =========================
    // EMS logging
    // =========================
    public void LogEmsCommand(
        int channel,
        int intensity,
        float durationMs
    )
    {
        if (currentTrialIndex < 0)
        {
            Debug.LogWarning("[EMSMainTestLogger] LogEmsCommand called but no trial is active!");
            return;
        }
        LogEvent("EMS_SENT", channel, intensity, durationMs);
    }

    public void LogGap(float durationMs)
    {
        if (currentTrialIndex < 0)
        {
            Debug.LogWarning("[EMSMainTestLogger] LogGap called but no trial is active!");
            return;
        }
        LogEvent("GAP_OFF", 0, 0, durationMs);
    }

    // =========================
    // Internal CSV write
    // =========================
    private void LogEvent(
        string eventType,
        int channel,
        int intensity,
        float durationMs
    )
    {
        if (csvWriter == null) return;

        double unixTimestamp =
            (DateTime.UtcNow - UnixEpoch).TotalSeconds;

        string line =
            $"{unixTimestamp:F3}," +
            $"{currentTrialID}," +
            $"{currentTrialIndex}," +
            $"{currentGuidanceType}," +
            $"{currentSequence}," +
            $"{eventType}," +
            $"{channel}," +
            $"{intensity}," +
            $"{(durationMs > 0 ? durationMs.ToString("F0") : "")}";

        csvWriter.WriteLine(line);
        csvWriter.Flush();
    }

    void OnApplicationQuit()
    {
        csvWriter?.Close();
        Debug.Log("[EMSMainTestLogger] Log saved.");
    }
}