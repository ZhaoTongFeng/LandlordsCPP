#pragma once
#include "BaseGameMode.h"

enum class CallState
{
    NO,
    CALL,
    Rob
};

class GameMode :
    public BaseGameMode
{
public:
    GameMode();
    ~GameMode();
    void ProcessInput()override;
    void UpdateGame(float deltaTime)override;
    void GenerateOutput(std::string& str)override;
protected:
    void HandCards()override;
    void Settle()override;

    //叫牌状态
    CallState mCallState;
    //记录参与叫牌的人数
    int mCallArr[3]{ 0 };
    int mCallArrCount = 0;
    //标记叫地主的玩家
    int mLandlordsIndex;

    //叫牌次数，小于等于四次
    int numCall;

    //叫牌点数
    int inputPoint;

    //出牌
    //将选中的牌放到一个列表中，检测是否符合牌型，并记录牌型
    bool gate = false;
    std::string mCardsBuffer;
    int mPreOutCards[20];
    int mPreOutCardsCount = 0;

    //一个出牌回合的开始不能不出牌
    bool isStart = true;

    //两家不要牌则判定为新回合开始
    int mMissCount = 0;

};

