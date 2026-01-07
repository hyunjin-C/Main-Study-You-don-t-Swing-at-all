using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro Namespace
using System.Net.Sockets; // TCP/IP Communication
using System.IO;         // Data Stream
using System.Collections; // Coroutines

public class PilotTestController : MonoBehaviour
{

    // --- [!! 수정됨 1.1: 5채널 + 개별 Save/Test/Adjust UI !!] ---
    [Header("UI Elements - Calibration Control")]
    public GameObject calibrationPanel;
    public TMP_Text calibrationInstructionText; // 상단 지시문
    public Button passButton;                 // 테스트 패스 버튼
    public Button resetButton;                // EMS 즉시 정지 버튼

    [Header("UI Elements - Direct Input")]
    public TMP_InputField directInputField_Ch1;
    public Button applyDirectInputButton_Ch1;
    public TMP_InputField directInputField_Ch2;
    public Button applyDirectInputButton_Ch2;
    public TMP_InputField directInputField_Ch3;
    public Button applyDirectInputButton_Ch3;
    public TMP_InputField directInputField_Ch4;
    public Button applyDirectInputButton_Ch4;
    public TMP_InputField directInputField_Ch5;
    public Button applyDirectInputButton_Ch5;
    public TMP_InputField directInputField_Ch6;
    public Button applyDirectInputButton_Ch6;

    [Header("UI Elements - Channel 1")]
    public TMP_Text currentIntensityText_Ch1; // Ch1 현재 조절값
    public Button increaseIntensityButton_Ch1; // Ch1 +
    public Button decreaseIntensityButton_Ch1; // Ch1 -
    public TMP_Text minPerceptionValueText_Ch1;
    public Button saveMinPerceptionButton_Ch1;
    public Button testMinPerceptionButton_Ch1;
    public TMP_Text maxPerceptionValueText_Ch1;
    public Button saveMaxPerceptionButton_Ch1;
    public Button testMaxPerceptionButton_Ch1;
    public TMP_Text minActuationValueText_Ch1;
    public Button saveMinActuationButton_Ch1;
    public Button testMinActuationButton_Ch1;
    public TMP_Text maxActuationValueText_Ch1;
    public Button saveMaxActuationButton_Ch1;
    public Button testMaxActuationButton_Ch1;

    [Header("UI Elements - Channel 2")]
    public TMP_Text currentIntensityText_Ch2; // Ch2 현재 조절값
    public Button increaseIntensityButton_Ch2; // Ch2 +
    public Button decreaseIntensityButton_Ch2; // Ch2 -
    public TMP_Text minPerceptionValueText_Ch2;
    public Button saveMinPerceptionButton_Ch2;
    public Button testMinPerceptionButton_Ch2;
    public TMP_Text maxPerceptionValueText_Ch2;
    public Button saveMaxPerceptionButton_Ch2;
    public Button testMaxPerceptionButton_Ch2;
    public TMP_Text minActuationValueText_Ch2;
    public Button saveMinActuationButton_Ch2;
    public Button testMinActuationButton_Ch2;
    public TMP_Text maxActuationValueText_Ch2;
    public Button saveMaxActuationButton_Ch2;
    public Button testMaxActuationButton_Ch2;

    [Header("UI Elements - Channel 3")]
    public TMP_Text currentIntensityText_Ch3; // Ch3 현재 조절값
    public Button increaseIntensityButton_Ch3; // Ch3 +
    public Button decreaseIntensityButton_Ch3; // Ch3 -
    public TMP_Text minPerceptionValueText_Ch3;
    public Button saveMinPerceptionButton_Ch3;
    public Button testMinPerceptionButton_Ch3;
    public TMP_Text maxPerceptionValueText_Ch3;
    public Button saveMaxPerceptionButton_Ch3;
    public Button testMaxPerceptionButton_Ch3;
    public TMP_Text minActuationValueText_Ch3;
    public Button saveMinActuationButton_Ch3;
    public Button testMinActuationButton_Ch3;
    public TMP_Text maxActuationValueText_Ch3;
    public Button saveMaxActuationButton_Ch3;
    public Button testMaxActuationButton_Ch3;

    [Header("UI Elements - Channel 4")]
    public TMP_Text currentIntensityText_Ch4; // Ch4 현재 조절값
    public Button increaseIntensityButton_Ch4; // Ch4 +
    public Button decreaseIntensityButton_Ch4; // Ch4 -
    public TMP_Text minPerceptionValueText_Ch4;
    public Button saveMinPerceptionButton_Ch4;
    public Button testMinPerceptionButton_Ch4;
    public TMP_Text maxPerceptionValueText_Ch4;
    public Button saveMaxPerceptionButton_Ch4;
    public Button testMaxPerceptionButton_Ch4;
    public TMP_Text minActuationValueText_Ch4;
    public Button saveMinActuationButton_Ch4;
    public Button testMinActuationButton_Ch4;
    public TMP_Text maxActuationValueText_Ch4;
    public Button saveMaxActuationButton_Ch4;
    public Button testMaxActuationButton_Ch4;

    [Header("UI Elements - Channel 5")]
    public TMP_Text currentIntensityText_Ch5; // Ch5 현재 조절값
    public Button increaseIntensityButton_Ch5; // Ch5 +
    public Button decreaseIntensityButton_Ch5; // Ch5 -
    public TMP_Text minPerceptionValueText_Ch5;
    public Button saveMinPerceptionButton_Ch5;
    public Button testMinPerceptionButton_Ch5;
    public TMP_Text maxPerceptionValueText_Ch5;
    public Button saveMaxPerceptionButton_Ch5;
    public Button testMaxPerceptionButton_Ch5;
    public TMP_Text minActuationValueText_Ch5;
    public Button saveMinActuationButton_Ch5;
    public Button testMinActuationButton_Ch5;
    public TMP_Text maxActuationValueText_Ch5;
    public Button saveMaxActuationButton_Ch5;
    public Button testMaxActuationButton_Ch5;

    [Header("UI Elements - Channel 6")]
    public TMP_Text currentIntensityText_Ch6; // Ch6 현재 조절값
    public Button increaseIntensityButton_Ch6; // Ch6 +
    public Button decreaseIntensityButton_Ch6; // Ch6 -
    public TMP_Text minPerceptionValueText_Ch6;
    public Button saveMinPerceptionButton_Ch6;
    public Button testMinPerceptionButton_Ch6;
    public TMP_Text maxPerceptionValueText_Ch6;
    public Button saveMaxPerceptionButton_Ch6;
    public Button testMaxPerceptionButton_Ch6;
    public TMP_Text minActuationValueText_Ch6;
    public Button saveMinActuationButton_Ch6;
    public Button testMinActuationButton_Ch6;
    public TMP_Text maxActuationValueText_Ch6;
    public Button saveMaxActuationButton_Ch6;
    public Button testMaxActuationButton_Ch6;


    [Header("UI Elements - Quick Test")]
    public Button testCh1Ch5MinActuationTogetherButton;


    [Header("UI Elements - Main Test")]
    public GameObject mainTestPanel;
    public TMP_Text testInstructionText;



    public Button modeOneFingerButton;  // 'One Finger' (3채널 로직) 선택 버튼
    public Button modeFourFingerButton; // 'Four Finger' (5채널 로직) 선택 버튼

    public Button straightPresetButton;
    public Button swingPresetButton;
    // ... (모든 리듬/모드 버튼들) ...
    public Button modePressReleaseButton;
    public Button modeOnlyFingerButton;
    public Button modeFingerForearmButton;
    public Button modeFingerForearmPressReleaseButton;
    public Button timingEighthButton;
    public Button timingFourthButton;
    public Button timingTripletButton;
    public Button length1to1Button;
    public Button length1_5to1Button;
    public Button length2to1Button;
    public Button velocityWeakWeakButton;
    public Button velocityWeakStrongButton;
    public Button velocityStrongStrongButton;


    [Header("System Status")]
    public TMP_Text statusText;

    [Header("EMS Parameters")]
    public int intensityStep = 1;
    public float pulseDurationSec = 0.5f; // 아두이노로 보내는 자극 기본 지속 시간
    public int testBPM = 40; 

    // --- Internal Variables ---
    //private TcpClient client;
    //private StreamWriter writer;
    //private bool isConnected = false;

    // --- [!! 수정됨 1.2: 6채널 변수 배열 !!] ---
    private const int ChannelCount = 6;
    // 영구 저장되는 값
    private int[] minPerceptions = new int[ChannelCount];
    private int[] minActuations = new int[ChannelCount];
    private int[] maxActuations = new int[ChannelCount];
    private int[] maxPerceptions = new int[ChannelCount];

    // +/- 버튼으로 조절되는 '임시 값'
    private int[] currentIntensities = new int[ChannelCount];

    // UI 텍스트 배열 (관리를 위해)
    private TMP_Text[] currentIntensityTexts = new TMP_Text[ChannelCount];
    private TMP_Text[] minPerceptionValueTexts = new TMP_Text[ChannelCount];
    private TMP_Text[] maxPerceptionValueTexts = new TMP_Text[ChannelCount];
    private TMP_Text[] minActuationValueTexts = new TMP_Text[ChannelCount];
    private TMP_Text[] maxActuationValueTexts = new TMP_Text[ChannelCount];
     
