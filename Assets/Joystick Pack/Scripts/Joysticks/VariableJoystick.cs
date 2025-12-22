using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VariableJoystick : Joystick
{
    public float MoveThreshold { get { return moveThreshold; } set { moveThreshold = Mathf.Abs(value); } }
    // 조이스틱을 움직이기 위한 최소한의 이동 임계값. 속성을 통해 접근하며, 음수로 설정되지 않도록 보정합니다.

    [SerializeField] private float moveThreshold = 1;
    [SerializeField] private JoystickType joystickType = JoystickType.Fixed;
    // 이동 임계값 및 조이스틱 타입을 설정하기 위한 시리얼라이즈된 필드들.

    private Vector2 fixedPosition = Vector2.zero;
    // 고정 조이스틱 모드에서 사용할 위치 정보.

    public void SetMode(JoystickType joystickType)
    {
        this.joystickType = joystickType;
        // 조이스틱 모드를 설정하는 메서드. 고정 모드일 경우 위치를 조절하고 활성화합니다.
        if (joystickType == JoystickType.Fixed)
        {
            background.anchoredPosition = fixedPosition;
            background.gameObject.SetActive(true);
        }
        else
            background.gameObject.SetActive(false);
    }

    protected override void Start()
    {
        base.Start();
        fixedPosition = background.anchoredPosition;
        SetMode(joystickType);
        // 초기화 메서드. 시작 시 고정 위치를 설정하고 초기 모드를 설정합니다.
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (joystickType != JoystickType.Fixed)
        {
            background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
            background.gameObject.SetActive(true);
        }
        base.OnPointerDown(eventData);
        // 터치 입력이 발생하면 조이스틱을 따라 움직이도록 설정하고, 필요 시 고정 모드일 때는 위치를 업데이트합니다.
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (joystickType != JoystickType.Fixed)
            background.gameObject.SetActive(false);

        base.OnPointerUp(eventData);
        // 터치 입력이 끝나면 동적 모드에서는 조이스틱을 숨기고, 필요 시 고정 모드일 때는 비활성화합니다.
    }

    protected override void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
    {
        if (joystickType == JoystickType.Dynamic && magnitude > moveThreshold)
        {
            Vector2 difference = normalised * (magnitude - moveThreshold) * radius;
            background.anchoredPosition += difference;
        }
        base.HandleInput(magnitude, normalised, radius, cam);
        // 입력 처리 메서드. 동적 모드에서 이동 임계값을 초과하는 경우에만 배경 위치를 업데이트합니다.
    }
}

public enum JoystickType { Fixed, Floating, Dynamic }