using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;

public static class Constants
{
    public const int BoatLayer = 6;
    public const int FishLayer = 7;
    public const int InteractibleLayer = 8;
    public const int FishingWallLayer = 15;
    public const int WaterSurfaceLayer = 16;

    public const float FixedRotationDur = .25f;

    public const float MinCatchDepth = -1f;
    public const float DeadPullMod = 2;

    public static string GetFishName(FishType fish)
    {
        switch (fish)
        {
            case FishType.Test:
                return "Test Fish";
            case FishType.Test2:
                return "Teste Fish";
            case FishType.Blue:
                return "Jeremy";
            case FishType.Orange:
                return "Rebecca";
            case FishType.Lime:
                return "Phil";
            case FishType.Dorito:
                return "Eduardo";
            case FishType.BlueStripe:
                return "Eliza";
            case FishType.Rainbow:
                return "Patrick";
            case FishType.BigGreen:
                return "Carly";
            case FishType.Blow:
                return "Ronald";
            case FishType.Purple:
                return "Felippe";
            case FishType.Pink:
                return "Duncan";
            case FishType.Red:
                return "Susan";
            default:
                return fish.ToString();
        }
    }
}

public enum FishType
{
    Test,
    Test2,
    Blue,
    Orange,
    Lime,
    Dorito,
    BlueStripe,
    Rainbow,
    BigGreen,
    Torble,
    Blow,
    Purple,
    Pink,
    Red
}