namespace Assets.Scripts.Input
{
    public static class UserInput
    {
        public static string Horizontal = "Horizontal";
        public static string Vertical = "Vertical";
        public static string Jump = "Jump";


        public static bool IsPressingJump()
        {
            return UnityEngine.Input.GetButtonDown(Jump);
        }

        public static float GetHorizontalValue()
        {
            return UnityEngine.Input.GetAxis(Horizontal);
        }

        public static float GetVerticalValue()
        {
            return UnityEngine.Input.GetAxis(Vertical);
        }

        public static bool GetLeftMouseButtonDown()
        {
            return UnityEngine.Input.GetMouseButtonDown(0);
        }

    }
}
