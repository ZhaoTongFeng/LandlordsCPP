#include "BaseCards.h"

BaseCards::BaseCards(const int num_cards)
{
    cards = new int[num_cards];
	ReSet();
}

BaseCards::~BaseCards()
{
    delete[] cards;
}

void BaseCards::ReSet()
{
	cards_count = 0;
	isFinish = false;
}

std::string BaseCards::GetCardName(int n)
{
    std::string str;
    if (n % 54 == 53) {
        //小王
        str = "LJ";
    }
    else if (n % 54 == 0) {
        //大王
        str = "BJ";
    }
    else {
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
    }
    return str;
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
