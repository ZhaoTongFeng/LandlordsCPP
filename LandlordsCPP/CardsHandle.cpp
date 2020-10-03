#include "CardsHandle.h"
#include "Cards.h"
#include "Player.h"

CardsHandle::CardsHandle(const int size):
	CardsBuffer(size)
{
	for (int i = 0; i < size; i++) {
		buf[i] = i + 1;
	}
}

int CardsHandle::HandCards(std::vector<class Player*>& players, Cards* darkCards)
{
	//ϴ��
	SetMax();
	Shuffle();

	int i, n;
	int firIndex = 0;

	//�������
	for (i = 0; i < 3; i++) {
		players[i]->ResetAllCardStack();
	}


	//����
	//�������ŵ���
	for (i = 0; i < 3; i++) {
		Pop(darkCards);
	}

	i = 0;
	CardsBuffer* cb = nullptr;
	//ʣ����ѭ���������
	while (!IsEmpty())
	{
		Pop(n);
		cb = players[i]->GetCardStack();
		cb->Push(n);
		//ȷ�����Ƶ���
		if (n != 53 && n != 54 && n % 54 == 10) {
			firIndex = i;
		}
		i = (i + 1) % 3;
	}

	//��������
	for (i = 0; i < 3; i++) {
		players[i]->GetCardStack()->SortRank(false);
	}
	return firIndex;
}



void CardsHandle::Settle(std::vector<class Player*>& players, int winTeam,int landlordIndex)
{
	int dou_coin, sig_coin;
	dou_coin = rate * baseBet - water;
	sig_coin = static_cast<int>(static_cast<float>(dou_coin) / 2.0f);
	Player* p = nullptr;
	for (int i = 0; i < 3; i++) {
		p = players[i];
		if (p->team == winTeam) {
			if (i == landlordIndex) {
				p->ChangeBalance(dou_coin);
			}
			else {
				p->ChangeBalance(sig_coin);
			}
		}
		else {
			if (i == landlordIndex) {
				p->ChangeBalance(-1 * dou_coin);
			}
			else {
				p->ChangeBalance(-1 * sig_coin);
			}
		}
	}
}
