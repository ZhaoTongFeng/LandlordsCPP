#pragma once

#include <vector>
#include "CardsBuffer.h"

enum class CardsEnum
{
    A,
    JJ,//2���Ƽ���Ƿ����
    AA,
    AAA,
    AAAA,//4���ƣ�����׺�β�Ƿ����
    AAAB,

    //��5Ϊ�ֽ���,5�ż����϶�Ҫ�ȼ��˳��
    ABCDE,//�ȼ�ⵥ˳�ӣ����ȼ���Ƿ�����
    AAABB,//�����ڶ����Ƿ���ͬ
    AAABC,//�����ڶ����Ƿ���ͬ
    AAAAB,

    AAAABC,//������ͬ

    AABBCC,//���е������Ƕ���
    AAABBB,//���е�����������

    AAABBBCD,//
    ERR
};


//������ƣ��������

class Cards :
    public CardsBuffer
{
public:

    Cards(const int num_cards);
    Cards(Cards* target);

    void MakeEmpty() override;
    
    //��ȡ��������
    const CardsEnum& GetCardsMode() { return mCardsEnum; }
    string GetCardsModeName()override;

    //���㵱ǰ������
    void ComputeCardsMode()override;

    //�Ƚ���������
    bool Compare(CardsBuffer* A, CardsBuffer* B)override;

    //����
    //��ҵ��ƣ��������ƣ���һ����
    //�ȼ��������Ƶ�����
    //����һ���ƽ��бȽ�
    //���������ƣ��򽫴������Ƴ�����������ƶ�����һ����
    //���ƿ��Կ��ɽ��ƴ������ת�Ƶ���һ����
    bool OutCards(Cards* cards_pre, Cards* cards_last);

private:

    //��ǰ����
    CardsEnum mCardsEnum = CardsEnum::ERR;
};

