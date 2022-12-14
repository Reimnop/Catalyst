using UnityEngine;

namespace Catalyst.Util;

public static class Utils
{
    public static bool IsInEditor()
    {
        if (EditorManager.inst == null)
        {
            return false;
        }

        if (!EditorManager.inst.isEditing)
        {
            return false;
        }

        return true;
    }
}