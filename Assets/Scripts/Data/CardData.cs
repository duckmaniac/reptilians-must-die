using System;

[Serializable]
public class CardData
{
    public enum Ability
    {
        None = 0,
        UFO = 1,
        Tower = 2,
        FoilHat = 3,
        Mimic = 4,
        Regeneration = 5,
        Betrayer = 6
    }

    public string title;
    public string description;
    public int cost;
    public int attack;
    public int health;
    public int avatarNumber;
    public bool isReptilian;
    public Ability ability;
}
