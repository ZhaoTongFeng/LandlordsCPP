#pragma once
#include "BaseGameMode.h"

//叫牌状态
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

    void ReStart()override;
    ////////////////////////////////////
    //发牌

    //三张底牌
    class Cards* mDarkCards = nullptr;
    /////////////////////////////////////
    //叫牌状态
    CallState mCallState = CallState::NO;

    //参与叫牌的人数及其下标，第一个人为叫地主的人
    int mCallArr[3]{ 0 };
    int mCallArrCount = 0;

    //标记叫地主的玩家
    int mLandlordsIndex = -1;

    //叫牌次数，小于等于四次
    int numCall = 0;

    //叫牌点数
    int inputPoint = -1;
    ////////////////////////////////////////
    //出牌
    //将选中的牌放到一个列表中，检测是否符合牌型，并记录牌型
    bool gate = false;

    //两家不要牌则判定为新回合开始
    int mMissCount = 0;

    //上一出牌
    class Cards* mLastCards = nullptr;

    //准备出的牌
    //将选择的牌放到这儿
    class Cards* mPreCards = nullptr;

};

