namespace Extensions.Unity
{

    using UnityEngine;

    public static class Vector3Extensions
    {
        public static Vector3 Copy(this Vector3 vector)
        {
            return new Vector3(vector.x, vector.y, vector.z);
        }
    }
}
