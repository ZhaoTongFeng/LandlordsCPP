#pragma once
#include "BaseCards.h"
class Cards :
    public BaseCards
{
public:
    Cards(const int num_cards);
    ~Cards();
    void ComputeAttribute()override;
};

