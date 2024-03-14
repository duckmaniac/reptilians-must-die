using System;
using System.Collections.Generic;

[Serializable]
public class LevelData
{
    public int level;
    public List<int> reptiliansCardNumbers;
    public List<int> playerCardNumbers;
    public string reptilianBossName;
    public int reptilianBossHealth;
    public int playerHealth;
    public int bossAvatarNumber;
    public int aiMode;
}
