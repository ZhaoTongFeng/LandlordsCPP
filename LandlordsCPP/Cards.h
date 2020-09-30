#pragma once
#include "BaseCards.h"
#include <vector>

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
    AAAAB,

    AAAABC,//四张相同

    AABBCC,//所有点数都是对子
    AAABBB,//所有点数都是三张

    AAABBBCD,//
    ERR
};


//飞机和四代二
struct CardsMode
{
    //0-4张牌出现的次数
    int A[5];

    //出牌的模式
    CardsEnum mCardsEnum = CardsEnum::ERR;

    //出的牌
    int* arr;

    //张数
    int num;
};

class Cards :
    public BaseCards
{
public:
    Cards(const int num_cards);
    ~Cards();
    void ComputeAttribute()override;
    //出牌之后将右边的全部往左移。不需要左移，只需要将没有被打出的牌按照原本的顺序添加到新的数组中
    bool OutCards(int* arr,int count);
};

