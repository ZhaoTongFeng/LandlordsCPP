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

void BaseCards::SortCards()
{
    if (cards_count < 2) {
        return;
    }
    //int lj = -1;
    //int bj = -1;
    //for (int i = 0; i < cards_count; i++) {
    //    if (cards[i] == 53) {
    //        lj = i;
    //        break;
    //    }
    //}
    int num=0;
    int temp;
    //if (bj != 0) {
    //    num++;
    //    temp = cards[0];
    //    cards[0] = cards[bj];
    //    cards[bj] = temp;
    //}
    //for (int i = 0; i < cards_count; i++) {
    //    if (cards[i] == 54) {
    //        bj = i;
    //        break;
    //    }
    //}
    //if (lj != 0) {
    //    num++;
    //    temp = cards[1];
    //    cards[1] = cards[lj];
    //    cards[lj] = temp;
    //}
    
    //跳过大小王进行排序
    for (int i = num; i < cards_count-num; i++) {
        for (int j = cards_count - num - 1; j > i + num; j--) {
            if (cards[j - 1]%13 < cards[j]%13) {
                temp = cards[j - 1];
                cards[j - 1] = cards[j];
                cards[j] = temp;
            }
        }
    }

}
