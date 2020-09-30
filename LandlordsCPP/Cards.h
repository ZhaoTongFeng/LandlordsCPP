#pragma once
#include "BaseCards.h"
#include <vector>

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
    AAAAB,
    AAAABC,//������ͬ
    AABBCC,//���е������Ƕ���
    AAABBB,//���е�����������
    AAABBBCD,//
    ERR
};


//�ɻ����Ĵ���
struct CardsMode
{
    //1-4���Ƴ��ֵĴ���
    int A[4];

    //���Ƶ�ģʽ
    CardsEnum mCardsEnum;

    //������
    int* arr;

    //����
    int num;
};

class Cards :
    public BaseCards
{
public:
    Cards(const int num_cards);
    ~Cards();
    void ComputeAttribute()override;
    //����֮���ұߵ�ȫ�������ơ�����Ҫ���ƣ�ֻ��Ҫ��û�б�������ư���ԭ����˳����ӵ��µ�������
    bool OutCards(int* arr,int count);
};

