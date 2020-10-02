#include "BaseCards.h"
#include "Util.h"

BaseCards::BaseCards(const int num_cards)
{
    cards = new int[num_cards];
	ReSet();
}

BaseCards::BaseCards(BaseCards* target)
{
    //手牌
    
    cards = new int[target->GetCardsCount()];
    memcpy_s(cards, target->GetCardsCount() * sizeof(int), target->GetCards(), target->GetCardsCount());

    //手牌数量
    cards_count = target->GetCardsCount();

}

BaseCards::~BaseCards()
{
    delete[] cards;
}

void BaseCards::ReSet()
{
	cards_count = 0;

}



std::string BaseCards::GetCardNameWithoutJoker(int n)
{
    std::string str;
    int temp = n % 13;
    if (temp == 11) {
        str = "J";
    }
    else if (temp == 12) {
        str = "Q";
    }
    else if (temp == 0) {
        str = "K";
    }
    else if (temp == 1) {
        str = "A";
    }
    else {
        str = std::to_string(temp);
    }
    return str;
}

void BaseCards::PushCard(int n)
{
    cards[cards_count++] = n;
    ComputeAttribute();
}

void BaseCards::SortCards()
{
    if (cards_count < 2) {
        return;
    }
    Util::SortRank(cards, cards_count, false);
}
