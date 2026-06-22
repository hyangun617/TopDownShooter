public class FireEventArgs
{
    public bool IsPressed { get; }
    public float HoldDuration { get; }
    public float AnalogValue { get; }

    public FireEventArgs(bool isPressed, float holdDuration = 0f, float analogValue = 1f)
    {
        IsPressed = isPressed;
        HoldDuration = holdDuration;
        AnalogValue = analogValue;
    }
}