using UnityEngine;

namespace Controllers
{
    public static class PlayerInput
    {
        public struct Input
        {
            public float acceleration;
            public float pitch;
            public float roll;
            public float yaw;
        }
        
        public static Input getInput()
        {
            var result = new Input();
            if (UnityEngine.Input.GetKey(KeyCode.UpArrow))
            {
                result.acceleration += 1f;
            }

            if (UnityEngine.Input.GetKey(KeyCode.DownArrow))
            {
                result.acceleration -= 1f;
            }

            if (UnityEngine.Input.GetKey(KeyCode.A))
            {
                result.roll += 1f;
            }
            if (UnityEngine.Input.GetKey(KeyCode.D))
            {
                result.roll -= 1f;
            }

            if (UnityEngine.Input.GetKey(KeyCode.W))
            {
                result.pitch += 1f;
            }
            if (UnityEngine.Input.GetKey(KeyCode.S))
            {
                result.pitch -= 1f;
            }

            if (UnityEngine.Input.GetKey(KeyCode.LeftArrow))
            {
                result.yaw -= 1f;
            }
            if (UnityEngine.Input.GetKey(KeyCode.RightArrow))
            {
                result.yaw += 1f;
            }

            return result;
        }
    }
}