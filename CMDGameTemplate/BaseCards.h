#pragma once
#include <string>

//һ�����ƻ�����Ϣ
//��������
class BaseCards
{
public:
	//���캯����ʼ����Ա����
	BaseCards(const int num_cards);

	virtual ~BaseCards();

	//��������
	virtual void ReSet();
	virtual void ComputeAttribute() {};

	//��ȡ����
	static std::string GetCardName(int n);
	//��ȡ���治����С��
	static std::string GetCardNameWithoutJoker(int n);

	//���һ������
	void PushCard(int n);
	const int* GetCards() { return cards; }

	int& GetCardsCount() { return cards_count; }

	bool IsFinish() { return isFinish; }
	void SetIsFinish(bool fin) { isFinish = fin; }

	//���򣬲�����ɫֻ����������ɨ��һ��鿴�Ƿ��д�С��������зŵ�����࣬�ٶ�ʣ�µ��ƽ�����Ȼ����

	//����֮���ұߵ�ȫ��������

	//����ʱ����ѡ�е��Ʒŵ�һ���б��У�����Ƿ�������ͣ�����¼����

protected:
	//����
	int* cards;

	//��������
	int cards_count;

	//�����Ʋ����ѽ���
	bool isFinish;
};

