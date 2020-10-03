#pragma once

#include "CardsBuffer.h"
#include <vector>

//ºÉ¹Ù
//Ï´ÅÆ£¬·¢ÅÆ£¬½áËã
class CardsHandle :
    public CardsBuffer
{
public:
    CardsHandle(const int size);
	~CardsHandle() {};
    int HandCards(std::vector<class Player*>& players, class Cards* darkCards);
    void Settle(std::vector<class Player*>& players,int winTeam, int landlordIndex);

private:
	//±¶Êı
	int rate = 1;

	//µ×·Ö
	int baseBet = 100;

	//³éË®
	int water = 20;
};