    private enum RhythmPlaybackMode { OneFinger, FourFinger }
    private RhythmPlaybackMode currentRhythmMode = RhythmPlaybackMode.OneFinger;

    // 4핑거 모드 (Ch1-4: Fingers, Ch5: Forearm)
    private enum FourFingerStimMode { PressRelease, OnlyFinger, FingerForearm, FingerForearmPressRelease }
    private FourFingerStimMode currentFourFingerMode = FourFingerStimMode.OnlyFinger;

    // 1핑거 모드 (Ch1: Press, Ch2: Forearm, Ch3: Release)
    private enum OneFingerStimMode { PressRelease, OnlyFinger, FingerForearm, FingerForearmPressRelease }
    private OneFingerStimMode currentOneFingerMode = OneFingerStimMode.PressRelease;

    private enum TimingMode { Eighth, Fourth, Triplet }
    private enum LengthMode { Straight, Swing1_5, Swing2_0 }
    private enum VelocityMode { WeakWeak, WeakStrong, StrongStrong }

    private TimingMode currentTimingMode = TimingMode.Eighth;
    private LengthMode currentLengthMode = LengthMode.Straight;
    private VelocityMode currentVelocityMode = VelocityMode.WeakWeak;

    private enum State
    {
        Idle,
        Connecting,
        Calibrating, // 캘리브레이션 모드 (단일 상태)
        ReadyToTest,
        Testing
    }
    private State currentState = State.Idle;
    private Coroutine currentTestCoroutine = null;

    [Header("Data Logging")]
    public EMSLogger emsLogger;
    public EMSCalibrationLogger calibrationLogger;

    float pressDuration1, releaseDuration1;
    float pressDuration2, releaseDuration2;


    void Awake()
    {
        if (calibrationPanel != null) calibrationPanel.SetActive(true);
        if (mainTestPanel != null) mainTestPanel.SetActive(false);
    }


    // --- Unity Lifecycle Functions ---
    void Start()
    {
        PopulateUIArrays();
        SetupUIListeners();
        LoadCalibrationData();

        // 연결 여부는 네트워크 매니저(ArduinoTcpClient)가 관리
        bool connected = (ArduinoTcpClient.Instance != null && ArduinoTcpClient.Instance.IsConnected);

        // 연결이 아직 안 됐더라도 캘리브레이션 UI는 켜두는 게 보통 편함
        // (연결되면 바로 테스트 펄스가 날아가도록)
        SetState(connected ? State.Calibrating : State.Calibrating);

        UpdateStatus(connected ? "Arduino Connected." : "Arduino not connected yet. Trying...");
    }

    void OnApplicationQuit()
{
    // Pilot/Main 둘 다 있어도 안전하게: 매니저에게 OFF 요청만
    if (ArduinoTcpClient.Instance != null)
        ArduinoTcpClient.Instance.SendOff();
}

    // --- [!! 신규 1.4: UI 배열 초기화 함수 !!] ---
    void PopulateUIArrays()
    {
        currentIntensityTexts[0] = currentIntensityText_Ch1;
        currentIntensityTexts[1] = currentIntensityText_Ch2;
        currentIntensityTexts[2] = currentIntensityText_Ch3;
        currentIntensityTexts[3] = currentIntensityText_Ch4;
        currentIntensityTexts[4] = currentIntensityText_Ch5;
        currentIntensityTexts[5] = currentIntensityText_Ch6;

        minPerceptionValueTexts[0] = minPerceptionValueText_Ch1;
        minPerceptionValueTexts[1] = minPerceptionValueText_Ch2;
        minPerceptionValueTexts[2] = minPerceptionValueText_Ch3;
        minPerceptionValueTexts[3] = minPerceptionValueText_Ch4;
        minPerceptionValueTexts[4] = minPerceptionValueText_Ch5;
        minPerceptionValueTexts[5] = minPerceptionValueText_Ch6;

        maxPerceptionValueTexts[0] = maxPerceptionValueText_Ch1;
        maxPerceptionValueTexts[1] = maxPerceptionValueText_Ch2;
        maxPerceptionValueTexts[2] = maxPerceptionValueText_Ch3;
        maxPerceptionValueTexts[3] = maxPerceptionValueText_Ch4;
        maxPerceptionValueTexts[4] = maxPerceptionValueText_Ch5;
        maxPerceptionValueTexts[5] = maxPerceptionValueText_Ch6;

        minActuationValueTexts[0] = minActuationValueText_Ch1;
        minActuationValueTexts[1] = minActuationValueText_Ch2;
        minActuationValueTexts[2] = minActuationValueText_Ch3;
        minActuationValueTexts[3] = minActuationValueText_Ch4;
        minActuationValueTexts[4] = minActuationValueText_Ch5;
        minActuationValueTexts[5] = minActuationValueText_Ch6;

        maxActuationValueTexts[0] = maxActuationValueText_Ch1;
        maxActuationValueTexts[1] = maxActuationValueText_Ch2;
        maxActuationValueTexts[2] = maxActuationValueText_Ch3;
        maxActuationValueTexts[3] = maxActuationValueText_Ch4;
        maxActuationValueTexts[4] = maxActuationValueText_Ch5;
        maxActuationValueTexts[5] = maxActuationValueText_Ch6;
    }

