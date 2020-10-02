#pragma once

#include <vector>
#include "CardsBuffer.h"

enum class CardsEnum
{
    A,
    JJ,//2张牌检测是否相等
    AA,
    AAA,
    AAAA,//4张牌，检测首和尾是否相等
    AAAB,

    //以5为分界线,5张及以上都要先检测顺子
    ABCDE,//先检测单顺子，首先检测是否连续
    AAABB,//倒数第二张是否相同
    AAABC,//倒数第二张是否相同
    AAAAB,

    AAAABC,//四张相同

    AABBCC,//所有点数都是对子
    AAABBB,//所有点数都是三张

    AAABBBCD,//
    ERR
};


//玩家手牌，打出的牌

class Cards :
    public CardsBuffer
{
public:

    Cards(const int num_cards);
    Cards(Cards* target);

    void MakeEmpty() override;
    
    //获取牌型名称
    const CardsEnum& GetCardsMode() { return mCardsEnum; }
    string GetCardsModeName()override;

    //计算当前的牌型
    void ComputeCardsMode()override;

    //比较两个牌型
    bool Compare(CardsBuffer* A, CardsBuffer* B)override;

    //打牌
    //玩家的牌，待出的牌，上一出牌
    //先检测待出的牌的牌型
    //和上一出牌进行比较
    //如果允许出牌，则将待出的牌出玩家手牌中移动到上一出牌
    //出牌可以看成将牌从这份牌转移到另一份牌
    bool OutCards(Cards* cards_pre, Cards* cards_last);

private:

    //当前牌型
    CardsEnum mCardsEnum = CardsEnum::ERR;
};

