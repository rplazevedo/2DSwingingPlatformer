using Assets.Scripts.UnityEnums;

namespace Assets.Scripts.Input
{
    public static class UserInput
    {
        public static bool IsPressingJump()
        {
            return UnityEngine.Input.GetButtonDown(Inputs.Jump.ToString());
        }
    }
}