    // --- [!! 수정됨 2.1: UI 리스너 (중복 제거 버전) !!] ---
    void SetupUIListeners()
    {
        // 공통 캘리브레이션 버튼
        passButton.onClick.AddListener(OnPassButtonClicked);
        resetButton.onClick.AddListener(ResetStimulation); // EMS 즉시 정지

        // 1. 채널별 '+ / -' 버튼
        if (increaseIntensityButton_Ch1 != null) increaseIntensityButton_Ch1.onClick.AddListener(() => IncreaseIntensity(1));
        if (decreaseIntensityButton_Ch1 != null) decreaseIntensityButton_Ch1.onClick.AddListener(() => DecreaseIntensity(1));
        if (increaseIntensityButton_Ch2 != null) increaseIntensityButton_Ch2.onClick.AddListener(() => IncreaseIntensity(2));
        if (decreaseIntensityButton_Ch2 != null) decreaseIntensityButton_Ch2.onClick.AddListener(() => DecreaseIntensity(2));
        if (increaseIntensityButton_Ch3 != null) increaseIntensityButton_Ch3.onClick.AddListener(() => IncreaseIntensity(3));
        if (decreaseIntensityButton_Ch3 != null) decreaseIntensityButton_Ch3.onClick.AddListener(() => DecreaseIntensity(3));
        if (increaseIntensityButton_Ch4 != null) increaseIntensityButton_Ch4.onClick.AddListener(() => IncreaseIntensity(4));
        if (decreaseIntensityButton_Ch4 != null) decreaseIntensityButton_Ch4.onClick.AddListener(() => DecreaseIntensity(4));
        if (increaseIntensityButton_Ch5 != null) increaseIntensityButton_Ch5.onClick.AddListener(() => IncreaseIntensity(5));
        if (decreaseIntensityButton_Ch5 != null) decreaseIntensityButton_Ch5.onClick.AddListener(() => DecreaseIntensity(5));
        if (increaseIntensityButton_Ch6 != null) increaseIntensityButton_Ch6.onClick.AddListener(() => IncreaseIntensity(6));
        if (decreaseIntensityButton_Ch6 != null) decreaseIntensityButton_Ch6.onClick.AddListener(() => DecreaseIntensity(6));

        // 2. 값별 'Save' 버튼
        if (saveMinPerceptionButton_Ch1 != null) saveMinPerceptionButton_Ch1.onClick.AddListener(SaveMinPerception_Ch1);
        if (saveMaxPerceptionButton_Ch1 != null) saveMaxPerceptionButton_Ch1.onClick.AddListener(SaveMaxPerception_Ch1);
        if (saveMinActuationButton_Ch1 != null) saveMinActuationButton_Ch1.onClick.AddListener(SaveMinActuation_Ch1);
        if (saveMaxActuationButton_Ch1 != null) saveMaxActuationButton_Ch1.onClick.AddListener(SaveMaxActuation_Ch1);

        if (saveMinPerceptionButton_Ch2 != null) saveMinPerceptionButton_Ch2.onClick.AddListener(SaveMinPerception_Ch2);
        if (saveMaxPerceptionButton_Ch2 != null) saveMaxPerceptionButton_Ch2.onClick.AddListener(SaveMaxPerception_Ch2);
        if (saveMinActuationButton_Ch2 != null) saveMinActuationButton_Ch2.onClick.AddListener(SaveMinActuation_Ch2);
        if (saveMaxActuationButton_Ch2 != null) saveMaxActuationButton_Ch2.onClick.AddListener(SaveMaxActuation_Ch2);

        if (saveMinPerceptionButton_Ch3 != null) saveMinPerceptionButton_Ch3.onClick.AddListener(SaveMinPerception_Ch3);
        if (saveMaxPerceptionButton_Ch3 != null) saveMaxPerceptionButton_Ch3.onClick.AddListener(SaveMaxPerception_Ch3);
        if (saveMinActuationButton_Ch3 != null) saveMinActuationButton_Ch3.onClick.AddListener(SaveMinActuation_Ch3);
        if (saveMaxActuationButton_Ch3 != null) saveMaxActuationButton_Ch3.onClick.AddListener(SaveMaxActuation_Ch3);

        if (saveMinPerceptionButton_Ch4 != null) saveMinPerceptionButton_Ch4.onClick.AddListener(SaveMinPerception_Ch4);
        if (saveMaxPerceptionButton_Ch4 != null) saveMaxPerceptionButton_Ch4.onClick.AddListener(SaveMaxPerception_Ch4);
        if (saveMinActuationButton_Ch4 != null) saveMinActuationButton_Ch4.onClick.AddListener(SaveMinActuation_Ch4);
        if (saveMaxActuationButton_Ch4 != null) saveMaxActuationButton_Ch4.onClick.AddListener(SaveMaxActuation_Ch4);

        if (saveMinPerceptionButton_Ch5 != null) saveMinPerceptionButton_Ch5.onClick.AddListener(SaveMinPerception_Ch5);
        if (saveMaxPerceptionButton_Ch5 != null) saveMaxPerceptionButton_Ch5.onClick.AddListener(SaveMaxPerception_Ch5);
        if (saveMinActuationButton_Ch5 != null) saveMinActuationButton_Ch5.onClick.AddListener(SaveMinActuation_Ch5);
        if (saveMaxActuationButton_Ch5 != null) saveMaxActuationButton_Ch5.onClick.AddListener(SaveMaxActuation_Ch5);

        if (saveMinPerceptionButton_Ch6 != null) saveMinPerceptionButton_Ch6.onClick.AddListener(SaveMinPerception_Ch6);
        if (saveMaxPerceptionButton_Ch6 != null) saveMaxPerceptionButton_Ch6.onClick.AddListener(SaveMaxPerception_Ch6);
        if (saveMinActuationButton_Ch6 != null) saveMinActuationButton_Ch6.onClick.AddListener(SaveMinActuation_Ch6);
        if (saveMaxActuationButton_Ch6 != null) saveMaxActuationButton_Ch6.onClick.AddListener(SaveMaxActuation_Ch6);

        // 3. 값별 'Test' 버튼
        if (testMinPerceptionButton_Ch1 != null) testMinPerceptionButton_Ch1.onClick.AddListener(TestMinPerception_Ch1);
        if (testMaxPerceptionButton_Ch1 != null) testMaxPerceptionButton_Ch1.onClick.AddListener(TestMaxPerception_Ch1);
        if (testMinActuationButton_Ch1 != null) testMinActuationButton_Ch1.onClick.AddListener(TestMinActuation_Ch1);
        if (testMaxActuationButton_Ch1 != null) testMaxActuationButton_Ch1.onClick.AddListener(TestMaxActuation_Ch1);

        if (testMinPerceptionButton_Ch2 != null) testMinPerceptionButton_Ch2.onClick.AddListener(TestMinPerception_Ch2);
        if (testMaxPerceptionButton_Ch2 != null) testMaxPerceptionButton_Ch2.onClick.AddListener(TestMaxPerception_Ch2);
        if (testMinActuationButton_Ch2 != null) testMinActuationButton_Ch2.onClick.AddListener(TestMinActuation_Ch2);
        if (testMaxActuationButton_Ch2 != null) testMaxActuationButton_Ch2.onClick.AddListener(TestMaxActuation_Ch2);

        if (testMinPerceptionButton_Ch3 != null) testMinPerceptionButton_Ch3.onClick.AddListener(TestMinPerception_Ch3);
        if (testMaxPerceptionButton_Ch3 != null) testMaxPerceptionButton_Ch3.onClick.AddListener(TestMaxPerception_Ch3);
        if (testMinActuationButton_Ch3 != null) testMinActuationButton_Ch3.onClick.AddListener(TestMinActuation_Ch3);
        if (testMaxActuationButton_Ch3 != null) testMaxActuationButton_Ch3.onClick.AddListener(TestMaxActuation_Ch3);

        if (testMinPerceptionButton_Ch4 != null) testMinPerceptionButton_Ch4.onClick.AddListener(TestMinPerception_Ch4);
        if (testMaxPerceptionButton_Ch4 != null) testMaxPerceptionButton_Ch4.onClick.AddListener(TestMaxPerception_Ch4);
        if (testMinActuationButton_Ch4 != null) testMinActuationButton_Ch4.onClick.AddListener(TestMinActuation_Ch4);
        if (testMaxActuationButton_Ch4 != null) testMaxActuationButton_Ch4.onClick.AddListener(TestMaxActuation_Ch4);

        if (testMinPerceptionButton_Ch5 != null) testMinPerceptionButton_Ch5.onClick.AddListener(TestMinPerception_Ch5);
        if (testMaxPerceptionButton_Ch5 != null) testMaxPerceptionButton_Ch5.onClick.AddListener(TestMaxPerception_Ch5);
        if (testMinActuationButton_Ch5 != null) testMinActuationButton_Ch5.onClick.AddListener(TestMinActuation_Ch5);
        if (testMaxActuationButton_Ch5 != null) testMaxActuationButton_Ch5.onClick.AddListener(TestMaxActuation_Ch5);

        if (testMinPerceptionButton_Ch6 != null) testMinPerceptionButton_Ch6.onClick.AddListener(TestMinPerception_Ch6);
        if (testMaxPerceptionButton_Ch6 != null) testMaxPerceptionButton_Ch6.onClick.AddListener(TestMaxPerception_Ch6);
        if (testMinActuationButton_Ch6 != null) testMinActuationButton_Ch6.onClick.AddListener(TestMinActuation_Ch6);
        if (testMaxActuationButton_Ch6 != null) testMaxActuationButton_Ch6.onClick.AddListener(TestMaxActuation_Ch6);

        if (testCh1Ch5MinActuationTogetherButton != null)
            testCh1Ch5MinActuationTogetherButton.onClick.AddListener(Test_Ch1Ch5_MinActuation_Together);


        // 4. Direct Input Apply 버튼
        if (applyDirectInputButton_Ch1 != null) applyDirectInputButton_Ch1.onClick.AddListener(() => ApplyDirectInput(1));
        if (applyDirectInputButton_Ch2 != null) applyDirectInputButton_Ch2.onClick.AddListener(() => ApplyDirectInput(2));
        if (applyDirectInputButton_Ch3 != null) applyDirectInputButton_Ch3.onClick.AddListener(() => ApplyDirectInput(3));
        if (applyDirectInputButton_Ch4 != null) applyDirectInputButton_Ch4.onClick.AddListener(() => ApplyDirectInput(4));
        if (applyDirectInputButton_Ch5 != null) applyDirectInputButton_Ch5.onClick.AddListener(() => ApplyDirectInput(5));
        if (applyDirectInputButton_Ch6 != null) applyDirectInputButton_Ch6.onClick.AddListener(() => ApplyDirectInput(6));

        // Master Mode
        if (modeOneFingerButton != null)
            modeOneFingerButton.onClick.AddListener(() =>
            {
                currentRhythmMode = RhythmPlaybackMode.OneFinger;
                Debug.Log("Master Mode Set: OneFinger (3-Channel Logic)");
                UpdateUIForState();
            });
        if (modeFourFingerButton != null)
            modeFourFingerButton.onClick.AddListener(() =>
            {
                currentRhythmMode = RhythmPlaybackMode.FourFinger;
                Debug.Log("Master Mode Set: FourFinger (5-Channel Logic)");
                UpdateUIForState();
            });

        // Sub Mode (OnlyFinger, FingerForearm) - (중복 제거: 1회만 등록)
        if (modeOnlyFingerButton != null)
            modeOnlyFingerButton.onClick.AddListener(() =>
            {

                if (currentRhythmMode == RhythmPlaybackMode.OneFinger)
                {
                    currentOneFingerMode = OneFingerStimMode.OnlyFinger;
                    Debug.Log("OneFinger Mode Set: Only Finger (Ch1->OFF)");
                }
                else
                {
                    currentFourFingerMode = FourFingerStimMode.OnlyFinger;
                    Debug.Log("FourFinger Mode Set: Only Finger (Ch1-4)");
                }
            });

        if (modeFingerForearmButton != null)
            modeFingerForearmButton.onClick.AddListener(() =>
            {
                if (currentRhythmMode == RhythmPlaybackMode.OneFinger)
                {
                    currentOneFingerMode = OneFingerStimMode.FingerForearm;
                    Debug.Log("OneFinger Mode Set: Finger & Forearm (Ch1+Ch5)");
                }
                else
                {
                    currentFourFingerMode = FourFingerStimMode.FingerForearm;
                    Debug.Log("FourFinger Mode Set: Finger & Forearm (Ch1-4 + Ch5)");
                }
            });

        // Sub Mode (PressRelease, FingerForearmPressRelease) - (중복 제거: 1회만 등록)
        if (modePressReleaseButton != null)
            modePressReleaseButton.onClick.AddListener(() =>
            {
                if (currentRhythmMode == RhythmPlaybackMode.OneFinger)
                {
                    currentOneFingerMode = OneFingerStimMode.PressRelease;
                    Debug.Log("OneFinger Mode Set: Press & Release (Ch1->Ch6)");
                }
                else
                {
                    currentFourFingerMode = FourFingerStimMode.PressRelease;
                    Debug.Log("FourFinger Mode Set: Only Finger + Press & Release (Ch1-4 -> Ch6)");
                }
            });

        if (modeFingerForearmPressReleaseButton != null)
            modeFingerForearmPressReleaseButton.onClick.AddListener(() =>
            {
                if (currentRhythmMode == RhythmPlaybackMode.OneFinger)
                {
                    currentOneFingerMode = OneFingerStimMode.FingerForearmPressRelease;
                    Debug.Log("OneFinger Mode Set: Finger & Forearm + Press & Release (Ch1+Ch5->Ch6)");
                }
                else
                {
                    currentFourFingerMode = FourFingerStimMode.FingerForearmPressRelease;
                    Debug.Log("FourFinger Mode Set: Finger & Forearm + Press & Release (Ch1-4+Ch5 -> Ch6)");
                }
            });

        if (straightPresetButton != null)
        {
            straightPresetButton.onClick.AddListener(() =>
            {
                currentTimingMode = TimingMode.Eighth;
                currentLengthMode = LengthMode.Straight;
                currentVelocityMode = VelocityMode.WeakWeak;
                string rhythmType = "Straight_1_1";
                string modeStr = (currentRhythmMode == RhythmPlaybackMode.OneFinger) ? currentOneFingerMode.ToString() : currentFourFingerMode.ToString();
                string task = GetTaskCode();
                GetGuideAndLocationCodes(out string guide, out string location);

                if (emsLogger != null)
                    emsLogger.StartNewTrial(rhythmType, task, guide, location);

                Debug.Log("Preset Set: Straight (8th, 1:1, WeakWeak)");
                StartRhythmTest();
            });
        }

        if (swingPresetButton != null)
        {
            swingPresetButton.onClick.AddListener(() =>
            {
                currentTimingMode = TimingMode.Eighth;
                currentLengthMode = LengthMode.Swing2_0;
                currentVelocityMode = VelocityMode.WeakStrong;
                string rhythmType = "Swing_2_1";
                string modeStr = (currentRhythmMode == RhythmPlaybackMode.OneFinger) ? currentOneFingerMode.ToString() : currentFourFingerMode.ToString();
                string task = GetTaskCode();
                GetGuideAndLocationCodes(out string guide, out string location);

                if (emsLogger != null)
                    emsLogger.StartNewTrial(rhythmType, task, guide, location);

                Debug.Log("Preset Set: Swing (8th, 2:1, WeakStrong)");
                StartRhythmTest();
            });
        }

        timingEighthButton.onClick.AddListener(() => { currentTimingMode = TimingMode.Eighth; StartRhythmTest(); });
        timingFourthButton.onClick.AddListener(() => { currentTimingMode = TimingMode.Fourth; StartRhythmTest(); });
        timingTripletButton.onClick.AddListener(() => { currentTimingMode = TimingMode.Triplet; StartRhythmTest(); });
        length1to1Button.onClick.AddListener(() => { currentLengthMode = LengthMode.Straight; StartRhythmTest(); });
        length1_5to1Button.onClick.AddListener(() => { currentLengthMode = LengthMode.Swing1_5; StartRhythmTest(); });
        length2to1Button.onClick.AddListener(() => { currentLengthMode = LengthMode.Swing2_0; StartRhythmTest(); });
        velocityWeakWeakButton.onClick.AddListener(() => { currentVelocityMode = VelocityMode.WeakWeak; StartRhythmTest(); });
        velocityWeakStrongButton.onClick.AddListener(() => { currentVelocityMode = VelocityMode.WeakStrong; StartRhythmTest(); });
        velocityStrongStrongButton.onClick.AddListener(() => { currentVelocityMode = VelocityMode.StrongStrong; StartRhythmTest(); });
    }

