#pragma once
#include "BaseCardsHandle.h"
class CardsHandle :
    public BaseCardsHandle
{
public:
    CardsHandle(const int maxCount);
    ~CardsHandle();
    void Shuffle()override;
    //void HandCard(class BaseCards* c)override;
    void ComputeAttribute()override;
};

