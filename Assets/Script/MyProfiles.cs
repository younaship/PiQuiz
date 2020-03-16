using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyProfiles
{
    public enum StringProfile { SelectedPatches }

    public static string GetProfiles(StringProfile profile)
    {
        switch (profile)
        {
            case StringProfile.SelectedPatches:
                return PlayerPrefs.GetString("salected_patch", null);
        }
        return null;
    }

    public static void SetProfiles(StringProfile profile,string value)
    {
        switch (profile)
        {
            case StringProfile.SelectedPatches:
                PlayerPrefs.SetString("salected_patch",value);
                break;
        }
    }
}