    void InitializeCurrentIntensities()
    {
        for (int i = 0; i < ChannelCount; i++)
        {
            currentIntensities[i] = 0;
            if (currentIntensityTexts[i] != null) currentIntensityTexts[i].text = "0";
        }
    }

    void UpdateAllCalibrationUI()
    {
        for (int i = 0; i < ChannelCount; i++)
        {
            if (minPerceptionValueTexts[i] != null) minPerceptionValueTexts[i].text = minPerceptions[i] > -1 ? minPerceptions[i].ToString() : "0";
            if (maxPerceptionValueTexts[i] != null) maxPerceptionValueTexts[i].text = maxPerceptions[i] > -1 ? maxPerceptions[i].ToString() : "0";
            if (minActuationValueTexts[i] != null) minActuationValueTexts[i].text = minActuations[i] > -1 ? minActuations[i].ToString() : "0";
            if (maxActuationValueTexts[i] != null) maxActuationValueTexts[i].text = maxActuations[i] > -1 ? maxActuations[i].ToString() : "0";
        }
    }

    void SetState(State newState)
    {
        currentState = newState;
        UpdateUIForState();
    }

    void UpdateUIForState()
    {
        //calibrationPanel.SetActive(currentState == State.Calibrating || currentState == State.ReadyToTest);
        

        bool isCalibrating = (currentState == State.Calibrating);
        bool canTest = (currentState == State.ReadyToTest || currentState == State.Testing); 

        if (calibrationPanel != null) calibrationPanel.SetActive(isCalibrating);
        if (mainTestPanel != null) mainTestPanel.SetActive(canTest);

        if (increaseIntensityButton_Ch1 != null) increaseIntensityButton_Ch1.interactable = isCalibrating;
        if (decreaseIntensityButton_Ch1 != null) decreaseIntensityButton_Ch1.interactable = isCalibrating;
        if (saveMinPerceptionButton_Ch1 != null) saveMinPerceptionButton_Ch1.interactable = isCalibrating;
        if (saveMinActuationButton_Ch1 != null) saveMinActuationButton_Ch1.interactable = isCalibrating;
        if (saveMaxActuationButton_Ch1 != null) saveMaxActuationButton_Ch1.interactable = isCalibrating;
        //if (testMinPerceptionButton_Ch1 != null) testMinPerceptionButton_Ch1.interactable = canTest;
        //if (testMinActuationButton_Ch1 != null) testMinActuationButton_Ch1.interactable = canTest;
        //if (testMaxActuationButton_Ch1 != null) testMaxActuationButton_Ch1.interactable = canTest;

        if (increaseIntensityButton_Ch2 != null) increaseIntensityButton_Ch2.interactable = isCalibrating;
        if (decreaseIntensityButton_Ch2 != null) decreaseIntensityButton_Ch2.interactable = isCalibrating;
        if (saveMinPerceptionButton_Ch2 != null) saveMinPerceptionButton_Ch2.interactable = isCalibrating;
        if (saveMinActuationButton_Ch2 != null) saveMinActuationButton_Ch2.interactable = isCalibrating;
        if (saveMaxActuationButton_Ch2 != null) saveMaxActuationButton_Ch2.interactable = isCalibrating;
        //if (testMinPerceptionButton_Ch2 != null) testMinPerceptionButton_Ch2.interactable = canTest;
        //if (testMinActuationButton_Ch2 != null) testMinActuationButton_Ch2.interactable = canTest;
        //if (testMaxActuationButton_Ch2 != null) testMaxActuationButton_Ch2.interactable = canTest;

        if (increaseIntensityButton_Ch3 != null) increaseIntensityButton_Ch3.interactable = isCalibrating;
        if (decreaseIntensityButton_Ch3 != null) decreaseIntensityButton_Ch3.interactable = isCalibrating;
        if (saveMinPerceptionButton_Ch3 != null) saveMinPerceptionButton_Ch3.interactable = isCalibrating;
        if (saveMinActuationButton_Ch3 != null) saveMinActuationButton_Ch3.interactable = isCalibrating;
        if (saveMaxActuationButton_Ch3 != null) saveMaxActuationButton_Ch3.interactable = isCalibrating;
        //if (testMinPerceptionButton_Ch3 != null) testMinPerceptionButton_Ch3.interactable = canTest;
        //if (testMinActuationButton_Ch3 != null) testMinActuationButton_Ch3.interactable = canTest;
        //if (testMaxActuationButton_Ch3 != null) testMaxActuationButton_Ch3.interactable = canTest;

        if (increaseIntensityButton_Ch4 != null) increaseIntensityButton_Ch4.interactable = isCalibrating;
        if (decreaseIntensityButton_Ch4 != null) decreaseIntensityButton_Ch4.interactable = isCalibrating;
        if (saveMinPerceptionButton_Ch4 != null) saveMinPerceptionButton_Ch4.interactable = isCalibrating;
        if (saveMinActuationButton_Ch4 != null) saveMinActuationButton_Ch4.interactable = isCalibrating;
        if (saveMaxActuationButton_Ch4 != null) saveMaxActuationButton_Ch4.interactable = isCalibrating;
        //if (testMinPerceptionButton_Ch4 != null) testMinPerceptionButton_Ch4.interactable = canTest;
        //if (testMinActuationButton_Ch4 != null) testMinActuationButton_Ch4.interactable = canTest;
        //if (testMaxActuationButton_Ch4 != null) testMaxActuationButton_Ch4.interactable = canTest;

        if (increaseIntensityButton_Ch5 != null) increaseIntensityButton_Ch5.interactable = isCalibrating;
        if (decreaseIntensityButton_Ch5 != null) decreaseIntensityButton_Ch5.interactable = isCalibrating;
        if (saveMinPerceptionButton_Ch5 != null) saveMinPerceptionButton_Ch5.interactable = isCalibrating;
        if (saveMinActuationButton_Ch5 != null) saveMinActuationButton_Ch5.interactable = isCalibrating;
        if (saveMaxActuationButton_Ch5 != null) saveMaxActuationButton_Ch5.interactable = isCalibrating;
        //if (testMinPerceptionButton_Ch5 != null) testMinPerceptionButton_Ch5.interactable = canTest;
        //if (testMinActuationButton_Ch5 != null) testMinActuationButton_Ch5.interactable = canTest;
        //if (testMaxActuationButton_Ch5 != null) testMaxActuationButton_Ch5.interactable = canTest;

        if (increaseIntensityButton_Ch6 != null) increaseIntensityButton_Ch6.interactable = isCalibrating;
        if (decreaseIntensityButton_Ch6 != null) decreaseIntensityButton_Ch6.interactable = isCalibrating;
        if (saveMinPerceptionButton_Ch6 != null) saveMinPerceptionButton_Ch6.interactable = isCalibrating;
        if (saveMinActuationButton_Ch6 != null) saveMinActuationButton_Ch6.interactable = isCalibrating;
        if (saveMaxActuationButton_Ch6 != null) saveMaxActuationButton_Ch6.interactable = isCalibrating;
        //if (testMinPerceptionButton_Ch6 != null) testMinPerceptionButton_Ch6.interactable = canTest;
        //if (testMinActuationButton_Ch6 != null) testMinActuationButton_Ch6.interactable = canTest;
        //if (testMaxActuationButton_Ch6 != null) testMaxActuationButton_Ch6.interactable = canTest; 

        bool testingButtonsActive = (currentState == State.ReadyToTest);

        if (modeOnlyFingerButton != null) modeOnlyFingerButton.gameObject.SetActive(testingButtonsActive);
        if (modeFingerForearmButton != null) modeFingerForearmButton.gameObject.SetActive(testingButtonsActive);
        if (modePressReleaseButton != null) modePressReleaseButton.gameObject.SetActive(testingButtonsActive);
        if (modeFingerForearmPressReleaseButton != null) modeFingerForearmPressReleaseButton.gameObject.SetActive(testingButtonsActive);

        if (modeOneFingerButton != null) modeOneFingerButton.interactable = testingButtonsActive;
        if (modeFourFingerButton != null) modeFourFingerButton.interactable = testingButtonsActive;

        if (modePressReleaseButton != null) modePressReleaseButton.interactable = testingButtonsActive;
        if (modeOnlyFingerButton != null) modeOnlyFingerButton.interactable = testingButtonsActive;
        if (modeFingerForearmButton != null) modeFingerForearmButton.interactable = testingButtonsActive;
        if (modeFingerForearmPressReleaseButton != null) modeFingerForearmPressReleaseButton.interactable = testingButtonsActive;
        if (straightPresetButton != null) straightPresetButton.interactable = testingButtonsActive;
        if (swingPresetButton != null) swingPresetButton.interactable = testingButtonsActive;
        if (timingEighthButton != null) timingEighthButton.interactable = testingButtonsActive;
        if (timingFourthButton != null) timingFourthButton.interactable = testingButtonsActive;
        if (timingTripletButton != null) timingTripletButton.interactable = testingButtonsActive;
        if (length1to1Button != null) length1to1Button.interactable = testingButtonsActive;
        if (length1_5to1Button != null) length1_5to1Button.interactable = testingButtonsActive;
        if (length2to1Button != null) length2to1Button.interactable = testingButtonsActive;
        if (velocityWeakWeakButton != null) velocityWeakWeakButton.interactable = testingButtonsActive;
        if (velocityWeakStrongButton != null) velocityWeakStrongButton.interactable = testingButtonsActive;
        if (velocityStrongStrongButton != null) velocityStrongStrongButton.interactable = testingButtonsActive;

        switch (currentState)
        {
            case State.Connecting: statusText.text = "Status: Connecting..."; break;
            case State.Calibrating:
                calibrationInstructionText.text = "Adjust intensity using +/- buttons and press 'Save' for the desired value.";
                break;
            case State.ReadyToTest:
                statusText.text = "Status: Ready for Main Test.";
                testInstructionText.text = "Press a button to test the pattern.";
                break;
            case State.Testing:
                statusText.text = "Status: Testing in progress...";
                testInstructionText.text = "Feeling the pattern...";
                break;
            default:
                {
                    bool connected = (ArduinoTcpClient.Instance != null && ArduinoTcpClient.Instance.IsConnected);
                    statusText.text = connected ? "Status: Idle" : "Status: Not Connected";
                    break;
                }
        }
    }

