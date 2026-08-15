using UnityEngine;

public class GamePhaseFeedback : MonoBehaviour
{
    [Header("Light")]
    [Tooltip("Scene light to tint for day/night. Wire this in the Inspector.")]
    [SerializeField] private Light _dayNightLight;
    [SerializeField] private Color _dayColor = new Color(1f, 0.95f, 0.85f);
    [SerializeField] private Color _nightColor = new Color(0.15f, 0.2f, 0.45f);
    [SerializeField] private float _dayIntensity = 1f;
    [SerializeField] private float _nightIntensity = 0.25f;
    [SerializeField] private float _transitionSpeed = 2f;

    [Header("Debug")]
    [SerializeField] private bool _showPhaseLabel = true;

    private Color _targetColor;
    private float _targetIntensity;
    private GamePhase _currentPhase = GamePhase.Day;

    private EventBinding<GamePhaseChangedEvent> _phaseChangedBinding;

    private void Awake()
    {
        ApplyPhase(GamePhase.Day);
    }

    private void OnEnable()
    {
        _phaseChangedBinding = new EventBinding<GamePhaseChangedEvent>(OnPhaseChanged);
        EventBus<GamePhaseChangedEvent>.Register(_phaseChangedBinding);
    }

    private void OnDisable()
    {
        EventBus<GamePhaseChangedEvent>.Deregister(_phaseChangedBinding);
    }

    private void OnPhaseChanged(GamePhaseChangedEvent evt)
    {
        ApplyPhase(evt.Phase);
    }

    private void ApplyPhase(GamePhase phase)
    {
        _currentPhase = phase;
        bool isNight = phase == GamePhase.Night;
        _targetColor = isNight ? _nightColor : _dayColor;
        _targetIntensity = isNight ? _nightIntensity : _dayIntensity;

        if (_dayNightLight != null)
        {
            _dayNightLight.color = _targetColor;
            _dayNightLight.intensity = _targetIntensity;
        }
    }

    private void Update()
    {
        if (_dayNightLight == null) return;

        _dayNightLight.color = Color.Lerp(_dayNightLight.color, _targetColor, Time.deltaTime * _transitionSpeed);
        _dayNightLight.intensity = Mathf.Lerp(_dayNightLight.intensity, _targetIntensity, Time.deltaTime * _transitionSpeed);
    }

    private void OnGUI()
    {
        if (!_showPhaseLabel) return;

        GUILayout.BeginArea(new Rect(10f, 10f, 260f, 40f));
        GUILayout.Label($"Phase: {_currentPhase}");
        GUILayout.EndArea();
    }
}
