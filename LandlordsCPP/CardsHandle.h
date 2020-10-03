#pragma once

#include "CardsBuffer.h"
#include <vector>

//�ɹ�
//ϴ�ƣ����ƣ�����
class CardsHandle :
    public CardsBuffer
{
public:
    CardsHandle(const int size);
	~CardsHandle() {};
    int HandCards(std::vector<class Player*>& players, class Cards* darkCards);
    void Settle(std::vector<class Player*>& players,int winTeam, int landlordIndex);

private:
	//����
	int rate = 1;

	//�׷�
	int baseBet = 100;

	//��ˮ
	int water = 20;
};

