// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("NozwIXAXDc/LPmQhJQreoU7YejVyyk7ovFI0GyJeMUUCjQ4mlewDtk2/9R0FMeX0g42csftT9xIH9Rmm2llXWGjaWVJa2llZWNUBrRcitMzlBEUHSHq0cg4BddW3yn89NBIoR2p7WWWJ04Rd/ZRD9tHqSD87HwrgaNpZemhVXlFy3hDer1VZWVldWFvy1HJoRqZFM+aIhcxcp5LBj2ZO5p7F33oCcywcQjWwcJyjp3qryRvJZn8WWDUFBhKNNCmRvP71NSj8VlSvb61HjqqAo40ZkRHS5MjTmHpxZgkChOOR8OspH8+smi6DtjQLtUyIVRqYTgaWwYOzVvireFOxkJhv7ESaOBJBxIifPygDOlh6qyE/s7pKyRfg+7PbYMhk51pbWVhZ");
        private static int[] order = new int[] { 6,4,13,4,12,11,6,12,12,13,11,12,13,13,14 };
        private static int key = 88;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
