using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System;

public class ArduinoTcpClient : MonoBehaviour
{
    public static ArduinoTcpClient Instance { get; private set; }

    [Header("Network Settings")]
    public string arduinoIpAddress = "192.168.0.150";
    public int arduinoPort = 80;

    [Header("Connection")]
    public int connectTimeoutMs = 5000;
    public int sendTimeoutMs = 3000;
    public int receiveTimeoutMs = 3000;
    public bool autoReconnect = true;
    public float reconnectIntervalSec = 2f;

    [Header("Send Rate Limit (Recommended)")]
    public bool enableRateLimit = true;
    public float minSendIntervalSec = 0.02f; // 20ms (너무 촘촘하면 Arduino가 끊을 수 있음)

    private TcpClient client;
    private NetworkStream stream;
    private float lastReconnectAttempt;
    private float lastSendTime;
    private bool isConnecting;

    public bool IsConnected => client != null && client.Connected && stream != null && stream.CanWrite;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Connect();
    }

    private void Update()
    {
        if (!autoReconnect) return;

        if (!IsConnected && !isConnecting && Time.time - lastReconnectAttempt >= reconnectIntervalSec)
        {
            lastReconnectAttempt = Time.time;
            Connect();
        }
    }

    public void Connect()
    {
        Cleanup(); // 혹시 남아있으면 정리

        try
        {
            isConnecting = true;
            Debug.Log($"[ArduinoTcpClient] Connecting to {arduinoIpAddress}:{arduinoPort}...");

            client = new TcpClient();
            client.SendTimeout = sendTimeoutMs;
            client.ReceiveTimeout = receiveTimeoutMs;
            client.NoDelay = true;

            var result = client.BeginConnect(arduinoIpAddress, arduinoPort, null, null);
            bool success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(connectTimeoutMs));

            if (!success)
                throw new Exception($"Connect timeout ({connectTimeoutMs}ms)");

            client.EndConnect(result);

            stream = client.GetStream();
            Debug.Log("[ArduinoTcpClient] ✅ Connected");
        }
        catch (Exception e)
        {
            Debug.LogError($"[ArduinoTcpClient] ❌ Connect failed: {e.Message}");
            Cleanup();
        }
        finally
        {
            isConnecting = false;
        }
    }

    public void Reconnect()
    {
        Debug.Log("[ArduinoTcpClient] Reconnect() called");
        Connect();
    }

    /// <summary>
    /// intensities: length 6, 0~255
    /// </summary>
    public bool Send(int[] intensities)
    {
        if (intensities == null || intensities.Length != 6)
        {
            Debug.LogError("[ArduinoTcpClient] Send failed: intensities must be length 6");
            return false;
        }

        if (!IsConnected)
        {
            Debug.LogWarning("[ArduinoTcpClient] Send skipped: not connected");
            return false;
        }

        if (enableRateLimit && Time.time - lastSendTime < minSendIntervalSec)
        {
            // 너무 빠른 전송은 Arduino쪽에서 연결 끊는 원인이 되기도 해서 기본 방어
            return false;
        }

        // Clamp + build command "000000000000000000\n" (18 digits + newline)
        for (int i = 0; i < 6; i++) intensities[i] = Mathf.Clamp(intensities[i], 0, 255);

        string cmd =
            intensities[0].ToString("D3") +
            intensities[1].ToString("D3") +
            intensities[2].ToString("D3") +
            intensities[3].ToString("D3") +
            intensities[4].ToString("D3") +
            intensities[5].ToString("D3") +
            "\n";

        try
        {
            byte[] data = Encoding.ASCII.GetBytes(cmd);
            stream.Write(data, 0, data.Length);
            stream.Flush();

            lastSendTime = Time.time;
            // Debug.Log($"[ArduinoTcpClient] SENT: {cmd.Trim()}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[ArduinoTcpClient] ❌ Send error: {e.Message}");
            Cleanup(); // 깨끗이 정리하고 Update에서 재연결하도록
            return false;
        }
    }

    public void SendOff()
    {
        Send(new int[6] { 0, 0, 0, 0, 0, 0 });
    }

    public void Disconnect()
    {
        try { SendOff(); } catch { }
        Cleanup();
        Debug.Log("[ArduinoTcpClient] Disconnected");
    }

    private void Cleanup()
    {
        try { stream?.Close(); } catch { }
        try { stream?.Dispose(); } catch { }
        stream = null;

        try { client?.Close(); } catch { }
        try { client?.Dispose(); } catch { }
        client = null;
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }
}