    void OnPassButtonClicked()
    {
        Debug.Log("Pass button clicked. Loading saved values and skipping to test.");
        LoadCalibrationData(); // 저장된 값 로드
        SetState(State.ReadyToTest); // 테스트 모드로
    }

    // --- [!! 신규 3.1: 개별 테스트 함수 (총 15개) !!] ---
    private void TestValue(int channel, int value, string calibType)
    {
        if (value < 0 || currentState == State.Testing)
        {
            Debug.LogWarning($"Cannot test CH{channel}. Value not set ({value}) or test in progress.");
            return;
        }
        Debug.Log($"Testing CH{channel} value: {value}");

        if (calibrationLogger != null)
            calibrationLogger.LogCalibrationTest(channel, value, calibType);
        else if (EMSCalibrationLogger.Instance != null)
            EMSCalibrationLogger.Instance.LogCalibrationTest(channel, value, calibType);

        int[] intensities = new int[ChannelCount];
        intensities[channel - 1] = value; // 1-based to 0-based
        SendEmsCommand(intensities[0], intensities[1], intensities[2], intensities[3], intensities[4], intensities[5], pulseDurationSec);
        StartCoroutine(StopStimulationAfterDelay(pulseDurationSec));
    }

    void TestMinPerception_Ch1() { TestValue(1, minPerceptions[0], "min_perception"); }
    void TestMaxPerception_Ch1() { TestValue(1, maxPerceptions[0], "max_perception"); }
    void TestMinActuation_Ch1() { TestValue(1, minActuations[0], "min_actuation"); }
    void TestMaxActuation_Ch1() { TestValue(1, maxActuations[0], "max_actuation"); }

    void TestMinPerception_Ch2() { TestValue(2, minPerceptions[1], "min_perception"); }
    void TestMaxPerception_Ch2() { TestValue(2, maxPerceptions[1], "max_perception"); }
    void TestMinActuation_Ch2() { TestValue(2, minActuations[1], "min_actuation"); }
    void TestMaxActuation_Ch2() { TestValue(2, maxActuations[1], "max_actuation"); }

    void TestMinPerception_Ch3() { TestValue(3, minPerceptions[2], "min_perception"); }
    void TestMaxPerception_Ch3() { TestValue(3, maxPerceptions[2], "max_perception"); }
    void TestMinActuation_Ch3() { TestValue(3, minActuations[2], "min_actuation"); }
    void TestMaxActuation_Ch3() { TestValue(3, maxActuations[2], "max_actuation"); }

    void TestMinPerception_Ch4() { TestValue(4, minPerceptions[3], "min_perception"); }
    void TestMaxPerception_Ch4() { TestValue(4, maxPerceptions[3], "max_perception"); }
    void TestMinActuation_Ch4() { TestValue(4, minActuations[3], "min_actuation"); }
    void TestMaxActuation_Ch4() { TestValue(4, maxActuations[3], "max_actuation"); }

    void TestMinPerception_Ch5() { TestValue(5, minPerceptions[4], "min_perception"); }
    void TestMaxPerception_Ch5() { TestValue(5, maxPerceptions[4], "max_perception"); }
    void TestMinActuation_Ch5() { TestValue(5, minActuations[4], "min_actuation"); }
    void TestMaxActuation_Ch5() { TestValue(5, maxActuations[4], "max_actuation"); }

