using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class MainTestController : MonoBehaviour
{

    private const int ChannelCount = 6;

    // Calibration values (loaded from PlayerPrefs)
    private int[] minPerceptions = new int[ChannelCount];
    private int[] maxPerceptions = new int[ChannelCount];
    private int[] minActuations = new int[ChannelCount];
    private int[] maxActuations = new int[ChannelCount];

    // =========================
    // Panels
    // =========================
    [Header("Panels")]
    public GameObject calibrationPanel;
    public GameObject mainTestPanel;

    // =========================
    // Audio
    // =========================
    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Practice Sequence Audio Buttons (10개)")]
    public Button[] practiceAudioButtons;
    public AudioClip[] practiceAudioClips;

    // =========================
    // Visual Guide - Piano Keys
    // =========================
    [Header("Visual Guide - Piano Keys")]
    public Renderer pianoKeyD;
    public Renderer pianoKeyE;
    public Renderer pianoKeyF;
    public Renderer pianoKeyG;

    [Header("Visual Guide - Colors")]
    public Color keyDefaultColor = Color.white;
    public Color keyHighlightColor = Color.yellow;

    private Dictionary<int, Renderer> channelToKeyRenderer;

    // =========================
    // Step 1 (Guidance)
    // =========================
    [Header("Step 1 Buttons (Guidance)")]
    public Button btnVisual;
    public Button btnElectricCue;
    public Button btnPressOnly;
    public Button btnPressRelease;

    [Header("Step 1 Confirm Button")]
    public Button btnSelectGuidance;

    [Header("Step 3 Output Text - Guidance")]
    public TMP_Text selectedGuidanceValueText;

    private string pendingGuidanceLabel = null;
    private string confirmedGuidanceLabel = null;

    // =========================
    // Step 2 (Practice Sequences)
    // =========================
    [Header("Step 2 Sequence Buttons (Assign in Inspector)")]
    public Button[] sequenceButtons;

    [Header("Step 3 Output Text - Practice Sequence")]
    public TMP_Text selectedPracticeSequenceValueText;

    private string confirmedSequence = null;

    // =========================
    // Step 3 (Run)
    // =========================
    [Header("Step 3 Buttons")]
    public Button btnSwing;

    [Header("Test Phase Buttons")]
    public Button btnPreTest;
    public Button btnPostTest;
    public Button btnTransferTest;
    public Button btnReconnect;

    [Header("Step 3 Trial Counter")]
    public TMP_Text trialCounterText;
    private int currentTrialCount = 0;
    private const int MaxTrials = 10;

    private string lastGuidanceType = "";
    private string lastSequence = "";

    [Header("EMS Timing (Base Notes)")]
    public float longDurationSec = 1.0f;
    public float shortDurationSec = 0.5f;

    [Header("Press-Only Gap")]
    public float pressOnlyGapSec = 0.5f;

    [Header("Press-Release Gap (Channel 6)")]
    public float prCh6PulseSec = 0.2f;
    public float prOffGapSec = 0.3f;

    private Coroutine currentStimCoroutine = null;

    // =========================
    // Unity Lifecycle
    // =========================
    private void Awake()
    {
        Debug.Log("[MainTestController] Awake() called");
        if (calibrationPanel != null) calibrationPanel.SetActive(true);
        if (mainTestPanel != null) mainTestPanel.SetActive(false);
    }

    private void Start()
    {
        Debug.Log("[MainTestController] Start() called");

        LoadCalibrationData();

        InitializePianoKeys();

        if (btnVisual != null) btnVisual.onClick.AddListener(() => OnGuidancePicked("1. Visual Guide"));
        if (btnElectricCue != null) btnElectricCue.onClick.AddListener(() => OnGuidancePicked("2. Electric Cue"));
        if (btnPressOnly != null) btnPressOnly.onClick.AddListener(() => OnGuidancePicked("3. Press-Only Muscle Guide"));
        if (btnPressRelease != null) btnPressRelease.onClick.AddListener(() => OnGuidancePicked("4. Press–Release Muscle Guide"));
        if (btnSelectGuidance != null) btnSelectGuidance.onClick.AddListener(ConfirmGuidanceSelection);

        HookSequenceButtons();

        if (btnSwing != null) btnSwing.onClick.AddListener(OnSwingClicked);

        if (btnPreTest != null) btnPreTest.onClick.AddListener(OnPreTestClicked);
        if (btnPostTest != null) btnPostTest.onClick.AddListener(OnPostTestClicked);
        if (btnTransferTest != null) btnTransferTest.onClick.AddListener(OnTransferTestClicked);

        if (btnReconnect != null) btnReconnect.onClick.AddListener(OnReconnectClicked);

        SetupPracticeAudioButtons();

        RefreshSelectedGuidanceUI();
        RefreshSelectedSequenceUI();
        UpdateTrialCounterUI();

        Debug.Log($"[Wiring Check] Visual={btnVisual != null}, Electric={btnElectricCue != null}, PressOnly={btnPressOnly != null}, PressRelease={btnPressRelease != null}, Select={btnSelectGuidance != null}, Swing={btnSwing != null}");
    }

    // =========================
    // Test Phase Button Handlers
    // =========================
    private void OnPreTestClicked()
    {
        Debug.Log("[MainTestController] ========== Pre-test Button Clicked ==========");
        EMSTestPhaseLogger.Instance?.LogPreTestClick();
    }

    private void OnPostTestClicked()
    {
        Debug.Log("[MainTestController] ========== Post-test Button Clicked ==========");
        EMSTestPhaseLogger.Instance?.LogPostTestClick();
    }

    private void OnTransferTestClicked()
    {
        Debug.Log("[MainTestController] ========== Transfer-test Button Clicked ==========");
        EMSTestPhaseLogger.Instance?.LogTransferTestClick();
    }

    // =========================
    // Manual Reconnection
    // =========================
    private void OnReconnectClicked()
    {
        Debug.Log("[MainTestController] Manual reconnection requested by user");
        ArduinoTcpClient.Instance?.Reconnect();   // 이런 메서드를 ArduinoTcpClient에 만들어두는 걸 추천
    }

    private void OnApplicationQuit()
    {
        Debug.Log("[MainTestController] OnApplicationQuit() called");
        StopAllStimulation();
        ArduinoTcpClient.Instance?.SendOff();

    }

    // =========================
    // Practice Audio Buttons
    // =========================
    private void SetupPracticeAudioButtons()
    {
        if (practiceAudioButtons == null || practiceAudioButtons.Length == 0)
        {
            Debug.LogWarning("[MainTestController] practiceAudioButtons array is null or empty");
            return;
        }

        if (practiceAudioClips == null || practiceAudioClips.Length == 0)
        {
            Debug.LogWarning("[MainTestController] practiceAudioClips array is null or empty");
            return;
        }

        int buttonCount = Mathf.Min(practiceAudioButtons.Length, practiceAudioClips.Length);
        Debug.Log($"[MainTestController] Setting up {buttonCount} practice audio buttons");

        for (int i = 0; i < buttonCount; i++)
        {
            if (practiceAudioButtons[i] == null)
            {
                Debug.LogWarning($"[MainTestController] practiceAudioButtons[{i}] is null");
                continue;
            }

            if (practiceAudioClips[i] == null)
            {
                Debug.LogWarning($"[MainTestController] practiceAudioClips[{i}] is null");
                continue;
            }

            int capturedIndex = i;
            practiceAudioButtons[i].onClick.AddListener(() => PlayPracticeAudio(capturedIndex));

            TMP_Text btnText = practiceAudioButtons[i].GetComponentInChildren<TMP_Text>();
            string sequenceName = btnText != null ? btnText.text : $"Button {i}";
            Debug.Log($"[Audio Setup] Button [{i}] '{sequenceName}' connected to clip '{practiceAudioClips[i].name}'");
        }
    }

    private void PlayPracticeAudio(int index)
    {
        if (audioSource == null)
        {
            Debug.LogWarning("[MainTestController] AudioSource is null");
            return;
        }

        if (index < 0 || index >= practiceAudioClips.Length || practiceAudioClips[index] == null)
        {
            Debug.LogWarning($"[MainTestController] Invalid audio clip at index {index}");
            return;
        }

        AudioClip clip = practiceAudioClips[index];
        Debug.Log($"[Audio Play] Playing practice audio [{index}]: {clip.name}");

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

    // =========================
    // Panel Switch
    // =========================
    public void OnCalibrationPass()
    {
        Debug.Log("[MainTestController] ===== OnCalibrationPass START =====");

        // 1) 데이터 로드
        LoadCalibrationData();

        // 2) 연결 확인 (ArduinoTcpClient 단일 책임)
        if (ArduinoTcpClient.Instance == null || !ArduinoTcpClient.Instance.IsConnected)
        {
            Debug.LogWarning("[MainTestController] Arduino not connected. Trying reconnect...");
            ArduinoTcpClient.Instance?.Reconnect();
            // 여기서 즉시 return 할지 말지는 선택인데, 보통은 return 권장
            // return;
        }

        Debug.Log($"[Status] ArduinoConnected={ArduinoTcpClient.Instance != null && ArduinoTcpClient.Instance.IsConnected}");

        // 3) 패널 전환
        if (calibrationPanel != null) calibrationPanel.SetActive(false);
        if (mainTestPanel != null) mainTestPanel.SetActive(true);

        currentTrialCount = 0;
        UpdateTrialCounterUI();

        RefreshSelectedGuidanceUI();
        RefreshSelectedSequenceUI();

        Debug.Log("[MainTestController] ===== OnCalibrationPass END =====");
    }


    // =========================
    // Step 1
    // =========================
    private void OnGuidancePicked(string label)
    {
        pendingGuidanceLabel = label;
        confirmedGuidanceLabel = pendingGuidanceLabel;
        RefreshSelectedGuidanceUI();

        Debug.Log($"[Guidance Picked+Confirmed] confirmed = {confirmedGuidanceLabel}");
    }

    private void ConfirmGuidanceSelection()
    {
        if (string.IsNullOrEmpty(pendingGuidanceLabel))
        {
            Debug.LogWarning("[MainTestController] No guidance picked yet.");
            return;
        }

        confirmedGuidanceLabel = pendingGuidanceLabel;
        Debug.Log($"[Guidance Confirmed] confirmed = {confirmedGuidanceLabel}");
        RefreshSelectedGuidanceUI();
    }

    private void RefreshSelectedGuidanceUI()
    {
        if (selectedGuidanceValueText == null) return;
        selectedGuidanceValueText.text =
            string.IsNullOrEmpty(confirmedGuidanceLabel) ? "(Not selected)" : confirmedGuidanceLabel;
    }

    // =========================
    // Step 2
    // =========================
    private void HookSequenceButtons()
    {
        if (sequenceButtons == null)
        {
            Debug.LogWarning("[MainTestController] sequenceButtons array is null");
            return;
        }

        Debug.Log($"[MainTestController] Hooking {sequenceButtons.Length} sequence buttons");

        foreach (var b in sequenceButtons)
        {
            if (b == null) continue;

            TMP_Text t = b.GetComponentInChildren<TMP_Text>(true);
            string seq = (t != null) ? t.text.Trim() : b.name.Trim();
            string capturedSeq = seq;

            b.onClick.AddListener(() => OnSequenceSelected(capturedSeq));
        }
    }

    private void OnSequenceSelected(string sequence)
    {
        confirmedSequence = sequence;
        Debug.Log($"[Sequence Selected] {confirmedSequence}");
        RefreshSelectedSequenceUI();
    }

    private void RefreshSelectedSequenceUI()
    {
        if (selectedPracticeSequenceValueText == null) return;
        selectedPracticeSequenceValueText.text =
            string.IsNullOrEmpty(confirmedSequence) ? "(Not selected)" : confirmedSequence;
    }

    // =========================
    // Step 3
    // =========================
    public void OnSwingClicked()
    {
        Debug.Log("[MainTestController] ========== OnSwingClicked() ENTER ==========");

        // ✅ 연결 상태 재확인
        if (ArduinoTcpClient.Instance == null || !ArduinoTcpClient.Instance.IsConnected)
        {
            Debug.LogError("[MainTestController] Arduino not connected!");
            // (선택) ArduinoTcpClient.Instance?.Reconnect();
            return;
        }

        Debug.Log($"[Connection Check] connected={ArduinoTcpClient.Instance.IsConnected}");


        if (string.IsNullOrEmpty(confirmedGuidanceLabel))
        {
            Debug.LogWarning("[MainTestController] Select Guidance first.");
            return;
        }

        if (string.IsNullOrEmpty(confirmedSequence))
        {
            Debug.LogWarning("[MainTestController] Select Practice Sequence first.");
            return;
        }

        string guidanceTypeCode = GetGuidanceTypeCode(confirmedGuidanceLabel);
        Debug.Log($"[MainTestController] guidanceTypeCode = '{guidanceTypeCode}'");

        if (string.IsNullOrEmpty(guidanceTypeCode))
        {
            Debug.LogWarning($"[MainTestController] Unknown guidance label: {confirmedGuidanceLabel}");
            return;
        }

        if (guidanceTypeCode == "PressOnly" || guidanceTypeCode == "PressRelease")
        {
            Debug.Log("[MainTestController] Checking Ch1-4 calibration...");

            for (int i = 0; i < 4; i++)
            {
                Debug.Log($"[Calib Check] Ch{i + 1}: min={minActuations[i]}, max={maxActuations[i]}");

                if (minActuations[i] < 0 || maxActuations[i] < 0)
                {
                    Debug.LogError($"[MainTestController] Calibration missing for Ch{i + 1} (need min/max actuation). min={minActuations[i]}, max={maxActuations[i]}");
                    return;
                }
            }
        }

        if (guidanceTypeCode == "ElectricCue")
        {
            Debug.Log("[MainTestController] Checking Ch1-4 perception calibration...");

            for (int i = 0; i < 4; i++)
            {
                Debug.Log($"[Calib Check] Ch{i + 1}: minPerception={minPerceptions[i]}, maxPerception={maxPerceptions[i]}");

                if (minPerceptions[i] < 0 || maxPerceptions[i] < 0)
                {
                    Debug.LogError($"[MainTestController] Calibration missing for Ch{i + 1} (need min/max perception). minPerception={minPerceptions[i]}, maxPerception={maxPerceptions[i]}");
                    return;
                }
            }
        }

        if (guidanceTypeCode == "PressRelease")
        {
            Debug.Log($"[MainTestController] Checking Ch6 calibration: min={minActuations[5]}, max={maxActuations[5]}");

            if (minActuations[5] < 0 || maxActuations[5] < 0)
            {
                Debug.LogError("[MainTestController] Calibration missing for Ch6 (need min/max actuation).");
                return;
            }
        }

        StopAllStimulation();

        string currentCondition = $"{guidanceTypeCode}_{confirmedSequence}";
        string lastCondition = $"{lastGuidanceType}_{lastSequence}";

        if (currentCondition != lastCondition && !string.IsNullOrEmpty(lastCondition))
        {
            Debug.Log($"[MainTestController] Condition changed: '{lastCondition}' → '{currentCondition}'");
            Debug.Log($"[MainTestController] Resetting trial counter from {currentTrialCount} to 0");
            currentTrialCount = 0;
        }

        lastGuidanceType = guidanceTypeCode;
        lastSequence = confirmedSequence;

        currentTrialCount++;
        UpdateTrialCounterUI();

        Debug.Log($"[MainTestController] Starting trial #{currentTrialCount}: guidance={guidanceTypeCode}, sequence={confirmedSequence}");
        EMSMainTestLogger.Instance?.StartNewTrial(guidanceTypeCode, confirmedSequence, currentTrialCount);

        if (guidanceTypeCode == "Visual")
        {
            Debug.Log("[MainTestController] Starting Visual Guide coroutine...");
            currentStimCoroutine = StartCoroutine(PlayVisualGuideSequence(confirmedSequence));
        }
        else if (guidanceTypeCode == "PressOnly")
        {
            Debug.Log("[MainTestController] Starting PressOnly coroutine...");
            currentStimCoroutine = StartCoroutine(PlayPressOnlySequence(confirmedSequence));
        }
        else if (guidanceTypeCode == "ElectricCue")
        {
            Debug.Log("[MainTestController] Starting ElectricCue coroutine...");
            currentStimCoroutine = StartCoroutine(PlayElectricCueSequence(confirmedSequence));
        }
        else if (guidanceTypeCode == "PressRelease")
        {
            Debug.Log("[MainTestController] Starting PressRelease coroutine...");
            currentStimCoroutine = StartCoroutine(PlayPressReleaseSequence(confirmedSequence));
        }
        else
        {
            Debug.LogWarning($"[MainTestController] '{guidanceTypeCode}' is not implemented yet.");
            EMSMainTestLogger.Instance?.StopTrial();
        }
    }

    // =========================
    // Visual Guide - Piano Key Initialization
    // =========================
    private void InitializePianoKeys()
    {
        channelToKeyRenderer = new Dictionary<int, Renderer>();

        if (pianoKeyD != null) channelToKeyRenderer[1] = pianoKeyD;
        if (pianoKeyE != null) channelToKeyRenderer[2] = pianoKeyE;
        if (pianoKeyF != null) channelToKeyRenderer[3] = pianoKeyF;
        if (pianoKeyG != null) channelToKeyRenderer[4] = pianoKeyG;

        Debug.Log($"[Visual Guide] Initialized {channelToKeyRenderer.Count} piano keys");

        ResetAllPianoKeys();
    }

    private void ResetAllPianoKeys()
    {
        if (channelToKeyRenderer == null) return;

        foreach (var kvp in channelToKeyRenderer)
        {
            if (kvp.Value != null && kvp.Value.material != null)
            {
                kvp.Value.material.color = keyDefaultColor;
            }
        }
    }

    private void SetPianoKeyColor(int channel, Color color)
    {
        if (channelToKeyRenderer == null) return;

        if (channelToKeyRenderer.ContainsKey(channel))
        {
            Renderer keyRenderer = channelToKeyRenderer[channel];
            if (keyRenderer != null && keyRenderer.material != null)
            {
                keyRenderer.material.color = color;
                Debug.Log($"[Visual Guide] Set key Ch{channel} to color {color}");
            }
        }
        else
        {
            Debug.LogWarning($"[Visual Guide] Channel {channel} not found in key dictionary");
        }
    }

    // =========================
    // Visual Guide: 2번 반복, 건반 색상 변경
    // =========================
    private IEnumerator PlayVisualGuideSequence(string sequence)
    {
        Debug.Log($"[PlayVisual] ===== START ===== sequence: {sequence} (will repeat 2 times)");

        string[] tokens = sequence.Split('-');
        Debug.Log($"[PlayVisual] Split into {tokens.Length} tokens: [{string.Join(", ", tokens)}]");

        if (tokens.Length != 4)
        {
            Debug.LogError($"[MainTestController] Invalid sequence format: {sequence} (expected 4 tokens)");
            EMSMainTestLogger.Instance?.StopTrial();
            yield break;
        }

        ResetAllPianoKeys();

        for (int repeat = 0; repeat < 2; repeat++)
        {
            Debug.Log($"[PlayVisual] ========== REPEAT {repeat + 1}/2 ==========");

            for (int i = 0; i < 4; i++)
            {
                int noteNumber = (repeat * 4) + i + 1;
                Debug.Log($"[PlayVisual] ========== Note {noteNumber}/8 (Repeat {repeat + 1}, Step {i + 1}/4) ==========");

                int ch = NoteToChannel(tokens[i]);
                Debug.Log($"[PlayVisual] Token '{tokens[i]}' -> Channel {ch}");

                if (ch < 1 || ch > 4)
                {
                    Debug.LogError($"[MainTestController] Invalid token '{tokens[i]}' in {sequence}");
                    EMSMainTestLogger.Instance?.StopTrial();
                    yield break;
                }

                bool isLong = (i == 0 || i == 2);
                float dur = isLong ? longDurationSec : shortDurationSec;
                float durMs = dur * 1000f;

                Debug.Log($"[PlayVisual] Ch{ch}, isLong={isLong}, dur={dur}s ({durMs}ms)");

                SetPianoKeyColor(ch, keyHighlightColor);

                EMSMainTestLogger.Instance?.LogEmsCommand(ch, 0, durMs);

                Debug.Log($"[PlayVisual] Key highlighted, waiting {dur}s...");
                yield return new WaitForSeconds(dur);

                SetPianoKeyColor(ch, keyDefaultColor);
                Debug.Log("[PlayVisual] Key color reset");

                bool isLastNote = (repeat == 1 && i == 3);
                if (!isLastNote && pressOnlyGapSec > 0f)
                {
                    Debug.Log($"[PlayVisual] Gap {pressOnlyGapSec}s ({pressOnlyGapSec * 1000f}ms)");
                    EMSMainTestLogger.Instance?.LogGap(pressOnlyGapSec * 1000f);
                    yield return new WaitForSeconds(pressOnlyGapSec);
                }
            }
        }

        Debug.Log("[PlayVisual] ===== COMPLETE ===== Resetting all keys");
        ResetAllPianoKeys();
        EMSMainTestLogger.Instance?.StopTrial();
        currentStimCoroutine = null;
    }

    // =========================
    // Press-Only: 2번 반복 (총 8 notes)
    // =========================
    private IEnumerator PlayPressOnlySequence(string sequence)
    {
        Debug.Log($"[PlayPressOnly] ===== START ===== sequence: {sequence} (will repeat 2 times)");

        string[] tokens = sequence.Split('-');
        Debug.Log($"[PlayPressOnly] Split into {tokens.Length} tokens: [{string.Join(", ", tokens)}]");

        if (tokens.Length != 4)
        {
            Debug.LogError($"[MainTestController] Invalid sequence format: {sequence} (expected 4 tokens)");
            EMSMainTestLogger.Instance?.StopTrial();
            yield break;
        }

        for (int repeat = 0; repeat < 2; repeat++)
        {
            Debug.Log($"[PlayPressOnly] ========== REPEAT {repeat + 1}/2 ==========");

            for (int i = 0; i < 4; i++)
            {
                int noteNumber = (repeat * 4) + i + 1;
                Debug.Log($"[PlayPressOnly] ========== Note {noteNumber}/8 (Repeat {repeat + 1}, Step {i + 1}/4) ==========");

                int ch = NoteToChannel(tokens[i]);
                Debug.Log($"[PlayPressOnly] Token '{tokens[i]}' -> Channel {ch}");

                if (ch < 1 || ch > 4)
                {
                    Debug.LogError($"[MainTestController] Invalid token '{tokens[i]}' in {sequence}");
                    EMSMainTestLogger.Instance?.StopTrial();
                    yield break;
                }

                bool isLong = (i == 0 || i == 2);
                float dur = isLong ? longDurationSec : shortDurationSec;
                float durMs = dur * 1000f;

                int intensity = isLong ? minActuations[ch - 1] : maxActuations[ch - 1];

                Debug.Log($"[PlayPressOnly] Ch{ch}, isLong={isLong}, dur={dur}s ({durMs}ms), intensity={intensity}");

                EMSMainTestLogger.Instance?.LogEmsCommand(ch, intensity, durMs);

                int[] intensities = new int[ChannelCount];
                intensities[ch - 1] = intensity;

                Debug.Log($"[PlayPressOnly] Sending EMS ON: Ch1={intensities[0]}, Ch2={intensities[1]}, Ch3={intensities[2]}, Ch4={intensities[3]}, duration={dur}s");
                SendEmsCommand(intensities[0], intensities[1], intensities[2], intensities[3], 0, 0, dur);

                Debug.Log($"[PlayPressOnly] Waiting {dur}s...");
                yield return new WaitForSeconds(dur);

                Debug.Log("[PlayPressOnly] Sending EMS OFF");
                SendEmsCommand(0, 0, 0, 0, 0, 0, 0f);

                bool isLastNote = (repeat == 1 && i == 3);
                if (!isLastNote && pressOnlyGapSec > 0f)
                {
                    Debug.Log($"[PlayPressOnly] Gap (OFF) {pressOnlyGapSec}s ({pressOnlyGapSec * 1000f}ms)");
                    EMSMainTestLogger.Instance?.LogGap(pressOnlyGapSec * 1000f);
                    yield return new WaitForSeconds(pressOnlyGapSec);
                }
            }
        }

        Debug.Log("[PlayPressOnly] ===== COMPLETE ===== Final OFF");
        SendEmsCommand(0, 0, 0, 0, 0, 0, 0f);
        EMSMainTestLogger.Instance?.StopTrial();
        currentStimCoroutine = null;
    }

    // =========================
    // Electric Cue: 2번 반복 (총 8 notes)
    // =========================
    private IEnumerator PlayElectricCueSequence(string sequence)
    {
        Debug.Log($"[PlayElectricCue] ===== START ===== sequence: {sequence} (will repeat 2 times)");

        string[] tokens = sequence.Split('-');
        Debug.Log($"[PlayElectricCue] Split into {tokens.Length} tokens: [{string.Join(", ", tokens)}]");

        if (tokens.Length != 4)
        {
            Debug.LogError($"[MainTestController] Invalid sequence format: {sequence} (expected 4 tokens)");
            EMSMainTestLogger.Instance?.StopTrial();
            yield break;
        }

        for (int repeat = 0; repeat < 2; repeat++)
        {
            Debug.Log($"[PlayElectricCue] ========== REPEAT {repeat + 1}/2 ==========");

            for (int i = 0; i < 4; i++)
            {
                int noteNumber = (repeat * 4) + i + 1;
                Debug.Log($"[PlayElectricCue] ========== Note {noteNumber}/8 (Repeat {repeat + 1}, Step {i + 1}/4) ==========");

                int ch = NoteToChannel(tokens[i]);
                Debug.Log($"[PlayElectricCue] Token '{tokens[i]}' -> Channel {ch}");

                if (ch < 1 || ch > 4)
                {
                    Debug.LogError($"[MainTestController] Invalid token '{tokens[i]}' in {sequence}");
                    EMSMainTestLogger.Instance?.StopTrial();
                    yield break;
                }

                bool isLong = (i == 0 || i == 2);
                float dur = isLong ? longDurationSec : shortDurationSec;
                float durMs = dur * 1000f;

                int minPerception = minPerceptions[ch - 1];
                int maxPerception = maxPerceptions[ch - 1];
                int perceptionRange = maxPerception - minPerception;

                float fraction = isLong ? 0.25f : 0.75f;
                int intensity = minPerception + Mathf.RoundToInt(perceptionRange * fraction);

                Debug.Log($"[PlayElectricCue] Ch{ch}, isLong={isLong}, dur={dur}s ({durMs}ms)");
                Debug.Log($"[PlayElectricCue] Perception range: {minPerception} ~ {maxPerception}, fraction={fraction}, intensity={intensity}");

                EMSMainTestLogger.Instance?.LogEmsCommand(ch, intensity, durMs);

                int[] intensities = new int[ChannelCount];
                intensities[ch - 1] = intensity;

                Debug.Log($"[PlayElectricCue] Sending EMS ON: Ch1={intensities[0]}, Ch2={intensities[1]}, Ch3={intensities[2]}, Ch4={intensities[3]}, duration={dur}s");
                SendEmsCommand(intensities[0], intensities[1], intensities[2], intensities[3], 0, 0, dur);

                Debug.Log($"[PlayElectricCue] Waiting {dur}s...");
                yield return new WaitForSeconds(dur);

                Debug.Log("[PlayElectricCue] Sending EMS OFF");
                SendEmsCommand(0, 0, 0, 0, 0, 0, 0f);

                bool isLastNote = (repeat == 1 && i == 3);
                if (!isLastNote && pressOnlyGapSec > 0f)
                {
                    Debug.Log($"[PlayElectricCue] Gap (OFF) {pressOnlyGapSec}s ({pressOnlyGapSec * 1000f}ms)");
                    EMSMainTestLogger.Instance?.LogGap(pressOnlyGapSec * 1000f);
                    yield return new WaitForSeconds(pressOnlyGapSec);
                }
            }
        }

        Debug.Log("[PlayElectricCue] ===== COMPLETE ===== Final OFF");
        SendEmsCommand(0, 0, 0, 0, 0, 0, 0f);
        EMSMainTestLogger.Instance?.StopTrial();
        currentStimCoroutine = null;
    }

    // =========================
    // Press-Release: 2번 반복, 마지막 노트 후에도 release 자극
    // =========================
    private IEnumerator PlayPressReleaseSequence(string sequence)
    {
        Debug.Log($"[PlayPressRelease] ===== START ===== sequence: {sequence} (will repeat 2 times)");

        string[] tokens = sequence.Split('-');
        Debug.Log($"[PlayPressRelease] Split into {tokens.Length} tokens: [{string.Join(", ", tokens)}]");

        if (tokens.Length != 4)
        {
            Debug.LogError($"[MainTestController] Invalid sequence format: {sequence}");
            EMSMainTestLogger.Instance?.StopTrial();
            yield break;
        }

        bool[] ch6UseMax = new bool[] { true, false, true, true, false, true, true, false };
        Debug.Log($"[PlayPressRelease] Ch6 intensity pattern for 8 gaps: [max, min, max, max, min, max, max, min]");

        int gapIndex = 0;

        for (int repeat = 0; repeat < 2; repeat++)
        {
            Debug.Log($"[PlayPressRelease] ========== REPEAT {repeat + 1}/2 ==========");

            for (int i = 0; i < 4; i++)
            {
                int noteNumber = (repeat * 4) + i + 1;
                Debug.Log($"[PlayPressRelease] ========== Note {noteNumber}/8 (Repeat {repeat + 1}, Step {i + 1}/4) ==========");

                int ch = NoteToChannel(tokens[i]);
                Debug.Log($"[PlayPressRelease] Token '{tokens[i]}' -> Channel {ch}");

                if (ch < 1 || ch > 4)
                {
                    Debug.LogError($"[MainTestController] Invalid token '{tokens[i]}' in {sequence}");
                    EMSMainTestLogger.Instance?.StopTrial();
                    yield break;
                }

                bool isLong = (i == 0 || i == 2);
                float dur = isLong ? longDurationSec : shortDurationSec;
                float durMs = dur * 1000f;

                int intensity = isLong ? minActuations[ch - 1] : maxActuations[ch - 1];

                Debug.Log($"[PlayPressRelease] Ch{ch}, isLong={isLong}, dur={dur}s ({durMs}ms), intensity={intensity}");

                EMSMainTestLogger.Instance?.LogEmsCommand(ch, intensity, durMs);

                int[] intensities = new int[ChannelCount];
                intensities[ch - 1] = intensity;

                Debug.Log($"[PlayPressRelease] Sending base note: Ch1={intensities[0]}, Ch2={intensities[1]}, Ch3={intensities[2]}, Ch4={intensities[3]}");
                SendEmsCommand(intensities[0], intensities[1], intensities[2], intensities[3], 0, 0, dur);
                yield return new WaitForSeconds(dur);

                Debug.Log("[PlayPressRelease] Base note finished, sending OFF");
                SendEmsCommand(0, 0, 0, 0, 0, 0, 0f);

                if (gapIndex < ch6UseMax.Length)
                {
                    bool useMax = ch6UseMax[gapIndex];
                    int ch6Intensity = useMax ? maxActuations[5] : minActuations[5];
                    float ch6Ms = prCh6PulseSec * 1000f;

                    Debug.Log($"[PlayPressRelease] Gap {gapIndex + 1}/8: Ch6 pulse {prCh6PulseSec}s ({ch6Ms}ms), intensity={ch6Intensity} ({(useMax ? "MAX" : "MIN")})");

                    EMSMainTestLogger.Instance?.LogEmsCommand(6, ch6Intensity, ch6Ms);

                    int[] gapIntensities = new int[ChannelCount];
                    gapIntensities[5] = ch6Intensity;

                    SendEmsCommand(0, 0, 0, 0, 0, gapIntensities[5], prCh6PulseSec);
                    yield return new WaitForSeconds(prCh6PulseSec);

                    Debug.Log($"[PlayPressRelease] Ch6 pulse finished, OFF gap {prOffGapSec}s ({prOffGapSec * 1000f}ms)");
                    SendEmsCommand(0, 0, 0, 0, 0, 0, 0f);

                    if (prOffGapSec > 0f)
                    {
                        EMSMainTestLogger.Instance?.LogGap(prOffGapSec * 1000f);
                        yield return new WaitForSeconds(prOffGapSec);
                    }

                    gapIndex++;
                }
                else
                {
                    Debug.LogWarning($"[PlayPressRelease] Gap index {gapIndex} exceeds ch6UseMax array length {ch6UseMax.Length}");
                }
            }
        }

        Debug.Log("[PlayPressRelease] ===== COMPLETE ===== Final OFF");
        SendEmsCommand(0, 0, 0, 0, 0, 0, 0f);
        EMSMainTestLogger.Instance?.StopTrial();
        currentStimCoroutine = null;
    }

    private int NoteToChannel(string token)
    {
        token = token.Trim().ToUpperInvariant();

        int result = -1;
        if (token == "D") result = 1;
        else if (token == "E") result = 2;
        else if (token == "F") result = 3;
        else if (token == "G") result = 4;

        Debug.Log($"[NoteToChannel] '{token}' -> {result}");
        return result;
    }

    private void StopAllStimulation()
    {
        Debug.Log("[MainTestController] StopAllStimulation() called");

        if (currentStimCoroutine != null)
        {
            StopCoroutine(currentStimCoroutine);
            currentStimCoroutine = null;
            Debug.Log("[MainTestController] Stopped active coroutine");
        }

        SendEmsCommand(0, 0, 0, 0, 0, 0, 0f);
    }

    private string GetGuidanceTypeCode(string confirmedLabel)
    {
        if (string.IsNullOrEmpty(confirmedLabel))
        {
            Debug.LogWarning("[GetGuidanceTypeCode] confirmedLabel is null or empty");
            return null;
        }

        Debug.Log($"[GetGuidanceTypeCode] Input: '{confirmedLabel}'");

        if (confirmedLabel.Contains("Visual")) return "Visual";
        if (confirmedLabel.Contains("Electric")) return "ElectricCue";
        if (confirmedLabel.Contains("Press-Only")) return "PressOnly";
        if (confirmedLabel.Contains("Press–Release") || confirmedLabel.Contains("Press-Release") || confirmedLabel.Contains("Press Release"))
            return "PressRelease";

        Debug.LogWarning($"[GetGuidanceTypeCode] No match found for '{confirmedLabel}'");
        return null;
    }

    private void SendEmsCommand(int intensityCh1, int intensityCh2, int intensityCh3, int intensityCh4, int intensityCh5, int intensityCh6, float durationInSeconds = 0f)
    {
        int[] intensities = new int[] { intensityCh1, intensityCh2, intensityCh3, intensityCh4, intensityCh5, intensityCh6 };
        if (ArduinoTcpClient.Instance == null)
        {
            Debug.LogError("[MainTestController] ArduinoTcpClient.Instance is null. Cannot send EMS.");
            return;
        }
        if (!ArduinoTcpClient.Instance.IsConnected)
        {
            Debug.LogWarning("[MainTestController] Arduino not connected. Skip sending EMS.");
            return;
        }

        ArduinoTcpClient.Instance.Send(intensities);
    }

    
    private void LoadCalibrationData()
    {
        Debug.Log("[MainTestController] Loading calibration data from PlayerPrefs...");

        for (int i = 0; i < ChannelCount; i++)
        {
            int ch = i + 1;
            minPerceptions[i] = PlayerPrefs.GetInt($"MinPerception_Ch{ch}", -1);
            maxPerceptions[i] = PlayerPrefs.GetInt($"MaxPerception_Ch{ch}", -1);
            minActuations[i] = PlayerPrefs.GetInt($"MinActuation_Ch{ch}", -1);
            maxActuations[i] = PlayerPrefs.GetInt($"MaxActuation_Ch{ch}", -1);

            Debug.Log($"[Calibration Loaded] Ch{ch}: minPerception={minPerceptions[i]}, maxPerception={maxPerceptions[i]}, minActuation={minActuations[i]}, maxActuation={maxActuations[i]}");
        }

        Debug.Log("[MainTestController] ✅ Loaded all calibration data from PlayerPrefs.");
    }

    private void UpdateTrialCounterUI()
    {
        if (trialCounterText != null)
        {
            trialCounterText.text = $"{currentTrialCount} / {MaxTrials}";
            Debug.Log($"[TrialCounter] Updated UI: {currentTrialCount} / {MaxTrials}");
        }
    }
}