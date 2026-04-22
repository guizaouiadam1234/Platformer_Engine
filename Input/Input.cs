using Microsoft.Xna.Framework.Input;

public static class Input
{
    private static KeyboardState _currentKeyState;
    private static KeyboardState _previousKeyState;

    public static void Update()
    {
        _previousKeyState = _currentKeyState;
        _currentKeyState = Keyboard.GetState();
    }

    public static bool IsKeyDown(Keys key) => _currentKeyState.IsKeyDown(key);

    // Returns true ONLY on the frame the key was first pressed (Useful for Jumping)
    public static bool HasBeenPressed(Keys key) =>
        _currentKeyState.IsKeyDown(key) && _previousKeyState.IsKeyUp(key);
}