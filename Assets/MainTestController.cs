using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainTestController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject calibrationPanel;
    public GameObject mainTestPanel;

    [Header("Score Play Audio")]
    public AudioSource audioSource;   // 씬에 AudioSource 하나 두고 연결
    public AudioClip scorePlayClip;   // mp3 import 후 여기 연결

    [Header("Step 1 Buttons (Guidance)")]
    public Button btnVisual;
    public Button btnElectricCue;
    public Button btnPressOnly;
    public Button btnPressRelease;

    [Header("Step 1 Confirm Button")]
    public Button btnSelectGuidance;

    [Header("Step 3 Output Text")]
    public TMP_Text selectedGuidanceValueText;

    // 내부 상태
    private string pendingGuidanceLabel = null;  // Step1에서 "선택만" 한 값
    private string confirmedGuidanceLabel = null; // Select 눌러 확정된 값

    private void Start()
    {
        // (선택) 초기 패널 상태 보정
        if (calibrationPanel != null) calibrationPanel.SetActive(true);
        if (mainTestPanel != null) mainTestPanel.SetActive(false);

        // Step1 버튼 연결(Inspector에서 OnClick으로 해도 되는데, 코드로도 가능)
        if (btnVisual != null) btnVisual.onClick.AddListener(() => OnGuidancePicked("1. Visual Guide"));
        if (btnElectricCue != null) btnElectricCue.onClick.AddListener(() => OnGuidancePicked("2. Electric Cue"));
        if (btnPressOnly != null) btnPressOnly.onClick.AddListener(() => OnGuidancePicked("3. Press-Only Muscle Guide"));
        if (btnPressRelease != null) btnPressRelease.onClick.AddListener(() => OnGuidancePicked("4. Press–Release Muscle Guide"));

        if (btnSelectGuidance != null) btnSelectGuidance.onClick.AddListener(ConfirmGuidanceSelection);

        // Step3 초기 표기
        RefreshSelectedGuidanceUI();
    }

    // =========================
    // 1) Calibration -> MainTest
    // =========================
    public void OnCalibrationPass()
    {
        if (calibrationPanel != null) calibrationPanel.SetActive(false);
        if (mainTestPanel != null) mainTestPanel.SetActive(true);
    }

    // =========================
    // 2) Score Play Button -> mp3 재생
    // =========================
    public void OnPlayScoreAudio()
    {
        if (audioSource == null)
        {
            Debug.LogError("[ExperimentUIController] AudioSource is not assigned.");
            return;
        }
        if (scorePlayClip == null)
        {
            Debug.LogError("[ExperimentUIController] scorePlayClip(AudioClip) is not assigned.");
            return;
        }

        audioSource.Stop();
        audioSource.clip = scorePlayClip;
        audioSource.Play();
    }

    // =========================
    // 3) Step1 선택 -> Select로 확정 -> Step3 반영
    // =========================
    private void OnGuidancePicked(string label)
    {
        pendingGuidanceLabel = label;
        // 여기서 버튼 하이라이트 같은 것도 줄 수 있음(원하면 다음 단계에서)
        Debug.Log($"[Guidance Picked] pending = {pendingGuidanceLabel}");
    }

    private void ConfirmGuidanceSelection()
    {
        if (string.IsNullOrEmpty(pendingGuidanceLabel))
        {
            Debug.LogWarning("[ExperimentUIController] No guidance picked yet.");
            return;
        }

        confirmedGuidanceLabel = pendingGuidanceLabel;
        Debug.Log($"[Guidance Confirmed] confirmed = {confirmedGuidanceLabel}");

        RefreshSelectedGuidanceUI();
    }

    private void RefreshSelectedGuidanceUI()
    {
        if (selectedGuidanceValueText == null) return;

        if (string.IsNullOrEmpty(confirmedGuidanceLabel))
            selectedGuidanceValueText.text = "(Not selected)";
        else
            selectedGuidanceValueText.text = confirmedGuidanceLabel;
    }
}