    void TestMinPerception_Ch6() { TestValue(6, minPerceptions[5], "min_perception"); }
    void TestMaxPerception_Ch6() { TestValue(6, maxPerceptions[5], "max_perception"); }
    void TestMinActuation_Ch6() { TestValue(6, minActuations[5], "min_actuation"); }
    void TestMaxActuation_Ch6() { TestValue(6, maxActuations[5], "max_actuation"); }

    void IncreaseIntensity(int channel)
    {
        if (currentState != State.Calibrating) return;

        int index = channel - 1;
        currentIntensities[index] += intensityStep;
        currentIntensityTexts[index].text = currentIntensities[index].ToString();

        SendTestPulse(channel, currentIntensities[index]);
    }

    void DecreaseIntensity(int channel)
    {
        if (currentState != State.Calibrating) return;

        int index = channel - 1;
        currentIntensities[index] -= intensityStep;
        if (currentIntensities[index] < 0) currentIntensities[index] = 0;
        currentIntensityTexts[index].text = currentIntensities[index].ToString();

        SendTestPulse(channel, currentIntensities[index]);
    }

    void SendTestPulse(int channel, int intensity)
    {
        int[] intensities = new int[ChannelCount];
        intensities[channel - 1] = intensity;
        SendEmsCommand(intensities[0], intensities[1], intensities[2], intensities[3], intensities[4], intensities[5], 1.0f);
        StartCoroutine(StopStimulationAfterDelay(1.0f));
    }

    void ApplyDirectInput(int channel)
    {
        if (currentState != State.Calibrating)
        {
            Debug.LogWarning("Direct input can only be applied during calibration.");
            return;
        }

        TMP_InputField inputField = null;
        switch (channel)
        {
            case 1: inputField = directInputField_Ch1; break;
            case 2: inputField = directInputField_Ch2; break;
            case 3: inputField = directInputField_Ch3; break;
            case 4: inputField = directInputField_Ch4; break;
            case 5: inputField = directInputField_Ch5; break;
            case 6: inputField = directInputField_Ch6; break;
        }

        if (inputField == null)
        {
            Debug.LogWarning($"Input field for channel {channel} not assigned.");
            return;
        }

        if (int.TryParse(inputField.text, out int value))
        {
            if (value < 0)
            {
                Debug.LogWarning($"Invalid value: {value}. Must be non-negative.");
                return;
            }

            int index = channel - 1;
            currentIntensities[index] = value;
            currentIntensityTexts[index].text = value.ToString();

            Debug.Log($"CH{channel} Direct Input Applied: {value}");
            SendTestPulse(channel, value);
        }
        else
        {
            Debug.LogWarning($"Invalid input for CH{channel}: '{inputField.text}'");
        }
    }

    void Test_Ch1Ch5_MinActuation_Together()
    {
        // 테스트 중이면 무시 (원하면 허용해도 됨)
        if (currentState == State.Testing) return;

        int ch1 = minActuations[0]; // Channel 1
        int ch5 = minActuations[4]; // Channel 5

        if (ch1 < 0 || ch5 < 0)
        {
            Debug.LogWarning($"Cannot test Ch1+Ch5: MinActuation not calibrated. (Ch1={ch1}, Ch5={ch5})");
            return;
        }

        float dur = 1.0f; // 1초

        // 로그 남기고 싶으면 (선택)
        LogSemanticEms(1, ch1, dur);
        LogSemanticEms(5, ch5, dur);

        // 동시에 1초 자극
        SendEmsCommand(ch1, 0, 0, 0, ch5, 0, dur);

        // dur 이후 OFF (캘리브레이션/Ready 상태에서만 꺼지게 이미 구현돼있음)
        StartCoroutine(StopStimulationAfterDelay(dur));
    }


    void ResetStimulation()
    {
        if (currentState == State.Testing) return;

        for (int i = 0; i < ChannelCount; i++)
        {
            currentIntensities[i] = 0;
            if (currentIntensityTexts[i] != null) currentIntensityTexts[i].text = "0";
        }
        SendEmsCommand(0, 0, 0, 0, 0, 0);
        Debug.Log("Stimulation RESET to 0 by button.");
    }

