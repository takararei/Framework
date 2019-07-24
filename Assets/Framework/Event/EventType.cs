public enum EventType
{
    DoNumChange,//钻石更新 
    LevelIntroduceUpdate,//关卡介绍 关卡介绍面板
    ItemIntroduceUpdate,//道具介绍 道具面板
    ItemCountUpdate,//道具数量更新
    AcItemUpdate,//成就更新
    
    Play_CoinUpdate,//Game 模式下 金币更新
    Play_LifeUpdate,//生命值更新
    Play_NowRoundUpdate,//回合更新
    
    GameOver,
    GameWin,
    GamePause,
    RestartGame,
    LeaveGameScene,

    HandleGrid,
    HandleEnemy,
    ShowEnemyInfoPanel,
    ShowTowerInfoPanel,
    SetStartPos,//设置游戏回合 开始按钮的位置

    UseItemInGame,//Gme 模式下 使用道具
    RemoveEnemy,
}
