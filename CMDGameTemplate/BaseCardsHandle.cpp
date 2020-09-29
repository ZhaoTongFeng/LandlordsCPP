#include "BaseCardsHandle.h"
#include "BaseCards.h"

BaseCardsHandle::BaseCardsHandle(const int maxCount):
	MAX_CARDS_COUNT(maxCount),
	cards(nullptr),
	cards_ptr(-1)
{
	cards = new int[MAX_CARDS_COUNT];
	//1-MAX_CARDS_COUNT
	for (int i = 0; i < MAX_CARDS_COUNT; i++) {
		cards[i] = i+1;
	}
}

BaseCardsHandle::~BaseCardsHandle()
{
	delete[] cards;
}

void BaseCardsHandle::HandCard(BaseCards* c)
{
	c->PushCard(cards[cards_ptr--]);
}

void BaseCardsHandle::HandCard(int& cardsEle)
{
	cardsEle = cards[cards_ptr--];
}
