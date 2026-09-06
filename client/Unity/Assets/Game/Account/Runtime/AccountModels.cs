using System;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace LinhGioi.Account
{
    public static class CharacterNameRules
    {
        public static bool IsValid(string name) => name != null && name.Length >= 3 && name.Length <= 16
            && Regex.IsMatch(name, "\\A[A-Za-z0-9_]+\\z");
    }

    [Serializable]
    public sealed class DevLoginRequest
    {
        public string devKey;
        public string displayName;

        public DevLoginRequest(string devKey, string displayName)
        {
            this.devKey = devKey;
            this.displayName = displayName;
        }
    }

    [Serializable]
    public sealed class CreateCharacterRequest
    {
        public string name;
        public string classId;

        public CreateCharacterRequest(string name, string classId)
        {
            this.name = name;
            this.classId = classId;
        }
    }

    [Serializable]
    public sealed class CreateCharacterInSlotRequest
    {
        public string name;
        public string classId;
        public int slot;

        public CreateCharacterInSlotRequest(string name, string classId, int slot)
        {
            this.name = name;
            this.classId = classId;
            this.slot = slot;
        }
    }

    [Serializable]
    public sealed class SaveCharacterPositionRequest
    {
        public float x;
        public float y;
        public float z;
        public float yawDegrees;

        public SaveCharacterPositionRequest(float x, float y, float z, float yawDegrees)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.yawDegrees = yawDegrees;
        }
    }

    [Serializable]
    public sealed class AccountResponse
    {
        public string accountId;
        public string displayName;
        public long createdAtUnixMs;
        public long updatedAtUnixMs;
    }

    [Serializable]
    public sealed class CharacterResponse
    {
        public int slot;
        public string characterId;
        public string accountId;
        public string name;
        public string classId;
        public long entityId;
        public float x;
        public float y;
        public float z;
        public float yawDegrees;
        public long createdAtUnixMs;
        public long updatedAtUnixMs;

        public Vector3 Position => new Vector3(x, y, z);

        public bool HasSamePosition(float expectedX, float expectedY, float expectedZ, float expectedYaw, float tolerance = 0.0001f)
        {
            return Mathf.Abs(x - expectedX) <= tolerance &&
                   Mathf.Abs(y - expectedY) <= tolerance &&
                   Mathf.Abs(z - expectedZ) <= tolerance &&
                   Mathf.Abs(yawDegrees - expectedYaw) <= tolerance;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}:{1}@({2:0.###},{3:0.###},{4:0.###}) yaw={5:0.###}", characterId, name, x, y, z, yawDegrees);
        }
    }

    [Serializable]
    public sealed class DevLoginResponse
    {
        public AccountResponse account;
        public bool created;
        public CharacterResponse[] characters;
    }

    [Serializable]
    public sealed class CharacterListResponse
    {
        public CharacterResponse[] characters;
    }
}
