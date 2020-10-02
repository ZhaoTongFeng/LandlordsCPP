#pragma once
#include <string>

//һ�����ƻ�����Ϣ
//��������
class BaseCards
{
public:
	BaseCards() {};
	//���캯����ʼ����Ա����
	BaseCards(const int num_cards);
	BaseCards(BaseCards* target);

	virtual ~BaseCards();

	//��������
	virtual void ReSet();

	//����һЩ����
	virtual void ComputeAttribute() {};


	//��ȡ���治����С��
	static std::string GetCardNameWithoutJoker(int n);

	//���һ������
	void PushCard(int n);


	int* GetCards() { return cards; }

	int& GetCardsCount() { return cards_count; }



	//���򣬲�����ɫֻ����������ɨ��һ��鿴�Ƿ��д�С��������зŵ�����࣬�ٶ�ʣ�µ��ƽ�����Ȼ����
	void SortCards();
	

protected:
	//����
	int* cards = nullptr;

	//��������
	int cards_count = 0;
};