    IEnumerator StopStimulationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (currentState == State.Calibrating || currentState == State.ReadyToTest)
        {
            SendEmsCommand(0, 0, 0, 0, 0, 0);
        }
    }

    private void SaveValue(int channel, string valueType)
    {
        int index = channel - 1;
        int valueToSave = currentIntensities[index];

        if (valueType == "MinPerception") minPerceptions[index] = valueToSave;
        else if (valueType == "MaxPerception") maxPerceptions[index] = valueToSave;
        else if (valueType == "MinActuation") minActuations[index] = valueToSave;
        else if (valueType == "MaxActuation") maxActuations[index] = valueToSave;

        PlayerPrefs.SetInt($"{valueType}_Ch{channel}", valueToSave);
        PlayerPrefs.Save();
        Debug.Log($"CH{channel} {valueType} Saved: {valueToSave}");

        UpdateAllCalibrationUI();
        SendEmsCommand(0, 0, 0, 0, 0, 0);
    }

    void SaveMinPerception_Ch1() { SaveValue(1, "MinPerception"); }
    void SaveMaxPerception_Ch1() { SaveValue(1, "MaxPerception"); }
    void SaveMinActuation_Ch1() { SaveValue(1, "MinActuation"); }
    void SaveMaxActuation_Ch1() { SaveValue(1, "MaxActuation"); }

    void SaveMinPerception_Ch2() { SaveValue(2, "MinPerception"); }
    void SaveMaxPerception_Ch2() { SaveValue(2, "MaxPerception"); }
    void SaveMinActuation_Ch2() { SaveValue(2, "MinActuation"); }
    void SaveMaxActuation_Ch2() { SaveValue(2, "MaxActuation"); } 

    void SaveMinPerception_Ch3() { SaveValue(3, "MinPerception"); }
    void SaveMaxPerception_Ch3() { SaveValue(3, "MaxPerception"); }
    void SaveMinActuation_Ch3() { SaveValue(3, "MinActuation"); }
    void SaveMaxActuation_Ch3() { SaveValue(3, "MaxActuation"); }

    void SaveMinPerception_Ch4() { SaveValue(4, "MinPerception"); }
    void SaveMaxPerception_Ch4() { SaveValue(4, "MaxPerception"); }
    void SaveMinActuation_Ch4() { SaveValue(4, "MinActuation"); }
    void SaveMaxActuation_Ch4() { SaveValue(4, "MaxActuation"); }

    void SaveMinPerception_Ch5() { SaveValue(5, "MinPerception"); }
    void SaveMaxPerception_Ch5() { SaveValue(5, "MaxPerception"); }
    void SaveMinActuation_Ch5() { SaveValue(5, "MinActuation"); }
    void SaveMaxActuation_Ch5() { SaveValue(5, "MaxActuation"); }

    void SaveMinPerception_Ch6() { SaveValue(6, "MinPerception"); }
    void SaveMaxPerception_Ch6() { SaveValue(6, "MaxPerception"); }
    void SaveMinActuation_Ch6() { SaveValue(6, "MinActuation"); }
    void SaveMaxActuation_Ch6() { SaveValue(6, "MaxActuation"); }

    void StartTestCoroutine(IEnumerator testCoroutine)
    {
        if (currentTestCoroutine != null)
        {
            StopCoroutine(currentTestCoroutine);
            SendEmsCommand(0, 0, 0, 0, 0, 0);
        }
        currentTestCoroutine = StartCoroutine(testCoroutine);
    }

    void StartRhythmTest()
    {
        if (currentRhythmMode == RhythmPlaybackMode.FourFinger)
        {
            for (int i = 0; i < 4; i++)
            {
                if (minActuations[i] < 0) { Debug.LogWarning($"Cannot start test: MinActuation (Ch{i + 1}) not calibrated."); return; }
                if ((currentVelocityMode == VelocityMode.WeakStrong || currentVelocityMode == VelocityMode.StrongStrong) && maxActuations[i] < 0)
                { Debug.LogWarning($"Cannot start test: MaxActuation (Ch{i + 1}) not calibrated."); return; }
            }
            if (currentFourFingerMode == FourFingerStimMode.FingerForearm || currentFourFingerMode == FourFingerStimMode.FingerForearmPressRelease)
            {
                if (minActuations[4] < 0 || maxActuations[4] < 0) { Debug.LogWarning($"Cannot start test: Ch5 (Forearm) not calibrated."); return; }
            }
            if (currentFourFingerMode == FourFingerStimMode.PressRelease || currentFourFingerMode == FourFingerStimMode.FingerForearmPressRelease)
            {
                if (minActuations[5] < 0) { Debug.LogWarning("Cannot start test: Ch6 (Release) MinActuation not calibrated."); return; }
                if (currentVelocityMode != VelocityMode.WeakWeak && maxActuations[5] < 0) { Debug.LogWarning("Cannot start test: Ch6 (Release) MaxActuation not calibrated."); return; }
            }

            Debug.Log($"--- Starting FourFinger Rhythm Test ---");
            Debug.Log($"Mode: {currentFourFingerMode} | Timing: {currentTimingMode} | Length: {currentLengthMode} | Velocity: {currentVelocityMode}");
            StartTestCoroutine(PlayRhythmPattern_FourFinger());
        }
        else
        {
            if (minActuations[0] < 0) { Debug.LogWarning("Cannot start test: MinActuation (Ch1) not calibrated."); return; }
            if (currentVelocityMode != VelocityMode.WeakWeak && maxActuations[0] < 0) { Debug.LogWarning("Cannot start test: MaxActuation (Ch1) not calibrated."); return; }

            if (currentOneFingerMode == OneFingerStimMode.FingerForearm || currentOneFingerMode == OneFingerStimMode.FingerForearmPressRelease)
            {
                if (minActuations[4] < 0) { Debug.LogWarning("Cannot start test: Ch5 (Forearm) MinActuation not calibrated."); return; }
                if (currentVelocityMode != VelocityMode.WeakWeak && maxActuations[4] < 0) { Debug.LogWarning("Cannot start test: Ch5 (Forearm) MaxActuation not calibrated."); return; }
            }
            if (currentOneFingerMode == OneFingerStimMode.PressRelease || currentOneFingerMode == OneFingerStimMode.FingerForearmPressRelease)
            {
                if (minActuations[5] < 0) { Debug.LogWarning("Cannot start test: Ch6 (Release) MinActuation not calibrated."); return; }
                if (currentVelocityMode != VelocityMode.WeakWeak && maxActuations[5] < 0) { Debug.LogWarning("Cannot start test: Ch6 (Release) MaxActuation not calibrated."); return; }
            }

            Debug.Log($"--- Starting OneFinger Rhythm Test ---"); 
            Debug.Log($"Mode: {currentOneFingerMode} | Timing: {currentTimingMode} | Length: {currentLengthMode} | Velocity: {currentVelocityMode}");
            StartTestCoroutine(PlayRhythmPattern_OneFinger());
        }
    }

    // --- [4핑거 리듬: Press 시간은 계산 유지, Rest(Gap) 시간만 변경] ---
    IEnumerator PlayRhythmPattern_FourFinger()
    {
        SetState(State.Testing);

        // 1. [유지] Press Duration 계산 로직 (BPM 기반)
        float beatDuration = 60f / testBPM; // 1.5s (at 40BPM)
        float noteDuration = beatDuration / 2f;
        if (currentTimingMode == TimingMode.Fourth) noteDuration = beatDuration;
        else if (currentTimingMode == TimingMode.Triplet) noteDuration = beatDuration / 3f;

        // Intensity Setup
        int[] fingerIntensities = new int[4];
        int[] forearmIntensities = new int[2];
        int[] releaseIntensities = new int[2];

        // Velocity Logic (Existing)
        if (currentVelocityMode == VelocityMode.WeakStrong)
        {
            fingerIntensities[0] = minActuations[0]; fingerIntensities[1] = maxActuations[1];
            fingerIntensities[2] = minActuations[2]; fingerIntensities[3] = maxActuations[3];
            forearmIntensities[0] = minActuations[4]; forearmIntensities[1] = maxActuations[4];
            releaseIntensities[0] = maxActuations[5]; releaseIntensities[1] = minActuations[5];
        }
        else if (currentVelocityMode == VelocityMode.StrongStrong)
        {
            for (int i = 0; i < 4; i++) fingerIntensities[i] = maxActuations[i];
            forearmIntensities[0] = maxActuations[4]; forearmIntensities[1] = maxActuations[4];
            releaseIntensities[0] = maxActuations[5]; releaseIntensities[1] = maxActuations[5];
        }
        else
        { // WeakWeak 
            for (int i = 0; i < 4; i++) fingerIntensities[i] = minActuations[i];
            forearmIntensities[0] = minActuations[4]; forearmIntensities[1] = minActuations[4];
            releaseIntensities[0] = minActuations[5]; releaseIntensities[1] = minActuations[5];
        }

        // 2. Press Duration Calculation (Keep Original Logic)
        float press1, press2;
        float totalNotePairDuration = noteDuration * 2;
        switch (currentLengthMode)
        {
            case LengthMode.Swing1_5:
                press1 = totalNotePairDuration * (1.5f / 2.5f);
                press2 = totalNotePairDuration * (1.0f / 2.5f);
                break;
            case LengthMode.Swing2_0:
                press1 = totalNotePairDuration * (2f / 3f); // 1.0s (at 40BPM)
                press2 = totalNotePairDuration * (1f / 3f); // 0.5s (at 40BPM)
                break;
            case LengthMode.Straight:
            default:
                press1 = totalNotePairDuration * 0.5f;
                press2 = totalNotePairDuration * 0.5f;
                break;
        }

        // 3. [변경] Gap(Rest) Duration Setting
        float gap1, gap2;
        if (currentLengthMode == LengthMode.Swing2_0)
        {
            // Swing2.0일 때만 요청하신 600ms / 300ms 적용
            gap1 = 0.6f;
            gap2 = 0.6f;
        }
        else
        {
            // 그 외 모드는 기본값 0.5s 유지
            gap1 = 0.5f;
            gap2 = 0.5f;
        }

        // Play Sequence
        if (currentTimingMode == TimingMode.Eighth)
        {
            yield return StartCoroutine(PlayNoteSequence_FourFinger(1, fingerIntensities[0], forearmIntensities[0], releaseIntensities[0], press1, gap1));
            yield return StartCoroutine(PlayNoteSequence_FourFinger(2, fingerIntensities[1], forearmIntensities[1], releaseIntensities[1], press2, gap2));
            yield return StartCoroutine(PlayNoteSequence_FourFinger(3, fingerIntensities[2], forearmIntensities[0], releaseIntensities[0], press1, gap1));
            yield return StartCoroutine(PlayNoteSequence_FourFinger(4, fingerIntensities[3], forearmIntensities[1], releaseIntensities[1], press2, gap2));
        }
        else if (currentTimingMode == TimingMode.Fourth)
        {
            yield return StartCoroutine(PlayNoteSequence_FourFinger(1, fingerIntensities[0], forearmIntensities[0], releaseIntensities[0], press1, gap1));
            yield return StartCoroutine(PlayNoteSequence_FourFinger(2, fingerIntensities[1], forearmIntensities[1], releaseIntensities[1], press1, gap1));
        }
        else if (currentTimingMode == TimingMode.Triplet)
        {
            // 셋잇단음표는 BPM 기반 계산값 사용 (Gap은 기본값)
            float triDur = beatDuration / 3f;
            yield return StartCoroutine(PlayNoteSequence_FourFinger(1, fingerIntensities[0], forearmIntensities[0], releaseIntensities[0], triDur, 0.5f));
            yield return StartCoroutine(PlayNoteSequence_FourFinger(2, fingerIntensities[1], forearmIntensities[1], releaseIntensities[1], triDur, 0.5f));
            yield return StartCoroutine(PlayNoteSequence_FourFinger(3, fingerIntensities[2], forearmIntensities[0], releaseIntensities[0], triDur, 0.5f));
        }

        SendEmsCommand(0, 0, 0, 0, 0, 0);
        if (emsLogger != null) emsLogger.StopTrial();
        SetState(State.ReadyToTest);
        currentTestCoroutine = null;
    }

    // --- [4핑거 시퀀스: Gap 세분화 로직 (Release 200/150ms)] ---
    IEnumerator PlayNoteSequence_FourFinger(int noteIndex, int fingerIntensity, int forearmIntensity, int releaseCh6Intensity, float pressDuration, float gapDuration)
    {
        int fingerChannel = noteIndex;
        int ch1 = 0, ch2 = 0, ch3 = 0, ch4 = 0, ch5 = 0;
        if (fingerChannel == 1) ch1 = fingerIntensity;
        else if (fingerChannel == 2) ch2 = fingerIntensity;
        else if (fingerChannel == 3) ch3 = fingerIntensity;
        else if (fingerChannel == 4) ch4 = fingerIntensity;

        if (currentFourFingerMode == FourFingerStimMode.FingerForearm || currentFourFingerMode == FourFingerStimMode.FingerForearmPressRelease)
        {
            ch5 = forearmIntensity;
        }

        Debug.Log($"[RhythmTest] CH{fingerChannel} Press: {pressDuration * 1000:F0}ms");

        // 1. PRESS (자극)
        LogSemanticEms(fingerChannel, fingerIntensity, pressDuration);
        SendEmsCommand(ch1, ch2, ch3, ch4, ch5, 0, pressDuration);
        yield return new WaitForSeconds(pressDuration);

        // 2. RELEASE / GAP (휴식)
        bool isPressReleaseMode = (currentFourFingerMode == FourFingerStimMode.PressRelease || currentFourFingerMode == FourFingerStimMode.FingerForearmPressRelease);

        if (isPressReleaseMode)
        {
            // [Gap 시간에 따른 Release 분기]
            // Gap이 0.6초(600ms) 이상이면 -> Release 200ms + Rest 나머지
            // Gap이 그보다 짧으면(300ms 등) -> Release 150ms + Rest 나머지 
            float releaseStimTime = (gapDuration >= 0.55f) ? 0.3f : 0.15f;
            float fullRestTime = gapDuration - releaseStimTime;

            Debug.Log($"[RhythmTest] CH6 Release: {releaseStimTime * 1000:F0}ms, Rest: {fullRestTime * 1000:F0}ms");

            // Release Stim
            LogSemanticEms(6, releaseCh6Intensity, releaseStimTime);
            SendEmsCommand(0, 0, 0, 0, 0, releaseCh6Intensity, releaseStimTime);
            yield return new WaitForSeconds(releaseStimTime);

            // Full Rest
            SendEmsCommand(0, 0, 0, 0, 0, 0, fullRestTime);
            yield return new WaitForSeconds(fullRestTime);
        }
        else
        {
            // 쉼표 모드: Gap 전체 휴식
            SendEmsCommand(0, 0, 0, 0, 0, 0, gapDuration);
            yield return new WaitForSeconds(gapDuration);
        }
    }

    // --- [1핑거 리듬: Press 계산 유지, Gap 변경] ---
    IEnumerator PlayRhythmPattern_OneFinger()
    {
        SetState(State.Testing);

        // 1. BPM 기반 계산 (유지)
        float beatDuration = 60f / testBPM;
        float noteDuration = beatDuration / 2f;
        int pairCount = 2;

        if (currentTimingMode == TimingMode.Fourth) { noteDuration = beatDuration; pairCount = 1; }
        else if (currentTimingMode == TimingMode.Triplet) { noteDuration = beatDuration / 3f; pairCount = 3; }

        // Intensity Mapping
        int ch1_1, ch5_1, ch6_1;
        int ch1_2, ch5_2, ch6_2;

        if (currentVelocityMode == VelocityMode.WeakStrong)
        {
            ch1_1 = minActuations[0]; ch5_1 = minActuations[4]; ch6_1 = maxActuations[5];
            ch1_2 = maxActuations[0]; ch5_2 = maxActuations[4]; ch6_2 = minActuations[5];
        }
        else if (currentVelocityMode == VelocityMode.StrongStrong)
        {
            ch1_1 = maxActuations[0]; ch5_1 = maxActuations[4]; ch6_1 = maxActuations[5];
            ch1_2 = maxActuations[0]; ch5_2 = maxActuations[4]; ch6_2 = maxActuations[5];
        }
        else
        {
            ch1_1 = minActuations[0]; ch5_1 = minActuations[4]; ch6_1 = minActuations[5];
            ch1_2 = minActuations[0]; ch5_2 = minActuations[4]; ch6_2 = minActuations[5];
        }

        // 2. Duration & Gap 설정
        float press1, press2;
        float total = noteDuration * 2;
        switch (currentLengthMode)
        {
            case LengthMode.Swing1_5:
                press1 = total * (1.5f / 2.5f); press2 = total * (1.0f / 2.5f);
                break;
            case LengthMode.Swing2_0:
                press1 = total * (2f / 3f); // 1.0s
                press2 = total * (1f / 3f); // 0.5s
                break;
            case LengthMode.Straight:
            default:
                press1 = total * 0.5f; press2 = total * 0.5f;
                break;
        }

        float gap1, gap2;
        if (currentLengthMode == LengthMode.Swing2_0)
        {
            gap1 = 0.6f;
            gap2 = 0.6f;
        }
        else
        {
            gap1 = 0.5f;
            gap2 = 0.5f;
        }

        for (int i = 0; i < pairCount; i++)
        {
            // 1. Long Note
            yield return StartCoroutine(PlaySingleNote_OneFinger(
                currentOneFingerMode, ch1_1, ch5_1, ch6_1, press1, gap1));

            // 2. Short Note (Only for Eighth mode pairing)
            if (currentTimingMode == TimingMode.Eighth)
            {
                yield return StartCoroutine(PlaySingleNote_OneFinger(
                    currentOneFingerMode, ch1_2, ch5_2, ch6_2, press2, gap2));
            }
            else if (currentTimingMode == TimingMode.Triplet && i < 2)
            {
                // Triplet handling (simplified)
            }
        }

        SendEmsCommand(0, 0, 0, 0, 0, 0);
        if (emsLogger != null) emsLogger.StopTrial();
        SetState(State.ReadyToTest);
        currentTestCoroutine = null;
    }

    // --- [1핑거 시퀀스: Gap 세분화] ---
    IEnumerator PlaySingleNote_OneFinger(OneFingerStimMode stimMode, int pressCh1, int pressCh5_Forearm, int releaseCh6, float pressDuration, float gapDuration)
    {
        LogSemanticEms(1, pressCh1, pressDuration);
        // 1. PRESS
        if (stimMode == OneFingerStimMode.FingerForearm || stimMode == OneFingerStimMode.FingerForearmPressRelease)
        {
            SendEmsCommand(pressCh1, 0, 0, 0, pressCh5_Forearm, 0, pressDuration);
        }
        else
        {
            SendEmsCommand(pressCh1, 0, 0, 0, 0, 0, pressDuration);
        }
        yield return new WaitForSeconds(pressDuration);

        // 2. RELEASE / GAP
        if (stimMode == OneFingerStimMode.FingerForearmPressRelease || stimMode == OneFingerStimMode.PressRelease)
        {
            // Gap에 따른 분기 (0.6s -> 0.2/0.4, 그 외 -> 0.15/나머지)
            float releaseStimTime = (gapDuration >= 0.55f) ? 0.3f : 0.15f;
            float fullRestTime = gapDuration - releaseStimTime;

            LogSemanticEms(6, releaseCh6, releaseStimTime);
            SendEmsCommand(0, 0, 0, 0, 0, releaseCh6, releaseStimTime);
            yield return new WaitForSeconds(releaseStimTime);

            SendEmsCommand(0, 0, 0, 0, 0, 0, fullRestTime);
            yield return new WaitForSeconds(fullRestTime);
        }
        else
        {
            SendEmsCommand(0, 0, 0, 0, 0, 0, gapDuration);
            yield return new WaitForSeconds(gapDuration);
        }
    }


    // --- [!! 수정됨 5.1: 6채널 네트워크 함수 !!] ---
    void SendEmsCommand(int intensityCh1, int intensityCh2, int intensityCh3, int intensityCh4, int intensityCh5, int intensityCh6, float durationInSeconds = 0f)
    {
        int[] intensities = new int[] { intensityCh1, intensityCh2, intensityCh3, intensityCh4, intensityCh5, intensityCh6 };
        ArduinoTcpClient.Instance?.Send(intensities);
    }


    private void LogSemanticEms(int channel, int intensity, float durationSec)
    {
        float durationMs = durationSec * 1000f;
        emsLogger?.LogEmsCommand(channel, intensity, durationMs);
    }



    void UpdateStatus(string message)
    {
        if (statusText != null) statusText.text = "Status: " + message;
        Debug.Log(message);
    }

    void LoadCalibrationData()
    {
        for (int i = 0; i < ChannelCount; i++) // ChannelCount는 6
        {
            int channel = i + 1;
            minPerceptions[i] = PlayerPrefs.GetInt($"MinPerception_Ch{channel}", -1);
            maxPerceptions[i] = PlayerPrefs.GetInt($"MaxPerception_Ch{channel}", -1);
            minActuations[i] = PlayerPrefs.GetInt($"MinActuation_Ch{channel}", -1);
            maxActuations[i] = PlayerPrefs.GetInt($"MaxActuation_Ch{channel}", -1);
        }
        Debug.Log("Loaded all 6-channel calibration data.");
        UpdateAllCalibrationUI();
    }

    private string GetTaskCode()
    {
        return (currentRhythmMode == RhythmPlaybackMode.OneFinger) ? "SF" : "MF";
    }

    private void GetGuideAndLocationCodes(out string guide, out string location)
    {
        guide = "OP";
        location = "OF";

        if (currentRhythmMode == RhythmPlaybackMode.OneFinger)
        {
            // OneFingerStimMode: PressRelease / OnlyFinger / FingerForearm / FingerForearmPressRelease
            switch (currentOneFingerMode)
            {
                case OneFingerStimMode.PressRelease:
                    guide = "PR"; location = "OF"; break;
                case OneFingerStimMode.OnlyFinger:
                    guide = "OP"; location = "OF"; break;
                case OneFingerStimMode.FingerForearm:
                    guide = "OP"; location = "FF"; break;
                case OneFingerStimMode.FingerForearmPressRelease:
                    guide = "PR"; location = "FF"; break;
            }
        }
        else
        {
            // FourFingerStimMode: PressRelease / OnlyFinger / FingerForearm / FingerForearmPressRelease
            switch (currentFourFingerMode)
            {
                case FourFingerStimMode.PressRelease:
                    guide = "PR"; location = "OF"; break;
                case FourFingerStimMode.OnlyFinger:
                    guide = "OP"; location = "OF"; break;
                case FourFingerStimMode.FingerForearm:
                    guide = "OP"; location = "FF"; break;
                case FourFingerStimMode.FingerForearmPressRelease:
                    guide = "PR"; location = "FF"; break;
            }
        }
    }

}